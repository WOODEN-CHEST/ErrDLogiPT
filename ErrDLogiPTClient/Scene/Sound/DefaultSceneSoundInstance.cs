using GHEngine.Audio.Modifier;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace ErrDLogiPTClient.Scene.Sound;

public class DefaultSceneSoundInstance : ILogiSoundInstance
{
    // Fields.
    public bool IsUpdateRequired
    {
        get => _isUpdateRequired;
        set
        {
            if (_isUpdateRequired = value)
            {
                return;
            }
            _isUpdateRequired = value;
            SoundDataUpdate?.Invoke(this, new(this));
        }
    }
    public TimeSpan Duration => WrappedSoundInstance.Sound.Duration;

    public float Volume
    {
        get => _volume;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid volume: {value}", nameof(value));
            }
            if (_volume == value)
            {
                return;
            }
            _volume = value;
            IsUpdateRequired = true;
        }
    }

    public float? CustomSampleRate
    {
        get => _customSampleRate;
        set
        {
            if (value.HasValue && (float.IsNaN(value.Value) || float.IsInfinity(value.Value) || (value < 0f)))
            {
                throw new ArgumentException($"Invalid custom sample rate: {value}", nameof(value));
            }
            if (_customSampleRate == value)
            {
                return;
            }
            _customSampleRate = value;
            IsUpdateRequired = true;
        }
    }

    public double Speed
    {
        get => _speed;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid speed: {value}", nameof(value));
            }
            if (_speed == value)
            {
                return;
            }
            _speed = value;
            IsUpdateRequired = true;
        }
    }


    public float? LowPassFrequency
    {
        get => _lowPassFrequency;
        set
        {
            if (value.HasValue && (float.IsNaN(value.Value) || float.IsInfinity(value.Value) || (value < 0f)))
            {
                throw new ArgumentException($"Invalid low pass frequency: {value}", nameof(value));
            }
            if (_lowPassFrequency == value)
            {
                return;
            }
            _lowPassFrequency = value;
            IsUpdateRequired = true;
        }
    }

    public float? HighPassFrequency
    {
        get => _highPassFrequency;
        set
        {
            if (value.HasValue && (float.IsNaN(value.Value) || float.IsInfinity(value.Value) || (value < 0f)))
            {
                throw new ArgumentException($"Invalid high pass frequency: {value}", nameof(value));
            }
            if (_highPassFrequency == value)
            {
                return;
            }
            _highPassFrequency = value;
            IsUpdateRequired = true;
        }
    }

    public float Pan
    {
        get => _pan;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid pan: {value}", nameof(value));
            }
            if (_pan == value)
            {
                return;
            }
            _pan = Math.Clamp(value, PanSoundModifier.PAN_LEFT, PanSoundModifier.PAN_RIGHT);
            IsUpdateRequired = true;
        }
    }

    public TimeSpan Position
    {
        get => _requestedPosition ?? _syncedPosition;
        set
        {
            _requestedPosition = value;
            IsUpdateRequired = true;
        }
    }
    public IPreSampledSoundInstance WrappedSoundInstance { get; private init; }

    public SceneSoundState State
    {
        get => _state;
        set
        {
            if (_state == value)
            {
                return;
            }

            _state = value;
            IsUpdateRequired = true;
        }
    }

    public bool IsLooped
    {
        get => _isLooped;
        set
        {
            if (value == _isLooped)
            {
                return;
            }

            _isLooped = value;
            IsUpdateRequired = true;
        }
    }

    public bool IsPositionChangeRequested => _requestedPosition != null;

    public LogiSoundCategory Category
    {
        get => _category;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            if (_category ==  value)
            {
                return;
            }
            _category = value;
            IsUpdateRequired = true;
        }
    }

    public event EventHandler<LogiSoundEventArgs>? SoundFinish;
    public event EventHandler<LogiSoundEventArgs>? SoundLoop;
    public event EventHandler<LogiSoundEventArgs>? SoundDataUpdate;


    // Private fields.
    private bool _isLooped = false;
    private float _volume = 1f;
    private float? _customSampleRate = null;
    private double _speed = 1d;
    private float? _lowPassFrequency = null;
    private float? _highPassFrequency = null;
    private float _pan = 0f;
    private TimeSpan? _requestedPosition = null;
    private TimeSpan _syncedPosition = TimeSpan.Zero;
    private SceneSoundState _state = SceneSoundState.Playing;
    private LogiSoundCategory _category;
    private bool _isUpdateRequired = false;


    // Constructors.
    public DefaultSceneSoundInstance(IPreSampledSoundInstance wrappedSound, LogiSoundCategory category)
    {
        WrappedSoundInstance = wrappedSound ?? throw new ArgumentNullException(nameof(wrappedSound));
        _category = category;
    }


    // Inherited methods.
    public void SyncWrappedProperties(TimeSpan newPosition)
    {
        _requestedPosition = null;
        _syncedPosition = newPosition;
        IsUpdateRequired = false;
    }

    public void InvokeLoopEvent()
    {
        SoundLoop?.Invoke(this, new(this));
    }

    public void InvokeFinishEvent()
    {
        SoundFinish?.Invoke(this, new(this));
    }
}