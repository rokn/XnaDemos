using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Lidgren.Network;

namespace Tanks
{
    public enum Packets
    {
        Login,
        Move,
        Start,
        Shoot
    }
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D mouseTexture;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static int width, height;
        private bool exit;
        public static Camera camera;
        public static SpriteFont Font;
        private static Tank tank;
        private static Tank enemyTank;
        private static List<Bullet> bullets;

        private static NetClient client;
        string hostIp = "192.168.1.102";
        private static bool hasStarted;
        private NetPeerConfiguration config;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            Terrain.Initialize(1800,300,GraphicsDevice);
            InitializeClient();
            bullets = new List<Bullet>();
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            camera = new Camera();
            tank = new Tank(new Vector2(), Color.Blue,true);
            enemyTank = new Tank(new Vector2(), Color.Red,false);
            exit = false;
            hasStarted = false;
            base.Initialize();
        }

        private void InitializeClient()
        {
            config = new NetPeerConfiguration("Tanks");
            client = new NetClient(config);
            NetOutgoingMessage outMsg = client.CreateMessage();
            client.Start();
            outMsg.Write((byte)Packets.Login);
            outMsg.Write(Terrain.Width);
            client.Connect(hostIp, 14242, outMsg);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse", Content);
            Font = Content.Load<SpriteFont>("Font");
            Bullet.Texture = Scripts.LoadTexture(@"Bullets\Bullet", Content);
            Bullet.gravityForce = 0.1f;
            Bullet.Explosion = Scripts.LoadTexture(@"Bullets\Explosion_Smallest", Content);
            Bullet.explosionRectangle = new Rectangle(0, 0, Bullet.Explosion.Width, Bullet.Explosion.Height);
            camera.Zoom = new Vector2((float)width / (float)Terrain.Width, 1f);
            camera.pos = new Vector2(Terrain.Width / 2, (height / 2));
            tank.Load(Content);
            enemyTank.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInput(gameTime);
                CheckForExit();
                MoveCamera();
                if (hasStarted)
                {
                    tank.Update();
                    enemyTank.Update();
                    UpdateBullets();
                }
                CheckForMessages();
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

            #region Camera Drawing
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(graphics.GraphicsDevice));

            if (hasStarted)
            {
                Terrain.Draw(spriteBatch);
                tank.Draw(spriteBatch);
                enemyTank.Draw(spriteBatch);
                DrawBullets();
            }

            spriteBatch.End();
            #endregion

            #region Normal Drawing
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            DrawMouse();
            spriteBatch.DrawString(Font, "Current Tank Stats:", new Vector2(30, 30), Color.Black);
            spriteBatch.DrawString(Font, "Power:" + tank.Power, new Vector2(30, 50), Color.Black);

            spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }


        private void CheckForMessages()
        {
            NetIncomingMessage incMsg;
            if ((incMsg = client.ReadMessage()) != null)
            {
                switch (incMsg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        if (incMsg.ReadByte() == (byte)Packets.Start)
                        {
                            UpdatePositions(incMsg);
                            hasStarted = true;
                        }
                        else if (incMsg.ReadByte() == (byte)Packets.Move)
                        {
                            UpdatePositions(incMsg);
                        }
                        break;
                    default:
                        Console.WriteLine("strange msg");
                        break;
                }
            }
        }

        private void UpdatePositions(NetIncomingMessage incMsg)
        {
            byte player = incMsg.ReadByte();
            if (player == 0)
            {
                tank.position.X = incMsg.ReadInt32();
                enemyTank.position.X = incMsg.ReadInt32();
            }
            else if (player == 1)
            {
                enemyTank.position.X = incMsg.ReadInt32();
                tank.position.X = incMsg.ReadInt32();
            }
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

        private void UpdateInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }

        private void UpdateBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update();
            }
        }

        private void DrawBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(spriteBatch);
            }
        }

        public static void AddBullet(Bullet bullet)
        {
            bullets.Add(bullet);
        }

        public static void RemoveBullet(Bullet bullet)
        {
            bullets.Remove(bullet);
        }

        private void MoveCamera()
        {
            //if (Scripts.KeyIsPressed(Keys.Left))
            //{
            //    camera.Move(new Vector2(-4, 0));
            //}
            //if (Scripts.KeyIsPressed(Keys.Right))
            //{
            //    camera.Move(new Vector2(4, 0));
            //}
            //if (Scripts.KeyIsPressed(Keys.Up))
            //{
            //    camera.Move(new Vector2(0, -4));
            //}
            //if (Scripts.KeyIsPressed(Keys.Down))
            //{
            //    camera.Move(new Vector2(0, 4));
            //}
            //if (Scripts.KeyIsPressed(Keys.Add))
            //{
            //    camera.HorizontalZoom(0.05f);
            //}
            //if (Scripts.KeyIsPressed(Keys.Subtract))
            //{
            //    camera.HorizontalZoom(-0.05f);
            //}
        }

        private void CheckForExit()
        {
            if (keyboard.JustPressed(Keys.Escape))
            {
                exit = true;
            }
        }

        public static void SendMessage(Packets messageType)
        {
            NetOutgoingMessage outMsg = client.CreateMessage();
            switch(messageType)
            {
                case Packets.Move:
                    outMsg.Write((byte)messageType);
                    outMsg.Write((int)tank.position.X);
                    client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered);
                    break;
            }
        }
    }
}