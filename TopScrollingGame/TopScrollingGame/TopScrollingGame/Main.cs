using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TopScrollingGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        public const int playingAreaX = 50;
        public const int playingAreaWidth = 1024;

        //public const int startingBackgroundSpeed = 5;
        private const string mouseSprite = @"Sprites\Cursor";

        private const string heroFolder = @"Sprites\Player\";
        private const float startingHealth = 100;

        public static Hero heroRef;
        public static ContentManager content;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static int width, height;
        public static SpriteFont Font;
        public static List<Projectile> temporaryProjectiles;
        public static GraphicsDeviceManager graphics;
        public static Camera camera;
        public static List<PowerUp> powerUps;
        public static Vector2 baseScreenSize = new Vector2(1920, 1080);
        public static Vector2 ScreenScalingFactor;
        public static SoundEffect backgroundMusic;
        public static SoundEffect bossMusic;
        private static SoundEffectInstance backgroundMusicInstance;
        private static ParticleEngine particleEngine;
        public static Background background;

        private FrameRateCounter frameCounter;
        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;
        private TextBox waveTextBox;
        private bool showTextBox;
        private Matrix globalTransformation;

        #endregion Variables

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            content = Content;
            content.RootDirectory = "Content";

            frameCounter = new FrameRateCounter(this);
            this.Components.Add(frameCounter);
        }

        public static float Health { get; set; }

        public static float MaxHealth { get; set; }

        public static bool IsMuted { get; set; }

        public static GameTime CurrentGameTime { get; private set; }

        public static ParticleEngine ParticleEngine
        {
            get
            {
                return particleEngine;
            }
            private set
            {
                particleEngine = value;
            }
        }

        protected override void Initialize()
        {
            GoFullscreenBorderless();
            showTextBox = false;
            waveTextBox = new TextBox(new Vector2(width / 2, height / 2), 5, "0");
            mouse = new MouseCursor((int)baseScreenSize.X, (int)baseScreenSize.X, 100);
            keyboard = new KeysInput();
            background = new Background();
            camera = new Camera();
            InitializeMusic();
            MaxHealth = startingHealth;
            Health = MaxHealth;
            IsMuted = false;
            MainHelper.Initialize();
            RestartGame();
            heroRef = WavesSystem.Creatures.Find(cr => cr is Hero) as Hero;
            powerUps = new List<PowerUp>();
            base.Initialize();
        }

        public static void InitializeRoom()
        {
            temporaryProjectiles = new List<Projectile>();
            GUI.Initialize();
        }

        private static void InitializeMusic()
        {
            backgroundMusic = content.Load<SoundEffect>(@"Sounds\BackgroundMusic");
            bossMusic = content.Load<SoundEffect>(@"Sounds\LaterLevelsMusic");
            backgroundMusicInstance = backgroundMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Play();
            backgroundMusicInstance.Volume = 0.2f;
        }

        public static void RestartGame()
        {
            WavesSystem.Initialize();
            InitializeRoom();
            Health = MaxHealth;
            background.texture = background.baseTexture;
            ChangeMusic(backgroundMusic);
        }

        public static void RestartAfterMessage()
        {
            if (!GUI.hasMessage)
            {
                RestartGame();
            }
        }

        protected override void LoadContent()
        {
            MainHelper.Load();
            waveTextBox.Load();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("Font");
            mouseTexture = Scripts.LoadTexture(mouseSprite);
            List<Texture2D> particles = new List<Texture2D>() { Scripts.LoadTexture(@"Sprites\Circle") };
            ParticleEngine = new ParticleEngine(particles, new Vector2());
        }

        private void CheckCastleHealth()
        {
            if (Health <= 0)
            {
                RestartGame();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                if (CurrentGameTime == null)
                {
                    CurrentGameTime = gameTime;
                }

                CheckCastleHealth();
                keyboard.Update(gameTime);
                mouse.UpdateMouse(gameTime);

                HandleInput(gameTime);

                WavesSystem.UpdateWaves(gameTime);

                temporaryProjectiles.ForEach(proj => proj.Update(gameTime));
                GUI.Update(gameTime);

                //camera.Rotation += 0.002f;  // UNCOMMENT TO UNLOCK SUPER TRIPPINESS
                powerUps.ForEach(pUp => pUp.Update());

                camera.Update();
                ParticleEngine.Update();
                if (WavesSystem.CurrentWave == 10 && WavesSystem.BossBeaten)
                {
                    RestartAfterMessage();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            CalculateGlobalTransformation();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, globalTransformation);

            background.Draw(spriteBatch);
            WavesSystem.Creatures.ForEach(creature => creature.Draw(spriteBatch));
            temporaryProjectiles.ForEach(proj => proj.Draw(spriteBatch));
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 1f);
            GUI.Draw(spriteBatch);
            particleEngine.Draw(spriteBatch);

            if (showTextBox)
            {
                waveTextBox.Draw(spriteBatch);
            }

            powerUps.ForEach(pUp => pUp.Draw(spriteBatch));

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void CalculateGlobalTransformation()
        {
            Vector3 screenScalingFactor;
            float horScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
            width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float verScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
            height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            ScreenScalingFactor = new Vector2(horScaling, verScaling);

            globalTransformation = Matrix.CreateScale(screenScalingFactor) * Matrix.CreateTranslation(-camera.zeroPos.X, -camera.zeroPos.Y, 0) * Matrix.CreateRotationZ(camera.Rotation) * Matrix.CreateTranslation(camera.zeroPos.X, camera.zeroPos.Y, 0);
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

        private void HandleInput(GameTime gameTime)
        {
            if (keyboard.IsHeld(Keys.Escape))
            {
                this.Exit();
            }
            if (keyboard.JustPressed(Keys.D1))
            {
                WavesSystem.StartWave(1);
            }
            else if (keyboard.JustPressed(Keys.D2))
            {
                WavesSystem.StartWave(2);
            }
            if (keyboard.JustPressed(Keys.D3))
            {
                WavesSystem.StartWave(3);
            }
            else if (keyboard.JustPressed(Keys.D4))
            {
                WavesSystem.StartWave(4);
            }
            if (keyboard.JustPressed(Keys.D5))
            {
                WavesSystem.StartWave(5);
            }
            else if (keyboard.JustPressed(Keys.D6))
            {
                WavesSystem.StartWave(6);
            }
            if (keyboard.JustPressed(Keys.D7))
            {
                WavesSystem.StartWave(7);
            }
            else if (keyboard.JustPressed(Keys.D8))
            {
                WavesSystem.StartWave(8);
            }
            if (keyboard.JustPressed(Keys.D9))
            {
                WavesSystem.StartWave(9);
            }
            else if (keyboard.JustPressed(Keys.E))
            {
                WavesSystem.StartWave(WavesSystem.CurrentWave + 1);
            }
            if (keyboard.JustPressed(Keys.M))
            {
                ChangeMusicState();
            }
            if (keyboard.JustPressed(Keys.Z))
            {
                Random rand = new Random();
                Main.powerUps.Add(new PowerUp(new Vector2(mouse.Position.X, mouse.Position.Y), (EffectType)rand.Next(1, MainHelper.PowerUpsCount)));
            }
            if (keyboard.JustPressed(Keys.F5))
            {
                showTextBox = true;
            }
            if (showTextBox)
            {
                waveTextBox.Update();
                if (keyboard.JustPressed(Keys.Enter))
                {
                    showTextBox = false;
                    WavesSystem.StartWave(int.Parse(waveTextBox.text));
                }
            }
        }

        public static void ChangeMusicState()
        {
            if (IsMuted)
            {
                IsMuted = false;
                backgroundMusicInstance.Resume();
            }
            else
            {
                IsMuted = true;
                backgroundMusicInstance.Pause();
            }
        }

        public static void ChangeMusic(SoundEffect newMusic)
        {
            backgroundMusicInstance.Stop();
            backgroundMusicInstance = newMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Volume = 0.2f;

            if (!IsMuted)
            {
                backgroundMusicInstance.Play();
            }
        }
    }
}