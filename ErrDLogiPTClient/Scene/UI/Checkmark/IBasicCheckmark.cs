using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Checkmark;

public interface IBasicCheckmark : IUIElement
{
    // Fields.
    Vector2 Position { get; set; }
    bool IsTargeted { get; set; }
    bool IsChecked { get; set; }
    RectangleF CheckmarkBounds { get; }
    IEnumerable<IPreSampledSound> CheckSounds { get; set; }
    IEnumerable<IPreSampledSound> UncheckSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverStartSounds { get; set; }
    IEnumerable<IPreSampledSound> HoverEndSounds { get; set; }
    float Volume { get; set; }
    LogiSoundCategory SoundCategory { get; set; }
    ElementClickMethod ClickMethod { get; set; }
    Color NormalColor { get; set; }
    Color HoverColor { get; set; }
    Color ClickColor { get; set; }
    Color CheckmarkColor { get; set; }
    TimeSpan HoverFadeDuration { get; set; }
    TimeSpan ClickFadeDuration { get; set; }
    float Scale { get; set; }

    event EventHandler<BasicCheckmarkCheckEventArgs>? CheckChange;
    event EventHandler<BasicCheckmarkPlaySoundEventArgs>? PlaySound;


    // Methods.
    bool IsPositionOverCheckmark(Vector2 position);
}