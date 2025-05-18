using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ErrDLogiPTClient
{
    public class LogiGame : Game
    {
        // Private fields.
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private IGameServices _services;
        private ILogger? _logger = null;
        private IDisplay _display;
        private IUserInput _input;
        private IAssetDefinitionCollection _assets;
        private IFrameExecutor _frameExecutor;
        private string _latestLogPath;


        // Constructors.
        public LogiGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        // Private methods.
        private void OnCrash(Exception? e)
        {
            if (e != null)
            {
                _logger?.Critical($"Game has crashed! {e}");
            }

            try
            {
                _logger?.Dispose();
                if (_latestLogPath != null)
                {
                    Process.Start("notepad", _latestLogPath);
                }
            }
            catch (Exception) { }

            Exit();
        }

        // Inherited methods.
        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                ILogger Logger =
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

            }
            catch (Exception e)
            {
                OnCrash(e);
            }
        }
    }
}
