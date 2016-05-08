using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TheSpirit
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
        private Player player;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = base.Content;

            GoFullscreenBorderless();

            Components.Add(new FrameRateCounter(this));

            keyboard = new KeysInput(this);
            Mouse = new MouseCursor(this, WindowWidth, WindowHeight, 150f);

            camera = new Camera();

            player = new Player();

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
            player.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                HandleMainInput();
                player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));
            player.Draw(spriteBatch);
            spriteBatch.End();
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