using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroSkipper : SceneComponentBase
{
    // Fields
    public event EventHandler<EventArgs>? SkippedIntro;


    // Protected fields.
    protected virtual Keys[] IntroSkipKeys
    {
        get => _introSkipKeys.ToArray();
        set => _introSkipKeys = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Private fields.
    private Keys[] _introSkipKeys = new Keys[] { Keys.Enter, Keys.Escape, Keys.Back, Keys.Space };
    private bool _wasIntroSkipped = false;


    // Constructors.
    public IntroSkipper(IGameScene scene, IGenericServices sceneServices) : base(scene, sceneServices) { }



    // Protected methods.
    protected virtual bool IsIntroSkipped(IUserInput input)
    {
        foreach (Keys Key in IntroSkipKeys)
        {
            if (input.WereKeysJustPressed(Key))
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void SkipIntro()
    {
        _wasIntroSkipped = true;
        SkippedIntro?.Invoke(this, EventArgs.Empty);
    }



    // Private methods.
    private void ListenForIntroSkip()
    {
        if (_wasIntroSkipped)
        {
            return;
        }

        IUserInput Input = SceneServices.GetRequired<IUserInput>();
        if (IsIntroSkipped(Input))
        {
            SkipIntro();
        }
    }


    // Inherited methods.
    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        ListenForIntroSkip();
    }
}