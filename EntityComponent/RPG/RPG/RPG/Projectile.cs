using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RPG
{
    public class Projectile
    {
        public event CollideWithBlockHandler CollideWithBlock;

        public event CollideWithEntityHandler CollideWithEntity;

        public delegate void CollideWithBlockHandler(Projectile proj, Rectangle block);

        public delegate void CollideWithEntityHandler(Projectile proj, Entity entity);

        private float moveSpeed;
        private Texture2D texture;
        private Entity homingTarget;
        private bool isHoming;
        private Vector2 position;
        private Vector2 target;
        private Entity owner;
        private Vector2 direction;
        private float rotation;
        private Vector2 origin;
        private Rectangle collisionRect;        

        public Projectile(Entity entityOwner, Vector2 startPosition, float speed, Texture2D projTexture)
        {
            position = startPosition;
            moveSpeed = speed;
            owner = entityOwner;
            rotation = 0f;
            texture = projTexture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            collisionRect = new Rectangle(0, 0, Math.Min(texture.Width, texture.Height), Math.Min(texture.Width, texture.Height));
            UpdateRectPosition();
        }

        public void SetHoming(Entity targetToHomeOn)
        {
            homingTarget = targetToHomeOn;
            UpdateHomingDirection();
            isHoming = true;
        }

        public void SetTarget(Vector2 newTarget)
        {
            target = newTarget;
            direction = MathAid.FindDirection(position, target);
            rotation = MathAid.FindRotation(position, target);
        }

        public void SetDirection(Vector2 newDirection)
        {
            direction = newDirection;
            rotation = (float)Math.Atan(direction.Y / direction.X);
        }

        public void Update()
        {
            if (isHoming)
            {
                UpdateHomingDirection();
            }

            position += direction * moveSpeed * Main.ElapsedSeconds;
            UpdateRectPosition();
            CheckForCollision();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, Layers.PROJECTILES);

            if(Main.ShowBoundingBoxes)
            {
                Scripts.ShowBoundingBox(spriteBatch, collisionRect);
            }
        }

        private void UpdateHomingDirection()
        {
            direction = MathAid.FindDirection(position, homingTarget.GetPosition());
            rotation = MathAid.FindRotation(position, homingTarget.GetPosition());
        }

        private void CheckForCollision()
        {
            foreach (Rectangle rect in Main.currentLevel.blockRects)
            {
                if (collisionRect.Intersects(rect))
                {
                    if (CollideWithBlock != null)
                    {
                        CollideWithBlock.Invoke(this, rect);
                        return;
                    }
                }
            }

            foreach (Entity entity in Main.Entities)
            {
                if (entity != owner && collisionRect.Intersects(entity.GetCollisionRect()))
                {
                    if (CollideWithEntity != null)
                    {
                        CollideWithEntity(this, entity);
                        return;
                    }
                }
            }
        }

        private void UpdateRectPosition()
        {
            collisionRect.X = (int)position.X - collisionRect.Width / 2;
            collisionRect.Y = (int)position.Y - collisionRect.Height / 2;
        }
    }
}