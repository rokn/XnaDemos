using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Snake
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        #region Variables

        public const int DefaultChunkSize = 5000;

        private bool exit;

        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;
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
        public static int GridSize;
        private TileMap grid,tiles;
        public static Vector2 playingGroundPosition;
        public static Vector2 playingGroundSize;
        public static Vector2 playingGroundDimensions;
        public static List<MapObject> mapObjects;
        private Snake snake;
        private bool renderedTileMaps;
        private static List<PlayerScore> highscores;
        private static PlayerScore currScore;
        private static bool highscoresShown;

        #endregion Variables

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            GoFullscreenBorderless();
            //Components.Add(new FrameRateCounter(this));
        }

        protected override void Initialize()
        {
            Resources.Load();
            GUI.Initialize();
            playingGroundDimensions = new Vector2(80, 50);
            mapObjects = new List<MapObject>();
            rand = new Random();
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            camera = new Camera();
            BackgroundColor = Color.CornflowerBlue;
            snake = new Snake();
            SpawnApple();            
            renderedTileMaps = false;
            currScore = new PlayerScore("",0);
            highscoresShown = false;
            base.Initialize();
        }        

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GUI.Load();
            mouseTexture = Scripts.LoadTexture("Mouse");
            Font = content.Load<SpriteFont>("Font");
            List<Texture2D> particles = new List<Texture2D>();

            particles.Add(Scripts.LoadTexture(@"Particles\Circle"));
            particles.Add(Scripts.LoadTexture(@"Particles\Line"));
            particles.Add(Scripts.LoadTexture(@"Particles\WhitePixel"));

            particleEngine = new ParticleEngine(particles, new Vector2());

            Texture2D gridTexture = Scripts.LoadTexture(@"Grid");
            Texture2D tilesTexture = Scripts.LoadTexture(@"Tiles");
            GridSize = gridTexture.Width;
            playingGroundPosition = new Vector2(((width / GridSize) / 2 - (playingGroundDimensions.X / 2)) * GridSize, ((height / GridSize) / 2 - (playingGroundDimensions.Y / 2)) * GridSize);
            playingGroundSize = playingGroundDimensions * GridSize;
            
            TileSet.SpriteSheets = new List<Texture2D>();
            TileSet.SpriteSheets.Add(gridTexture);
            TileSet.SpriteSheets.Add(tilesTexture);
            TileSet.TileWidth = GridSize;
            TileSet.TileHeight = GridSize;

            grid = new TileMap(playingGroundPosition, (int)playingGroundDimensions.X + 1, (int)playingGroundDimensions.Y + 1, false);            
            tiles = new TileMap(new Vector2(), width / GridSize + 1, height / GridSize + 1, true);

            for (int i = 0; i < tiles.Width; i++)
            {
                for (int b = 0; b < tiles.Height; b++)
                {
                    tiles.ChangeBaseTile(i, b, 0, 1);
                }
            }

            
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInputs(gameTime);
                HandleMainInput();

                particleEngine.Update();
                snake.Update();
                GUI.Update(gameTime, currScore.Score);
            }

            if (exit)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if(!renderedTileMaps)
            {
                tiles.RenderTileMap(spriteBatch);
                renderedTileMaps = true;
            }
            GraphicsDevice.Clear(BackgroundColor);
            MainDraw();
            GuiDraw();
            base.Draw(gameTime);
        }

        #region Helper Methods

        private void MainDraw()
        {            
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(GraphicsDevice));

            particleEngine.Draw(spriteBatch);
            DrawMapObjects();
            tiles.Draw(spriteBatch, Layer.TileDefault);
            grid.Draw(spriteBatch, Layer.Grid, 0.3f);
            spriteBatch.End();
        }

        public void DrawMapObjects()
        {
            float depth = Layer.SnakeLayer;

            foreach (var part in mapObjects)
            {
                Color color = Color.White;
                if (part is SnakePart)
                {
                    color = Color.Red;
                }
                part.Draw(spriteBatch, color, depth);
                depth -= 0.0000001f;
            }
        }

        private void GuiDraw()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            
            DrawMouse();

            GUI.Draw(spriteBatch);

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
            if(keyboard.JustPressed(Keys.R))
            {
                if(snake.RealDead && !highscoresShown)
                {
                    snake = new Snake();
                    currScore.Score = 0;
                    GUI.RemoveCurrentMessage();
                }
            }

            if(highscoresShown)
            {
                if(keyboard.JustPressed(Keys.Space))
                {
                    highscoresShown = false;
                    GUI.RemoveCurrentMessage();
                    ShowGameOverMsg();
                }
            }
        }

        private void UpdateInputs(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }        

        internal static void RemoveApple(Apple apple)
        {
            apple.Die();
            mapObjects.Remove(apple);
            currScore.Score += 10;
            SpawnApple();
        }

        private static void SpawnApple()
        {
            List<Point> freePlaces = new List<Point>();

            for (int i = 0; i < playingGroundDimensions.X; i++)
            {
                for (int b = 0; b < playingGroundDimensions.Y; b++)
                {
                    freePlaces.Add(new Point(i, b));
                }
            }

            foreach (var obj in mapObjects)
            {
                freePlaces.Remove(obj.Position);
            }

            mapObjects.Add(new Apple(freePlaces[rand.Next(freePlaces.Count)]));
        }

        public static void LoadHighscores()
        {
            if(!File.Exists("Highscores.bin"))
            {
                CreateHighscoresFile();
            }

            highscores = new List<PlayerScore>();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Highscores.bin", FileMode.Open, FileAccess.Read, FileShare.Read);

            for (int i = 0; i < 10; i++)
            {
                PlayerScore score = (PlayerScore)formatter.Deserialize(stream);
                highscores.Add(score);
            }

            stream.Close();
        }

        public static void SaveHighscores()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Highscores.bin", FileMode.Create, FileAccess.Write, FileShare.None);

            for (int i = 0; i < 10; i++)
            {
                formatter.Serialize(stream, highscores[i]);
            }

            stream.Close();
        }

        private static void CreateHighscoresFile()
        {
            PlayerScore score = new PlayerScore("Computer", 0);            

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("Highscores.bin", FileMode.Create, FileAccess.Write, FileShare.None);

            for (int i = 0; i < 10; i++)
			{
                formatter.Serialize(stream, score);
			}

            stream.Close();
        }

        public static void ShowGameOverMsg()
        {
            GUI.AddMessage("GAME OVER\nPress R to restart", Color.Red, true);
        }

        #endregion Helper Methods

        public static bool CheckHighScore()
        {
            LoadHighscores();

            for (int i = 0; i < 10; i++)
			{
			    if(highscores[i].Score < currScore.Score)
                {
                    return true;
                }
			}

            return false;
        }

        public static void GetNameForHighscore()
        {
            currScore.Name = Microsoft.VisualBasic.Interaction.InputBox("Please insert a name for highscores", "YOU SET A NEW HIGHSCORE", "", -1, -1);
        }

        public static void UpdateHighscores()
        {
            for (int i = 0; i < 10; i++)
            {
                if (highscores[i].Score < currScore.Score)
                {
                    highscores.Insert(i, currScore);
                    break;
                }
            }

            SaveHighscores();
        }

        public static void ShowHighscores()
        {
            highscoresShown = true;

            StringBuilder str = new StringBuilder();
            str.Append("Highscores: \n");

            for (int i = 0; i < 10; i++)
            {
                str.Append(i+"."+highscores[i].Name+" : "+highscores[i].Score + "\n");
            }

            str.Append("Press SPACE to continue");

            GUI.AddMessage(str.ToString(), Color.White, true);
        }
    }
}