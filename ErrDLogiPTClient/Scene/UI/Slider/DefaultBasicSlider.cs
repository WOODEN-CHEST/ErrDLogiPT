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
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class DefaultBasicSlider : IBasicSlider
{
    // Fields.
    public float Length
    {
        get => _length;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid length: {value}", nameof(value));
            }
            if (value < 0f)
            {
                throw new ArgumentException("Length must be >= 0");
            }
            _length = value;
            UpdateSizes();
            _previousRenderAspectRatio = null;
        }
    }
    public float Scale
    {
        get => _scale;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid scale: {value}", nameof(value));
            }
            if (value < 0f)
            {
                throw new ArgumentException("Scale must be >= 0");
            }
            _scale = value;
            UpdateSizes();
        }
    }
    public bool IsTargeted { get; set; } = false;

    public SliderOrientation Orientation
    {
        get => _orientation;
        set
        {
            _orientation = value;
            UpdateOrientation();
            UpdateRotations();
            _previousRenderAspectRatio = null;
        }
    }

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _previousRenderAspectRatio = null;
        }
    }
    public double SliderFactor
    {
        get => _factor;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid slider factor: {value}", nameof(value));
            }
            _factor = Math.Clamp(value, SliderFactorMin, SliderFactorMax);
            UpdateHandle();
        }
    }
    public double? Step
    {
        get => _step;
        set => _step = value;
    }

    public IEnumerable<IPreSampledSound> GrabSounds
    {
        get => _grabSounds!;
        set => _grabSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public IEnumerable<IPreSampledSound> ReleaseSounds
    {
        get => _releaseSounds!;
        set => _releaseSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public IEnumerable<IPreSampledSound> IncrementSounds
    {
        get => _incrementSounds!;
        set => _incrementSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public float Volume { get; set; } = 1f;

    public LogiSoundCategory SoundCategory
    {
        get => _soundCategory;
        set => _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Color NormalColor
    {
        get => _colorCalculator.NormalColor;
        set => _colorCalculator.NormalColor = value;
    }

    public Color HoverColor
    {
        get => _colorCalculator.HoverColor;
        set => _colorCalculator.HoverColor = value;
    }

    public Color GrabColor
    {
        get => _colorCalculator.ClickColor;
        set => _colorCalculator.ClickColor = value;
    }

    public bool IsTextShadowEnabled { get; set; } = true;

    public float TextShadowBrightness
    {
        get => _displayTextShadow.Brightness;
        set => _displayTextShadow.Brightness = value;
    }

    public Func<double, string>? ValueDisplayProvider { get; set; } = null;

    public string? ValueDisplayOverride
    {
        get => _displayTextOverride;
        set
        {
            _displayTextOverride = value;
            if (value != null)
            {
                _displayText.First().Text = value;
                _displayTextShadow.First().Text = value;
            }
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            _trackDetector.IsEnabled = value;
            _handleDetector.IsEnabled = value;
        }
    }

    public bool IsVisible { get; set; } = true;

    public Color TrackColor
    {
        get => _track.Mask;
        set => _track.Mask = value;
    }

    public Color HandleColor
    {
        get => _handle.Mask;
        set => _handle.Mask = value;
    }

    public TimeSpan HoverFadeDuration
    {
        get => _colorCalculator.HoverFadeDuration;
        set => _colorCalculator.HoverFadeDuration = value;
    }

    public TimeSpan GrabFadeDuration
    {
        get => _colorCalculator.ClickFadeDuration;
        set => _colorCalculator.ClickFadeDuration = value;
    }

    public double SliderFactorMax => 1d;
    public double SliderFactorMin => 0d;

    public event EventHandler<BasicSliderGrabEventArgs>? Grab;
    public event EventHandler<BasicSliderReleaseEventArgs>? Release;
    public event EventHandler<BasicSliderFactorChangeEventArgs>? FactorChange;
    public event EventHandler<BasicSliderPlaySoundEventArgs>? PlaySound;


    // Private static fields.
    private const int ANIMATION_FRAME_COUNT = 2;
    private const int ANIMATION_FRAME_INDEX_TRACK = 0;
    private const int ANIMATION_FRAME_INDEX_HANDLE = 1;
    private const float HANDLE_SCALE = 1.5f;
    private const float TEXT_MAX_SIZE_SCALE_Y = 2f;
    private const float TEXT_MAX_OFFSET_SCALE = 2.5f;


    // Private fields.
    private readonly IUserInput _input;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly ILogiSoundEngine _soundEngine;

    private readonly SpriteItem _track;
    private readonly SpriteItem _handle;

    private readonly TextBox _displayText;
    private readonly TextBox _displayTextShadow;
    private string? _displayTextOverride = null;

    private Vector2 _position = Vector2.Zero;

    private SliderOrientation _orientation = SliderOrientation.Horizontal;
    private Vector2 _orientationVector = new Vector2(1f, 0f);
    private float _orientationRotation = 0f;

    private bool _isEnabled = true;

    private ClickDetector _handleDetector;
    private ClickDetector _trackDetector;

    private float _length = 1f;
    private float _scale = 1f;
    private double? _step = null;
    private double _factor = 0d;
    private float _minLocationCoordinate;
    private float _maxLocationCoordinate;

    private RandomSequence<IPreSampledSound> _grabSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _releaseSounds = new(Array.Empty<IPreSampledSound>());
    private RandomSequence<IPreSampledSound> _incrementSounds = new(Array.Empty<IPreSampledSound>());
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;

    private readonly ElementColorCalculator _colorCalculator = new();

    private bool _isGrabbed = false;
    private Vector2? _grabStartLocation = null;

    private float? _previousRenderAspectRatio = null;
    private float _previousInputAreaAspectRatio = 0f;


    // Constructors.
    public DefaultBasicSlider(IUserInput input,
        ILogiSoundEngine soundEngine,
        ISceneAssetProvider assetProvider,
        ISpriteAnimation animation,
        GHFontFamily font)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(soundEngine, nameof(soundEngine));
        ArgumentNullException.ThrowIfNull(animation, nameof(animation));
        ArgumentNullException.ThrowIfNull(assetProvider, nameof(assetProvider));

        _input = input;
        _soundEngine = soundEngine;
        _assetProvider = assetProvider;

        if (animation.FrameCount <  ANIMATION_FRAME_COUNT)
        {
            throw new ArgumentException($"Expected {ANIMATION_FRAME_COUNT} or more frames in animation, " +
                $"got {animation.FrameCount}", nameof(animation));
        }

        _track = new(animation.CreateInstance());
        _handle = new(animation.CreateInstance());
        _track.Animation.FrameIndex = ANIMATION_FRAME_INDEX_TRACK;
        _handle.Animation.FrameIndex = ANIMATION_FRAME_INDEX_HANDLE;
        InitializeSprite(_track);
        InitializeSprite(_handle);

        _displayText = new() { new TextComponent(font) };
        _displayTextShadow = new() { new TextComponent(font) };
        InitializeTextBox(_displayText);
        InitializeTextBox(_displayTextShadow);

        _trackDetector = new(input);
        _handleDetector = new(input);

        UpdateOrientation();
        UpdateRotations();
        UpdateSizes();
    }



    // Private methods.
    private void InitializeSprite(SpriteItem sprite)
    {
        sprite.Origin = new(0.5f, 0.5f);
    }

    private void InitializeTextBox(TextBox textBox)
    {
        textBox.Origin = new(0.5f, 0.5f);
        textBox.FitMethod = TextFitMethod.Resize;
        textBox.IsNewlineAllowed = false;
        textBox.IsSplittingAllowed = false;
    }

    private void UpdateOrientation()
    {
        switch (_orientation)
        {
            case SliderOrientation.Horizontal:
                _orientationVector = new(0f, 1f);
                _orientationRotation = 0f;
                break;

            case SliderOrientation.Vertical:
                _orientationVector = new(1f, 0f);
                _orientationRotation = -MathF.PI / 2f;
                break;

            default:
                throw new InvalidOperationException($"Invalid orientation: {Orientation} ({(int)Orientation})");
        }
    }

    private void UpdateSizes()
    {
        _track.Size = new();

        _track.Size = new(_track.FrameSize.X / _track.FrameSize.Y * _scale * _length, _scale);
        float HandleYSize = _track.Size.Y * HANDLE_SCALE;
        _handle.Size = new(_handle.FrameSize.X / _handle.FrameSize.Y * HandleYSize, HandleYSize);

        Vector2 TextBoxMaxSize = new Vector2(
            _track.Size.X * Math.Max(1f, _input.InputAreaRatio), 
            _track.Size.Y * TEXT_MAX_SIZE_SCALE_Y);

        _displayText.MaxSize = TextBoxMaxSize;
        _displayTextShadow.MaxSize = TextBoxMaxSize;
        _previousRenderAspectRatio = null;
    }

    private void UpdateRotations()
    {
        _track.Rotation = _orientationRotation;
        _handle.Rotation = _orientationRotation;
        _displayText.Rotation = _orientationRotation;
        _displayTextShadow.Rotation = _orientationRotation;
    }

    private void UpdateRenderPositions(float aspectRatio)
    {
        _track.Position = _position;
        _displayText.Position = _position + (Vector2.Rotate(_orientationVector, -MathF.PI / 2f) * TEXT_MAX_OFFSET_SCALE);
        UpdateHandle();
    }

    private void UpdateHandle()
    {
        Vector2 MovementAmount = _orientationVector * (_maxLocationCoordinate - _minLocationCoordinate);
        _handle.Position = _position + (MovementAmount * (float)_factor) - (MovementAmount / 2f);
    }

    private void UpdateFactorFromPosition(Vector2 clickPosition)
    {
        float AreaLength = Vector2.Dot(GHMath.GetWindowAdjustedVector(_track.Size, _input.InputAreaRatio), _orientationVector);
        float CoordinateCurrent = Vector2.Dot(_orientationVector, clickPosition);
        float CoordinateMin = Vector2.Dot(_position, _orientationVector) - (AreaLength / 2f);

        float UnclampedFactor = (AreaLength - CoordinateMin) / AreaLength;
        SliderFactor = Math.Clamp((float)UnclampedFactor, SliderFactorMin, SliderFactorMax);
    }

    private void UpdateClickBounds()
    {
        float AspectRatio = _input.InputAreaRatio;

        Vector2 AdjustedTrackSize = GHMath.GetWindowAdjustedVector(_track.Size, AspectRatio);

        Vector2 RotatedHalfSize = Vector2.Rotate(AdjustedTrackSize / 2f, _orientationRotation);
        Vector2 TrackCorner1 = _track.Position - RotatedHalfSize;
        Vector2 TrackCorner2 = _track.Position + RotatedHalfSize;
        Vector2 TrackDimensions = TrackCorner2 - TrackCorner1;

        
        _trackDetector.ElementBounds = new(TrackCorner1.X, TrackCorner2.Y, TrackDimensions.X, TrackDimensions.Y);
    }

    private void OnHandleHoverStartEvent(object? sender, ClickDetectorHoverStartEventArgs args)
    {
        _colorCalculator.OnHoverStart();
    }

    private void OnHandleHoverEndEvent(object? sender, ClickDetectorHoverEndEventArgs args)
    {
        _colorCalculator.OnHoverEnd();
    }

    private void OnHandleClickStartEvent(object? sender, ClickDetectorClickStartEventArgs args)
    {
        _isGrabbed = true;
    }

    private void OnHandleClickEndEvent(object? sender, ClickDetectorClickEndEventArgs args)
    {
        _isGrabbed = false;
    }

    private void OnTrackClickStartEvent(object? sender, ClickDetectorClickStartEventArgs args)
    {
        if (_isGrabbed)
        {
            return;
        }

        UpdateFactorFromPosition(args.ClickStartLocation);
    }


    // Inherited methods.
    public void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_track);
        _assetProvider.RegisterRenderedItem(_handle);
        _assetProvider.RegisterRenderedItem(_displayText);
        _assetProvider.RegisterRenderedItem(_displayTextShadow);

        _handleDetector.HoverStart += OnHandleHoverStartEvent;
        _handleDetector.HoverEnd += OnHandleHoverEndEvent;
        _handleDetector.ClickStart += OnHandleClickStartEvent;
        _handleDetector.ClickEnd += OnHandleClickEndEvent;
        _trackDetector.ClickStart += OnTrackClickStartEvent;
    }


    public void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_track);
        _assetProvider.UnregisterRenderedItem(_handle);
        _assetProvider.UnregisterRenderedItem(_displayText);
        _assetProvider.UnregisterRenderedItem(_displayTextShadow);

        _handleDetector.HoverStart -= OnHandleHoverStartEvent;
        _handleDetector.HoverEnd -= OnHandleHoverEndEvent;
        _handleDetector.ClickStart -= OnHandleClickStartEvent;
        _handleDetector.ClickEnd -= OnHandleClickEndEvent;
        _trackDetector.ClickStart -= OnTrackClickStartEvent;
    }

    public bool IsPositionOverEntireSlider(Vector2 position)
    {
        return _trackDetector.IsPositionOverClickArea(position);
    }

    public bool IsPositionOverHandle(Vector2 position)
    {
        return _handleDetector.IsPositionOverClickArea(position);
    }

    public bool IsPositionOverTrack(Vector2 position)
    {
        return IsPositionOverHandle(position) || IsPositionOverTrack(position);
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        if ((_previousRenderAspectRatio == null) || (_previousRenderAspectRatio.Value != renderer.AspectRatio))
        {
            _previousRenderAspectRatio = renderer.AspectRatio;
            UpdateRenderPositions(renderer.AspectRatio);
        }

        _track.Render(renderer, time);
        _handle.Render(renderer, time);

        if (IsTextShadowEnabled)
        {
            _displayTextShadow.Render(renderer, time);
        }
        _displayText.Render(renderer, time);
    }

    public void Update(IProgramTime time)
    {
        if (_previousInputAreaAspectRatio != _input.InputAreaRatio)
        {
            _previousInputAreaAspectRatio = _input.InputAreaRatio;
            UpdateClickBounds();
        }

        _handleDetector.Update(time);
        _trackDetector.Update(time);
    }
}