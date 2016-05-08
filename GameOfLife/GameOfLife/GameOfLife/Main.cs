using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameOfLife
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        #region Constatnts        
        #endregion Constatnts

        #region Main stuff

        public static GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        public static Camera camera;
        public static int width, height;
        public static ContentManager content;
        public static SpriteFont Font;
        public static Vector2 EarPosition;
        private Color backgroundColor;
        private Field field;

        #endregion Main stuff

        #region Input

        public static MouseCursor mouse;
        public static KeysInput keyboard;
        private Texture2D mouseTexture;

        #endregion Input

        #endregion Variables

        #region Constructor

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            GoFullscreenBorderless();
            this.Components.Add(new FrameRateCounter(this));
        }

        #endregion Constructor

        #region Initialize

        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 150f);
            keyboard = new KeysInput();
            camera = new Camera();
            IsMouseVisible = true;
            backgroundColor = Color.White;
            int cellSize = 6;
            field = new Field(width / cellSize+1, height / cellSize+1, cellSize, cellSize, Vector2.Zero);
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            base.Initialize();
        }

        #endregion Initialize

        #region Load

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            Font = content.Load<SpriteFont>("Font") as SpriteFont;
            mouseTexture = Scripts.LoadTexture("Mouse");
            GUI.Load();
        }

        #endregion Load

        #region Update

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                keyboard.Update(gameTime);
                mouse.UpdateMouse(gameTime);
                HandleMainInput();
                UpdateCamera(gameTime);
                field.Update(gameTime);
            }

            GUI.Update(gameTime);

            base.Update(gameTime);
        }

        #endregion Update

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            MainDraw();
            GuiDraw();
            base.Draw(gameTime);
        }

        #endregion Draw

        #region Helper Methods

        private void MainDraw()
        {
            GraphicsDevice.Clear(backgroundColor);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));
            field.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void GuiDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(mouseTexture.Width / 2, mouseTexture.Height / 2), 1f, SpriteEffects.None, 1f);

            GUI.Draw(spriteBatch);

            spriteBatch.End();
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

            //width = 840;
            //height = 1000;

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
        }

        private void UpdateCamera(GameTime gameTime)
        {
            float amount = 0.5f;

            if (keyboard.IsHeld(Keys.LeftShift))
            {
                amount *= 5;
            }

            if (keyboard.IsHeld(Keys.LeftControl))
            {
                amount /= 10;
            }

            if (Scripts.KeyIsPressed(Keys.Left))
            {
                camera.Move(new Vector2(-amount, 0));
            }

            if (Scripts.KeyIsPressed(Keys.Right))
            {
                camera.Move(new Vector2(amount, 0));
            }

            if (Scripts.KeyIsPressed(Keys.Up))
            {
                camera.Move(new Vector2(0, -amount));
            }

            if (Scripts.KeyIsPressed(Keys.Down))
            {
                camera.Move(new Vector2(0, amount));
            }

            if (Main.mouse.HasScrolledUp())
            {
                camera.ChangeZoom(0.01f);
            }

            if (Main.mouse.HasScrolledDown())
            {
                camera.ChangeZoom(-0.01f);
            }
        }

        private void HandleMainInput()
        {
            if (keyboard.JustPressed(Keys.Escape))
            {
                this.Exit();
            }
        }
        #endregion Helper Methods
    }
}