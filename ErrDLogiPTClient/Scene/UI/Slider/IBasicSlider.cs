using ErrDLogiPTClient.Scene.Sound;
using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public interface IBasicSlider : IUIElement
{
    // Fields.
    float Length { get; set; }
    float Scale { get; set; }
    bool IsTargeted { get; set; }
    SliderOrientation Orientation { get; set; }
    Vector2 Position { get; set; }
    double SliderFactorMax { get; }
    double SliderFactorMin { get; }
    double SliderFactor { get; set; }
    double? Step { get; set; }
    IEnumerable<IPreSampledSound> GrabSounds { get; set; }
    IEnumerable<IPreSampledSound> ReleaseSounds { get; set; }
    IEnumerable<IPreSampledSound> IncrementSounds { get; set; }
    float Volume { get; set; }
    LogiSoundCategory SoundCategory { get; set; }
    Color NormalColor { get; set; }
    Color HoverColor { get; set; }
    Color GrabColor { get; set; }
    Color TrackColor { get; set; }
    Color HandleColor { get; set; }
    TimeSpan HoverFadeDuration { get; set; }
    TimeSpan GrabFadeDuration { get; set; }
    bool IsTextShadowEnabled { get; set; }
    float TextShadowBrightness { get; set; }
    Func<double, string>? ValueDisplayProvider { get; set; }
    string? ValueDisplayOverride { get; set; }

    event EventHandler<BasicSliderGrabEventArgs>? Grab;
    event EventHandler<BasicSliderReleaseEventArgs>? Release;
    event EventHandler<BasicSliderFactorChangeEventArgs>? FactorChange;
    event EventHandler<BasicSliderPlaySoundEventArgs>? PlaySound;


    // Methods.
    bool IsPositionOverHandle(Vector2 position);
    bool IsPositionOverTrack(Vector2 position);
    bool IsPositionOverEntireSlider(Vector2 position);
}