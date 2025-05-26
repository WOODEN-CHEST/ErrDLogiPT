using GHEngine;
using GHEngine.Audio;
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
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu.UI;

public class MainMenuBasicButton : ITimeUpdatable, IRenderableItem
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

    public string Text
    {
        get => _buttonText.Text;
        set
        {
            _buttonText.Components.First().Text = value;
        }
    }

    public float Length
    {
        get => _buttonLength;
        set
        {
            if (float.IsNaN(value))
            {
                throw new ArgumentException("Button length cannot be NaN");
            }
            if (float.IsInfinity(value))
            {
                throw new ArgumentException("Button length cannot be infinity");
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
            _scale = value;
            
            UpdateSize();
            UpdateButtonArea();
        }
    }

    public bool IsButtonTargeted
    {
        get => _isTargeted;
        set
        {
            _isTargeted = value;
        }
    }

    public float Volume
    {
        get => _volume;
        set => _volume = value;
    }

    public IEnumerable<ISound> ClickSounds
    {
        get => _clickSounds.ToArray()!;
        set => _clickSounds.SetItems(value?.ToArray() ?? Array.Empty<ISound>());
    }

    public IEnumerable<ISound> HoverSounds
    {
        get => _onHoverSounds.ToArray()!;
        set => _onHoverSounds.SetItems(value?.ToArray() ?? Array.Empty<ISound>());
    }

    public IEnumerable<ISound> UnhoverSounds
    {
        get => _onUnhoverSounds.ToArray()!;
        set => _onUnhoverSounds.SetItems(value?.ToArray() ?? Array.Empty<ISound>());
    }


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
    private readonly IAudioEngine _audioEngine;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly SpriteItem _leftPartSprite;
    private readonly SpriteItem _middlePartSprite;
    private readonly SpriteItem _rightPartSprite;
    private readonly TextBox _buttonText;

    private float _buttonLength = 1f;
    private Vector2 _position = Vector2.Zero;
    private float _scale = 1f;
    private RectangleF _buttonArea;

    private bool _isTargeted = false;
    private bool _isEnabled = false;
    private bool _isDisabledOnCLick;
    private DeltaValue<bool> _isHovered;

    private RandomSequence<ISound> _clickSounds = new(Array.Empty<ISound>());
    private RandomSequence<ISound> _onHoverSounds = new(Array.Empty<ISound>());
    private RandomSequence<ISound> _onUnhoverSounds = new(Array.Empty<ISound>());

    private float _volume = 1f;

    private Color _normalButtonColor = Color.White;
    private Color _highlightColor = new Color(173, 255, 110, 255);
    private double _highlightFadeFactor = 0d;


    // Constructors
    public MainMenuBasicButton(IAudioEngine audioEngine,
        IUserInput input,
        ISceneAssetProvider? assetProvider,
        ISpriteAnimation animation,
        GHFontFamily font,
        float buttonLength)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(animation, nameof(animation));
        ArgumentNullException.ThrowIfNull(font, nameof(font));
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));

        if (animation.FrameCount < 3)
        {
            throw new ArgumentException("basic main menu button animation must have at least 3 frames", nameof(animation));
        }

        _input = input;

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

        Length =  buttonLength;
        
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
            new Vector2((_middlePartSprite.Size.X / 2f) - POSITION_MARGIN_OF_ERROR, 0f), aspectRatio);

        Vector2 Right = Middle + GHMath.GetWindowAdjustedVector(
            new Vector2((_middlePartSprite.Size.X / 2f) - POSITION_MARGIN_OF_ERROR, 0f), aspectRatio);

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


        _buttonArea = new(BottomLeft.X, BottomLeft.Y, Dimensions.X, Dimensions.Y);
    }

    private void PlayRandomSound(RandomSequence<ISound> sounds)
    {
        if (sounds.Count == 0)
        {
            return;
        }

        ISound TargetSound = sounds.Get()!;
        ISoundInstance Instance = TargetSound.CreateInstance();

    }

    private void OnButtonTargetUpdate()
    {
        if (_isHovered.Current && !_isHovered.Previous && (_onHoverSounds.Count > 0))
        {
            PlayRandomSound(_onHoverSounds);
        }
    }

    private void OnButtonNotTargetUpdate()
    {

    }

    private void ButtonGenericUpdate(IProgramTime time)
    {
        _highlightFadeFactor = Math.Clamp(
            _highlightFadeFactor + (time.PassedTime.TotalSeconds * (_isTargeted ? 1d : -1d) / COLOR_FADE_DURATION.TotalSeconds),
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

        _isTargeted = IsEnabled && ((_isTargeted && (_input.VirtualMousePositionPrevious == _input.VirtualMousePositionCurrent))
            || IsPositionOverButton(_input.VirtualMousePositionCurrent));

        _isHovered.SetValue(_isTargeted);
        ButtonGenericUpdate(time);
        if (_isTargeted)
        {
            OnButtonTargetUpdate();
        }
        else
        {
            OnButtonNotTargetUpdate();
        }
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        UpdateRenderPositions(renderer.AspectRatio); 
        _leftPartSprite.Render(renderer, time);
        _middlePartSprite.Render(renderer, time);
        _rightPartSprite.Render(renderer, time);
    }
}