using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class GlobalServiceInitializer
{
    // Private static fields.
    private const int AUDIO_LATENCY_MS = 10;



    // Methods.
    public GenericServices CreateGlobalServices(Game game, 
        GraphicsDeviceManager graphicsManager,
        string executableRootPath)
    {
        IGamePathStructure Structure = new DefaultGamePathStructure(executableRootPath);
        IDisplay Display = CreateDisplay(game.Window, graphicsManager);
        ILogger Logger = CreateLogger(Structure);
        IUserInput Input = CreateInput(game, Display);
        IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection();
        IAssetStreamOpener AssetStreamOpener = new GHAssetStreamOpener();
        GHGenericAssetLoader AssetLoader = new();


        IFrameExecutor FrameExecutor = new DefaultFrameExecutor(_graphics.GraphicsDevice, Display);

        /* Issues regarding blend state exist, see https://github.com/MonoGame/MonoGame/issues/6978 */
        FrameExecutor.Renderer.RenderBlendState = BlendState.NonPremultiplied;
        FrameExecutor.Renderer.ScreenColor = Color.Black;
        ISceneExecutor SceneExecutor = new DefaultSceneExecutor(this, Logger);

        LogiGameServices = new();
        LogiGameServices.Set<ILogger>(Logger);
        LogiGameServices.Set<IGamePathStructure>(Structure);
        LogiGameServices.Set<IDisplay>(Display);
        LogiGameServices.Set<IUserInput>(Input);
        LogiGameServices.Set<ILogiSoundEngine>(SoundEngine);
        LogiGameServices.Set<IAssetDefinitionCollection>(AssetDefinitions);
        LogiGameServices.Set<IAssetStreamOpener>(AssetStreamOpener);
        LogiGameServices.Set<IAssetLoader>(AssetLoader);
        LogiGameServices.Set<IAssetProvider>(AssetProvider);
        LogiGameServices.Set<IFrameExecutor>(FrameExecutor);
        LogiGameServices.Set<ISceneExecutor>(SceneExecutor);
        LogiGameServices.Set<IModifiableProgramTime>(new GenericProgramTime());
        LogiGameServices.Set<ISceneFactoryProvider>(new DefaultSceneFactoryProvider());
        LogiGameServices.Set<IFulLScreenToggler>(new DefaultFullScreenToggler(LogiGameServices));

        IModManager ModManager = new DefaultModManager(LogiGameServices);
        ILogiAssetLoader AssetManager = new DefaultLogiAssetLoader(LogiGameServices);
        LogiGameServices.Set<IModManager>(ModManager);
        LogiGameServices.Set<ILogiAssetLoader>(AssetManager);
    }



    // Private methods.
    private IDisplay CreateDisplay(GameWindow window, GraphicsDeviceManager graphicsManager)
    {
        IDisplay Display = new GHDisplay(graphicsManager, window);
        Display.Initialize();
        return Display;
    }

    private void InitializeGame(Game game)
    {
        game.IsFixedTimeStep = false;
    }

    private ILogger CreateLogger(IGamePathStructure structure)
    {
        ArgumentNullException.ThrowIfNull(structure, nameof(structure));

        Directory.CreateDirectory(structure.LogRoot);

        // To not clutter the log archive directory while debugging.
#if !DEBUG
        if (File.Exists(structure.LatestLogPath))
        {
            ILogArchiver Archiver = new GHLogArchiver();
            Directory.CreateDirectory(structure.LogArchiveRoot);
            Archiver.Archive(structure.LogArchiveRoot, structure.LatestLogPath);
        }
#endif
        ILogger Logger = new GHLogger(structure.LatestLogPath);
        return Logger;
    }

    private IUserInput CreateInput(Game game, IDisplay display)
    {
        IUserInput Input = new GHUserInput(game.Window, game);
        display.ScreenSizeChange += (sender, args) => Input.InputAreaSizePixels = (Vector2)args.NewSize;
        Input.InputAreaSizePixels = (Vector2)display.CurrentWindowSize;
        return Input;
    }

    private ILogiSoundEngine CreateAudioEngine()
    {
        ILogiSoundEngine SoundEngine = new DefaultLogiSoundEngine(new GHAudioEngine(AUDIO_LATENCY_MS));
        SoundEngine.Start();
        return SoundEngine;
    }

    private IAssetProvider CreateAssetProvider(IAssetStreamOpener streamOpener,
        IAssetDefinitionCollection definitions,
        IAssetLoader loader,
        IL)
    {
        IAssetProvider AssetProvider = new GHAssetProvider(AssetLoader, AssetDefinitions, Logger);
        AssetLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(AssetStreamOpener, _graphics.GraphicsDevice));
        AssetLoader.SetTypeLoader(AssetType.Sound, new SoundLoader(AssetStreamOpener, SoundEngine.Format));
        AssetLoader.SetTypeLoader(AssetType.Font, new FontLoader(AssetStreamOpener, _graphics.GraphicsDevice));
        return AssetProvider;
    }
}