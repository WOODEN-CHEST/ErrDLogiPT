using ErrDLogiPTClient.Scene.Sound;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownPlaySoundEventArgs<T> : BasicDropdownEventArgs<T>
{
    // Fields.
    public ILogiSoundInstance?   Sound { get; set; }
    public UISoundOrigin Origin { get; set; }


    // Constructors.
    public BasicDropdownPlaySoundEventArgs(IBasicDropdownList<T> dropdown,
        ILogiSoundInstance? sound,
        UISoundOrigin origin) : base(dropdown)
    {
        Sound = sound;
        Origin = origin;
    }
}