using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tanks
{
    public class Terrain
    {
        public static int[] heightMap;

        private static Texture2D texture;
        private static Vector2 position;
        private static Color[] colorData;
        private static Rectangle rect;
        private static int width;
        private static int height;

        public static int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public static Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public static int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public static void Initialize(int terrainWidth,int terrainHeight,GraphicsDevice graphicsDevice)
        {
            Width = terrainWidth;
            Height = terrainHeight;
            colorData = new Color[Width * Height];
            texture = new Texture2D(graphicsDevice, Width, Height);
            position = new Vector2(0, Game.height - Height);
            InitializeHeightMap();
            rect = new Rectangle((int)position.X, (int)position.Y, Width, Height);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.4f);
        }

        private static void InitializeHeightMap()
        {
            heightMap = new int[Width];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    colorData[x + y * Width] = Color.Green;
                }
                heightMap[x] = (int)position.Y;
            }
        }

        public static void DestroyTerrain(Texture2D destructionTexture, Rectangle destructionnRectangle)
        {
            Rectangle intersectRect = MathAid.GetIntersectingRectangle(destructionnRectangle, rect);

            Color[] destructionTextureData = new Color[destructionTexture.Width * destructionTexture.Height];
            destructionTexture.GetData(destructionTextureData);

            Point startPos1 = new Point(intersectRect.X - destructionnRectangle.X, intersectRect.Y - destructionnRectangle.Y);
            Point startPos2 = new Point(intersectRect.X - rect.X, intersectRect.Y - rect.Y);

            for (int x = 0; x < intersectRect.Width; x++)
            {
                for (int y = 0; y < intersectRect.Height; y++)
                {
                    if (destructionTextureData[(startPos1.X + x) + (startPos1.Y + y) * destructionTexture.Width].A != 0)
                    {
                        int X = startPos2.X + x;
                        int Y = startPos2.Y + y;
                        if (colorData[X + Y * Width].A != 0)
                        {
                            colorData[X + (heightMap[X]-(int)Position.Y)*width] = new Color();
                            heightMap[X]++;
                        }
                    }
                }
            }
            texture.SetData(colorData);
        }
    }
}