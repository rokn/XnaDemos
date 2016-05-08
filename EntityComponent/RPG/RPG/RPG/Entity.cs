using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG
{
    public class Entity
    {
        protected const float RangedProjectileSpeed = 600f;
        protected const int MeleeAttackTimeToStay = 120;
        protected const float MinSpeed = 50f;
        protected const float MaxSpeed = 350f;
        protected Dictionary<Direction, Animation> walkingAnimations;
        protected Rectangle collisionRect;
        protected Rectangle walkingCollisionRect;
        protected int width;
        protected int height;
        protected Vector2 position;
        protected Vector2 origin;
        protected Direction animationDirection;
        protected Vector2 moveDirection;
        protected float moveSpeed;
        protected bool isMoving;
        protected bool isDead;
        protected Vector2 moveTarget;
        protected float moveAngle;
        protected float health;
        protected float maxHealth;
        protected float armor;
        protected Texture2D basicAttackTexture;
        protected Vector2 basicAttackOrigin;
        protected float basicAttackDamage;
        protected float basicAttackSpeed;
        protected float basicAttackRange;
        protected bool isRanged;
        protected Entity basicAttackTarget;
        protected bool isAtacking;
        protected TimeSpan basicAttackTimer;
        protected List<Projectile> projectiles;
        protected TimeSpan meleeBasicTimer;
        protected bool isMeleeShown;
        protected HealthBar healthBar;

        public Entity()
        {
            isDead = false;
            position = new Vector2();
            origin = new Vector2();
            moveDirection = new Vector2();
            animationDirection = Direction.Down;
            moveSpeed = 1f;
            basicAttackTimer = new TimeSpan();
            isRanged = false;
            projectiles = new List<Projectile>();
        }

        protected float Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;

                if (health > maxHealth)
                {
                    health = maxHealth;
                }

                if (health <= 0)
                {
                    Die();
                }

                if(healthBar != null)
                {
                    healthBar.UpdateHealth(health, maxHealth);
                }
            }
        }        

        public virtual void Load()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!CheckBasicAttackTimer())
            {
                UpdateBasicAttackTimer(gameTime);
            }

            if (isMoving)
            {
                UpdatePosition();
            }

            if (isAtacking)
            {
                UpdateBasicAttack(gameTime);
            }

            if(!isRanged)
            {
                if(isMeleeShown)
                {
                    UpdateMeleeTimer(gameTime);

                    if(meleeBasicTimer.TotalMilliseconds <= 0)
                    {
                        isMeleeShown = false;
                    }
                }
            }

            UpdateWalkingAnimation();
            UpdateProjectiles();
        }        

        public virtual void Draw(SpriteBatch spriteBatch)
        {            
            walkingAnimations[animationDirection].Draw(spriteBatch, Color.White, 1.0f, Layers.ENTITY + 0.000001f * position.Y);

            if (Main.ShowBoundingBoxes)
            {
                Scripts.ShowBoundingBox(spriteBatch, collisionRect);
                Scripts.ShowBoundingBox(spriteBatch, walkingCollisionRect);
            }

            DrawProjectiles(spriteBatch);

            if(!isRanged && isMeleeShown)
            {
                DrawMeleeAttack(spriteBatch);
            }

            healthBar.Draw(spriteBatch, Layers.ENTITY + 0.000001f * position.Y);
        }        

        public void DirectTowardsRotation(float rInDegrees)
        {
            if (rInDegrees > -45 && rInDegrees <= 45)
            {
                animationDirection = Direction.Right;
            }
            else if (rInDegrees > 45 && rInDegrees <= 135)
            {
                animationDirection = Direction.Down;
            }
            else if (rInDegrees > 135 || rInDegrees <= -135)
            {
                animationDirection = Direction.Left;
            }
            else if (rInDegrees > -135 && rInDegrees <= -45)
            {
                animationDirection = Direction.Up;
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void Move(Vector2 target)
        {
            SetMoveTarget(target);
            isAtacking = false;
        }

        public void BasicAttack(Entity target)
        {
            isAtacking = true;
            basicAttackTarget = target;

            if (!CheckIfWithinBasicAttackRange())
            {
                SetMoveTarget(basicAttackTarget.GetPosition());
            }
        }

        public Rectangle GetCollisionRect()
        {
            return collisionRect;
        }
       
        public Rectangle GetWalkingRect()
        {
            return walkingCollisionRect;
        }

        public float GetCurrentHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public bool Damage(float damage)
        {
            if(armor >= 0)
            {
                damage *= 100 / (100 + armor);
            }
            else
            {
                damage *= 2 - (100 / (100 - armor));
            }

            Health -= damage;

            return isDead;
        }

        public void Die()
        {
            Main.Entities.Remove(this);
            isDead = true;
        }

        protected void SetWalkingAnimation(string asset)
        {
            walkingAnimations = Scripts.LoadEntityWalkAnimation(Scripts.LoadTexture(asset));

            width = walkingAnimations[Direction.Down].GetWidth();
            height = walkingAnimations[Direction.Down].GetHeight();

            walkingCollisionRect = new Rectangle(0, 0, width, height - height / 3);
            collisionRect = new Rectangle(0, 0, width, height);
            origin = new Vector2(width / 2, height / 2);
            StopAllWalkAnimations();
            UpdateCollisionRect();
            UpdateWalkingRect();
        }

        protected void UpdateBasicAttack(GameTime gameTime)
        {
            if (CheckIfWithinBasicAttackRange())
            {
                if (isMoving)
                {
                    StopMoving();
                }

                if (CheckBasicAttackTimer())
                {
                    DirectTowardsRotation(MathHelper.ToDegrees(MathAid.FindRotation(position, basicAttackTarget.position)));

                    if (isRanged)
                    {                        
                        Projectile proj = new Projectile(this, position, RangedProjectileSpeed, basicAttackTexture);
                        proj.SetHoming(basicAttackTarget);
                        proj.CollideWithEntity += new Projectile.CollideWithEntityHandler(BasicAttackRangedOnHit);
                        projectiles.Add(proj);
                    }
                    else
                    {
                        meleeBasicTimer = new TimeSpan(0, 0, 0, 0, MeleeAttackTimeToStay);
                        isMeleeShown = true;

                        if(basicAttackTarget.Damage(basicAttackDamage))
                        {
                            isAtacking = false;
                        }
                    }

                    ResetBasicAttackTimer();
                }
            }
            else if (!isMoving)
            {
                SetMoveTarget(basicAttackTarget.GetPosition());
            }
        }

        protected void SetMoveTarget(Vector2 newMoveTarget)
        {
            DirectTowardsMouse();
            if (!isMoving)
            {
                isMoving = true;
                walkingAnimations[animationDirection].ChangeAnimatingState(true);
            }

            moveTarget = newMoveTarget;
            moveDirection = MathAid.FindDirection(position, moveTarget);
            moveAngle = MathAid.FindRotation(position, moveTarget);
        }

        protected void UpdatePosition()
        {
            if (!walkingAnimations[animationDirection].isAnimating)
            {
                walkingAnimations[animationDirection].ChangeAnimatingState(true);
            }
            float oldPosition = position.X;
            position.X += moveDirection.X * moveSpeed * Main.ElapsedSeconds;
            UpdateWalkingRect();

            if (!CheckForWalkingCollision())
            {
                UpdateCollisionRect();

                if (collisionRect.Contains(moveTarget))
                {
                    StopMoving();
                }
            }
            else
            {
                position.X = oldPosition;
                UpdateWalkingRect();
            }

            oldPosition = position.Y;
            position.Y += moveDirection.Y * moveSpeed * Main.ElapsedSeconds;
            UpdateWalkingRect();

            if (!CheckForWalkingCollision())
            {
                UpdateCollisionRect();

                if (collisionRect.Contains(moveTarget))
                {
                    StopMoving();
                }
            }
            else
            {
                position.Y = oldPosition;
                UpdateWalkingRect();
            }

            moveDirection = MathAid.FindDirection(position, moveTarget);
            moveAngle = MathAid.FindRotation(position, moveTarget);
            DirectTowardsRotation(MathHelper.ToDegrees(moveAngle));
        }

        protected void StopMoving()
        {
            isMoving = false;
            StopAllWalkAnimations();
            FixBlurness();
        }

        protected void UpdateWalkingRect()
        {
            walkingCollisionRect.X = (int)position.X - (int)origin.X;
            walkingCollisionRect.Y = (int)position.Y - (int)origin.Y + height / 3;
        }

        protected void UpdateCollisionRect()
        {
            collisionRect.X = (int)position.X - (int)origin.X;
            collisionRect.Y = (int)position.Y - (int)origin.Y;
        }

        protected void DirectTowardsMouse()
        {
            float angle = MathAid.FindRotation(position, MyMouse.RealPosition);
            DirectTowardsRotation(MathHelper.ToDegrees(angle));
        }

        protected void UpdateAnimationSpeed()
        {
            float lerpAmount = (moveSpeed - MinSpeed) / (MaxSpeed - MinSpeed);
            int animationSpeed = (int)MathHelper.Lerp(25, 1, lerpAmount);
            walkingAnimations.ForEach(animation => animation.Value.stepsPerFrame = animationSpeed);
        }

        protected void StopAllWalkAnimations()
        {
            walkingAnimations.ForEach(animation => animation.Value.ChangeAnimatingState(false));
        }

        protected bool CheckForWalkingCollision()
        {
            foreach (var rect in Main.currentLevel.blockRects)
            {
                if (walkingCollisionRect.Intersects(rect))
                {
                    return true;
                }
            }

            foreach (var entity in Main.Entities)
            {
                if (entity != this && walkingCollisionRect.Intersects(entity.GetWalkingRect()))
                {
                    return true;
                }
            }

            return false;
        }

        protected void UpdateWalkingAnimation()
        {
            walkingAnimations[animationDirection].Update(position, 0.0f);
        }

        protected void BasicAttackRangedOnHit(Projectile proj, Entity entity)
        {
            projectiles.Remove(proj);

            if(entity.Damage(basicAttackDamage))
            {
                if(isAtacking && entity == basicAttackTarget)
                {
                    isAtacking = false;
                    StopMoving();
                }
            }
        }

        private void FixBlurness()
        {
            position.X = (float)Math.Round(position.X);
            position.Y = (float)Math.Round(position.Y);
        }

        private bool CheckIfWithinBasicAttackRange()
        {
            if (isRanged)
            {
                return Vector2.Distance(position, basicAttackTarget.GetPosition()) <= basicAttackRange;
            }
            else
            {
                return position.Distance(basicAttackTarget.collisionRect) <= basicAttackRange + collisionRect.GetDiagonal()/2;
            }
        }

        private void DrawProjectiles(SpriteBatch spriteBatch)
        {
            projectiles.ForEach(proj => proj.Draw(spriteBatch));
        }

        private void UpdateBasicAttackTimer(GameTime gameTime)
        {
            basicAttackTimer = basicAttackTimer.Subtract(gameTime.ElapsedGameTime);
        }

        private bool CheckBasicAttackTimer()
        {
            return basicAttackTimer.TotalMilliseconds <= 0;
        }

        private void UpdateProjectiles()
        {
            projectiles.ForEach(proj => proj.Update());
        }

        private void ResetBasicAttackTimer()
        {
            basicAttackTimer = new TimeSpan(0, 0, 0, 0, (int)(1000 / basicAttackSpeed));
        }

        private void UpdateMeleeTimer(GameTime gameTime)
        {
            meleeBasicTimer = meleeBasicTimer.Subtract(gameTime.ElapsedGameTime);
        }

        private void DrawMeleeAttack(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(basicAttackTexture, basicAttackTarget.position, null, Color.White, MathHelper.ToDegrees(moveAngle), basicAttackOrigin, 1f, SpriteEffects.None, Layers.MELEEATACKS);
        }
    }
}