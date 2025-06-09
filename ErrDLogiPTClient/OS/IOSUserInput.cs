using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS;

public interface IOSUserInput : IUserInput
{
    // Fields.
    RectangleF UserInputArea { get; set; }
    bool IsOSFocused { get; set; }
}