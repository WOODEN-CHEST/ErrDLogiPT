using GHEngine;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public interface ISceneSoundEngine : ITimeUpdatable
{
    // Fields.
    float MasterVolume { get; set; }
    int SoundCount { get; }


    // Methods.
    void StopSounds();
    void StopSounds(SceneSoundCategory category);
    ISceneSoundInstance CreateSoundInstance(IPreSampledSound sound, SceneSoundCategory category);
    void SetCategoryVolume(SceneSoundCategory category, float volume);
    float GetCategoryVolume(SceneSoundCategory category);
    void ScheduleAction(Action action);
}