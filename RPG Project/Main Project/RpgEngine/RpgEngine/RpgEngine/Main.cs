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

namespace Cars
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static int width, height;
        private bool exit;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse", Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInput(gameTime);
                if (keyboard.JustPressed(Keys.Escape))
                {
                    exit = true;
                }

                //IntPtr hWnd = this.Window.Handle;
                //var control = System.Windows.Forms.Control.FromHandle(hWnd);
                //var form = control.FindForm();
                //form.Location = new System.Drawing.Point(Mouse.GetState().X, Mouse.GetState().Y);
            }
            if (exit)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            //spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            //DrawMouse();
            //spriteBatch.End();
            //base.Draw(gameTime);
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
            //form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            form.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            form.BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
        }

        private void UpdateInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }
    }
}