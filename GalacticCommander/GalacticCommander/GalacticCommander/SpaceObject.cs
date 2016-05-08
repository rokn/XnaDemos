using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalacticCommander
{
    public enum SpaceObjectType
    {
        Star,
        Planet
    }

    public class SpaceObject
    {
        private Vector2 Position;
        private Texture2D texture;
        private SpaceObjectType type;
        private Vector2 Origin;
        private float Depth;
        private float Size;
        private Color color;

        public SpaceObject(SpaceObjectType spaceObejctType, Chunk chunk)
        {
            type = spaceObejctType;
            texture = Resources.SpaceObjects[(int)type];

            switch (type)
            {
                case SpaceObjectType.Star:
                    Size = (float)Main.rand.NextDouble() + 0.2f;
                    Depth = 0.1f - (0.1f - (Size / 100));
                    break;

                case SpaceObjectType.Planet:

                    texture = Resources.SpaceObjects[Main.rand.Next(0, Resources.PlanetsCount)];
                    Size = (float)Main.rand.NextDouble() - 0.1f;
                    if (Size < 0)
                    {
                        Size *= -1;
                    }

                    Depth = 0.11f - (0.1f - (Size / 100));
                    break;

                default:
                    Depth = 0.0f;
                    Size = 0.0f;
                    break;
            }
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            float posX = Main.rand.Next((int)chunk.Position.X + (int)Origin.X, ((int)chunk.Position.X + Main.DefaultChunkSize) - (int)Origin.X);
            float posY = Main.rand.Next((int)chunk.Position.Y + (int)Origin.Y, ((int)chunk.Position.Y + Main.DefaultChunkSize) - (int)Origin.Y);

            Position = new Vector2(posX, posY);

            color = Color.Lerp(Color.Black, Color.White, Size / 0.8f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsWithinCamera())
            {
                spriteBatch.Draw(texture, Position, null, color, 0.0f, Origin, Size, SpriteEffects.None, Depth);
            }
        }

        private bool IsWithinCamera()
        {
            Vector2 camPos = Main.camera.Position;
            return Position.X + Origin.X > camPos.X && Position.X - Origin.X < camPos.X + Main.width && Position.Y + Origin.Y > camPos.Y && Position.Y - Origin.Y < camPos.Y + Main.height;
        }
    }
}