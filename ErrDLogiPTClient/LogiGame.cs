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
                string? LatestLogPath = LogiGameServices?.Get<IGamePathStructure>()?.LatestLogPath;
                if (LatestLogPath != null)
                {
                    Process.Start("notepad", LatestLogPath);
                }
            }
            catch (Exception) { }
            Exit();
        }

        private void InitializeGame()
        {
            string ExecutableRootPath = Path.GetDirectoryName(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location)!;


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
