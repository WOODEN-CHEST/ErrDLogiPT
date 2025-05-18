using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
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
        // Private fields.
        private readonly GraphicsDeviceManager _graphics;
        private IGameServices? _services;
        private string _latestLogPath;


        // Constructors.
        public LogiGame()
        {
            _graphics = new GraphicsDeviceManager(this);
        }


        // Methods.
        public void StopGame()
        {
            _services?.Dispose();
            Exit();
        }


        // Private methods.
        private void OnCrash(Exception? e)
        {
            if (e != null)
            {
                _services?.Logger?.Critical($"Game has crashed! {e}");
            }

            try
            {
                _services?.Logger?.Dispose();
                if (_latestLogPath != null)
                {
                    Process.Start("notepad", _latestLogPath);
                }
            }
            catch (Exception) { }

            _services?.Dispose();
            Exit();
        }

        private void InitializeGame()
        {
            string ExecutableRootPath = Path.GetDirectoryName(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location)!;

            IGamePathStructure Structure = new DefaultGamePathStructure(ExecutableRootPath);
            ILogger Logger = new LogInitializer().InitializeLogger(Structure);
            IDisplay Display = new GHDisplay(_graphics, Window);
            IUserInput Input = new GHUserInput(Window, this);

            IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection();
            GHAssetStreamOpener AssetStreamOpener = new();
            GHGenericAssetLoader AssetLoader = new();
            IAssetProvider AssetProvider = new GHAssetProvider(AssetLoader, AssetDefinitions, Logger);

            IFrameExecutor FrameExecutor = new DefaultFrameExecutor(_graphics.GraphicsDevice, Display);
            ISceneManager SceneManager = new DefaultSceneManager();

            _services = new DefaultGameServices()
            {
                FrameExecutor = FrameExecutor,
                Logger = Logger,
                Input = Input,
                Display = Display,
                AssetDefinitions = AssetDefinitions,
                AssetLoader = AssetLoader,
                AssetProvider = AssetProvider,
                AssetStreamOpener = AssetStreamOpener,
                Structure = Structure,
                Time = new GenericProgramTime(),
                SceneManager = SceneManager
            };

            IModManager ModManager = new DefaultModManager(_services);
            ModManager.LoadMods();

            IGameScene StartingScene = null!;
            _services.SceneManager.SetNextScene(StartingScene);
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
                IModifiableProgramTime ProgramTime = _services!.Time;
                TimeSpan ElapsedTime = gameTime.ElapsedGameTime;
                ProgramTime.TotalTime += ElapsedTime;
                ProgramTime.PassedTime = ElapsedTime;

                _services.Input.RefreshInput();
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
                _services!.FrameExecutor.Render(_services.Time);
            }
            catch (Exception e)
            {
                OnCrash(e);
            }
        }
    }
}
