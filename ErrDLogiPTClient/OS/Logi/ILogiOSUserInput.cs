using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS;

public interface ILogiOSUserInput : ITimeUpdatable
{
    // Fields.
    RectangleF UserInputArea { get; set; }
    bool IsOSFocused { get; set; }
    float InputAspectRatio { get; }
    bool IsInputUpdated { get; set; }

    int MouseScrollChangeAmount { get; }
    int KeysDownCountCurrent { get; }
    int KeysDownCountPrevious { get; }
    int MouseButtonsPressedCountCurrent { get; }
    int MouseButtonsPressedCountPrevious { get; }

    Vector2 InputAreaPixels { get; set;}
    Vector2 MousePositionCurrent { get; }
    Vector2 MousePositionPrevious { get; }


    // Methods.
    bool AreKeysDown(params Keys[] keys);
    bool AreKeysUp(params Keys[] keys);
    bool WereKeysJustPressed(params Keys[] keys);
    bool WereKeysJustReleased(params Keys[] keys);
    bool WereKeysDown(params Keys[] keys);
    bool WereKeysUp(params Keys[] keys);
    bool AreMouseButtonsDown(params MouseButton[] buttons);
    bool AreMouseButtonsUp(params MouseButton[] buttons);
    bool WereMouseButtonsDown(params MouseButton[] buttons);
    bool WereMouseButtonsUp(params MouseButton[] buttons);
    bool WereMouseButtonsJustPressed(params MouseButton[] buttons);
    bool WereMouseButtonsJustReleased(params MouseButton[] buttons);
}