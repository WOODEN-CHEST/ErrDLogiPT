using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrapperServiceDisplay : ServiceWrapper<IDisplay>, IDisplay
{
    // Fields.
    public IntVector WindowedSize
    {
        get => ServiceObject.WindowedSize;
        set => ServiceObject.WindowedSize = value;
    }

    public IntVector FullScreenSize
    {
        get => ServiceObject.FullScreenSize;
        set => ServiceObject.FullScreenSize = value;
    }

    public IntVector CurrentWindowSize
    {
        get => ServiceObject.CurrentWindowSize;
        set => ServiceObject.CurrentWindowSize = value;
    }

    public IntVector ScreenSize => ServiceObject.ScreenSize;

    public bool IsFullScreen
    {
        get => ServiceObject.IsFullScreen;
        set => ServiceObject.IsFullScreen = value;
    }
    public bool IsUserResizingAllowed
    {
        get => ServiceObject.IsUserResizingAllowed;
        set => ServiceObject.IsUserResizingAllowed = value;
    }

    public event EventHandler<ScreenSizeChangeEventArgs>? ScreenSizeChange;


    // Constructors.
    public WrapperServiceDisplay(IGenericServices services) : base(services) { }
    

    // Private methods.
    private void OnScreenSizeChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        ScreenSizeChange?.Invoke(this, new(args.NewSize));
    }


    // Inherited methods.
    public void Dispose() { }

    public void Initialize() { }

    protected override void InitService(IDisplay service)
    {
        service.ScreenSizeChange += OnScreenSizeChangeEvent;
    }

    protected override void DeinitService(IDisplay service)
    {
        service.ScreenSizeChange -= OnScreenSizeChangeEvent;
    }
}