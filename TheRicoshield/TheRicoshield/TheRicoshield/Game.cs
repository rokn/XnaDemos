using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TheRicoshield
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D mouseTexture;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static int width,height;
        private bool exit;
        public static Player player;
        Ball ball;
        private static List<SolidObject> solidObjects;
        private bool debug;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
            debug = false;
        }

        public static List<SolidObject> SolidObjects
        {
            get
            {
                return solidObjects;
            }
        }

        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            player = new Player();
            solidObjects = new List<SolidObject>();
            ball = new Ball(new Vector2(600,500));

            solidObjects.Add(new Wall(new Vector2(0, 0), true));
            solidObjects.Add(new Wall(new Vector2(0, 0), false));
            solidObjects.Add(new Wall(new Vector2(1664, 0), false));
            solidObjects.Add(new Wall(new Vector2(0, 1034), true));

            solidObjects.Add(new Block(new Vector2(50, 50), true));
            solidObjects.Add(new Block(new Vector2(150, 50), true));
            solidObjects.Add(new Block(new Vector2(250, 50), true));
            solidObjects.Add(new Block(new Vector2(350, 50), true));
            solidObjects.Add(new Block(new Vector2(450, 50), true));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse", Content);
            ball.Load(Content);
            ball.Speed = new Vector2(0, 6f);
            player.Load(Content);
            foreach(SolidObject obj in solidObjects)
            {
                obj.Load(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInput(gameTime);

                if(keyboard.JustPressed(Keys.Escape))
                {
                    exit = true;
                }
                if(keyboard.JustPressed(Keys.F10))
                {
                    debug = true;
                }
                if (debug)
                {
                    if (keyboard.JustPressed(Keys.Add))
                    {
                        ball.Speed = new Vector2(ball.Speed.X + 1, ball.Speed.Y);
                    }
                    if (keyboard.JustPressed(Keys.Subtract))
                    {
                        ball.Speed = new Vector2(ball.Speed.X - 1, ball.Speed.Y);
                    }
                    if (keyboard.JustPressed(Keys.Multiply))
                    {
                        ball.Speed = new Vector2(ball.Speed.X, ball.Speed.Y + 1);
                    }
                    if (keyboard.JustPressed(Keys.Divide))
                    {
                        ball.Speed = new Vector2(ball.Speed.X, ball.Speed.Y - 1);
                    }
                    if(mouse.LeftClick())
                    {
                        Block block = new Block(mouse.Position);
                        block.Load(Content);
                        solidObjects.Add(block);
                    }
                }

                player.Update();
                ball.Update();
                foreach (SolidObject obj in solidObjects)
                {
                    obj.Update();
                }
            }
            if(exit)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            DrawMouse();
            player.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            foreach (SolidObject obj in solidObjects)
            {
                obj.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMouse()
        {
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(),1f, SpriteEffects.None, 1f);
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

        private void UpdateInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }
    }
}
