﻿using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Intro;
using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
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
        private GameServices? _services;
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
            _latestLogPath = Structure.LatestLogPath;

            IDisplay Display = new GHDisplay(_graphics, Window);
            Display.Initialize();

            IUserInput Input = new GHUserInput(Window, this);

            IAudioEngine AudioEngine = new GHAudioEngine(10);
            AudioEngine.Start();

            IAssetDefinitionCollection AssetDefinitions = new GHAssetDefinitionCollection();
            GHAssetStreamOpener AssetStreamOpener = new();
            GHGenericAssetLoader AssetLoader = new();
            IAssetProvider AssetProvider = new GHAssetProvider(AssetLoader, AssetDefinitions, Logger);
            AssetLoader.SetTypeLoader(AssetType.Animation, new AnimationLoader(AssetStreamOpener, _graphics.GraphicsDevice));
            AssetLoader.SetTypeLoader(AssetType.Sound, new SoundLoader(AssetStreamOpener, AudioEngine.WaveFormat));
            AssetLoader.SetTypeLoader(AssetType.Font, new FontLoader(AssetStreamOpener, _graphics.GraphicsDevice));

            IFrameExecutor FrameExecutor = new DefaultFrameExecutor(_graphics.GraphicsDevice, Display);
            ISceneExecutor SceneManager = new DefaultSceneExecutor(Logger);

            IModManager ModManager = new DefaultModManager(Logger);
            ILogiAssetLoader AssetManager = new DefaultLogiAssetLoader(ModManager, Structure, Logger, AssetDefinitions, AssetStreamOpener);

            _services = new GameServices()
            {
                FrameExecutor = FrameExecutor,
                Logger = Logger,
                Input = Input,
                Display = Display,
                AudioEngine = AudioEngine,
                AssetProvider = AssetProvider,
                Structure = Structure,
                Time = new GenericProgramTime(),
                SceneExecutor = SceneManager,
                ModManager = ModManager,
                AssetManager = AssetManager,
            };

            IGameScene StartingScene = new IntroScene(_services);
            _services.SceneExecutor.ScheduleNextSceneSet(StartingScene);
            _services.SceneExecutor.ScheduleJumpToNextScene(true);
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
                _services.SceneExecutor.Update(ProgramTime);
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
