using GHEngine;
using GHEngine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroSkipper : SceneComponentBase<IntroScene>
{
    // Fields
    public event EventHandler<EventArgs>? SkippedIntro;


    // Private fields.
    private readonly Keys[] _introSkipKeys = new Keys[] { Keys.Enter, Keys.Escape, Keys.Back };
    private bool _wasIntroSkipped = false;
    private readonly IUserInput _input;


    // Constructors.
    public IntroSkipper(IntroScene scene, IUserInput input) : base(scene)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }


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

        foreach (Keys Key in _introSkipKeys)
        {
            if (_input.WereKeysJustPressed(Key))
            {
                SkipIntro();
            }
        }
    }


    // Inherited methods.
    public override void Update(IProgramTime time)
    {
        base.Update(time);
        ListenForIntroSkip();
    }
}