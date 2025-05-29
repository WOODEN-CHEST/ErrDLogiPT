using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Audio.Source;
using GHEngine.Collections;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErrDLogiPTClient.Scene.UI;

public class UIBasicButton : ITimeUpdatable, IRenderableItem
{
    // Fields.
    public bool IsEnabled { get; set; }
    public bool IsDisabledOnClick { get; set; }
    public bool IsVisible { get; set; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateButtonArea();
        }
    }

    public Color ButtonColor
    {
        get => _normalButtonColor;
        set
        {
            _normalButtonColor = value;
            UpdateRenderedColors();
        }
    }

    public Color HighlightColor
    {
        get => _highlightColor;
        set
        {
            _highlightColor = value;
            UpdateRenderedColors();
        }
    }

    public Color ClickColor { get; set; } = new Color(79, 229, 240, 255);

    public string Text
    {
        get => _buttonText.Text;
        set
        {
            _buttonText.Components.First().Text = value;
        }
    }

    public bool IsTextShadowed { get; set; }

    public float Length
    {
        get => _buttonLength;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid button length: {value}");
            }
            if (value < 0f)
            {
                throw new ArgumentException("Button length cannot be < 0");
            }
            _buttonLength = value;
        }
    }

    public float Scale
    {
        get => _scale;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid scale: {value}");
            }
            if (value < 0f)
            {
                throw new ArgumentException("Button scale cannot be < 0");
            }

            _scale = value;
            
            UpdateSize();
            UpdateButtonArea();
        }
    }

    public bool IsButtonTargeted
    {
        get => _clickDetector.IsTargeted;
        set => _clickDetector.IsTargeted = value;
    }

    public float Volume
    {
        get => _volume;
        set => _volume = value;
    }

    public IEnumerable<IPreSampledSound> ClickSounds
    {
        get => _clickSounds.ToArray()!;
        set => _clickSounds.SetItems(value?.ToArray() ?? Array.Empty<IPreSampledSound>());
    }

    public IEnumerable<IPreSampledSound> HoverSounds
    {
        get => _onHoverSounds.ToArray()!;
        set => _onHoverSounds.SetItems(value?.ToArray() ?? Array.Empty<IPreSampledSound>());
    }

    public IEnumerable<IPreSampledSound> UnhoverSounds
    {
        get => _onUnhoverSounds.ToArray()!;
        set => _onUnhoverSounds.SetItems(value?.ToArray() ?? Array.Empty<IPreSampledSound>());
    }

    public LogiSoundCategory SoundCategory
    {
        get => _soundCategory;
        set => _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ButtonClickMethod ClickMethod { get; set; } = ButtonClickMethod.ActivateOnFullClick;

    public RectangleF ButtonArea => _clickDetector.ElementBounds;

    public event EventHandler<BasicButtonClickEndEventArgs>? ClickEnd;
    public event EventHandler<BasicButtonClickEndEventArgs>? ClickEnd;
    public event EventHandler<EventArgs>? HoverStart;
    public event EventHandler<EventArgs>? HoverEnd;


    // Private static fields.
    private const int FRAME_INDEX_LEFT_PART = 0;
    private const int FRAME_INDEX_MIDDLE_PART = 1;
    private const int FRAME_INDEX_RIGHT_PART = 2;
    private const float POSITION_MARGIN_OF_ERROR = 0.0025f;
    private static readonly TimeSpan COLOR_FADE_DURATION = TimeSpan.FromSeconds(0.1d);
    private const double HIGHLIGH_FADE_FACTOR_MAX = 1d;
    private const double HIGHLIGH_FADE_FACTOR_MIN = 0d;


    // Private fields.
    private readonly IUserInput _input;
    private readonly ILogiSoundEngine _soundEngine;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly SpriteItem _leftPartSprite;
    private readonly SpriteItem _middlePartSprite;
    private readonly SpriteItem _rightPartSprite;
    private readonly TextBox _buttonText;

    private float _buttonLength = 1f;
    private Vector2 _position = Vector2.Zero;
    private float _scale = 1f;

    private bool _isEnabled = false;
    private bool _isDisabledOnCLick;
    private DeltaValue<bool> _isHovered = new();

    private RandomSequence<IPreSampledSound> _clickSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _onHoverSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _onUnhoverSounds = new(Array.Empty<IPreSampledSound>());
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;
    private float _volume = 1f;

    private Color _normalButtonColor = Color.White;
    private Color _highlightColor = new Color(173, 255, 110, 255);
    private double _highlightFadeFactor = 0d;

    private readonly ClickDetector _clickDetector;


    // Constructors
    public UIBasicButton(ILogiSoundEngine soundEngine,
        IUserInput input,
        ISceneAssetProvider? assetProvider,
        ISpriteAnimation animation,
        GHFontFamily font)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(animation, nameof(animation));
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        ArgumentNullException.ThrowIfNull(soundEngine, nameof(soundEngine));
        ArgumentNullException.ThrowIfNull(assetProvider, nameof(assetProvider));

        _input = input;
        _assetProvider = assetProvider;
        _clickDetector = new(input);

        if (animation.FrameCount < 3)
        {
            throw new ArgumentException("basic main menu button animation must have at least 3 frames", nameof(animation));
        }

        _leftPartSprite = new(animation.CreateInstance());
        _middlePartSprite = new(animation.CreateInstance());
        _rightPartSprite = new(animation.CreateInstance());

        _leftPartSprite.Animation.FrameIndex = FRAME_INDEX_LEFT_PART;
        _middlePartSprite.Animation.FrameIndex = FRAME_INDEX_MIDDLE_PART;
        _rightPartSprite.Animation.FrameIndex = FRAME_INDEX_RIGHT_PART;

        _leftPartSprite.Origin = new(1f, 0.5f);
        _middlePartSprite.Origin = new(0.5f, 0.5f);
        _rightPartSprite.Origin = new(0f, 0.5f);

        _buttonText = new TextBox()
        {
            new(font, string.Empty),
        };
        _buttonText.Origin = new(0.5f, 0.5f);
        _buttonText.FitMethod = TextFitMethod.Resize;
        
        UpdateSize();
        UpdateButtonArea();
        UpdateRenderedColors();
    }


    // Methods.
    public void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_buttonText);
        _assetProvider.RegisterRenderedItem(_leftPartSprite);
        _assetProvider.RegisterRenderedItem(_middlePartSprite);
        _assetProvider.RegisterRenderedItem(_rightPartSprite);
    }

    public void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_buttonText);
        _assetProvider.UnregisterRenderedItem(_leftPartSprite);
        _assetProvider.UnregisterRenderedItem(_middlePartSprite);
        _assetProvider.UnregisterRenderedItem(_rightPartSprite);
    }

    public bool IsPositionOverButton(Vector2 position)
    {
        Vector2 BottomLeft = new(_buttonArea.X, _buttonArea.Y);
        Vector2 TopRight = new(_buttonArea.X + _buttonArea.Width, _buttonArea.Y + _buttonArea.Height);
        return GHMath.IsPointInArea(position, BottomLeft, TopRight, 0f);
    }


    // Private methods.
    private (Vector2 Left, Vector2 Middle, Vector2 Right) GetSpritePositions(float aspectRatio)
    {
        /* Margin of error is added to fix visible seams due to floating point inaccuracies.
        * Okay, that was the idea, but seams still happen due to how MonoGame handles transparency.*/
        Vector2 Middle = _position;

        Vector2 Left = Middle - GHMath.GetWindowAdjustedVector(
            new Vector2(_middlePartSprite.Size.X / 2f - POSITION_MARGIN_OF_ERROR, 0f), aspectRatio);

        Vector2 Right = Middle + GHMath.GetWindowAdjustedVector(
            new Vector2(_middlePartSprite.Size.X / 2f - POSITION_MARGIN_OF_ERROR, 0f), aspectRatio);

        return(Left, Middle, Right);
    }

    private void UpdateRenderPositions(float windowAspectRatio)
    {
        var Positions = GetSpritePositions(windowAspectRatio);

        _leftPartSprite.Position = Positions.Left;
        _middlePartSprite.Position = Positions.Middle;
        _rightPartSprite.Position = Positions.Right;

        _middlePartSprite.Position = _position;
    }

    private void UpdateSize()
    {
        Vector2 MiddleFrameSize = _middlePartSprite.FrameSize;
        _middlePartSprite.Size = new Vector2(1f * _buttonLength, MiddleFrameSize.Y / MiddleFrameSize.X) * _scale;

        _leftPartSprite.Size = new(_leftPartSprite.FrameSize.X / _leftPartSprite.FrameSize.Y
            * _middlePartSprite.Size.Y, _middlePartSprite.Size.Y);

        _rightPartSprite.Size = new(_rightPartSprite.FrameSize.X / _rightPartSprite.FrameSize.Y
            * _middlePartSprite.Size.Y, _middlePartSprite.Size.Y);
    }

    private void UpdateButtonArea()
    {
        float InputRatio = _input.InputAreaRatio;
        var Positions = GetSpritePositions(InputRatio);
        Vector2 AdjustedLeftSpriteSize = GHMath.GetWindowAdjustedVector(_leftPartSprite.Size, InputRatio);
        Vector2 AdjustedRightSpriteSize = GHMath.GetWindowAdjustedVector(_rightPartSprite.Size, InputRatio);

        Vector2 BottomLeft = new Vector2(Positions.Left.X - AdjustedLeftSpriteSize.X,
            Positions.Left.Y - AdjustedLeftSpriteSize.Y / 2f);

        Vector2 TopRight = new Vector2(Positions.Right.X + AdjustedRightSpriteSize.X,
            Positions.Right.Y + AdjustedRightSpriteSize.Y / 2f);

        Vector2 Dimensions = TopRight - BottomLeft;

        _clickDetector.ElementBounds = new(BottomLeft.X, BottomLeft.Y, Dimensions.X, Dimensions.Y);
    }

    private void PlayRandomSound(RandomSequence<IPreSampledSound> sounds)
    {
        if (sounds.Count == 0)
        {
            return;
        }

        IPreSampledSound TargetSound = sounds.Get()!;
        _soundEngine.CreateSoundInstance(TargetSound, SoundCategory);
    }




    private void ButtonGenericUpdate(IProgramTime time)
    {
        _highlightFadeFactor = Math.Clamp(
            _highlightFadeFactor + time.PassedTime.TotalSeconds * (_isTargeted ? 1d : -1d) / COLOR_FADE_DURATION.TotalSeconds,
            HIGHLIGH_FADE_FACTOR_MIN,
            HIGHLIGH_FADE_FACTOR_MAX);

        UpdateRenderedColors();
    }

    private void UpdateRenderedColors()
    {
        Color FinalSpriteColor = FloatColor.InterpolateRGB(_normalButtonColor, _highlightColor, (float)_highlightFadeFactor);

        _leftPartSprite.Mask = FinalSpriteColor;
        _middlePartSprite.Mask = FinalSpriteColor;
        _rightPartSprite.Mask = FinalSpriteColor;
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        UpdateButtonArea();

        ButtonGenericUpdate(time);
        if (IsButtonTargeted)
        {
            OnButtonTargetUpdate(time);
        }
        else
        {
            OnButtonNotTargetUpdate();
        }
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        UpdateRenderPositions(renderer.AspectRatio); 
        _leftPartSprite.Render(renderer, time);
        _middlePartSprite.Render(renderer, time);
        _rightPartSprite.Render(renderer, time);
    }
}