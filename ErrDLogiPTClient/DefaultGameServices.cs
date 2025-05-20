using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultGameServices : IGameServices
{
    // Fields.
    public required IFrameExecutor FrameExecutor { get; set; }
    public required ILogger? Logger { get; set; }
    public required IUserInput Input { get; set; }
    public required IDisplay Display { get; set; }
    public required GHAssetStreamOpener AssetStreamOpener { get; init; }
    public required IAssetDefinitionCollection AssetDefinitions { get; init; }
    public required GHGenericAssetLoader AssetLoader { get; init; }
    public required IAssetProvider AssetProvider { get; init; }
    public required IGamePathStructure Structure { get; set; }
    public required IModifiableProgramTime Time { get; set; }
    public required ISceneManager SceneManager { get; set; }
    public required IAudioEngine AudioEngine { get; set; }



    // Inherited methods.
    public void Dispose()
    {
        Display.Dispose();
        Logger?.Dispose();
        AssetProvider.ReleaseAllAssets();
        AudioEngine.Dispose();
    }
}