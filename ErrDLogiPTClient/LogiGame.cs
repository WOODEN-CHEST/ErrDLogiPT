using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Intro;
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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ErrDLogiPTClient
{
    public class LogiGame : Game
    {
        // Fields.
        public GenericServices? LogiGameServices { get; private set; }


        // Private fields.
        private readonly GraphicsDeviceManager _graphics;
        private string _latestLogPath;


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
                Logger?.Dispose();
                if (_latestLogPath != null)
                {
                    Process.Start("notepad", _latestLogPath);
                }
            }
            catch (Exception) { }
            Exit();
        }

        private void InitializeGame()
        {
            string ExecutableRootPath = Path.GetDirectoryName(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location)!;
            IGamePathStructure Structure = new DefaultGamePathStructure(ExecutableRootPath);

            ILogger Logger = new LogInitializer().InitializeLogger(Structure);
            _latestLogPath = Structure.LatestLogPath;

            IDisplay Display = new GHDisplay(_graphics, Window);
            IsFixedTimeStep = false;
            Display.Initialize();

            IUserInput Input = new GHUserInput(Window, this);
            Display.ScreenSizeChange += (sender, args) => Input.InputAreaSizePixels = (Vector2)args.NewSize;
            Input.InputAreaSizePixels = (Vector2)Display.CurrentWindowSize;

            ILogiSoundEngine SoundEngine = new DefaultLogiSoundEngine(new GHAudioEngine(10));
            SoundEngine.Start();
            
            IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection();
            GHAssetStreamOpener AssetStreamOpener = new();
            GHGenericAssetLoader AssetLoader = new();
            IAssetProvider AssetProvider = new GHAssetProvider(AssetLoader, AssetDefinitions, Logger);
            AssetLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(AssetStreamOpener, _graphics.GraphicsDevice));
            AssetLoader.SetTypeLoader(AssetType.Sound, new SoundLoader(AssetStreamOpener, SoundEngine.Format));
            AssetLoader.SetTypeLoader(AssetType.Font, new FontLoader(AssetStreamOpener, _graphics.GraphicsDevice));

            IFrameExecutor FrameExecutor = new DefaultFrameExecutor(_graphics.GraphicsDevice, Display);

            // Issues regarding blend state exist, see https://github.com/MonoGame/MonoGame/issues/6978
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

            IGameScene StartingScene = new IntroScene(LogiGameServices);
            SceneExecutor.ScheduleNextSceneSet(StartingScene);
            SceneExecutor.ScheduleJumpToNextScene(true);
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
}
