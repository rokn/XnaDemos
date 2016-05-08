using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    public class Apple : MapObject
    {
        private Texture2D texture;

        public Apple(Point pos)
            : base(pos)
        {
            texture = Resources.Apple;
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth)
        {
            spriteBatch.Draw(texture, Scripts.GetPositionOfGridPlace(Position), null, color, 0.0f, new Vector2(), 1.0f, SpriteEffects.None, depth);
        }

        public void Die()
        {
            Main.particleEngine.GenerateDeathEffect(Scripts.GetPositionOfGridPlace(Position),texture);
        }
    }
}