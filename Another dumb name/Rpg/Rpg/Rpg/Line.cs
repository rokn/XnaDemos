using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    class Line
    {
        public Vector2 A;
        public Vector2 B;
        public float Thickness;

        public Line() { }
        public Line(Vector2 a, Vector2 b, float thickness = 1)
        {
            A = a;
            B = b;
            Thickness = thickness;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Vector2 tangent = B - A;
            float rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            const float ImageThickness = 9;
            float thicknessScale = Thickness / ImageThickness;

            Vector2 capOrigin = new Vector2(Rpg.lightningArc.Width, Rpg.lightningArc.Height / 2f);
            Vector2 middleOrigin = new Vector2(0, Rpg.lightningCenter.Height / 2f);
            Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

            spriteBatch.Draw(Rpg.lightningCenter, A, null, color, rotation, middleOrigin, middleScale, SpriteEffects.None, 0.4f);
            spriteBatch.Draw(Rpg.lightningArc, A, null, color, rotation, capOrigin, thicknessScale, SpriteEffects.None, 0.4f);
            spriteBatch.Draw(Rpg.lightningArc, B, null, color, rotation + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0.4f);
        }
    }
}
