using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleships
{
    class Boat
    {
        float rotation;
        int x,y;
        public Texture2D textures;
        public Boat(int X, int Y, Texture2D txture,float rot)
        {
            x = X;
            y = Y;
            textures = txture;
            rotation=rot;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures, new Vector2(x, y), null, Color.White, rotation, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
        }
    }
}
