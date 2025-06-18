using ErrDLogiPTClient.Service;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrapperServiceUserInput : ServiceWrapper<IUserInput>, IUserInput
{
    // Fields.
    public int MouseScrollChangeAmount => ServiceObject.MouseScrollChangeAmount;

    public int KeysDownCountCurrent => ServiceObject.KeysDownCountCurrent;

    public int KeysDownCountPrevious => ServiceObject.KeysDownCountPrevious;

    public Vector2 VirtualMousePositionCurrent => ServiceObject.VirtualMousePositionCurrent;

    public Vector2 VirtualMousePositionPrevious => ServiceObject.VirtualMousePositionPrevious;

    public Vector2 ActualMousePositionCurrent
    {
        get => ServiceObject.ActualMousePositionCurrent;
        set => ServiceObject.ActualMousePositionCurrent = value;
    }

    public Vector2 ActualMousePositionPrevious => ServiceObject.ActualMousePositionPrevious;

    public int MouseButtonsPressedCountCurrent => ServiceObject.MouseButtonsPressedCountCurrent;

    public int MouseButtonsPressedCountPrevious => ServiceObject.MouseButtonsPressedCountPrevious;

    public bool IsWindowFocused => ServiceObject.IsWindowFocused;

    public bool IsAltF4Allowed
    {
        get => ServiceObject.IsAltF4Allowed;
        set => ServiceObject.IsAltF4Allowed = value;
    }

    public bool IsMouseVisible
    {
        get => ServiceObject.IsMouseVisible;
        set => ServiceObject.IsMouseVisible = value;
    }

    public MouseCursor CurrentCursor
    {
        set => ServiceObject.CurrentCursor = value;
    }

    public Vector2 InputAreaSizePixels
    {
        get => ServiceObject.InputAreaSizePixels;
        set => ServiceObject.InputAreaSizePixels = value;
    }

    public float InputAreaRatio
    {
        get => ServiceObject.InputAreaRatio;
        set => ServiceObject.InputAreaRatio = value;
    }

    public event EventHandler<TextInputEventArgs>? TextInput;
    public event EventHandler<FileDropEventArgs>? FileDrop;


    // Constructors.
    public WrapperServiceUserInput(IGenericServices services) : base(services) { }



    // Private methods.
    private void OnTextInputEvent(object? sender, TextInputEventArgs args)
    {
        TextInput?.Invoke(this, args);
    }

    private void OnFileDropEvent(object? sender, FileDropEventArgs args)
    {
        FileDrop?.Invoke(this, args);
    }


    // Inherited methods.
    public bool AreKeysDown(params Keys[] keys)
    {
        return ServiceObject.AreKeysDown(keys);
    }

    public bool AreKeysUp(params Keys[] keys)
    {
        return ServiceObject.AreKeysUp(keys);
    }

    public bool AreMouseButtonsDown(params MouseButton[] buttons)
    {
        return ServiceObject.AreMouseButtonsDown(buttons);
    }

    public bool AreMouseButtonsUp(params MouseButton[] buttons)
    {
        return ServiceObject.AreMouseButtonsUp(buttons);
    }

    public void RefreshInput()
    {
        ServiceObject.RefreshInput();
    }

    public bool WereKeysDown(params Keys[] keys)
    {
        return ServiceObject.WereKeysDown(keys);
    }

    public bool WereKeysJustPressed(params Keys[] keys)
    {
        return ServiceObject.WereKeysJustPressed(keys);
    }

    public bool WereKeysJustReleased(params Keys[] keys)
    {
        return ServiceObject.WereKeysJustReleased(keys);
    }

    public bool WereKeysUp(params Keys[] keys)
    {
        return ServiceObject.WereKeysUp(keys);
    }

    public bool WereMouseButtonsDown(params MouseButton[] buttons)
    {
        return ServiceObject.WereMouseButtonsDown(buttons);
    }

    public bool WereMouseButtonsJustPressed(params MouseButton[] buttons)
    {
        return ServiceObject.WereMouseButtonsJustPressed(buttons);
    }

    public bool WereMouseButtonsJustReleased(params MouseButton[] buttons)
    {
        return ServiceObject.WereMouseButtonsJustReleased(buttons);
    }

    public bool WereMouseButtonsUp(params MouseButton[] buttons)
    {
        return ServiceObject.WereMouseButtonsUp(buttons);
    }

    protected override void InitService(IUserInput service)
    {
        service.TextInput += OnTextInputEvent;
        service.FileDrop += OnFileDropEvent;
    }

    protected override void DeinitService(IUserInput service)
    {
        service.TextInput -= OnTextInputEvent;
        service.FileDrop -= OnFileDropEvent;
    }
}