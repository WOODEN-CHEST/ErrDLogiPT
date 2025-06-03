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

namespace ErrDLogiPTClient.Scene.UI.Button;

public class DefaultBasicButton : IBasicButton
{
    // Fields.
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            _clickDetector.IsEnabled = value;
        }
    }

    public bool IsDisabledOnClick { get; set; } = false;

    public bool IsVisible { get; set; } = true;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _previousRenderAspectRatio = null;
            UpdateButtonInputArea();
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

    public Color ClickColor
    {
        get => _clickColor;
        set
        {
            _clickColor = value;
            UpdateRenderedColors();
        }
    }

    public TimeSpan HoverFadeDuration
    {
        get => _hoverFadeDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Hover fade duration must be >= 0");
            }
            _hoverFadeDuration = value;
        }
    }

    public TimeSpan ClickFadeDuration
    {
        get => _clickFadeDuration;
        set
        {
            if (value.Ticks < 0)
            {
                throw new ArgumentException("Click fade duration must be >= 0");
            }
            _clickFadeDuration = value;
        }
    }

    public string Text
    {
        get => _text.Text;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _text.First().Text = value;
            _textShadow.First().Text = value;
        }
    }

    public Vector2 TextPadding
    {
        get => _textPadding;
        set
        {
            _textPadding = value;
            UpdateButtonInputArea();
        }
    }

    public bool IsTextShadowEnabled { get; set; } = true;

    public float TextShadowBrightness
    {
        get => _textShadowBrightness;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid shadow brightness: {value} (expected [0;1])");
            }
            _textShadowBrightness = Math.Clamp(value, SHADOWN_BRIGHTNESS_MIN, SHADOWN_BRIGHTNESS_MAX);
            UpdateTextShadowColor();
        }
    }

    public Vector2 ShadowOffset
    {
        get => _shadowOffset;
        set => _shadowOffset = value;
    }

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

            UpdateRenderSize();
            UpdateButtonInputArea();
            if (_previousRenderAspectRatio != null)
            {
                UpdateRenderPositions(_previousRenderAspectRatio.Value);
            }
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
            
            UpdateRenderSize();
            UpdateButtonInputArea();
            if (_previousRenderAspectRatio != null)
            {
                UpdateRenderPositions(_previousRenderAspectRatio.Value);
            }
        }
    }

    public bool IsTargeted
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

    public IEnumerable<IPreSampledSound> HoverStartSounds
    {
        get => _hoverStartSounds.ToArray()!;
        set => _hoverStartSounds.SetItems(value?.ToArray() ?? Array.Empty<IPreSampledSound>());
    }

    public IEnumerable<IPreSampledSound> HoverEndSounds
    {
        get => _hoverEndSounds.ToArray()!;
        set => _hoverEndSounds.SetItems(value?.ToArray() ?? Array.Empty<IPreSampledSound>());
    }

    public LogiSoundCategory SoundCategory
    {
        get => _soundCategory;
        set => _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ButtonClickMethod ClickMethod
    {
        get => _clickDetector.ClickMethod;
        set => _clickDetector.ClickMethod = value;
    }

    public IEnumerable<UIElementClickType> DetectedClickTypes
    {
        get => _detectedClickTypes;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _detectedClickTypes.Clear();

            foreach (UIElementClickType ClickType in value)
            {
                _detectedClickTypes.Add(ClickType);
            }
        }
    }

    public Action<BasicButtonMainClickArgs>? MainClickAction { get; set; }
    public Action<BasicButtonMainHoverStartArgs>? MainHoverStartAction { get; set; }
    public Action<BasicButtonMainHoverEndArgs>? MainHoverEndAction { get; set; }

    public RectangleF ButtonBounds => _clickDetector.ElementBounds;

    public event EventHandler<BasicButtonClickStartEventArgs>? ClickStart;
    public event EventHandler<BasicButtonClickEndEventArgs>? ClickEnd;
    public event EventHandler<BasicButtonHoverStartEventArgs>? HoverStart;
    public event EventHandler<BasicButtonHoverEndEventArgs>? HoverEnd;
    public event EventHandler<BasicButtonSoundEventArgs>? PlaySound;
    public event EventHandler<BasicButtonScrollEventArgs>? Scroll;


    // Private static fields.
    private const int FRAME_INDEX_LEFT_PART = 0;
    private const int FRAME_INDEX_MIDDLE_PART = 1;
    private const int FRAME_INDEX_RIGHT_PART = 2;
    private const int FRAME_COUNT = 3;
    private const float POSITION_MARGIN_OF_ERROR = 0.0025f;
    private const double COLOR_FADE_FACTOR_MAX = 1d;
    private const double COLOR_FADE_FACTOR_MIN = 0d;
    private const float SHADOWN_BRIGHTNESS_MIN = 0f;
    private const float SHADOWN_BRIGHTNESS_MAX = 1f;
    private const float TEXT_COLOR_MULTIPLIER = 1.5f;


    // Private fields.
    private readonly IUserInput _input;
    private readonly ILogiSoundEngine _soundEngine;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly SpriteItem _leftPartSprite;
    private readonly SpriteItem _middlePartSprite;
    private readonly SpriteItem _rightPartSprite;
    private readonly HashSet<UIElementClickType> _detectedClickTypes = new() { UIElementClickType.Left };

    private readonly TextBox _text;
    private readonly TextBox _textShadow;
    private float _textShadowBrightness = 0.25f;

    private float _buttonLength = 1f;
    private Vector2 _position = Vector2.Zero;
    private float _scale = 1f;
    private Vector2 _textPadding = new(0.125f);
    private Vector2 _shadowOffset = new(0.08f);
    private bool _isEnabled = true;

    private bool _isHovered = false;
    private bool _wasClickStarted = false;
    private bool _isClickColorFullyActive = false;

    private RandomSequence<IPreSampledSound> _clickSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _hoverStartSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _hoverEndSounds = new(Array.Empty<IPreSampledSound>());
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;
    private float _volume = 1f;

    private Color _normalButtonColor = Color.White;
    private Color _highlightColor = Color.White;
    private Color _clickColor = Color.White;
    private double _highlightFadeFactor = 0d;
    private double _clickFadeFactor = 0d;
    private TimeSpan _hoverFadeDuration = TimeSpan.Zero;
    private TimeSpan _clickFadeDuration = TimeSpan.Zero;

    /* Ratios are cached so that calculations (specifically the text box updating) wouldn't happen every second.
     * This may not be too bad for the sprites, but updating the text box is rather expensive. */
    private float?_previousRenderAspectRatio = null;
    private float _previousInputAspectRatio = 0f;

    private readonly ClickDetector _clickDetector;


    // Constructors
    public DefaultBasicButton(ILogiSoundEngine soundEngine,
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
        _clickDetector = new(input) { IsEnabled = true };
        _soundEngine = soundEngine;

        if (animation.FrameCount < FRAME_COUNT)
        {
            throw new ArgumentException(
                $"basic main menu button animation must have at least {FRAME_COUNT} frames", nameof(animation));
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

        _text = new TextBox() { new(font, string.Empty) };
        _textShadow = new TextBox() { new(font, string.Empty) };
        InitializeTextBox(_text);
        InitializeTextBox(_textShadow);

        UpdateRenderSize();
        UpdateButtonInputArea();
        UpdateRenderedColors();
        UpdateTextShadowColor();
    }


    // Private methods.
    private void InitializeTextBox(TextBox textBox)
    {
        textBox.Origin = new(0.5f, 0.5f);
        textBox.FitMethod = TextFitMethod.Resize;
        textBox.Alignment = TextAlignOption.Center;
        textBox.IsSplittingAllowed = false;
    }

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

        _text.Position = Positions.Middle;
        _textShadow.Position = Positions.Middle + GHMath.GetWindowAdjustedVector(_shadowOffset, windowAspectRatio) * _scale;

        _previousRenderAspectRatio = windowAspectRatio;
    }

    private void UpdateRenderSize()
    {
        Vector2 MiddleFrameSize = _middlePartSprite.FrameSize;
        _middlePartSprite.Size = new Vector2(MiddleFrameSize.X / MiddleFrameSize.Y * Length, 1f) * _scale;

        _leftPartSprite.Size = new(_leftPartSprite.FrameSize.X / _leftPartSprite.FrameSize.Y
            * _middlePartSprite.Size.Y, _middlePartSprite.Size.Y);

        _rightPartSprite.Size = new(_rightPartSprite.FrameSize.X / _rightPartSprite.FrameSize.Y
            * _middlePartSprite.Size.Y, _middlePartSprite.Size.Y);
    }

    private void UpdateButtonInputArea()
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

        /* There is a bug here where if the window becomes super wide, the text renders the wrong size (too large).
         * Couldn't fix it. */
        Vector2 RelativeTextPadding = Dimensions * TextPadding;
        Vector2 MaxDrawSize = new Vector2(Dimensions.X - RelativeTextPadding.X, Dimensions.Y - RelativeTextPadding.Y)
            * new Vector2(Math.Max(1f, InputRatio), 1f);
        _text.MaxSize = MaxDrawSize;
        _textShadow.MaxSize = MaxDrawSize;

        _previousInputAspectRatio = InputRatio;
    }

    private void UpdateTextShadowColor()
    {
        _textShadow.Brightness = _textShadowBrightness;
    }

    private void OnClickStartEvent(object? sender, ClickDetectorClickStartEventArgs args)
    {
        if (!_detectedClickTypes.Contains(args.ClickType))
        {
            return;
        }

        BasicButtonClickStartEventArgs EventArgs = new(this, args.ClickType, args.ClickStartLocation);
        ClickStart?.Invoke(this, EventArgs);

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
        }

        _wasClickStarted = true;
        _isClickColorFullyActive = true;
        EventArgs.ExecuteActions();
    }

    private void OnClickEndEvent(object? sender, ClickDetectorClickEndEventArgs args)
    {
        bool IsFullClickValid = _wasClickStarted && args.WasClickedInBounds;
        _wasClickStarted = false;
        _isClickColorFullyActive = false;

        if (ClickMethod == ButtonClickMethod.ActivateOnFullClick && !IsFullClickValid || !_detectedClickTypes.Contains(args.ClickType))
        {
            return;
        }

        IPreSampledSound? ClickSound = GetRandomSound(_clickSounds);

        BasicButtonClickEndEventArgs EventArgs = new(this,
            args.ClickType,
            args.ClickStartLocation, 
            args.ClickEndLocation, 
            args.ClickDuration)
        {
            Sound = ClickSound
        };

        ClickEnd?.Invoke(this, EventArgs);

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
        }

        if (EventArgs.Sound != null)
        {
            PlayUISound(UISoundOrigin.Click, EventArgs.Sound);
        }

        _clickFadeFactor = 1d;
        if (IsDisabledOnClick)
        {
            IsEnabled = false;
        }
        MainClickAction?.Invoke(new(this, EventArgs.ClickType, EventArgs.ClickStartLocation,
            EventArgs.ClickEndLocation, EventArgs.ClickDuration));
        EventArgs.ExecuteActions();
    }

    private void OnHoverStartEvent(object? sender, ClickDetectorHoverStartEventArgs args)
    {
        BasicButtonHoverStartEventArgs EventArgs = new(this)
        {
            Sound = GetRandomSound(_hoverStartSounds)
        };
        HoverStart?.Invoke(this, EventArgs);
        

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
            
        }

        if (EventArgs.Sound != null)
        {
            PlayUISound(UISoundOrigin.HoverStart, EventArgs.Sound);
        }

        _isHovered = true;
        _isClickColorFullyActive = _wasClickStarted;
        MainHoverStartAction?.Invoke(new(this));
        EventArgs.ExecuteActions();
    }

    private void OnHoverEndEvent(object? sender, ClickDetectorHoverEndEventArgs args)
    {
        BasicButtonHoverEndEventArgs EventArgs = new(this)
        {
            Sound = GetRandomSound(_hoverStartSounds)
        };
        HoverEnd?.Invoke(this, EventArgs);
        
        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
        }

        if (EventArgs.Sound != null)
        {
            PlayUISound(UISoundOrigin.HoverEnd, EventArgs.Sound);
        }

        _isClickColorFullyActive = false;
        _isHovered = false;
        MainHoverEndAction?.Invoke(new(this));
        EventArgs.ExecuteActions();
    }

    private void OnScrollEvent(object? sender, ClickDetectorScrollEventArgs args)
    {
        BasicButtonScrollEventArgs EventArgs = new(this, args.ScrollAmount);
        Scroll?.Invoke(this, EventArgs);
        EventArgs.ExecuteActions();
    }

    private IPreSampledSound? GetRandomSound(RandomSequence<IPreSampledSound> sounds)
    {
        return sounds.Count == 0 ? null : sounds.Get();
    }

    private void PlayUISound(UISoundOrigin origin, IPreSampledSound sound)
    {
        ILogiSoundInstance SoundInstance = _soundEngine.CreateSoundInstance(sound, SoundCategory);
        SoundInstance.Volume = Volume;

        BasicButtonSoundEventArgs EventArgs = new(this, UISoundOrigin.Click, SoundInstance);
        PlaySound?.Invoke(this, EventArgs);

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
        }

        if (EventArgs.Sound != null)
        {
            _soundEngine.AddSoundInstance(EventArgs.Sound);
        }

        EventArgs.ExecuteActions();
    }

    private void ButtonGenericUpdate(IProgramTime time)
    {
        /* Snapping to max or min fade factors if the timespan is short is so that there is no division by zero. */
        double PassedTimeSeconds = time.PassedTime.TotalSeconds;
  
        if (_hoverFadeDuration.Ticks <= time.PassedTime.Ticks)
        {
            _highlightFadeFactor = _isHovered ? COLOR_FADE_FACTOR_MAX : COLOR_FADE_FACTOR_MIN;
        }
        else
        {
            _highlightFadeFactor = Math.Clamp(
                _highlightFadeFactor + PassedTimeSeconds * (_isHovered ? 1d : -1d) / HoverFadeDuration.TotalSeconds,
                COLOR_FADE_FACTOR_MIN,
                COLOR_FADE_FACTOR_MAX);
        }

        if (_isClickColorFullyActive)
        {
            _clickFadeFactor = COLOR_FADE_FACTOR_MAX;
        }
        else if (_clickFadeDuration.Ticks <= time.PassedTime.Ticks)
        {
            _clickFadeFactor = COLOR_FADE_FACTOR_MIN;
        }
        else
        {
            _clickFadeFactor = Math.Clamp(
                _clickFadeFactor - PassedTimeSeconds / ClickFadeDuration.TotalSeconds,
                COLOR_FADE_FACTOR_MIN,
                COLOR_FADE_FACTOR_MAX);
        }

        UpdateRenderedColors();
    }

    private void UpdateRenderedColors()
    {
        FloatColor ColorStage1 = FloatColor.InterpolateRGB(_normalButtonColor, _highlightColor, (float)_highlightFadeFactor);
        FloatColor ColorStage2 = FloatColor.InterpolateRGB(ColorStage1, _clickColor, (float)_clickFadeFactor);
        Color FinalSpriteColor = ColorStage2;
        Color FinalTextColor = ColorStage2 * TEXT_COLOR_MULTIPLIER;

        _leftPartSprite.Mask = FinalSpriteColor;
        _middlePartSprite.Mask = FinalSpriteColor;
        _rightPartSprite.Mask = FinalSpriteColor;

        _text.Mask = FinalTextColor;
        _textShadow.Mask = FinalSpriteColor;
    }


    // Inherited methods.
    public void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_text);
        _assetProvider.RegisterRenderedItem(_leftPartSprite);
        _assetProvider.RegisterRenderedItem(_middlePartSprite);
        _assetProvider.RegisterRenderedItem(_rightPartSprite);

        _clickDetector.ClickStart += OnClickStartEvent;
        _clickDetector.ClickEnd += OnClickEndEvent;
        _clickDetector.HoverStart += OnHoverStartEvent;
        _clickDetector.HoverEnd += OnHoverEndEvent;
        _clickDetector.Scroll += OnScrollEvent;
    }

    public void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_text);
        _assetProvider.UnregisterRenderedItem(_leftPartSprite);
        _assetProvider.UnregisterRenderedItem(_middlePartSprite);
        _assetProvider.UnregisterRenderedItem(_rightPartSprite);

        _clickDetector.ClickStart -= OnClickStartEvent;
        _clickDetector.ClickEnd -= OnClickEndEvent;
        _clickDetector.HoverStart -= OnHoverStartEvent;
        _clickDetector.HoverEnd -= OnHoverEndEvent;
        _clickDetector.Scroll -= OnScrollEvent;
    }

    public bool IsPositionOverButton(Vector2 position)
    {
        return _clickDetector.IsPositionOverClickArea(position);
    }


    public void Update(IProgramTime time)
    {
        _clickDetector.Update(time);

        if (_previousInputAspectRatio != _input.InputAreaRatio)
        {
            UpdateButtonInputArea();
        }
        
        ButtonGenericUpdate(time);
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        if (_previousRenderAspectRatio == null || _previousRenderAspectRatio.Value != renderer.AspectRatio)
        {
            UpdateRenderPositions(renderer.AspectRatio);
        }
        
        _leftPartSprite.Render(renderer, time);
        _rightPartSprite.Render(renderer, time);
        _middlePartSprite.Render(renderer, time);

        if (IsTextShadowEnabled)
        {
            _textShadow.Render(renderer, time);
        }
        _text.Render(renderer, time);
    }
}