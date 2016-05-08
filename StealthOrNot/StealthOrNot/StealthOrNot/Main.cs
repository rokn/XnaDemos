using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StealthOrNot
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        #region Constatnts

        private const float AmbientLightning = 0.035f;
        private const int levelWidth1 = 80;
        private const int levelHeight1 = 50;
        private const int TileSetsCount = 2;
        public const bool inEditMode = false;
        public static bool showBoundingBoxes;
        public const float GravityForce = 0.5f;
        public const int LampDefaultSize = 350;

        #endregion Constatnts

        #region Main stuff

        public static GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        public static int width, height;
        public static ContentManager content;
        public static SpriteFont Font;
        public static Vector2 EarPosition;

        private static HealthBar healthBar;
        private static Vector2 healthBarOffset;
        private static HealthBar powerBar;
        private static Vector2 powerBarOffset;

        public static string SavePath
        {
            get
            {
                return "Levels";
                //return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StealthOrNot");
            }
        }
        public static string SaveName
        {
            get
            {
                return SavePath + @"\Level1";
            }
        }

        #endregion Main stuff

        #region Input

        public static MouseCursor mouse;
        public static KeysInput keyboard;
        private Texture2D mouseTexture;

        #endregion Input

        #region Game stuff

        public static List<Rectangle> blockRects;
        public static List<Laser> lasers;
        public static Texture2D BoundingBox;
        public static Camera camera;
        public static bool LightsOn;
        public static bool HasStarted;
        public static bool HasNetworking;
        public static bool MusicIsMuted;
        public static Player MainPlayer;
        public static List<Player> mainPlayers;
        public static List<Player> enemyPlayers;
        public static int raysCast;
        public static ParticleEngine particleEngine;
        public static Vector2 HelicopterStartPosition;
        public static Vector2 PoliceManOffset;
        private bool sideChosen;

        #endregion Game stuff

        #region Tile stuff

        public static int currTileset;
        public static TileMap tilemap;

        #endregion Tile stuff

        #region Light stuff

        public static List<Light> Lights;
        public static List<Lamp> Lamps;
        private static RenderTarget2D lightsTarget;
        private static RenderTarget2D mainTarget;
        private static Effect StealthOrNotEffect;
        private PrimitiveBatch primitiveBatch;
        public static List<Line> lines;
        public static List<FalseLight> FalseLights;

        #endregion Light stuff

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

            lines = new List<Line>();
            Lights = new List<Light>();
            blockRects = new List<Rectangle>();
            Lamps = new List<Lamp>();
            lasers = new List<Laser>();
            FalseLights = new List<FalseLight>();
            mainPlayers = new List<Player>();
            enemyPlayers = new List<Player>();

            HasStarted = false;
            HasNetworking = true;
            LightsOn = false;

            tilemap = new TileMap(Vector2.Zero, levelWidth1, levelHeight1);
            TileSet.SpriteSheet = new List<Texture2D>();
            TileSet.TileHeight = 64;
            TileSet.TileWidth = 64;

            if (!inEditMode)
            {
                showBoundingBoxes = false;
                sideChosen = false;
                healthBarOffset = new Vector2(30, 30);
                healthBar = new HealthBar(174, healthBarOffset, Color.Green, Color.Red);
                healthBar.ShowText = true;

                powerBarOffset = new Vector2(250, 30);
                powerBar = new HealthBar(174, powerBarOffset, Color.Yellow, Color.Red);
                powerBar.ShowText = true;
                powerBar.ShowTextInPercent = true;

                HelicopterStartPosition = new Vector2(-4000, 800);
                PoliceManOffset = new Vector2(10, 30);
            }
            else
            {
                currTileset = 0;
                showBoundingBoxes = true;
                HasStarted = true;
                MusicIsMuted = false;
            }

            LoadLevel();
            GUI.Initialize();
            base.Initialize();
        }

        #endregion Initialize

        #region Load

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Light.Device = GraphicsDevice;
            GUI.Load();
            Font = content.Load<SpriteFont>("Font") as SpriteFont;
            mouseTexture = Scripts.LoadTexture("Mouse");

            BoundingBox = Scripts.LoadTexture("WhitePixel");

            particleEngine = new ParticleEngine(new List<Texture2D> { BoundingBox, Scripts.LoadTexture("Circle") }, Vector2.Zero);

            #region Light Stuff

            primitiveBatch = new PrimitiveBatch(GraphicsDevice);
            StealthOrNotEffect = content.Load<Effect>("Effect1");
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(
                GraphicsDevice, width, height);
            mainTarget = new RenderTarget2D(
                GraphicsDevice, width, height);

            #endregion Light Stuff

            LoadTilesets();

            if (inEditMode)
            {
                Editor.Initialize();
                Editor.Load();
            }
            else
            {
                GUI.AddMessage("Press 'P' to be from the police or 'T' to be a terrorist.", Color.White, true);
                healthBar.Load(true, "Player");
                powerBar.Load(true, "Player");
            }
        }

        #endregion Load

        #region Update

        protected override void Update(GameTime gameTime)
        {
            raysCast = 0;
            if (this.IsActive)
            {
                keyboard.Update(gameTime);
                mouse.UpdateMouse(gameTime);
                HandleMainInput();
            }

            GUI.Update(gameTime);

            if (inEditMode)
            {
                EditorUpdate();
            }
            else
            {
                NormalUpdate(gameTime);
            }

            if (LightsOn)
            {
                Lights.ForEach(l => l.Update());
            }

            base.Update(gameTime);
        }

        #endregion Update

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            if (LightsOn)
            {
                LightsDraw();
            }
            else
            {
                MainDraw();
            }

            GuiDraw();

            base.Draw(gameTime);
        }

        #endregion Draw

        #region Helper Methods

        private void NormalUpdate(GameTime gameTime)
        {
            if (HasStarted)
            {
                mainPlayers.ForEach(player => player.Update(gameTime));
                enemyPlayers.ForEach(player => player.Update(gameTime));
                lasers.ForEach(laser => laser.Update());
                healthBar.Update(healthBarOffset, MainPlayer.Health, MainPlayer.MaxHealth);
                powerBar.Update(powerBarOffset, MainPlayer.Power, MainPlayer.MaxPower);
                particleEngine.Update();
                Helicopter.Update();
            }

            if (HasNetworking)
            {
                Networking.Update();
            }
        }

        private void EditorUpdate()
        {
            if (this.IsActive)
            {
                Editor.Update();
                UpdateCamera();

                if (keyboard.JustPressed(Keys.Add))
                {
                    if (currTileset < TileSetsCount - 1)
                    {
                        currTileset++;
                    }
                    else
                    {
                        currTileset = 0;
                    }
                    Editor.Initialize();
                }

                if (keyboard.JustPressed(Keys.Subtract))
                {
                    if (currTileset > 0)
                    {
                        currTileset--;
                    }
                    else
                    {
                        currTileset = TileSetsCount - 1;
                    }
                    Editor.Initialize();
                }
            }
        }

        private void MainDraw()
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            if (HasStarted)
            {
                tilemap.Draw(spriteBatch);

                lasers.ForEach(laser => laser.Draw(spriteBatch));

                if (!inEditMode)
                {
                    mainPlayers.ForEach(player => player.Draw(spriteBatch));
                    enemyPlayers.ForEach(player => player.Draw(spriteBatch));
                }

                Lamps.ForEach(lamp => lamp.Draw(spriteBatch));

                particleEngine.Draw(spriteBatch);

                Helicopter.Draw(spriteBatch);

                if (showBoundingBoxes)
                {
                    foreach (var rect in blockRects)
                    {
                        spriteBatch.Draw(BoundingBox, rect, rect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                    }
                }
            }
            if (inEditMode)
            {
                Editor.Draw(spriteBatch, false);
            }

            spriteBatch.End();
        }

        private void LightsDraw()
        {
            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            Lights.ForEach(l => l.Draw(primitiveBatch, GraphicsDevice));
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));
            FalseLights.ForEach(fl => fl.Draw(spriteBatch));
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(BoundingBox, new Rectangle(0, 0, width, height), Color.White * AmbientLightning);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(mainTarget);
            MainDraw();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.DarkSlateGray);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            StealthOrNotEffect.Parameters["lightMask"].SetValue(lightsTarget);
            StealthOrNotEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, new Vector2(), Color.White);
            spriteBatch.End();
        }

        private void GuiDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            if (MainPlayer is PoliceMan)
            {
                lasers.ForEach(l => l.AlarmDraw(spriteBatch));
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(mouseTexture.Width / 2, mouseTexture.Height / 2), 1f, SpriteEffects.None, 1f);

            if (inEditMode)
            {
                Editor.Draw(spriteBatch, true);
            }

            GUI.Draw(spriteBatch);

            if (HasStarted && !inEditMode)
            {
                healthBar.Draw(spriteBatch, 0.9f);
                powerBar.Draw(spriteBatch, 0.90001f);
            }

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

        private void UpdateCamera()
        {
            float amount = 14;

            if (keyboard.IsHeld(Keys.LeftShift))
            {
                amount *= 2;
            }

            if (keyboard.IsHeld(Keys.LeftControl))
            {
                amount /= 2;
            }

            if (Scripts.KeyIsPressed(Keys.A))
            {
                camera.Move(new Vector2(-amount, 0));
            }

            if (Scripts.KeyIsPressed(Keys.D))
            {
                camera.Move(new Vector2(amount, 0));
            }

            if (Scripts.KeyIsPressed(Keys.W))
            {
                camera.Move(new Vector2(0, -amount));
            }

            if (Scripts.KeyIsPressed(Keys.S))
            {
                camera.Move(new Vector2(0, amount));
            }
        }

        private void HandleMainInput()
        {
            if (IsActive)
            {
                if (keyboard.JustPressed(Keys.Escape))
                {
                    this.Exit();
                }

                if (!inEditMode)
                {
                    if (!sideChosen)
                    {
                        if (keyboard.JustPressed(Keys.P))
                        {
                            MainPlayer = new PoliceMan(HelicopterStartPosition + PoliceManOffset, true, "name0", null);
                            mainPlayers.Add(MainPlayer);
                            GUI.AddMessage("Press 'C' to create a server or 'J' to join a game.", Color.White, true);
                            sideChosen = true;
                        }
                        else if (keyboard.JustPressed(Keys.T))
                        {
                            MainPlayer = new Stealther(new Vector2(3000, 1400), true, "name0", null);
                            mainPlayers.Add(MainPlayer);
                            GUI.AddMessage("Press 'C' to create a server or 'J' to join a game.", Color.White, true);
                            sideChosen = true;
                        }
                    }
                    if (HasNetworking && sideChosen)
                    {
                        if (keyboard.JustPressed(Keys.C))
                        {
                            Networking.InitializeServer();
                            GUI.AddMessage("Press 'S' to start the game.", Color.White, true);
                        }
                        else if (keyboard.JustPressed(Keys.J))
                        {
                            Networking.InitializeClient("37.157.138.76");
                        }
                    }
                }

                if (keyboard.JustPressed(Keys.F1))
                {
                    HasNetworking = false;
                    HasStarted = true;
                    GUI.AddMessage("", Color.White, false);
                    MainPlayer = new Stealther(new Vector2(3000, 1400), true, "name0", null);
                    enemyPlayers.Add(new PoliceMan(HelicopterStartPosition + PoliceManOffset, false, "name1", null));
                    mainPlayers.Add(MainPlayer);
                    Helicopter.Initialize(HelicopterStartPosition, new Vector2(150, HelicopterStartPosition.Y));
                }

                if (!HasStarted && Networking.IsInitialized && Networking.IsHost)
                {
                    if (keyboard.JustPressed(Keys.S))
                    {
                        Networking.Start();
                        GUI.AddMessage("", Color.White, false);
                    }
                }

                if (keyboard.JustPressed(Keys.F3))
                {
                    LightsOn = !LightsOn;
                }

                if (keyboard.JustPressed(Keys.F2))
                {
                    showBoundingBoxes = !showBoundingBoxes;
                }

                if (keyboard.JustPressed(Keys.M))
                {
                    MusicIsMuted = !MusicIsMuted;
                }
            }
        }

        private void LoadLevel()
        {
            if (File.Exists(SaveName + ".bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SaveName + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);

                for (int i = 0; i < Main.tilemap.Width; i++)
                {
                    for (int b = 0; b < Main.tilemap.Height; b++)
                    {
                        try
                        {
                            tilemap.tileMap[i, b] = (TileCell)formatter.Deserialize(stream);
                        }
                        catch (SerializationException)
                        {
                            break;
                        }
                    }
                }

                stream.Close();

                Stream rectStream = new FileStream(SaveName + "Rects.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                blockRects = (List<Rectangle>)formatter.Deserialize(rectStream);
                rectStream.Close();

                Stream lampStream = new FileStream(Main.SaveName + "Lamps.bin", FileMode.Open, FileAccess.Read, FileShare.Read);

                if (lampStream.Position != lampStream.Length)
                {
                    List<Vector2> LampPositions = (List<Vector2>)formatter.Deserialize(lampStream);
                    LampPositions.ForEach(pos => Lamps.Add(new Lamp(pos, LampDefaultSize)));
                }
                lampStream.Close();
            }
        }

        private void LoadTilesets()
        {
            for (int i = 0; i < TileSetsCount; i++)
            {
                TileSet.SpriteSheet.Add(Scripts.LoadTexture(@"TileSets\Tileset_" + i.ToString()));
            }
        }

        #endregion Helper Methods
    }
}