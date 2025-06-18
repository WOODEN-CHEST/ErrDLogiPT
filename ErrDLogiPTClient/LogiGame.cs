using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Registry;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Intro;
using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Assets;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ErrDLogiPTClient;

public class LogiGame : Game
{
    // Fields.
    public IGenericServices? LogiGameServices { get; private set; }


    // Private fields.
    private readonly GraphicsDeviceManager _graphics;


    // Constructors.
    public LogiGame()
    {
        _graphics = new GraphicsDeviceManager(this);
    }


    // Private methods.
    private void CleanupOnExit()
    {
        if (LogiGameServices == null)
        {
            return;
        }
        
        LogiGameServices.Get<IAssetProvider>()?.ReleaseAllAssets();
        LogiGameServices.Get<ILogiSoundEngine>()?.Dispose();
        LogiGameServices.Get<IDisplay>()?.Dispose();
        LogiGameServices.Get<IModManager>()?.DeinitializeMods(LogiGameServices);
        LogiGameServices.Get<ILogger>()?.Dispose();
    }

    private void OnCrash(Exception? e)
    {
        ILogger? Logger = LogiGameServices?.Get<ILogger>();
        if (e != null)
        {
            Logger?.Critical($"Game has crashed! {e}");
        }
        CleanupOnExit();

        try
        {
            string? LatestLogPath = LogiGameServices?.Get<IGamePathStructure>()?.LatestLogPath;
            if (LatestLogPath != null)
            {
                Process.Start("notepad", LatestLogPath);
            }
        }
        catch (Exception) { } // We're fucked if this happens anyway so who cares.
        Exit();
    }

    private void InitializeGame()
    {
        string ExecutableRootPath = Path.GetDirectoryName(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location)!;

        GlobalServiceInitializer Initializer = new();
        LogiGameServices = Initializer.CreateGlobalServices(this, _graphics, ExecutableRootPath);

        LoadMods();
        InitAssets();
        InitGenericSettings();
        ReadConfig();
        InitRegistry();
        InitMods();
        PrepareNextScene();
    }

    private void LoadMods()
    {
        IGamePathStructure Structure = LogiGameServices!.GetRequired<IGamePathStructure>();
        IModManager ModManager = LogiGameServices.GetRequired<IModManager>();
        ModManager.LoadMods(Structure.ModRoot);
    }

    private void InitAssets()
    {
        ILogiAssetManager AssetLoader = LogiGameServices!.GetRequired<ILogiAssetManager>();
        AssetLoader.SetAssetRootPaths(Array.Empty<string>());
        AssetLoader.LoadAssetDefinitions();
    }

    private void InitGenericSettings()
    {
        IDisplay Display = LogiGameServices!.GetRequired<IDisplay>();
        IUserInput Input = LogiGameServices.GetRequired<IUserInput>();
        LogiGameServices!.GetRequired<IFulLScreenToggler>().RestoreFullScreenSize();

        Display.IsUserResizingAllowed = true;
        Input.IsMouseVisible = true;
        Input.IsAltF4Allowed = true;
    }

    private void InitMods()
    {
        LogiGameServices!.GetRequired<IModManager>().InitializeMods(LogiGameServices);
    }

    private void PrepareNextScene()
    {
        ISceneExecutor SceneExecutor = LogiGameServices!.GetRequired<ISceneExecutor>();
        IGameScene StartingScene = new IntroScene(LogiGameServices);
        SceneExecutor.ScheduleNextSceneSet(StartingScene);
        SceneExecutor.ScheduleJumpToNextScene(true);
    }

    private void InitRegistry()
    {
        RegistryInitializer Initializer = new();
        LogiGameServices!.Set<IGameRegistryStorage>(Initializer.CreateRegistryStorage());
    }

    private void ReadConfig()
    {
        // TODO: Read configuration.
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        try
        {
            InitializeGame();
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        try
        {
            IModifiableProgramTime? ProgramTime = LogiGameServices!.Get<IModifiableProgramTime>();
            if (ProgramTime == null)
            {
                throw new InvalidOperationException("Missing time service for LogiGame update method, can't continue.");
            }
            TimeSpan ElapsedTime = gameTime.ElapsedGameTime;
            ProgramTime.TotalTime += ElapsedTime;
            ProgramTime.PassedTime = ElapsedTime;

            LogiGameServices.Get<IUserInput>()?.RefreshInput();
            LogiGameServices.Get<ISceneExecutor>()?.Update(ProgramTime);
            LogiGameServices.Get<ILogiSoundEngine>()?.Update(ProgramTime);
            LogiGameServices.Get<IFulLScreenToggler>()?.Update(ProgramTime);
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        try
        {
            IModifiableProgramTime? ProgramTime = LogiGameServices!.Get<IModifiableProgramTime>();
            if (ProgramTime == null)
            {
                throw new InvalidOperationException("Missing time service for LogiGame render method, can't continue.");
            }
            LogiGameServices!.Get<IFrameExecutor>()?.Render(ProgramTime);
        }
        catch (Exception e)
        {
            OnCrash(e);
        }
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        base.OnExiting(sender, args);
        CleanupOnExit();
    }
}
