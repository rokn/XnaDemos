using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerOfOne
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private SpriteBatch spriteBatch;
        private static bool exit;

        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        public static MouseCursor Mouse;
        public static KeysInput keyboard;
        public static Camera camera;
        public static int WindowWidth;
        public static int WindowHeight;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = base.Content;

            GoFullscreenBorderless();

            Components.Add(new FrameRateCounter(this));

            keyboard = new KeysInput(this);
            Mouse = new MouseCursor(this, WindowWidth, WindowHeight, 150f);

            Components.Add(Mouse);
            Components.Add(keyboard);
        }

        protected override void Initialize()
        {
            exit = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                HandleMainInput();
            }

            if (exit)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        private void GoFullscreenBorderless()
        {
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            WindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            WindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        private void HandleMainInput()
        {
            if (keyboard.JustPressed(Keys.Escape))
            {
                ExitGame();
            }
        }

        public static void ExitGame()
        {
            exit = true;
        }
    }
}