using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TopScrollingGame
{
    public class PowerUp
    {
        public const int verticalSpeed = 4;

        public Rectangle rect;
        private Texture2D texture;
        private Texture2D baseTexture;

        public PowerUp(Vector2 position, EffectType type)
        {
            baseTexture = MainHelper.PowerUpTextures[0];
            texture = MainHelper.PowerUpTextures[(int)type];
            Vector2 Origin = new Vector2(baseTexture.Width, baseTexture.Height);
            rect = Scripts.InitRectangle(position - Origin, baseTexture);
            Type = type;
        }

        public EffectType Type { get; set; }

        public void Update()
        {
            rect.Y += verticalSpeed;

            if (rect.Y > GUI.castlePosition.Y)
            {
                Main.powerUps.Remove(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(baseTexture, rect, null, Color.White, 0f, new Vector2(), SpriteEffects.None, 0.6f);
            spriteBatch.Draw(texture, rect, null, Color.White, 0f, new Vector2(), SpriteEffects.None, 0.62f);
        }

        public void Collect()
        {
            new Effect(Type, Main.heroRef);
        }
    }
}