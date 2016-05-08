using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;


namespace Wizards
{
    public enum Packets
    {
        MOVE,
        UPDATE,
        CONNECT
    }

    public enum Menu
    {
        MainMenu,
        Server,
        WaintingForPlayers
    }

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static NetClient Client;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static SpriteFont ButtonFont,Font;
        public static int width, height;
        Dictionary<Menu, List<Button>> menus;
        public static Menu currMenu;
        Texture2D mouseTexture;
        public static bool exit,joinToServer;
        public TextBox ipTextBox;
        public static List<Player> players;
        static System.Timers.Timer update;
        private bool IsRunning = true;
        private string serverIp;
        private NetPeerConfiguration Config;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 150f);
            keyboard = new KeysInput();
            menus = new Dictionary<Menu, List<Button>>();
            players = new List<Player>();
            InitializeMenus();
            currMenu = Menu.MainMenu;
            ipTextBox = new TextBox(new Vector2(width / 2-150, 500), 20, "localhost");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ButtonFont = Content.Load<SpriteFont>("ButtonFont");
            Font = Content.Load<SpriteFont>("Font");
            mouseTexture = Scripts.LoadTexture("Mouse", Content);
            ipTextBox.Load(Content);
            LoadMenus();
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
                UpdateMenus();
            }
            if(exit)
            {
                this.Exit();
            }
            if(joinToServer)
            {
                JoinToServer();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            DrawMenus();
            DrawMouse();
            spriteBatch.End();
            base.Draw(gameTime);
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

        private void InitializeMenus()
        {
            List<Button> mainMenu = new List<Button>();
            mainMenu.Add(new Button(new Vector2(width / 2 - 65, 200), ButtonType.Menu, ButtonName.Play));
            mainMenu.Add(new Button(new Vector2(width / 2 - 65, 600), ButtonType.Menu, ButtonName.Quit));
            menus.Add(Menu.MainMenu,mainMenu);

            List<Button> serverMenu = new List<Button>();
            serverMenu.Add(new Button(new Vector2(width / 2 - 65, 600), ButtonType.Menu, ButtonName.Join));
            menus.Add(Menu.Server, serverMenu);
        }

        private void LoadMenus()
        {
            foreach(KeyValuePair<Menu,List<Button>> kvp in menus)
            {
                foreach(Button button in kvp.Value)
                {
                    button.Load(Content);
                }
            }
        }

        private void UpdateMenus()
        {
            foreach (Button button in menus[currMenu])
            {
                button.Update();
            }
            if(currMenu == Menu.Server)
            {
                ipTextBox.Update();
            }
        }

        private void DrawMenus()
        {
            foreach (Button button in menus[currMenu])
            {
                button.Draw(spriteBatch);
            }
            if (currMenu == Menu.Server)
            {
                ipTextBox.Draw(spriteBatch);
            }
        }

        private void JoinToServer()
        {
            serverIp = ipTextBox.text;

            Client = new NetClient(Config);
            NetOutgoingMessage outmsg = Client.CreateMessage();
            Client.Start();

            outmsg.Write((byte)Packets.CONNECT);
            outmsg.Write("Pesho");

            Client.Connect(serverIp, 14242, outmsg);

        }
    }
}
