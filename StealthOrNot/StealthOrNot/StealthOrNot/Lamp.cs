using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    [Serializable]
    public class Lamp
    {
        public Vector2 Position;
        public Rectangle rect;

        [NonSerialized]
        private Texture2D texture;

        [NonSerialized]
        private Light light;

        private Vector2 Origin;

        public Lamp(Vector2 position, float size)
        {
            Position = position;
            texture = Scripts.LoadTexture("Lamp");
            Origin = new Vector2(texture.Width / 2, 0);
            rect = Scripts.InitRectangle(Position - Origin * 4, texture.Width * 4, texture.Height);
            light = new Light(position + new Vector2(0, texture.Height - 3), Color.LightYellow * 0.5f, size, 180, false);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, Origin, 1f, SpriteEffects.None, 0.5f);

            if (Main.showBoundingBoxes)
            {
                spriteBatch.Draw(Main.BoundingBox, rect, rect, Color.Black * 0.8f, 0f, new Vector2(), SpriteEffects.None, 0.9f);
            }
        }

        public void Remove()
        {
            Main.Lights.Remove(light);
            Main.Lamps.Remove(this);
        }

        public void SwitchState()
        {
            light.SwitchState();
        }
    }
}