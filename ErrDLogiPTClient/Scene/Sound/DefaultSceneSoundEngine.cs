using GHEngine;
using GHEngine.Audio;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class DefaultSceneSoundEngine : ISceneSoundEngine
{
    // Fields.
    public float MasterVolume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SoundCount => _sounds.Count;


    // Protected fields.
    protected IEnumerable<SceneSoundCategory> UsedCategories => _volumeByCategory.Keys;


    // Private fields.
    protected readonly Dictionary<SceneSoundCategory, float> _volumeByCategory = new();
    private HashSet<ISceneSoundInstance> _sounds = new();
    private readonly IAudioEngine _engine;

    private float _masterVolume = 1f;


    // Constructors.
    public DefaultSceneSoundEngine(IAudioEngine engine)
    {
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
    }


    // Inherited methods.
    public virtual ISceneSoundInstance CreateSoundInstance(IPreSampledSound sound, SceneSoundCategory category)
    {
        ISceneSoundInstance Instance = new DefaultSceneSoundInstance();


        return Instance;
    }

    public virtual float GetCategoryVolume(SceneSoundCategory category)
    {
        throw new NotImplementedException();
    }

    public virtual void SetCategoryVolume(SceneSoundCategory category, float volume)
    {
        throw new NotImplementedException();
    }

    public virtual void StopSounds()
    {
        throw new NotImplementedException();
    }

    public virtual void StopSounds(SceneSoundCategory category)
    {
        throw new NotImplementedException();
    }

    public virtual void Update(IProgramTime time)
    {
        throw new NotImplementedException();
    }
}