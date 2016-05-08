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

namespace GalacticCommander
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        public const int DefaultChunkSize = 5000;

        private bool exit;

        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;
        private Ship mainShip;
        private List<Chunk> chunks;
        private Color BackgroundColor;

        public static Camera camera;
        public static ContentManager content;
        public static GraphicsDeviceManager graphics;
        public static int width, height;
        public static KeysInput keyboard;
        public static MouseCursor mouse;
        public static Random rand;
        public static SpriteFont Font;
        public static ParticleEngine particleEngine;

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
            Resources.Load();
            rand = new Random();
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            camera = new Camera();
            mainShip = new Ship(camera.pos);
            BackgroundColor = new Color((int)15, (int)15, (int)15);

            chunks = new List<Chunk>();
            chunks.Add(new Chunk(new Vector2(-DefaultChunkSize, -DefaultChunkSize)));
            chunks.Add(new Chunk(new Vector2(0, -DefaultChunkSize)));
            chunks.Add(new Chunk(new Vector2(DefaultChunkSize, -DefaultChunkSize)));
            chunks.Add(new Chunk(new Vector2(-DefaultChunkSize, 0)));
            chunks.Add(new Chunk(new Vector2(0, 0)));
            chunks.Add(new Chunk(new Vector2(DefaultChunkSize, 0)));
            chunks.Add(new Chunk(new Vector2(-DefaultChunkSize, DefaultChunkSize)));
            chunks.Add(new Chunk(new Vector2(0, DefaultChunkSize)));
            chunks.Add(new Chunk(new Vector2(DefaultChunkSize, DefaultChunkSize)));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse");
            Font = content.Load<SpriteFont>("Font");
            mainShip.Load();

            List<Texture2D> particles = new List<Texture2D>();

            particles.Add(Scripts.LoadTexture(@"Particles\Circle"));
            particles.Add(Scripts.LoadTexture(@"Particles\Line"));
            particles.Add(Scripts.LoadTexture(@"Particles\WhitePixel"));

            particleEngine = new ParticleEngine(particles, new Vector2());
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInputs(gameTime);
                HandleMainInput();
                mainShip.Update();

                particleEngine.Update();
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

            mainShip.Draw(spriteBatch);
            chunks.ForEach(chunk => chunk.Draw(spriteBatch));
            particleEngine.Draw(spriteBatch);

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
        }

        private void UpdateInputs(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }

        #endregion Helper Methods
    }
}