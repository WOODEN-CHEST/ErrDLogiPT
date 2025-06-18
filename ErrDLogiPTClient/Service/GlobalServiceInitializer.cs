using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Wrapper;
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
using NAudio.Wave;
using System;
using System.IO;

namespace ErrDLogiPTClient.Service;

public class GlobalServiceInitializer
{
    // Private static fields.
    private const int AUDIO_LATENCY_MS = 10;


    // Methods.
    public GlobalServices CreateGlobalServices(Game game, 
        GraphicsDeviceManager graphicsManager,
        string executableRootPath)
    {
        GlobalServices Services = new();
        IGamePathStructure Structure = new DefaultGamePathStructure(executableRootPath);
        IDisplay Display = CreateDisplay(game.Window, graphicsManager);

        ILogger Logger = CreateLogger(Structure);
        Services.Set(Logger);

        IUserInput Input = CreateInput(game, Display);
        ILogiSoundEngine SoundEngine = CreateAudioEngine();

        IFrameExecutor FrameExecutor = CreateFrameExecutor(graphicsManager.GraphicsDevice, Display);
        ISceneExecutor SceneExecutor = new DefaultSceneExecutor(game, Services);
        IModManager ModManager = new DefaultModManager(Services);
        ILogiAssetManager AssetManager = new DefaultLogiAssetManager(Services, graphicsManager.GraphicsDevice, SoundEngine.Format);

        Services.Set<ILogger>(Logger);
        Services.Set<IGamePathStructure>(Structure);
        Services.Set<IDisplay>(Display);
        Services.Set<IUserInput>(Input);
        Services.Set<ILogiSoundEngine>(SoundEngine);
        Services.Set<IFrameExecutor>(FrameExecutor);
        Services.Set<ISceneExecutor>(SceneExecutor);
        Services.Set<IModifiableProgramTime>(new GenericProgramTime());
        Services.Set<ISceneFactoryProvider>(new DefaultSceneFactoryProvider());
        Services.Set<IFulLScreenToggler>(new DefaultFullScreenToggler(Services));
        Services.Set<IModManager>(ModManager);
        Services.Set<ILogiAssetManager>(AssetManager);

        return Services;
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

    private IAssetLoader CreateAssetLoader(IAssetStreamOpener streamOpener,
        GraphicsDevice graphicsDevice,
        WaveFormat audioFormat)
    {
        GHGenericAssetLoader AssetLoader = new();

        AssetLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(streamOpener, graphicsDevice));
        AssetLoader.SetTypeLoader(AssetType.Sound, new SoundLoader(streamOpener, audioFormat));
        AssetLoader.SetTypeLoader(AssetType.Font, new FontLoader(streamOpener, graphicsDevice));

        return AssetLoader;
    }

    private IFrameExecutor CreateFrameExecutor(GraphicsDevice graphicsDevice, IDisplay display)
    {
        IFrameExecutor FrameExecutor = new DefaultFrameExecutor(graphicsDevice, display);

        /* Issues regarding blend state exist, see https://github.com/MonoGame/MonoGame/issues/6978 */
        FrameExecutor.Renderer.RenderBlendState = BlendState.NonPremultiplied;
        FrameExecutor.Renderer.ScreenColor = Color.Black;

        return FrameExecutor;
    }
}