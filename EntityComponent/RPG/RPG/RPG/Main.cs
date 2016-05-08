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

namespace RPG
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        public static int WindowWidth;
        public static int WindowHeight;
        public static Camera camera;
        public static Level currentLevel;
        public static bool InEditMode;
        public static bool ShowBoundingBoxes;
        public static float ElapsedSeconds;
        public static List<Entity> Entities;

        private static Player mainPlayer;
        private static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private static ContentManager contentRef;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            contentRef = Content;
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            Components.Add(new MyKeyboard(this));
            Components.Add(new MyMouse(this, Color.Green));
            Components.Add(new FrameRateCounter(this));
            camera = new Camera();
            currentLevel = Level.LoadLevel("First.level");
            ShowBoundingBoxes = false;
            mainPlayer = new Player(true, Classes.Wizard);
            Entities = new List<Entity>();
            Entities.Add(mainPlayer);
            Entities.Add(new Player(false,Classes.Wizard));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Resources.LoadFonts();
            Resources.LoadMisc();
            TileSet.LoadTileSets();
            Editor.Initialize(0);
            Editor.Load();
            LoadEntities();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            ElapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
            HandleMainInput();

            UpdateEntities(gameTime);

            if (InEditMode)
            {
                Editor.Update();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            MainDraw();
            GUIDraw();
            base.Draw(gameTime);
        }

        private void GUIDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            if (InEditMode)
            {
                Editor.Draw(spriteBatch, true);
            }

            spriteBatch.End();
        }

        private void MainDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
            currentLevel.DrawLevel(spriteBatch);
            DrawEntities();

            if (InEditMode)
            {
                Editor.Draw(spriteBatch, false);
            }

            spriteBatch.End();
        }

        public static ContentManager GetContentManager()
        {
            return contentRef;
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

        public static GraphicsDevice GetGraphicsDevice()
        {
            return graphics.GraphicsDevice;
        }

        private void HandleMainInput()
        {
            if (MyKeyboard.JustPressed(Keys.Escape))
                this.Exit();

            if (MyKeyboard.JustPressed(Keys.F1))
            {
                InEditMode = !InEditMode;
            }

            if (MyKeyboard.JustPressed(Keys.F2))
            {
                ShowBoundingBoxes = !ShowBoundingBoxes;
            }
        }

        private void LoadEntities()
        {
            Entities.ForEach(entity => entity.Load());
        }

        private void UpdateEntities(GameTime gameTime)
        {
            Entities.ForEach(entity => entity.Update(gameTime));
        }

        private void DrawEntities()
        {
            Entities.ForEach(entity => entity.Draw(spriteBatch));
        }
    }
}