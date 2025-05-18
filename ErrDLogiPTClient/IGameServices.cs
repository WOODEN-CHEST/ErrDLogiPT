using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

/// <summary>
/// An object of this type holds various "services" (aka frequently used objects) which the game may need.
/// This is mainly used to pass around these services in bulk to scenes of the game, though it is recommended
/// to pass only the needed services individually to the components of a scene.
/// </summary>
public interface IGameServices : IDisposable
{
    IFrameExecutor FrameExecutor { get; set; }
    ILogger? Logger { get; set; }
    IUserInput Input { get; set; }
    IDisplay Display { get; set; }
    GHAssetStreamOpener AssetStreamOpener { get; }
    IAssetDefinitionCollection AssetDefinitions { get; }
    GHGenericAssetLoader AssetLoader { get; }
    IAssetProvider AssetProvider { get; }
    IGamePathStructure Structure { get; set; }
    IModifiableProgramTime Time { get; set; }
    ISceneManager SceneManager { get; set; }
    IModManager ModManager { get; set; }
}