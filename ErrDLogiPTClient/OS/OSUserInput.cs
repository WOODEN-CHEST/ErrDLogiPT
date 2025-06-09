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

public class OSUserInput : IOSUserInput
{
    // Fields.
    public int MouseScrollChangeAmount => IsOSFocused ? _wrappedInput.MouseScrollChangeAmount : 0;

    public int KeysDownCountCurrent => IsOSFocused ? _wrappedInput.KeysDownCountCurrent : 0;

    public int KeysDownCountPrevious => IsOSFocused ? _wrappedInput.KeysDownCountPrevious : 0;

    public Vector2 VirtualMousePositionCurrent => GetOSVirtualPosition(_wrappedInput.VirtualMousePositionCurrent);

    public Vector2 VirtualMousePositionPrevious => GetOSVirtualPosition(_wrappedInput.VirtualMousePositionPrevious);

    public Vector2 ActualMousePositionCurrent
    {
        get => GetOSVirtualPosition(_wrappedInput.VirtualMousePositionCurrent) * InputAreaSizePixels;
        set => throw new NotSupportedException("OS user input does not support setting actual mouse position");
    }
        

    public Vector2 ActualMousePositionPrevious =>
        GetOSVirtualPosition(_wrappedInput.VirtualMousePositionPrevious) * InputAreaSizePixels;

    public int MouseButtonsPressedCountCurrent => IsOSFocused ? _wrappedInput.MouseButtonsPressedCountCurrent : 0;

    public int MouseButtonsPressedCountPrevious => IsOSFocused ? _wrappedInput.MouseButtonsPressedCountPrevious : 0;

    public bool IsWindowFocused => _wrappedInput.IsWindowFocused;

    public bool IsAltF4Allowed
    {
        get => _wrappedInput.IsAltF4Allowed;
        set => _wrappedInput.IsAltF4Allowed = value;
    }

    public bool IsMouseVisible
    {
        get => _wrappedInput.IsMouseVisible;
        set => _wrappedInput.IsMouseVisible = value;
    }

    public MouseCursor CurrentCursor
    {
        set => throw new NotSupportedException("OS user input does not support setting current cursor");
    }

    public Vector2 InputAreaSizePixels
    {
        get => _wrappedInput.InputAreaSizePixels * new Vector2(UserInputArea.Width, UserInputArea.Height); 
        set => throw new NotSupportedException("OS user input does not support setting input area size pixels");
    }

    public float InputAreaRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsOSFocused { get; set; } = true;

    public RectangleF UserInputArea { get; set; } = new(0f, 0f, 1f, 1f);

    public event EventHandler<TextInputEventArgs>? TextInput;
    public event EventHandler<FileDropEventArgs>? FileDrop;


    // Private fields.
    private readonly IUserInput _wrappedInput;


    // Constructors.
    public OSUserInput(IUserInput wrappedInput)
    {
        _wrappedInput = wrappedInput ?? throw new ArgumentNullException(nameof(wrappedInput));
    }


    // Private methods.
    private Vector2 GetOSVirtualPosition(Vector2 position)
    {
        Vector2 OSVirtualPosition = new(
            (position.X - UserInputArea.X) / UserInputArea.Width, 
            (position.Y - UserInputArea.Y) / UserInputArea.Height);

        return OSVirtualPosition;
    }


    // Methods.
    public bool AreKeysDown(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.AreKeysDown(keys);
    }

    public bool AreKeysUp(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.AreKeysUp(keys);
    }

    public bool AreMouseButtonsDown(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.AreMouseButtonsDown(buttons);
    }

    public bool AreMouseButtonsUp(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.AreMouseButtonsUp(buttons);
    }

    public void RefreshInput() { }

    public bool WereKeysDown(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.WereKeysDown(keys);
    }

    public bool WereKeysJustPressed(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.WereKeysJustPressed(keys);
    }

    public bool WereKeysJustReleased(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.WereKeysJustReleased(keys);
    }

    public bool WereKeysUp(params Keys[] keys)
    {
        return IsOSFocused && _wrappedInput.WereKeysUp(keys);
    }

    public bool WereMouseButtonsDown(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.WereMouseButtonsDown(buttons);
    }

    public bool WereMouseButtonsJustPressed(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.WereMouseButtonsJustPressed(buttons);
    }

    public bool WereMouseButtonsJustReleased(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.WereMouseButtonsJustReleased(buttons);
    }

    public bool WereMouseButtonsUp(params MouseButton[] buttons)
    {
        return IsOSFocused && _wrappedInput.WereMouseButtonsUp(buttons);
    }
}