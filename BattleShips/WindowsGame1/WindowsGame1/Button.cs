using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleships
{
    class Button
    {
        public Rectangle rect;
        protected Vector2 position;
        protected Texture2D texture;
        public Button(Vector2 Position, Texture2D Text)
        {
            position = Position;
            texture = Text;
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
