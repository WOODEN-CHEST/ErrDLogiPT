﻿using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Audio.Source;
using GHEngine.Frame;
using GHEngine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuUIExecutor : SceneComponentBase<MainMenuScene>
{
    // Private static fields.
    private const string LAYER_NAME_BACKGROUND = "background";
    private const string LAYER_NAME_FOREGROUND = "foreground";


    // Private fields.
    private readonly IGameFrame _frame;
    private readonly ILayer _backgroundLayer;
    private readonly ILayer _foregroundLayer;

    private readonly IFrameExecutor _frameExecutor;
    private readonly UIElementFactory _uiElementFactory;
    private readonly IUserInput _input;

    private UIBasicButton _button;
    private ILogiSoundEngine _soundEngine;
    private ISceneAssetProvider _assetProvider;

    private double _speed = 1d;
    private const double SPEED_CHANGE = 0.1d;


    // Constructors.
    public MainMenuUIExecutor(MainMenuScene scene, 
        IFrameExecutor frameExecutor,
        ISceneAssetProvider assetProvider, 
        IUserInput input,
        ILogiSoundEngine soundEngine)
        : base(scene)
    {
        _frameExecutor = frameExecutor ?? throw new ArgumentNullException(nameof(frameExecutor));

        _frame = new GHGameFrame();

        _backgroundLayer = new GHLayer(LAYER_NAME_BACKGROUND);
        _foregroundLayer = new GHLayer(LAYER_NAME_FOREGROUND);
        _frame.AddLayer(_backgroundLayer);
        _frame.AddLayer(_foregroundLayer);
        _input = input;

        _soundEngine = soundEngine;
        _assetProvider = assetProvider;

        _uiElementFactory = new(assetProvider, input, soundEngine);
    }


    // Private methods.



    // Inherited methods.
    public override void OnLoad()
    {
        base.OnLoad();

        _uiElementFactory.LoadAssets();
        _button = _uiElementFactory.CreateButton(4f);
        _button.Position = new(0.5f, 0.5f);
        _button.Scale = 0.1f;
        _button.Text = "Hello World!";
        
        _button.ClickSounds = new IPreSampledSound[] { _assetProvider.GetAsset<IPreSampledSound>(AssetType.Sound, "test") };
        _button.Volume = 0.25f;

        _button.Scroll += (sender, args) =>
        {
            if (args.ScrollAmount > 0)
            {
                _speed += SPEED_CHANGE;
            }
            else
            {
                _speed -= SPEED_CHANGE;
            }
        };

        _button.PlaySound += (sender, args) => 
        {
            if ((args.Origin == UISoundOrigin.Click) && (args.Sound != null))
            {
                args.Sound.Speed = _speed;
            }
        };
    }

    public override void OnStart()
    {
        base.OnStart();

        _frameExecutor.SetFrame(_frame);

        _button.Initialize();
        _button.Scale = 0.2f;
        _foregroundLayer.AddItem(_button);
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);

        if (_input.WereKeysJustPressed(Keys.Tab))
        {
            _button.IsButtonTargeted = true;
        }
            
        _button.Update(time);
    }
}