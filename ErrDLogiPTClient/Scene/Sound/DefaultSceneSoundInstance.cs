using GHEngine.Audio.Modifier;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class DefaultSceneSoundInstance : ISceneSoundInstance
{
    // Fields.
    public bool IsUpdateRequired { get; private set; }
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

    public TimeSpan Position { get; set; }

    public IPreSampledSoundInstance WrappedSoundInstance { get; private init; }


    // Private fields.
    private readonly ISceneSoundEngine _engine;

    private float _volume = 1f;
    private float? _customSampleRate = null;
    private double _speed = 1d;
    private float? _lowPassFrequency = null;
    private float? _highPassFrequency = null;
    private float _pan = 0f;


    // Constructors.
    public DefaultSceneSoundInstance(ISceneSoundEngine engine, IPreSampledSoundInstance wrappedSound)
    {
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        WrappedSoundInstance = wrappedSound ?? throw new ArgumentNullException(nameof(wrappedSound));
    }


    // Inherited methods.
    public void Continue()
    {

    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void SyncProperties()
    {
        IsUpdateRequired = false;
    }
}