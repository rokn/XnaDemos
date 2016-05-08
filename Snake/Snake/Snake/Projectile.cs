using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake
{
    public class Projectile
    {
        private Line line;
        public Vector2 Position, lineStartOffset, lineEndOffset;
        public Vector2 Direction;
        protected float Speed;
        public Vector2 Origin;
        public float Rotation;
        private Texture2D texture;

        public Projectile(Vector2 position, float rotation, float speed)
        {
            Load();
            Rotation = rotation;
            Position = position;
            Speed = speed;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            lineStartOffset = new Vector2(0, Origin.Y);
            lineStartOffset = new Vector2(texture.Width, Origin.Y);
            Direction = MathAid.AngleToVector(Rotation);
        }

        private void Load()
        {
            texture = Scripts.LoadTexture(@"Projectiles\Projectile_0");
        }

        public void Update()
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            float Remainder = (float)Speed - (float)Math.Truncate(Speed);

            //for (int i = 0; i < Math.Abs((int)Speed); i++)
            //{
            //    Rectangle newRect = new Rectangle(rect.X + horizontalDir, rect.Y, rect.Width, rect.Height);

            //    if (IsCollidingWithBlocks(newRect) || !IsWithinBoundary(newRect))
            //    {
            //        HorizontalCollision();
            //        break;
            //    }
            //    else
            //    {
            //        rect = newRect;
            //        Position.X = rect.X + rectOffset.X;
            //    }
            //}

            //Position.X += horizontalRemainder;
        }
    }
}