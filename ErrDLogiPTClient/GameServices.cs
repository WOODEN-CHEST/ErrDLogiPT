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

public class GameServices
{
    // Fields.
    public required IFrameExecutor FrameExecutor { get; init; }
    public required ILogger? Logger { get; init; }
    public required IUserInput Input { get; init; }
    public required IDisplay Display { get; init; }
    public required IAssetProvider AssetProvider { get; init; }
    public required IGamePathStructure Structure { get; init; }
    public required IModifiableProgramTime Time { get; init; }
    public required ISceneExecutor SceneExecutor { get; init; }
    public required IAudioEngine AudioEngine { get; init; }
    public required IModManager ModManager { get; init; }
    public required ILogiAssetLoader AssetManager { get; init; }


    // Methods.
    public void Dispose()
    {
        Display.Dispose();
        Logger?.Dispose();
        AssetProvider.ReleaseAllAssets();
        AudioEngine.Stop();
        AudioEngine.Dispose();
    }
}