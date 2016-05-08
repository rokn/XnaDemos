using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheRicoshield
{
    public class Wall : SolidObject
    {
        private bool horizontal;
        private Texture2D texture; 

        public Wall(Vector2 position,bool horizont)
        {
            Position = position;
            horizontal = horizont;
        }

        public override void Load(ContentManager Content)
        {
            if(horizontal)
            {
                texture = Scripts.LoadTexture(@"Walls\HorizontalWall", Content);
            }
            else
            {
                texture = Scripts.LoadTexture(@"Walls\VerticalWall", Content);
            }
            Rect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.3f);
        }
    }
}
