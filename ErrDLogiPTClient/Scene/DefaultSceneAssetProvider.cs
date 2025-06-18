using ErrDLogiPTClient.Service;
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
    private IGenericServices _globalServices;

    private bool _isInitialized = false;
    private readonly HashSet<GHFontFamily> _fonts = new();
    private readonly HashSet<TextBox> _registeredTextBoxes = new();


    // Constructors.
    public DefaultSceneAssetProvider(IGameScene scene, IGenericServices sceneServices)
    {
        _scene = scene ?? throw new ArgumentNullException(nameof(scene));
        _globalServices = sceneServices ?? throw new ArgumentNullException(nameof(sceneServices));
    }


    // Private methods.
    private void OnWindowSizeChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        UpdateAssets();
    }

    private ILogiAssetManager GetAssetManager()
    {
        return _globalServices.GetRequired<ILogiAssetManager>();
    }


    // Inherited methods.
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        IDisplay? Display = _globalServices.Get<IDisplay>();
        if (Display != null)
        {
            Display.ScreenSizeChange += OnWindowSizeChangeEvent;
        }
        
        _isInitialized = true;
    }

    public void Deinitialize()
    {
        if (!_isInitialized)
        {
            return;
        }

        IDisplay? Display = _globalServices.Get<IDisplay>();
        if (Display != null)
        {
            Display.ScreenSizeChange -= OnWindowSizeChangeEvent;
        }
        _isInitialized = false;
    }

    public T GetAsset<T>(AssetType type, string name) where T : class
    {
        T? Asset = GetAssetManager().GetAsset<T>(_scene, type, name);

        if (Asset == null)
        {
            throw new SceneAssetMissingException($"Missing scene asset of type \"{type}\", " +
                $"name \"{name}\" for scene {_scene.GetType().FullName}");
        }

        if (Asset is GHFontFamily FontAsset)
        {
            _fonts.Add(FontAsset);
        }

        return Asset;
    }

    public void ReleaseAsset(AssetType type, string name)
    {
        GetAssetManager().ReleaseAsset(_scene, type, name);
    }

    public void ReleaseAsset(object asset)
    {
        GetAssetManager().ReleaseAsset(_scene, asset);
    }

    public void ReleaseAllAssets()
    {
        GetAssetManager().ReleaseUserAssets(_scene);
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

    public void InitializeWrapper() { }

    public void DeinitializeWrapper() { }
}