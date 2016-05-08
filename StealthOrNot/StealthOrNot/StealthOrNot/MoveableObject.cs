using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public abstract class MoveableObject
    {
        public Vector2 Position, rectOffset;
        public Rectangle rect;
        public double horizontalSpeed;
        public double verticalSpeed;
        protected float Speed;
        protected float gravityForce;
        public Vector2 origin;

        public MoveableObject(Vector2 pos)
        {
            Position = pos;
        }

        protected void UpdateMovement()
        {
            float horizontalRemainder = (float)horizontalSpeed - (float)Math.Truncate(horizontalSpeed);
            float verticalRemainder = (float)verticalSpeed - (float)Math.Truncate(verticalSpeed);
            int horizontalDir = Math.Sign(horizontalSpeed);
            int verticalDir = Math.Sign(verticalSpeed);

            for (int i = 0; i < Math.Abs((int)horizontalSpeed); i++)
            {
                Rectangle newRect = new Rectangle(rect.X + horizontalDir, rect.Y, rect.Width, rect.Height);

                if (IsCollidingWithBlocks(newRect) || !IsWithinBoundary(newRect))
                {
                    HorizontalCollision();
                    break;
                }
                else
                {
                    rect = newRect;
                    Position.X = rect.X + rectOffset.X;
                }
            }

            Position.X += horizontalRemainder;

            for (int i = 0; i < Math.Abs((int)verticalSpeed); i++)
            {
                Rectangle newRect = new Rectangle(rect.X, rect.Y + verticalDir, rect.Width, rect.Height);

                if (IsCollidingWithBlocks(newRect) || !IsWithinBoundary(newRect))
                {
                    VerticalCollision();
                    break;
                }
                else
                {
                    rect = newRect;
                    Position.Y = rect.Y + rectOffset.Y;
                }
            }

            Position.Y += verticalRemainder;
        }

        protected virtual void VerticalCollision()
        {
            if (verticalSpeed > 0)
            {
                HitGround();
            }
            verticalSpeed = 0;
        }

        protected virtual void HorizontalCollision()
        {
            horizontalSpeed = 0;
        }

        protected virtual bool IsCollidingWithBlocks(Rectangle rectangle)
        {
            foreach (var block in Main.blockRects)
            {
                if (rectangle.Intersects(block))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool CheckIsOnGround()
        {
            Rectangle newRect = new Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height);
            return IsCollidingWithBlocks(newRect);
        }

        protected virtual bool IsWithinBoundary(Rectangle rectangle)
        {
            return rectangle.X > 0 && rectangle.X + rectangle.Width < Main.tilemap.Width * TileSet.TileWidth && rectangle.Y > 0 && rectangle.Y + rectangle.Height < Main.tilemap.Height * TileSet.TileHeight;
        }

        protected virtual void HitGround()
        {
        }

        public virtual void UpdatePosition(Vector2 newPosition)
        {
            Position = newPosition;
            rect = Scripts.InitRectangle(Position - rectOffset, rect.Width, rect.Height);
        }
    }
}