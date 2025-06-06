using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class BasicSliderFactorChangeEventArgs : BasicSliderEventArgs
{
    // Fields.
    public double FactorPrevious { get; }
    public double FactorNew { get; set; }


    // Constructors.
    public BasicSliderFactorChangeEventArgs(IBasicSlider slider, double factorPrevious, double factorNew) : base(slider)
    {
        FactorPrevious = factorPrevious;
        FactorNew = factorNew;
    }
}