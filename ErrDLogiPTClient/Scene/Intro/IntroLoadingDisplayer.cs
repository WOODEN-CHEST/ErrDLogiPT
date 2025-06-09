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

public class IntroLoadingDisplayer : SceneComponentBase<IntroScene>
{
    // Fields.
    public bool IsDisplayed
    {
        get => _shouldDisplay;
        set
        {
            if (_shouldDisplay == value)
            {
                return;
            }

            _shouldDisplay = value;

            bool IsLoaded;
            lock (_lockObject)
            {
                IsLoaded = _isLoaded;
            }
            if (!IsLoaded)
            {
                return;
            }

            UpdateVisibility();
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
    private readonly object _lockObject = new();
    private readonly ILayer _targetLayer;
    private TextBox _loadingText;
    private int _periodCount = 0;
    private TextComponent _mainComponent;
    private TimeSpan _timeSinceTextUpdate = TimeSpan.Zero;

    private bool _shouldDisplay = false;
    private bool _isTextVisible = false;
    private bool _isLoaded = false;
    

    // Constructors,
    public IntroLoadingDisplayer(IntroScene scene, GenericServices sceneServices, ILayer targetLayer) : base(scene, sceneServices)
    {
        _targetLayer = targetLayer ?? throw new ArgumentNullException(nameof(targetLayer));
    }


    // Private methods.
    private void OnLoadFinish()
    {
        lock (_lockObject)
        {
            _isLoaded = true;
        }
    }

    private void UpdateVisibility()
    {
        _isTextVisible = _shouldDisplay;
        if (_shouldDisplay)
        {
            _targetLayer.AddItem(_loadingText);
        }
        else
        {
            _targetLayer.RemoveItem(_loadingText);
        }
    }

    private void UpdateText(IProgramTime time)
    {
        if (!_isTextVisible)
        {
            UpdateVisibility();
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


    // Inherited methods.
    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        if (!IsDisplayed)
        {
            return;
        }

        bool IsLoaded;
        lock (_lockObject)
        {
            IsLoaded = _isLoaded;
        }

        if (!IsLoaded)
        {
            return;
        }
        UpdateText(time);
    }

    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();

        Task.Run(() =>
        {
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
            AssetProvider.RegisterRenderedItem(_loadingText);
            _loadingText.PrepareTexturesForRendering(Enumerable.Empty<char>());

            OnLoadFinish();
        });
    }
}