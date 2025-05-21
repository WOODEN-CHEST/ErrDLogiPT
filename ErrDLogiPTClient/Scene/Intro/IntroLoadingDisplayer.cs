using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

internal class IntroLoadingDisplayer : SceneComponentBase<IntroScene>
{
    // Fields.
    public bool IsDisplayed
    {
        get => _isDisplayed;
        set
        {
            if (_isDisplayed == value)
            {
                return;
            }

            _isDisplayed = value;

            if (_loadingText == null)
            {
                return;
            }

            if (value)
            {
                _targetLayer.AddItem(_loadingText);
            }
            else
            {
                _targetLayer.RemoveItem(_loadingText);
            }
        }
    }


    // Private static fields.
    private const string ASSET_NAME_FONT_MAIN = "main";
    private const float FONT_SIZE = 0.1f;
    private const int MAX_PERIOD_COUNT = 3;
    private const string LOADING_TEXT_VALUE = "Loading";
    private const char PERIOD = '.';
    private static readonly TimeSpan TIME_PER_TEXT_UPDATE = TimeSpan.FromSeconds(1d);

    // Private fields.
    private readonly ILayer _targetLayer;
    private TextBox _loadingText;
    private int _periodCount = 0;
    private TextComponent _mainComponent;
    private bool _isDisplayed = false;
    private TimeSpan _timeSinceTextUpdate = TimeSpan.Zero;


    // Constructors,
    public IntroLoadingDisplayer(IntroScene scene, 
        ISceneAssetProvider assetProvider, 
        IGameServices services,
        ILayer targetLayer)
        : base(scene, assetProvider, services)
    {
        _targetLayer = targetLayer ?? throw new ArgumentNullException(nameof(targetLayer));
    }


    // Inherited methods.
    public override void Update(IProgramTime time)
    {
        base.Update(time);

        if (!IsDisplayed)
        {
            return;
        }

        _timeSinceTextUpdate += time.PassedTime;
        if (_timeSinceTextUpdate < TIME_PER_TEXT_UPDATE)
        {
            return;
        }

        _timeSinceTextUpdate = TimeSpan.Zero;

        _periodCount = (_periodCount + 1) % (MAX_PERIOD_COUNT + 1);
        _mainComponent.Text = LOADING_TEXT_VALUE + string.Join(string.Empty, Enumerable.Repeat(PERIOD, _periodCount));
    }

    public override void OnLoad()
    {
        base.OnLoad();

        GHFontFamily Font = AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_FONT_MAIN);

        _loadingText = new()
        {
            Position = new(0.5f)
        };
        
        _mainComponent = new(Font, LOADING_TEXT_VALUE)
        {
            FontSize = FONT_SIZE,
            Mask = Color.White
        };

        _loadingText.Add(_mainComponent);
        _periodCount = 0;
        _loadingText.Origin = new(0.5f);
    }
}