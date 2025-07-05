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

public class DefaultLogiOSUserInput : ILogiOSUserInput
{
    // Fields.
    public RectangleF UserInputArea { get; set; } = new(0f, 0f, 1f, 1f);
    public bool IsOSFocused { get; set; } = true;
    public int MouseScrollChangeAmount => _mouseState.Current.ScrollWheelValue - _mouseState.Previous.ScrollWheelValue;
    public int KeysDownCountCurrent => _keyboardState.Current.GetPressedKeyCount();
    public int KeysDownCountPrevious => _keyboardState.Previous.GetPressedKeyCount();
    public int MouseButtonsPressedCountCurrent => CountMouseButtonsDown(_mouseState.Current);
    public int MouseButtonsPressedCountPrevious => CountMouseButtonsDown(_mouseState.Previous);
    public Vector2 MousePositionCurrent => throw new NotImplementedException();
    public Vector2 MousePositionPrevious => throw new NotImplementedException();
    public float InputAspectRatio => InputAreaPixels.X / InputAreaPixels.Y;
    public bool IsInputUpdated { get; set; } = true;
    public Vector2 InputAreaPixels { get; set; } = Vector2.One;



    // Private fields.
    private DeltaValue<KeyboardState> _keyboardState = new();
    private DeltaValue<MouseState> _mouseState = new();


    // Private methods.
    private int CountMouseButtonsDown(MouseState state)
    {
        int Count = 0;

        if (state.LeftButton == ButtonState.Pressed)
        {
            Count++;
        }
        if (state.MiddleButton == ButtonState.Pressed)
        {
            Count++;
        }
        if (state.RightButton == ButtonState.Pressed)
        {
            Count++;
        }

        return Count;
    }


    // Inherited methods.
    public bool AreKeysDown(params Keys[] keys)
    {
        foreach (Keys Key  in keys)
        {
            if (_keyboardState.Current.IsKeyUp(Key))
            {
                return false;
            }
        }
        return true;
    }

    public bool AreKeysUp(params Keys[] keys)
    {
        foreach (Keys Key in keys)
        {
            if (_keyboardState.Current.IsKeyDown(Key))
            {
                return false;
            }
        }
        return true; ;
    }

    public bool AreMouseButtonsDown(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public bool AreMouseButtonsUp(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public bool WereKeysDown(params Keys[] keys)
    {
        throw new NotImplementedException();
    }

    public bool WereKeysJustPressed(params Keys[] keys)
    {
        throw new NotImplementedException();
    }

    public bool WereKeysJustReleased(params Keys[] keys)
    {
        throw new NotImplementedException();
    }

    public bool WereKeysUp(params Keys[] keys)
    {
        throw new NotImplementedException();
    }

    public bool WereMouseButtonsDown(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public bool WereMouseButtonsJustPressed(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public bool WereMouseButtonsJustReleased(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public bool WereMouseButtonsUp(params MouseButton[] buttons)
    {
        throw new NotImplementedException();
    }

    public void Update(IProgramTime time)
    {
        if (!IsInputUpdated)
        {
            return;
        }

        _keyboardState.SetValue(Keyboard.GetState());
        _mouseState.SetValue(Mouse.GetState());
    }
}