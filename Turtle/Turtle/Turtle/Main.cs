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

namespace Turtle
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        public const int DefaultChunkSize = 5000;

        private bool exit;

        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;
        private Color BackgroundColor;

        public static Camera camera;
        public static ContentManager content;
        public static GraphicsDeviceManager graphics;
        public static int width, height;
        public static KeysInput keyboard;
        public static MouseCursor mouse;
        public static Random rand;
        public static SpriteFont Font;
        private Turtle turtle;
        private Turtle turtle2;

        #endregion Variables

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            GoFullscreenBorderless();
            Components.Add(new FrameRateCounter(this));
        }

        protected override void Initialize()
        {
            rand = new Random();
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            camera = new Camera();
            BackgroundColor = Color.Black;
            turtle = new Turtle(10, 45, new Vector2(300, 300), 0, "FFRRFFLLFFRRFFLLFFRRFFLLFFRRFFFFRRFFLLFFRRFFLLFFRRFFLLFFRRFFFFRRFFLLFFRRFFLLFFRRFFLLFFRRFFFFRRFFLLFFRRFFLLFFRRFFLLFFRRFFF");
            turtle2 = new Turtle(1, 1, new Vector2(300, 300), 180, "FFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRRFFRR");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse");
            Font = content.Load<SpriteFont>("Font");
            //List<Texture2D> particles = new List<Texture2D>();

            //particles.Add(Scripts.LoadTexture(@"Particles\Circle"));
            //particles.Add(Scripts.LoadTexture(@"Particles\Line"));
            //particles.Add(Scripts.LoadTexture(@"Particles\WhitePixel"));

            //particleEngine = new ParticleEngine(particles, new Vector2());
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInputs(gameTime);
                HandleMainInput();
                turtle.Update();
                turtle2.Update();
            }

            if (exit)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);
            MainDraw();
            GuiDraw();
            base.Draw(gameTime);
        }

        #region Helper Methods

        private void MainDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            turtle.Draw(spriteBatch);
            turtle2.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void GuiDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            DrawMouse();
            spriteBatch.End();
        }

        private void DrawMouse()
        {
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(), 1f, SpriteEffects.None, 1f);
        }

        private void GoFullscreenBorderless()
        {
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        private void HandleMainInput()
        {
            if (keyboard.JustPressed(Keys.Escape))
            {
                exit = true;
            }

            float cameraMoveSpeed = 6;

            if (keyboard.IsHeld(Keys.LeftShift))
            {
                cameraMoveSpeed *= 2;
            }

            if (Scripts.KeyIsPressed(Keys.W))
            {
                camera.Move(new Vector2(0, -cameraMoveSpeed));
            }
            if (Scripts.KeyIsPressed(Keys.S))
            {
                camera.Move(new Vector2(0, cameraMoveSpeed));
            }
            if (Scripts.KeyIsPressed(Keys.D))
            {
                camera.Move(new Vector2(cameraMoveSpeed, 0));
            }
            if (Scripts.KeyIsPressed(Keys.A))
            {
                camera.Move(new Vector2(-cameraMoveSpeed, 0));
            }

            if (mouse.HasScrolledDown())
            {
                camera.Zoom -= new Vector2(0.15f);
            }

            if (mouse.HasScrolledUp())
            {
                camera.Zoom += new Vector2(0.15f);
            }

            if (keyboard.JustPressed(Keys.R))
            {
                camera = new Camera();
            }
        }

        private void UpdateInputs(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }

        #endregion Helper Methods
    }
}