using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheRicoshield
{
    public class SolidObject
    {

        public Rectangle Rect { get; protected set; }

        public Vector2 Position { get; protected set; }

        public virtual void CollisonWithBall(Ball ball)
        {
        }

        public virtual void Load(ContentManager Content)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
