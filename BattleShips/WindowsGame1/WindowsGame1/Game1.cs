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

namespace Battleships
{
    enum Boats
    {
        Boat,
        Submarine,
        Battleship,
        Aircraft
    }
    enum Menus
    {
        Main,
        Join,
        Game
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int maxBoats = 4,maxSubmarine = 3,maxBattleship = 2,maxAircraft = 1;
        bool ipInput = false, getName = false, now = false, placingBoats = true, canPlace = true, then = true, place = false, hasBegun =false,Begun=false,myTurn=false;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static NetClient Client;
        // Clients list of characters
        static List<Character> GameStateList;
        // Create timer that tells client, when to send update
        static System.Timers.Timer update;
        // Indicates if program is running
        static bool IsRunning = false;
        string name, opponent_name="";
        List<Texture2D> boat, submarine, battleship, carrier;
        TileMap tiles;
        Texture2D bars, bars_back, mouse_texture, tile_selected, txtBox, txtBack, texture_x, texture_o,cannon_back,cannon,cannonball;
        Button ipOk;
        int menuChoice;
        public static MouseCursor mouse;
        Rectangle rect1,rect2;
        SpriteFont font;
        Menus activeMenu;
        TextBox textBoxIP,textBoxName;
        Menu mainMenu, joinMenu;
        Boats currBoat=Boats.Submarine;
        float rotation = 0;
        string IP;
        int boats = 0,submarines=0,battleships=0,aircrafts=0,rot=0,count,attack_x,attack_y;
        KeysInput input;
        Boat[,] boats_field;
        int[,] my_field,enemy_field;
        List<Place> places;
        float my_rotation,enemy_rotation;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 896;
            graphics.PreferredBackBufferHeight = 512;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            boat = new List<Texture2D>();
            submarine = new List<Texture2D>();
            battleship = new List<Texture2D>();
            carrier = new List<Texture2D>();
            mainMenu = new Menu();
            joinMenu = new Menu();
            activeMenu = Menus.Main;
            input = new KeysInput();
            enemy_field = new int[10, 10];
            my_field = new int[10, 10];
            boats_field = new Boat[10, 10];
            places = new List<Place>();
            for (int i = 0; i < 10; i++)
            {
                for (int b = 0; b < 10; b++)
                {
                    boats_field[i,b]=null;
                }
            }
            base.Initialize();
        }
        #region Load
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tiles = new TileMap(graphics.GraphicsDevice.Viewport.Width,graphics.GraphicsDevice.Viewport.Height,LoadTexture("WaterTile"));
            boat.Add(LoadTexture("boat1"));
            boat.Add(LoadTexture("boat2"));
            submarine.Add(LoadTexture("submarine1"));
            submarine.Add(LoadTexture("submarine2"));
            submarine.Add(LoadTexture("submarine3"));
            battleship.Add(LoadTexture("battleship1"));
            battleship.Add(LoadTexture("battleship2"));
            battleship.Add(LoadTexture("battleship3"));
            battleship.Add(LoadTexture("battleship4"));
            carrier.Add(LoadTexture("aircraft1"));
            carrier.Add(LoadTexture("aircraft2"));
            carrier.Add(LoadTexture("aircraft3"));
            carrier.Add(LoadTexture("aircraft4"));
            carrier.Add(LoadTexture("aircraft5"));
            bars = LoadTexture("bars");
            bars_back = LoadTexture("bars_back");
            tile_selected = LoadTexture("tile_selected");
            mouse_texture = LoadTexture("spr_mouse");
            mouse = new MouseCursor(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 100,mouse_texture);
            rect1 = new Rectangle(5 * 32, 2 * 32, 320, 320);
            rect2 = new Rectangle(6 * 32 + bars.Width, 2 * 32, 320, 320);
            font = Content.Load<SpriteFont>("SpriteFont1");
            mainMenu.Add(new Vector2(80, 80), Content.Load<Texture2D>("ButtonStart"));
            joinMenu.Add(new Vector2(80, 80), Content.Load<Texture2D>("ButtonJoin"));
            ipOk = new Button(new Vector2(300, 300), Content.Load<Texture2D>("ButtonOk"));
            txtBox = Content.Load<Texture2D>("TextBox");
            txtBack = Content.Load<Texture2D>("BackText");
            texture_x = LoadTexture("x");
            texture_o = LoadTexture("o");
            cannon_back = LoadTexture("CannonBottom");
            cannon = LoadTexture("Cannon");
            cannonball = LoadTexture("CannonBall");
        }
        #endregion
        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (now == true)
            {
                now = false;
            }
            if (then == false)
            {
                count++;
                if (count >= 5)
                {
                    place = true;
                }
            }
            if (IsActive)
            {
                mouse.UpdateMouse(gameTime);
                input.Update(gameTime);
            }
            switch (activeMenu)
            {
                case Menus.Main:
                    menuChoice = mainMenu.update();
                    if (menuChoice != -1)
                    {
                        if (menuChoice == 0)
                        {
                            activeMenu = Menus.Join;
                        }
                    }
                    break;
                case Menus.Join:
                    menuChoice = joinMenu.update();
                    if (menuChoice != -1)
                    {
                        if (menuChoice == 0)
                        {
                            ipInput = true;
                            textBoxIP = new TextBox(new Vector2(250, 130), txtBox, font, 16, txtBack, "IP to connect:", "localhost");
                        }
                    }
                    break;
            }
            if (ipInput)
            {
                textBoxIP.Update(gameTime);
                if ((rectContains(ipOk.rect, mouse.Position))&&(mouse.LeftClicked()))
                {
                    IP = textBoxIP.Destroy();
                    textBoxIP = null;
                    textBoxName = new TextBox(new Vector2(250, 130), txtBox, font, 16, txtBack, "What is your name", "MyName");
                    getName = true;
                    ipInput = false;
                    now = true;
                }
            }
            if ((getName)&&(now ==false))
            {
                textBoxName.Update(gameTime);
                if ((rectContains(ipOk.rect, mouse.Position)) && (mouse.LeftClicked()))
                {
                    name = textBoxName.Destroy();
                    textBoxName = null;
                    getName = false;
                    #region Internet Stuff
                    // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
                    NetPeerConfiguration Config = new NetPeerConfiguration("game");
                    // Create new client, with previously created configs
                    Client = new NetClient(Config);
                    // Create new outgoing message
                    NetOutgoingMessage outmsg = Client.CreateMessage();
                    //LoginPacket lp = new LoginPacket("Katu");
                    // Start client
                    Client.Start();
                    // Write byte ( first byte informs server about the message type ) ( This way we know, what kind of variables to read)
                    outmsg.Write((byte)PacketTypes.LOGIN);
                    // Write String "Name" . Not used, but just showing how to do it
                    outmsg.Write(name);
                    // Connect client, to ip previously requested from user 
                    Client.Connect(IP, 14242, outmsg);
                    Console.WriteLine("Client Started");
                    // Create the list of characters
                    GameStateList = new List<Character>();
                    // Set timer to tick every 50ms
                    update = new System.Timers.Timer(50);
                    // When time has elapsed ( 50ms in this case ), call "update_Elapsed" funtion
                    update.Elapsed += new System.Timers.ElapsedEventHandler(update_Elapsed);
                    // Funtion that waits for connection approval info from server
                    WaitForStartingInfo();
                    // Start the timer
                    update.Start();
                    #endregion
                    activeMenu = Menus.Game;
                }
            }
            if (activeMenu == Menus.Game)
            {
                if (GameStateList.Count > 1)
                {
                    IsRunning = true;
                    then = false;

                }
            }
            if (IsRunning)
            {
                if ((GameStateList.Count > 1)&&(opponent_name==""))
                {
                    foreach (Character ch in GameStateList)
                    {
                        if (name != ch.Name)
                        {
                            opponent_name = ch.Name;
                        }
                    }
                }
                #region placing;
                    if (rectContains(rect1, mouse.Position))
                    {
                        if ((mouse.LeftClicked()) && (place))
                        {
                            if (canPlace)
                            {
                                switch (currBoat)
                                {
                                    case Boats.Boat:
                                        if (boats < maxBoats)
                                        {
                                            for (int i = 0; i < boat.Count; i++)
                                            {
                                                Color color = Color.White;
                                                Vector2 origin = new Vector2(0, 0);
                                                int x, y, place_x = 0, place_y = 0;
                                                x = 160;
                                                y = 64;
                                                place_x = (int)((mouse.Position.X - 160) / 32);
                                                place_y = (int)((mouse.Position.Y - 64) / 32);
                                                x += place_x * 32;
                                                y += place_y * 32;
                                                switch (rot)
                                                {
                                                    case 0:
                                                        x += i * 32;
                                                        place_x += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x - 1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y+1));
                                                            places.Add(new Place(place_x - 1, place_y-1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == boat.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x + 1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 1:
                                                        x += 32;
                                                        y += i * 32;
                                                        place_y += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y - 1));
                                                            places.Add(new Place(place_x+1, place_y - 1));
                                                            places.Add(new Place(place_x-1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x-1, place_y));
                                                        places.Add(new Place(place_x+1, place_y));
                                                        if (i == boat.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y + 1));
                                                            places.Add(new Place(place_x+1, place_y + 1));
                                                            places.Add(new Place(place_x-1, place_y + 1));
                                                        }
                                                        break;
                                                    case 2:
                                                        x -= i * 32;
                                                        x += 32;
                                                        y += 32;
                                                        place_x -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x+1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == boat.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x-1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 3:
                                                        y -= i * 32;
                                                        y += 32;
                                                        place_y -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y+1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        places.Add(new Place(place_x-1, place_y));
                                                        places.Add(new Place(place_x+1, place_y));
                                                        if (i == boat.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y-1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                }
                                                rotation = MathHelper.ToRadians(rot * 90);
                                                boats_field[place_x, place_y] = new Boat(x, y, boat[i], rotation);
                                            }
                                            boats++;
                                        }
                                        break;
                                    case Boats.Submarine:
                                        if (submarines < maxSubmarine)
                                        {
                                            for (int i = 0; i < submarine.Count; i++)
                                            {
                                                Color color = Color.White;
                                                Vector2 origin = new Vector2(0, 0);
                                                int x, y, place_x = 0, place_y = 0;
                                                x = 160;
                                                y = 64;
                                                place_x = (int)((mouse.Position.X - 160) / 32);
                                                place_y = (int)((mouse.Position.Y - 64) / 32);
                                                x += place_x * 32;
                                                y += place_y * 32;
                                                switch (rot)
                                                {
                                                    case 0:
                                                        x += i * 32;
                                                        place_x += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x-1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == submarine.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x+1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 1:
                                                        x += 32;
                                                        y += i * 32;
                                                        place_y += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y-1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x-1, place_y));
                                                        places.Add(new Place(place_x+1, place_y));
                                                        if (i == submarine.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y+1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        break;
                                                    case 2:
                                                        x -= i * 32;
                                                        x += 32;
                                                        y += 32;
                                                        place_x -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x+1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == submarine.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x-1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 3:
                                                        y -= i * 32;
                                                        y += 32;
                                                        place_y -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y+1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        places.Add(new Place(place_x-1, place_y));
                                                        places.Add(new Place(place_x+1, place_y));
                                                        if (i == submarine.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y-1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                }
                                                rotation = MathHelper.ToRadians(rot * 90);
                                                boats_field[place_x, place_y] = new Boat(x, y, submarine[i], rotation);
                                            }
                                            submarines++;
                                        }
                                        break;
                                    case Boats.Battleship:
                                        if (battleships < maxBattleship)
                                        {
                                            for (int i = 0; i < battleship.Count; i++)
                                            {
                                                Color color = Color.White;
                                                Vector2 origin = new Vector2(0, 0);
                                                int x, y, place_x = 0, place_y = 0;
                                                x = 160;
                                                y = 64;
                                                place_x = (int)((mouse.Position.X - 160) / 32);
                                                place_y = (int)((mouse.Position.Y - 64) / 32);
                                                x += place_x * 32;
                                                y += place_y * 32;
                                                switch (rot)
                                                {
                                                    case 0:
                                                        x += i * 32;
                                                        place_x += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x - 1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == battleship.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x + 1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 1:
                                                        x += 32;
                                                        y += i * 32;
                                                        place_y += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y - 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x - 1, place_y));
                                                        places.Add(new Place(place_x + 1, place_y));
                                                        if (i == battleship.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        break;
                                                    case 2:
                                                        x -= i * 32;
                                                        x += 32;
                                                        y += 32;
                                                        place_x -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x + 1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == battleship.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x - 1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 3:
                                                        y -= i * 32;
                                                        y += 32;
                                                        place_y -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        places.Add(new Place(place_x - 1, place_y));
                                                        places.Add(new Place(place_x + 1, place_y));
                                                        if (i == battleship.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y - 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                }
                                                rotation = MathHelper.ToRadians(rot * 90);
                                                boats_field[place_x, place_y] = new Boat(x, y, battleship[i], rotation);
                                            }
                                            battleships++;
                                        }
                                        break;
                                    case Boats.Aircraft:
                                        if (aircrafts < maxAircraft)
                                        {
                                            for (int i = 0; i < carrier.Count; i++)
                                            {
                                                Color color = Color.White;
                                                Vector2 origin = new Vector2(0, 0);
                                                int x, y, place_x = 0, place_y = 0;
                                                x = 160;
                                                y = 64;
                                                place_x = (int)((mouse.Position.X - 160) / 32);
                                                place_y = (int)((mouse.Position.Y - 64) / 32);
                                                x += place_x * 32;
                                                y += place_y * 32;
                                                switch (rot)
                                                {
                                                    case 0:
                                                        x += i * 32;
                                                        place_x += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x - 1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == carrier.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x + 1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 1:
                                                        x += 32;
                                                        y += i * 32;
                                                        place_y += i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y - 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x - 1, place_y));
                                                        places.Add(new Place(place_x + 1, place_y));
                                                        if (i == carrier.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        break;
                                                    case 2:
                                                        x -= i * 32;
                                                        x += 32;
                                                        y += 32;
                                                        place_x -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x + 1, place_y));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                        }
                                                        places.Add(new Place(place_x, place_y - 1));
                                                        places.Add(new Place(place_x, place_y + 1));
                                                        if (i == carrier.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x - 1, place_y));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                    case 3:
                                                        y -= i * 32;
                                                        y += 32;
                                                        place_y -= i;
                                                        if (i == 0)
                                                        {
                                                            places.Add(new Place(place_x, place_y + 1));
                                                            places.Add(new Place(place_x + 1, place_y + 1));
                                                            places.Add(new Place(place_x - 1, place_y + 1));
                                                        }
                                                        places.Add(new Place(place_x - 1, place_y));
                                                        places.Add(new Place(place_x + 1, place_y));
                                                        if (i == carrier.Count - 1)
                                                        {
                                                            places.Add(new Place(place_x, place_y - 1));
                                                            places.Add(new Place(place_x + 1, place_y - 1));
                                                            places.Add(new Place(place_x - 1, place_y - 1));
                                                        }
                                                        break;
                                                }
                                                rotation = MathHelper.ToRadians(rot * 90);
                                                boats_field[place_x, place_y] = new Boat(x, y, carrier[i], rotation);
                                            }
                                            aircrafts++;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    if (input.JustPressed(Keys.Left))
                {
                    if (rot > 0)
                    {
                        rot--;
                    }
                    else
                    {
                        rot = 3;
                    }
                }
                if (input.JustPressed(Keys.Right))
                {
                    if (rot < 3)
                    {
                        rot++;
                    }
                    else
                    {
                        rot = 0;
                    }
                }
                if (input.JustPressed(Keys.Up))
                {
                    if ((byte)currBoat < 3)
                    {
                        currBoat++;
                    }
                    else
                    {
                        currBoat = Boats.Boat;
                    }
                }
                if (input.JustPressed(Keys.Down))
                {
                    if ((byte)currBoat > 0)
                    {
                        currBoat--;
                    }
                    else
                    {
                        currBoat = Boats.Aircraft;
                    }
                }
                #endregion
                if ((boats == maxBoats) && (submarines == maxSubmarine) && (battleships == maxBattleship) && (aircrafts == maxAircraft)&&(hasBegun==false))
                {
                    hasBegun = true;
                    NetOutgoingMessage outmsg ;
                    outmsg = Client.CreateMessage();
                    outmsg.Write((byte)PacketTypes.BEGIN);
                    Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                }
                if (Begun)
                {
                    if ((myTurn) && (mouse.LeftClicked()) && (rectContains(rect2, mouse.Position)))
                    {
                        attack_x = (int)((mouse.Position.X - 512) / 32);
                        attack_y = (int)((mouse.Position.Y - 64) / 32);
                        if (enemy_field[attack_x, attack_y] == 0)
                        {
                            NetOutgoingMessage outmsg = Client.CreateMessage();
                            outmsg.Write((byte)PacketTypes.ATTACK);
                            outmsg.Write(attack_x);
                            outmsg.Write(attack_y);
                            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                            Vector2 direction = new Vector2(656,464) - new Vector2(512+attack_x*32,64+attack_y*32);
                            my_rotation = (float)Math.Atan2(direction.Y, direction.X)-MathHelper.ToRadians(90);
                        }
                    }
                    bool over = true;
                    for (int i = 0; i < 10; i++)
                    {
                        for (int b = 0; b < 10; b++)
                        {
                            if (boats_field[i, b] != null)
                            {
                                over = false;
                            }
                        }
                    }
                    if (over)
                    {
                        this.Exit();
                    }
                }
            }
            if (input.IsReleased(Keys.Escape))
            {
                Client.Disconnect("RageQuit!!!");
                this.Exit();
            }
            base.Update(gameTime);
        }
        #endregion
        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            tiles.Draw(spriteBatch);
            if (activeMenu == Menus.Game)
            {
                if (GameStateList.Count < 2)
                {
                    spriteBatch.DrawString(font, "Waiting for opponents...",new Vector2(370,208-7), Color.Black);
                }
            }
            if (IsRunning)
            {
                spriteBatch.Draw(bars_back, new Vector2(5 * 32, 2 * 32), Color.White);
                spriteBatch.Draw(bars_back, new Vector2(6 * 32 + bars.Width, 2 * 32), Color.White);
                spriteBatch.Draw(bars, new Vector2(5 * 32, 2 * 32), Color.White);
                spriteBatch.Draw(bars, new Vector2(6 * 32 + bars.Width, 2 * 32), Color.White);
                spriteBatch.DrawString(font, "You: " + name, new Vector2(260, 40), Color.Black);
                spriteBatch.DrawString(font, "Enemy: " + opponent_name, new Vector2(612, 40), Color.Black);
                if (Begun)
                {
                    if (myTurn)
                    {
                        spriteBatch.DrawString(font, "Your Turn", new Vector2(612, 390), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, "Enemy Turn", new Vector2(260, 390), Color.Black);
                    }
                }
                spriteBatch.Draw(cannon_back, new Vector2(256, 416), Color.White);
                spriteBatch.Draw(cannon_back, new Vector2(608, 416), Color.White);
                spriteBatch.Draw(cannon, new Vector2(304, 464), null, Color.White, enemy_rotation, new Vector2(48, 48), 1.0f, SpriteEffects.None, 0);
                spriteBatch.Draw(cannon, new Vector2(656, 464), null, Color.White, my_rotation, new Vector2(48, 48), 1.0f, SpriteEffects.None, 0);
                foreach (Boat bt in boats_field)
                {
                    if (bt != null)
                    {
                        bt.Draw(spriteBatch);
                    }
                }
                #region PlaceBoats
                if (placingBoats)
                {
                    for (int i = 0; i < boat.Count; i++)
                    {
                        spriteBatch.Draw(boat[i], new Vector2(i * 32, 50), Color.White);
                    }
                    for (int i = 0; i < submarine.Count; i++)
                    {
                        spriteBatch.Draw(submarine[i], new Vector2(i * 32, 150), Color.White);
                    }
                    for (int i = 0; i < battleship.Count; i++)
                    {
                        spriteBatch.Draw(battleship[i], new Vector2(i * 32, 250), Color.White);
                    }
                    for (int i = 0; i < carrier.Count; i++)
                    {
                        spriteBatch.Draw(carrier[i], new Vector2(i*32, 350), Color.White);
                    };
                    spriteBatch.DrawString(font, "Boats: "+(maxBoats - boats).ToString(), new Vector2(6, 86), Color.White);
                    spriteBatch.DrawString(font, "Submarines: " + (maxSubmarine - submarines).ToString(), new Vector2(6, 186), Color.White);
                    spriteBatch.DrawString(font, "Battleships: " + (maxBattleship - battleships).ToString(), new Vector2(6, 286), Color.White);
                    spriteBatch.DrawString(font, "Carriers : " + (maxAircraft - aircrafts).ToString(), new Vector2(6, 386), Color.White);
                    if (rectContains(rect1, mouse.Position))
                    {
                        canPlace = true;
                        switch (currBoat)
                        {
                            case Boats.Boat:
                                if (boats < maxBoats)
                                {
                                    for (int i = 0; i < boat.Count; i++)
                                    {
                                        Color color = Color.White;
                                        Vector2 origin = new Vector2(0, 0);
                                        int x, y, place_x, place_y;
                                        x = 160;
                                        y = 64;
                                        place_x = (int)((mouse.Position.X - 160) / 32);
                                        place_y = (int)((mouse.Position.Y - 64) / 32);
                                        x += 32 * place_x;
                                        y += 32 * place_y;
                                        switch (rot)
                                        {
                                            case 0:
                                                x += i * 32;
                                                place_x += i;
                                                if ((x > 448) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 1:
                                                x += 32;
                                                y += i * 32;
                                                place_y += i;
                                                if ((y > 352) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 2:
                                                x -= i * 32;
                                                x += 32;
                                                y += 32;
                                                place_x -= i;
                                                if ((x < 192) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 3:
                                                y -= i * 32;
                                                y += 32;
                                                place_y -= i;
                                                if ((y < 96) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                        }
                                        foreach (Place plc in places)
                                        {
                                            if ((plc.x == place_x)&&(plc.y==place_y))
                                            {
                                                color = Color.Red;
                                                canPlace = false;
                                            }
                                        }
                                        rotation = MathHelper.ToRadians(rot * 90);
                                        spriteBatch.Draw(boat[i], new Vector2(x, y), null, color, rotation, origin, 1.0f, SpriteEffects.None, 0f);
                                    }
                                }
                                break;
                            case Boats.Submarine:
                                if (submarines < maxSubmarine)
                                {
                                    for (int i = 0; i < submarine.Count; i++)
                                    {
                                        Color color = Color.White;
                                        Vector2 origin = new Vector2(0, 0);
                                        int x, y, place_x, place_y;
                                        x = 160;
                                        y = 64;
                                        place_x = (int)((mouse.Position.X - 160) / 32);
                                        place_y = (int)((mouse.Position.Y - 64) / 32);
                                        x += 32 * place_x;
                                        y += 32 * place_y;
                                        switch (rot)
                                        {
                                            case 0:
                                                x += i * 32;
                                                place_x += i;
                                                if ((x > 448) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 1:
                                                x += 32;
                                                y += i * 32;
                                                place_y += i;
                                                if ((y > 352) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 2:
                                                x -= i * 32;
                                                x += 32;
                                                y += 32;
                                                place_x -= i;
                                                if ((x < 192) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 3:
                                                y -= i * 32;
                                                y += 32;
                                                place_y -= i;
                                                if ((y < 96) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                        }
                                        foreach (Place plc in places)
                                        {
                                            if ((plc.x == place_x) && (plc.y == place_y))
                                            {
                                                color = Color.Red;
                                                canPlace = false;
                                            }
                                        }
                                        rotation = MathHelper.ToRadians(rot * 90);
                                        spriteBatch.Draw(submarine[i], new Vector2(x, y), null, color, rotation, origin, 1.0f, SpriteEffects.None, 0f);
                                    }
                                }
                                break;
                            case Boats.Battleship:
                                if (battleships < maxBattleship)
                                {
                                    for (int i = 0; i < battleship.Count; i++)
                                    {
                                        Color color = Color.White;
                                        Vector2 origin = new Vector2(0, 0);
                                        int x, y, place_x, place_y;
                                        x = 160;
                                        y = 64;
                                        place_x = (int)((mouse.Position.X - 160) / 32);
                                        place_y = (int)((mouse.Position.Y - 64) / 32);
                                        x += 32 * place_x;
                                        y += 32 * place_y;
                                        switch (rot)
                                        {
                                            case 0:
                                                x += i * 32;
                                                place_x += i;
                                                if ((x > 448) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 1:
                                                x += 32;
                                                y += i * 32;
                                                place_y += i;
                                                if ((y > 352) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 2:
                                                x -= i * 32;
                                                x += 32;
                                                y += 32;
                                                place_x -= i;
                                                if ((x < 192) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 3:
                                                y -= i * 32;
                                                y += 32;
                                                place_y -= i;
                                                if ((y < 96) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                        }
                                        foreach (Place plc in places)
                                        {
                                            if ((plc.x == place_x) && (plc.y == place_y))
                                            {
                                                color = Color.Red;
                                                canPlace = false;
                                            }
                                        }
                                        rotation = MathHelper.ToRadians(rot * 90);
                                        spriteBatch.Draw(battleship[i], new Vector2(x, y), null, color, rotation, origin, 1.0f, SpriteEffects.None, 0f);
                                    }
                                }
                                break;
                            case Boats.Aircraft:
                                if (aircrafts < maxAircraft)
                                {
                                    for (int i = 0; i < carrier.Count; i++)
                                    {
                                        Color color = Color.White;
                                        Vector2 origin = new Vector2(0, 0);
                                        int x, y, place_x, place_y;
                                        x = 160;
                                        y = 64;
                                        place_x = (int)((mouse.Position.X - 160) / 32);
                                        place_y = (int)((mouse.Position.Y - 64) / 32);
                                        x += 32 * place_x;
                                        y += 32 * place_y;
                                        switch (rot)
                                        {
                                            case 0:
                                                x += i * 32;
                                                place_x += i;
                                                if ((x > 448) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 1:
                                                x += 32;
                                                y += i * 32;
                                                place_y += i;
                                                if ((y > 352) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 2:
                                                x -= i * 32;
                                                x += 32;
                                                y += 32;
                                                place_x -= i;
                                                if ((x < 192) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                            case 3:
                                                y -= i * 32;
                                                y += 32;
                                                place_y -= i;
                                                if ((y < 96) || (boats_field[place_x, place_y] != null))
                                                {
                                                    color = Color.Red;
                                                    canPlace = false;
                                                }
                                                break;
                                        }
                                        foreach (Place plc in places)
                                        {
                                            if ((plc.x == place_x) && (plc.y == place_y))
                                            {
                                                color = Color.Red;
                                                canPlace = false;
                                            }
                                        }
                                        rotation = MathHelper.ToRadians(rot * 90);
                                        spriteBatch.Draw(carrier[i], new Vector2(x, y), null, color, rotation, origin, 1.0f, SpriteEffects.None, 0f);
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    canPlace = false;
                }
                #endregion
                for (int i = 0; i < 10; i++)
                {
                    for (int b = 0; b < 10; b++)
                    {
                        if (my_field[i, b] == 1)
                        {
                            spriteBatch.Draw(texture_x, new Vector2(160 + 32 * i, 64 + 32 * b), Color.White);
                        }
                        else if (my_field[i, b] == 2)
                        {
                            spriteBatch.Draw(texture_o, new Vector2(160 + 32 * i, 64 + 32 * b), Color.White);
                        }
                        if (enemy_field[i, b] == 1)
                        {
                            spriteBatch.Draw(texture_x, new Vector2(512 + 32 * i, 64 + 32 * b), Color.White);
                        }
                        else if (enemy_field[i, b] == 2)
                        {
                            spriteBatch.Draw(texture_o, new Vector2(512 + 32 * i, 64 + 32 * b), Color.White);
                        }
                    }
                }
                if (rectContains(rect2, mouse.Position))
                {
                    spriteBatch.Draw(tile_selected, new Vector2(512, 64) + new Vector2(32 * (int)((mouse.Position.X - 512) / 32), 32 * (int)((mouse.Position.Y - 64) / 32)), Color.White);
                }
            }
            switch (activeMenu)
            {
                case Menus.Main:
                    mainMenu.draw(spriteBatch);
                    break;
                case Menus.Join:
                    joinMenu.draw(spriteBatch);
                    break;
            }
            if (ipInput)
            {
                textBoxIP.Draw(spriteBatch);
                ipOk.Draw(spriteBatch);
            }
            if (getName)
            {
                textBoxName.Draw(spriteBatch);
                ipOk.Draw(spriteBatch);
            }
            mouse.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
        #region Scripts;
        public static bool rectContains(Rectangle rect, Vector2 point)
        {
            bool contain = false;
            if (((int)point.X > rect.X) && ((int)point.X < rect.X + rect.Width) && ((int)point.Y > rect.Y) && ((int)point.Y < rect.Y + rect.Height))
            {
                contain = true;
            }
            return contain;
        }
        public Texture2D LoadTexture(string path)
        {
            return Content.Load<Texture2D>(path);
        }
        /// <summary>
        /// Check for new incoming messages from server
        /// </summary>
        public void CheckServerMessages()
        {
            // Create new incoming message holder
            NetIncomingMessage inc;

            // While theres new messages
            //
            // THIS is exactly the same as in WaitForStartingInfo() function
            // Check if its Data message
            // If its WorldState, read all the characters to list
            while ((inc = Client.ReadMessage()) != null)
            {
                if (inc.MessageType == NetIncomingMessageType.Data)
                {
                    switch(inc.ReadByte())
                    {
                        case (byte)PacketTypes.WORLDSTATE:
                            GameStateList.Clear();
                            int jii = 0;
                            jii = inc.ReadInt32();
                            for (int i = 0; i < jii; i++)
                            {
                                Character ch = new Character();
                                inc.ReadAllProperties(ch);
                                GameStateList.Add(ch);
                            }
                        break;
                        case (byte)PacketTypes.BEGIN:
                            Begun = true;
                            myTurn = inc.ReadBoolean();
                        break;
                        case (byte)PacketTypes.ATTACK:
                        NetOutgoingMessage outmsg = Client.CreateMessage();
                        outmsg.Write((byte)PacketTypes.RETURN);
                        int x, y;
                        x = inc.ReadInt32();
                        y = inc.ReadInt32();
                        if (boats_field[x,y] != null)
                        {
                            outmsg.Write(true);
                            boats_field[x, y] = null;
                            my_field[x, y] = 1;
                        }
                        else
                        {
                            outmsg.Write(false);
                            my_field[x, y] = 2;
                        }
                        Vector2 direction = new Vector2(304, 464) - new Vector2(160+x*32,64+y*32);
                        enemy_rotation = (float)Math.Atan2(direction.Y, direction.X) - MathHelper.ToRadians(90);
                        myTurn = true;
                        Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered, 0);
                        break;
                        case (byte)PacketTypes.RETURN:
                            bool attack = inc.ReadBoolean();
                            if (attack)
                            {
                                enemy_field[attack_x, attack_y] = 1;
                            }
                            else
                            {
                                enemy_field[attack_x, attack_y] = 2;
                            }
                            myTurn = false;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Every 50ms this is fired
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void update_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Check if server sent new messages
            CheckServerMessages();
        }
        // Before main looping starts, we loop here and wait for approval message
        private static void WaitForStartingInfo()
        {
            // When this is set to true, we are approved and ready to go
            bool CanStart = false;

            // New incomgin message
            NetIncomingMessage inc;

            // Loop untill we are approved
            while (!CanStart)
            {

                // If new messages arrived
                if ((inc = Client.ReadMessage()) != null)
                {
                    // Switch based on the message types
                    switch (inc.MessageType)
                    {

                        // All manually sent messages are type of "Data"
                        case NetIncomingMessageType.Data:

                            // Read the first byte
                            // This way we can separate packets from each others
                            if (inc.ReadByte() == (byte)PacketTypes.WORLDSTATE)
                            {
                                // Worldstate packet structure
                                //
                                // int = count of players
                                // character obj * count

                                //Console.WriteLine("WorldState Update");

                                // Empty the gamestatelist
                                // new data is coming, so everything we knew on last frame, does not count here
                                // Even if client would manipulate this list ( hack ), it wont matter, becouse server handles the real list
                                GameStateList.Clear();

                                // Declare count
                                int count = 0;

                                // Read int
                                count = inc.ReadInt32();

                                // Iterate all players
                                for (int i = 0; i < count; i++)
                                {

                                    // Create new character to hold the data
                                    Character ch = new Character();

                                    // Read all properties ( Server writes characters all props, so now we can read em here. Easy )
                                    inc.ReadAllProperties(ch);

                                    // Add it to list
                                    GameStateList.Add(ch);
                                }

                                // When all players are added to list, start the game
                                CanStart = true;
                            }
                            break;

                        default:
                            // Should not happen and if happens, don't care
                            Console.WriteLine(inc.ReadString() + " Strange message");
                            break;
                    }
                }
            }
        }
        #endregion
    }
    class Character
    {
        public string Name { get; set; }
        public NetConnection Connection { get; set; }
        public Character(string name,NetConnection conn)
        {
            Name = name;
            Connection = conn;
        }
        public Character()
        {
        }
    }
    enum PacketTypes
    {
        LOGIN,
        BEGIN,
        WORLDSTATE,
        ATTACK,
        RETURN
    }
}
