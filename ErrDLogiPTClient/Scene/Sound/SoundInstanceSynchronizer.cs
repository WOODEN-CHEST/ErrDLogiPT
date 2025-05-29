using GHEngine.Audio;
using GHEngine.Audio.Modifier;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class SoundInstanceSynchronizer
{
    // Methods.
    public void SynchronizeSound(IPreSampledSoundInstance sound, SoundPropertySnapshot dataSnapshot)
    {
        sound.Sampler.Volume = dataSnapshot.Volume;
        sound.Sampler.CustomSampleRate = dataSnapshot.CustomSampleRate;
        sound.Sampler.SampleSpeed = dataSnapshot.Speed;
        sound.IsLooped = dataSnapshot.IsLooped;
        sound.State = ConvertSoundState(dataSnapshot.State);

        if (dataSnapshot.NewPosition != null)
        {
            sound.Position = dataSnapshot.NewPosition.Value;
        }

        ISoundModifier[] ModifierSnapshot = sound.Modifiers;
        EnsureLowPass(sound, dataSnapshot, ModifierSnapshot);
        EnsureHighPass(sound, dataSnapshot, ModifierSnapshot);
        EnsurePan(sound, dataSnapshot, ModifierSnapshot);
    }


    // Private methods.
    private SoundInstanceState ConvertSoundState(LogiSoundState state)
    {
        return state switch
        {
            LogiSoundState.Playing => SoundInstanceState.Playing,
            LogiSoundState.Stopped => SoundInstanceState.Stopped,
            _ => throw new ArgumentNullException($"Invalid state: {state} ({(int)state})")
        };
    }

    private T? GetModifier<T>(ISoundModifier[] modifierSnapshot) where T : class, ISoundModifier
    {
        foreach (ISoundModifier Modifier in modifierSnapshot)
        {
            if (Modifier is T CastModifier)
            {
                return CastModifier;
            }
        }
        return null;
    }

    private T GetOrAddModifier<T>(IPreSampledSoundInstance sound,
        ISoundModifier[] modifierSnapshot,
        Func<T> modifierCreator)
        where T : class, ISoundModifier
    {
        T? Modifier = GetModifier<T>(modifierSnapshot);
        if (Modifier != null)
        {
            return Modifier;
        }

        Modifier = modifierCreator.Invoke();
        sound.AddModifier(Modifier);
        return Modifier;
    }

    private void RemoveModifier<T>(IPreSampledSoundInstance sound, 
        ISoundModifier[] modifierSnapshot,
        Predicate<T>? extraPredicate)
        where T : class, ISoundModifier
    {
        T? Modifier = GetModifier<T>(modifierSnapshot);
        if ((Modifier != null) && (extraPredicate?.Invoke(Modifier) ?? true))
        {
            sound.RemoveModifier(Modifier);
        }
    }

    private void EnsureLowPass(IPreSampledSoundInstance sound, 
        SoundPropertySnapshot dataSnapshot,
        ISoundModifier[] modifierSnapshot)
    {
        if (dataSnapshot.LowPassFrequency == null)
        {
            RemoveModifier<BiQuadSoundModifier>(sound, modifierSnapshot, modifier => modifier.PassType == BiQuadPassType.Low);
        }
        else
        {
            BiQuadSoundModifier Modifier = GetOrAddModifier(sound, modifierSnapshot, 
                () => new BiQuadSoundModifier() { PassType = BiQuadPassType.Low });

            Modifier.Frequency = dataSnapshot.LowPassFrequency.Value;
        }
    }

    private void EnsureHighPass(IPreSampledSoundInstance sound,
    SoundPropertySnapshot dataSnapshot,
    ISoundModifier[] modifierSnapshot)
    {
        if (dataSnapshot.HighPassFrequency == null)
        {
            RemoveModifier<BiQuadSoundModifier>(sound, modifierSnapshot, modifier => modifier.PassType == BiQuadPassType.High);
        }
        else
        {
            BiQuadSoundModifier Modifier = GetOrAddModifier(sound, modifierSnapshot,
                () => new BiQuadSoundModifier() { PassType = BiQuadPassType.High });

            Modifier.Frequency = dataSnapshot.HighPassFrequency.Value;
        }
    }

    private void EnsurePan(IPreSampledSoundInstance sound,
    SoundPropertySnapshot dataSnapshot,
    ISoundModifier[] modifierSnapshot)
    {
        float MARGIN_OF_ERROR = 0.001f;
        if (Math.Abs(dataSnapshot.Pan) <= MARGIN_OF_ERROR)
        {
            RemoveModifier<PanSoundModifier>(sound, modifierSnapshot, null);
        }
        else
        {
            PanSoundModifier Modifier = GetOrAddModifier(sound, modifierSnapshot, () => new PanSoundModifier());
            Modifier.Pan = dataSnapshot.Pan;
        }
    }
}