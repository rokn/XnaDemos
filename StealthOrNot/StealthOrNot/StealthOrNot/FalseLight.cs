using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public class FalseLight
    {
        private Texture2D texture;
        private Vector2 origin;

        public FalseLight(Vector2 position, Color clr, Vector2 Origin)
        {
            Position = position;
            color = clr;
            IsOn = true;
            origin = Origin;
            Size = 1.0f;
            Main.FalseLights.Add(this);
        }

        public Color color { get; set; }
        public bool IsOn { get; private set; }
        public Vector2 Position { get; private set; }
        public float Rotation { get; set; }
        public float Size { get; set; }

        public void SetTexture(Texture2D newTexture)
        {
            texture = newTexture;
        }

        public void SwitchState()
        {
            IsOn = !IsOn;
        }

        public void ChangePosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsOn)
            {
                if (texture != null)
                {
                    spriteBatch.Draw(texture, Position, null, color, Rotation, origin, Size, SpriteEffects.None, 0.5f);
                }
            }
        }
    }
}