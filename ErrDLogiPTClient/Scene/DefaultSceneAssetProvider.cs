using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class DefaultSceneAssetProvider : ISceneAssetProvider
{
    // Private fields.
    private readonly object _lockObject = new();
    private readonly IGameScene _scene;
    private readonly IAssetProvider _assetProvider;
    private readonly IDisplay _display;

    private bool _isInitialized = false;
    private readonly HashSet<GHFontFamily> _fonts = new();
    private readonly HashSet<TextBox> _registeredTextBoxes = new();

    // Constructors.
    public DefaultSceneAssetProvider(IGameScene scene, IAssetProvider assetProvider, IDisplay display)
    {
        _scene = scene ?? throw new ArgumentNullException(nameof(scene));
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _display = display ?? throw new ArgumentNullException(nameof(_display));
    }


    // Private methods.
    private void OnWindowSizeChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        UpdateAssets();
    }


    // Inherited methods.
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _display.ScreenSizeChange += OnWindowSizeChangeEvent;
        _isInitialized = true;
    }

    public void Deinitialize()
    {
        if (!_isInitialized)
        {
            return;
        }

        _display.ScreenSizeChange -= OnWindowSizeChangeEvent;
        _isInitialized = false;
    }

    public T GetAsset<T>(AssetType type, string name) where T : class
    {
        T? Asset = _assetProvider.GetAsset<T>(_scene, type, name);

        if (Asset == null)
        {
            throw new SceneAssetMissingException($"Missing scene asset of type \"{type}\", " +
                $"name \"{name}\" for scene {_scene.GetType().FullName}");
        }

        return Asset;
    }

    public void ReleaseAsset(AssetType type, string name)
    {
        _assetProvider.ReleaseAsset(_scene, type, name);
    }

    public void ReleaseAsset(object asset)
    {
        _assetProvider.ReleaseAsset(_scene, asset);
    }

    public void ReleaseAllAssets()
    {
        _assetProvider.ReleaseUserAssets(_scene);
    }

    public void RegisterRenderedItem(TextBox item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        lock (_lockObject)
        {
            _registeredTextBoxes.Add(item);
        }
    }

    public void RegisterRenderedItem(SpriteItem item) { }  // For now does nothing.

    public void UnregisterRenderedItem(TextBox item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        lock (_lockObject)
        {
            _registeredTextBoxes.Remove(item);
        }
    }

    public void UnregisterRenderedItem(SpriteItem item) { } // For now does nothing.

    public void UpdateAssets()
    {
        lock (_lockObject)
        {
            foreach (GHFontFamily Font in _fonts)
            {
                Font.ClearOldFonts(0);
            }

            foreach (TextBox Box in _registeredTextBoxes)
            {
                TextBoxAssetLoader.LoadTextures(Box);
            }
        }
    }
}