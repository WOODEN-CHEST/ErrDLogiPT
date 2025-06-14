using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroSkipper : SceneComponentBase
{
    // Fields
    public event EventHandler<EventArgs>? SkippedIntro;


    // Private fields.
    private readonly Keys[] _introSkipKeys = new Keys[] { Keys.Enter, Keys.Escape, Keys.Back, Keys.Space };
    private bool _wasIntroSkipped = false;


    // Constructors.
    public IntroSkipper(IGameScene scene, GenericServices sceneServices) : base(scene, sceneServices) { }


    // Private methods.
    private void SkipIntro()
    {
        _wasIntroSkipped = true;
        SkippedIntro?.Invoke(this, EventArgs.Empty);
    }

    private void ListenForIntroSkip()
    {
        if (_wasIntroSkipped)
        {
            return;
        }

        IUserInput Input = SceneServices.GetRequired<IUserInput>();

        foreach (Keys Key in _introSkipKeys)
        {
            if (Input.WereKeysJustPressed(Key))
            {
                SkipIntro();
            }
        }
    }


    // Inherited methods.
    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        ListenForIntroSkip();
    }
}