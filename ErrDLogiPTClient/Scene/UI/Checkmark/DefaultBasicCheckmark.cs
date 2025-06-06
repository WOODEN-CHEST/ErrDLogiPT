using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Audio.Source;
using GHEngine.Collections;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Checkmark;

public class DefaultBasicCheckmark : IBasicCheckmark
{
    // Fields.
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdatePositions();
        }
    }

    public bool IsTargeted
    {
        get => _clickDetector.IsTargeted;
        set => _clickDetector.IsTargeted = value;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            UpdateIsChecked();
        }
    }

    public RectangleF CheckmarkBounds => _clickDetector.ElementBounds;
    
    public IEnumerable<IPreSampledSound> CheckSounds
    {
        get => _checkSounds!;
        set => _checkSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public IEnumerable<IPreSampledSound> UncheckSounds
    {
        get => _uncheckSounds!;
        set => _uncheckSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public IEnumerable<IPreSampledSound> HoverStartSounds
    {
        get => _hoverStartSounds!;
        set => _hoverStartSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public IEnumerable<IPreSampledSound> HoverEndSounds
    {
        get => _hoverEndSounds!;
        set => _hoverEndSounds.SetItems(value?.ToArray() ?? throw new ArgumentNullException(nameof(value)));
    }

    public float Volume { get; set; } = 1f;

    public LogiSoundCategory SoundCategory
    {
        get => _soundCategory;
        set => _soundCategory = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ElementClickMethod ClickMethod
    {
        get => _clickDetector.ClickMethod;
        set => _clickDetector.ClickMethod = value;
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
    public Color ClickColor
    {
        get => _colorCalculator.ClickColor;
        set => _colorCalculator.ClickColor = value;
    }

    public Color CheckmarkColor
    {
        get => _checkmark.Mask;
        set => _checkmark.Mask = value;
    }

    public TimeSpan HoverFadeDuration
    {
        get => _colorCalculator.HoverFadeDuration;
        set => _colorCalculator.HoverFadeDuration = value;
    }

    public TimeSpan ClickFadeDuration
    {
        get => _colorCalculator.ClickFadeDuration;
        set => _colorCalculator.ClickFadeDuration = value;
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
            UpdateProperties();
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled; 
        set
        {
            _isEnabled = value;
            _clickDetector.IsEnabled = value;
        }
    }

    public bool IsVisible { get; set; } = true;

    public event EventHandler<BasicCheckmarkCheckEventArgs>? CheckChange;
    public event EventHandler<BasicCheckmarkPlaySoundEventArgs>? PlaySound;


    // Private static fields
    private const int ANIMATION_FRAME_COUNT = 2;
    private const int ANIMATION_FRAME_INDEX_BOX = 0;
    private const int ANIMATION_FRAME_INDEX_CHECKMARK = 1;
    private const float CHECKMARK_SCALE = 0.8f;



    // Private fields.
    private readonly IUserInput _input;
    private readonly ILogiSoundEngine _soundEngine;
    private readonly ISceneAssetProvider _assetProvider;
    private readonly ClickDetector _clickDetector;

    private readonly SpriteItem _box;
    private readonly SpriteItem _checkmark;

    private readonly RandomSequence<IPreSampledSound> _hoverStartSounds = new(Array.Empty<IPreSampledSound>());
    private readonly RandomSequence<IPreSampledSound> _hoverEndSounds = new(Array.Empty<IPreSampledSound>());
    private readonly RandomSequence<IPreSampledSound> _checkSounds = new(Array.Empty<IPreSampledSound>());
    private readonly RandomSequence<IPreSampledSound> _uncheckSounds = new(Array.Empty<IPreSampledSound>());
    private float _volume = 1f;
    private LogiSoundCategory _soundCategory = LogiSoundCategory.UI;

    private Vector2 _position = Vector2.Zero;
    private float _scale;

    private ElementColorCalculator _colorCalculator = new();

    private bool _isChecked = false;
    private bool _isEnabled = true;

    private float _previousInputAspectRatio = 0f;



    // Constructors.
    public DefaultBasicCheckmark(IUserInput input,
        ILogiSoundEngine soundEngine,
        ISpriteAnimation animation,
        ISceneAssetProvider assetProvider)
    {
        ArgumentNullException.ThrowIfNull(input, nameof(input));
        ArgumentNullException.ThrowIfNull(soundEngine, nameof(soundEngine));
        ArgumentNullException.ThrowIfNull(animation, nameof(animation));
        ArgumentNullException.ThrowIfNull(assetProvider, nameof(assetProvider));
        if (animation.FrameCount < ANIMATION_FRAME_COUNT)
        {
            throw new ArgumentNullException($"Animation must have at least {ANIMATION_FRAME_COUNT} frames", nameof(animation));
        }

        _input = input;
        _soundEngine = soundEngine;
        _assetProvider = assetProvider;

        _clickDetector = new(_input);

        _box = new(animation.CreateInstance());
        _checkmark = new(animation.CreateInstance());
        _box.Animation.FrameIndex = ANIMATION_FRAME_INDEX_BOX;
        _checkmark.Animation.FrameIndex = ANIMATION_FRAME_INDEX_CHECKMARK;
        InitSpriteItem(_box);
        InitSpriteItem(_checkmark);
    }


    // Private methods.
    private void InitSpriteItem(SpriteItem item)
    {
        item.Origin = new Vector2(0.5f, 0.5f);
    }

    private void UpdatePositions()
    {
        _box.Position = Position;
        _checkmark.Position = Position;
    }

    private void UpdateProperties()
    {
        _box.Size = new Vector2(_box.FrameSize.X / _box.FrameSize.Y * _scale, _scale);
        _checkmark.Size = new Vector2(_checkmark.FrameSize.X / _checkmark.FrameSize.Y * _scale, _scale) * CHECKMARK_SCALE;

        Vector2 CornerMin = _box.Position - GHMath.GetWindowAdjustedVector(_box.Size / 2f, _input.InputAreaRatio);
        Vector2 CornerMax = _box.Position + GHMath.GetWindowAdjustedVector(_box.Size / 2f, _input.InputAreaRatio);
        Vector2 Dimensions = CornerMax - CornerMin;

        _clickDetector.ElementBounds = new(CornerMin.X, CornerMin.Y, Dimensions.X, Dimensions.Y);
        _previousInputAspectRatio = _input.InputAreaRatio;
    }

    private void UpdateIsChecked()
    {
        _checkmark.IsVisible = IsChecked;
    }

    private void OnClickStartEvent(object? sender, ClickDetectorClickStartEventArgs args)
    {
        _colorCalculator.OnClickStart();
    }

    private void OnClickEndEvent(object? sender, ClickDetectorClickEndEventArgs args)
    {
        if (!args.WasClickedInBounds)
        {
            return;
        }

        BasicCheckmarkCheckEventArgs EventArgs = new(this, !IsChecked);
        CheckChange?.Invoke(this, EventArgs);

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return;
        }

        IsChecked = EventArgs.IsChecked;
        _colorCalculator.OnClickEnd();
        PlayCheckSound(EventArgs.IsChecked);
        EventArgs.ExecuteActions();
    }

    private void OnHoverStartEvent(object? sender, ClickDetectorHoverStartEventArgs args)
    {
        _colorCalculator.OnHoverStart();
        PlayHoverSound(true);
    }

    private void OnHoverEndEvent(object? sender, ClickDetectorHoverEndEventArgs args)
    {
        _colorCalculator.OnHoverEnd();
        PlayHoverSound(false);
    }

    private void PlayCheckSound(bool isChecked)
    {
        RandomSequence<IPreSampledSound> SoundBank;
        BasicCheckmarkSoundOrigin Origin;

        if (isChecked)
        {
            SoundBank = _checkSounds;
            Origin = BasicCheckmarkSoundOrigin.Check;
        }
        else
        {
            SoundBank = _uncheckSounds;
            Origin = BasicCheckmarkSoundOrigin.Uncheck;
        }
        
        PlayGenericSound(SoundBank, Origin);
    }

    private void PlayHoverSound(bool isHovering)
    {
        RandomSequence<IPreSampledSound> SoundBank;
        BasicCheckmarkSoundOrigin Origin;

        if (isHovering)
        {
            SoundBank = _hoverStartSounds;
            Origin = BasicCheckmarkSoundOrigin.HoverStart;
        }
        else
        {
            SoundBank = _hoverEndSounds;
            Origin = BasicCheckmarkSoundOrigin.HoverEnd;
        }

        PlayGenericSound(SoundBank, Origin);
    }

    private void PlayGenericSound(RandomSequence<IPreSampledSound> soundBank, BasicCheckmarkSoundOrigin origin)
    {
        if (soundBank.Count == 0)
        {
            return;
        }

        ILogiSoundInstance Sound = _soundEngine.CreateSoundInstance(soundBank.Get()!, SoundCategory);
        Sound.Volume = Volume;
        BasicCheckmarkPlaySoundEventArgs EventArgs = new(this, Sound, origin);
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


    // Inherited methods.
    public void Initialize()
    {
        _assetProvider.RegisterRenderedItem(_box);
        _assetProvider.RegisterRenderedItem(_checkmark);

        _clickDetector.ClickStart += OnClickStartEvent;
        _clickDetector.ClickEnd += OnClickEndEvent;
        _clickDetector.HoverStart += OnHoverStartEvent;
        _clickDetector.HoverEnd += OnHoverEndEvent;
    }

    public void Deinitialize()
    {
        _assetProvider.UnregisterRenderedItem(_box);
        _assetProvider.UnregisterRenderedItem(_checkmark);

        _clickDetector.ClickStart -= OnClickStartEvent;
        _clickDetector.ClickEnd -= OnClickEndEvent;
        _clickDetector.HoverStart -= OnHoverStartEvent;
        _clickDetector.HoverEnd -= OnHoverEndEvent;
    }

    public bool IsPositionOverCheckmark(Vector2 position)
    {
        return _clickDetector.IsPositionOverClickArea(position);
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (!IsVisible)
        {
            return;
        }

        _box.Render(renderer, time);
        if (IsChecked)
        {
            _checkmark.Render(renderer, time);
        }
    }

    public void Update(IProgramTime time)
    {
        if (_previousInputAspectRatio != _input.InputAreaRatio)
        {
            UpdateProperties();
        }
        
        _clickDetector.Update(time);
        _colorCalculator.Update(time);
        _box.Mask = _colorCalculator.FinalColor;
    }
}