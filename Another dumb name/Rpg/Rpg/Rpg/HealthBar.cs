using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    class HealthBar
    {
        private Rectangle source;
        private int maxWidth;
        private Texture2D texture,backGroundTexture;
        private Vector2 Position,Origin,backgroundPosition,backOrigin;
        private Color color, colorFrom, colorTo;
        int type;

        public HealthBar(int MaxWidth,Vector2 position,Color from,Color to)
        {
            Position = position;
            maxWidth = MaxWidth;
            colorFrom = from;
            colorTo = to;
            color = colorFrom;
        }

        public void Load(ContentManager Content,int type)
        {
            this.type = type;
            if (type == 0)
            {
                texture = Scripts.LoadTexture(@"GUI\HealthBar_Bar", Content);
                backGroundTexture = Scripts.LoadTexture(@"GUI\HealthBar_BackGround", Content);
            }
            else if (type == 1)
            {
                texture = Scripts.LoadTexture(@"Enemies\HealthBar", Content);
            }
            Origin = new Vector2(maxWidth/2, texture.Height / 2);
            if (type == 0)
            {
                backgroundPosition = Position + Origin;
                backOrigin = new Vector2(backGroundTexture.Width / 2, backGroundTexture.Height / 2);
            }
            source = new Rectangle(0, 0, maxWidth, texture.Height);
        }

        public void Update(float hp,float maxHp)
        {
            if (hp != maxHp)
            {
                color = Color.Lerp(colorTo, colorFrom, hp / maxHp);
            }
            else
            {
                color = colorFrom;
            }
            source.Width = (int)(maxWidth * (hp / maxHp));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(type == 1)
            {
                Console.WriteLine(type);
            }
            spriteBatch.Draw(texture, Position, source, color, 0, new Vector2(), 1f, SpriteEffects.None, 0.981f);
            if (type == 0)
            {
                spriteBatch.Draw(backGroundTexture, backgroundPosition, null, Color.White, 0, backOrigin, 1f, SpriteEffects.None, 0.98f);
            }
        }
    }
}
