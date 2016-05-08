using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StealthOrNot
{
    public class Player : MoveableObject
    {
        private enum Foot
        {
            Left,
            Right
        }

        #region Variables

        protected const string BaseLoadDir = @"Player\";

        public Rectangle headRect, bodyRect, legsRect, walkingRect;
        protected Vector2 headOffset, bodyOffset, legsOffset, healthBarOffset;
        protected Texture2D walking;
        protected Animation walkingAnimation;
        public bool isOnGround, CheckForGround, collidedWithGround;
        protected float jumpStrength;
        public int dir;
        protected SpriteEffects effects;

        protected Texture2D handTexture;
        protected Texture2D secondHandTexture;
        protected Vector2 handShoulderPosition;
        protected Vector2 handPosition;
        protected Vector2 handOrigin;
        public float handRotation;
        protected float secondHandRotation;

        protected Texture2D weaponTexture;
        protected Vector2 weaponOffset;
        protected Vector2 weaponPosition;
        protected Vector2 weaponOrigin;
        protected Vector2 weaponFireOffset;
        protected Sound weaponUseSound;
        protected ProjectileType projType;

        public List<Projectile> projectiles;

        public float Health;
        public float MaxHealth;
        protected float Damage;

        protected bool Controllable;
        private FalseLight light;

        public NetConnection Connection;
        public string Name;

        public float Power;
        public float MaxPower;

        protected string PowerText;

        public List<Throwable> Throwables;

        protected int throwablesCount;

        protected ThrowableType throwableType;

        protected float Alpha;

        protected Sound stepSound;
        private Foot previousFoot;
        protected bool playStepSound;
        protected float stepSoundDistance;

        public bool IsInHelicopter;
        protected bool IsOnLadder;

        #endregion Variables

        public Player(Vector2 pos, bool controllable, string name, NetConnection connection)
            : base(pos)
        {
            handShoulderPosition = new Vector2(32, 42);
            handOrigin = new Vector2(14, 6);

            Speed = 8f;
            horizontalSpeed = 0f;
            verticalSpeed = 0f;
            gravityForce = Main.GravityForce;

            jumpStrength = 12f;
            isOnGround = false;
            dir = 1;
            effects = SpriteEffects.None;
            projectiles = new List<Projectile>();
            CheckForGround = true;
            Controllable = controllable;

            Health = 100;
            MaxHealth = 100;

            Name = name;
            Connection = connection;

            Throwables = new List<Throwable>();

            Alpha = 1.0f;

            Power = 200;
            MaxPower = 200;

            previousFoot = Foot.Left;
            playStepSound = true;
            weaponUseSound = new Sound(0, Position, 0.4f);
            stepSound = new Sound(stepSoundDistance, Position, 0.2f);
            collidedWithGround = false;
            IsOnLadder = false;
            Load();
        }

        public virtual void Load()
        {
            origin = new Vector2(32, walking.Height / 2);
            rectOffset = new Vector2(origin.X - 16, origin.Y);
            rect = Scripts.InitRectangle(Position - rectOffset, 36, walking.Height);
            walkingRect = Scripts.InitRectangle(Position - origin, 64, walking.Height);
            List<Rectangle> walkingRects = Scripts.GetSourceRectangles(0, 11, walkingRect.Width, walkingRect.Height, walking);
            walkingAnimation = new Animation(walkingRects, walking, 6, true);
            walkingAnimation.ChangeAnimatingState(false);

            headOffset = new Vector2(-11, -60);
            headRect = Scripts.InitRectangle(Position - headOffset, 25, 25);
            bodyOffset = new Vector2(-11, -34);
            bodyRect = Scripts.InitRectangle(Position - bodyOffset, 25, 48);
            legsOffset = new Vector2(-11, 15);
            legsRect = Scripts.InitRectangle(Position - legsOffset, 25, 48);

            if (Controllable)
            {
                Texture2D lightTexture = Scripts.LoadTexture(BaseLoadDir + "PlayerLight");
                light = new FalseLight(Position, Color.White, new Vector2(lightTexture.Width / 2, lightTexture.Height / 2));
                light.SetTexture(lightTexture);
            }

            stepSound.Load("StepSound");
        }

        public virtual void Update(GameTime gameTime)
        {
            projectiles.ForEach(proj => proj.Update());
            walkingAnimation.Update(Position, 0f);

            if (CheckForGround)
            {
                bool check = CheckIsOnGround();

                if (!isOnGround)
                {
                    if (check)
                    {
                        VerticalCollision();
                    }
                }

                isOnGround = check;
            }

            CheckForInput();

            if (!isOnGround && !IsOnLadder)
            {
                verticalSpeed += gravityForce;
            }

            UpdateMovement();
            UpdateBoundingBoxes();
            HandleAnimations();

            if (Controllable)
            {
                Main.camera.FollowTarget(Position);

                light.ChangePosition(Position);
                Main.EarPosition = Position;
            }

            if (Health <= 0)
            {
                if (Controllable)
                {
                    GUI.AddMessage("Game Over", Color.White, false, 2000);
                }

                if (Main.mainPlayers.Contains(this))
                {
                    Main.mainPlayers.Remove(this);
                }
                else
                {
                    Main.enemyPlayers.Remove(this);
                }
            }

            UpdateHand();

            if (!CheckForGround)
            {
                CheckForGround = true;
            }

            Throwables.ForEach(thr => thr.Update(gameTime));

            UpdateStepSound();
            weaponUseSound.Update(Position);
        }

        private void UpdateStepSound()
        {
            if (Main.keyboard.JustPressed(Keys.Y))
            {
            }
            stepSound.Update(Position);
            if (walkingAnimation.Index == 3)
            {
                if (previousFoot == Foot.Left)
                {
                    previousFoot = Foot.Right;
                    PlayFootSound();
                }
            }
            else if (walkingAnimation.Index == 9)
            {
                if (previousFoot == Foot.Right)
                {
                    previousFoot = Foot.Left;
                    PlayFootSound();
                }
            }
        }

        private void PlayFootSound()
        {
            if (playStepSound)
            {
                stepSound.Play();
            }
        }

        private void UpdateBoundingBoxes()
        {
            headRect.Location = (Position + headOffset).ToPoint();
            bodyRect.Location = (Position + bodyOffset).ToPoint();
            legsRect.Location = (Position + legsOffset).ToPoint();
            walkingRect = MathAid.UpdateRectViaVector(walkingRect, Position - origin);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            projectiles.ForEach(proj => proj.Draw(spriteBatch));

            spriteBatch.Draw(secondHandTexture, handPosition, null, Color.White * Alpha, secondHandRotation, handOrigin, 1f, effects, 0.49f);

            walkingAnimation.Draw(spriteBatch, 0.5f, Color.White * Alpha, walkingRect, effects);

            if (weaponTexture != null)
            {
                spriteBatch.Draw(weaponTexture, weaponPosition, null, Color.White * Alpha, handRotation, weaponOrigin, 1f, effects, 0.505f);
            }

            spriteBatch.Draw(handTexture, handPosition, null, Color.White * Alpha, handRotation, handOrigin, 1f, effects, 0.51f);

            if (Main.showBoundingBoxes)
            {
                spriteBatch.Draw(Main.BoundingBox, rect, rect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                spriteBatch.Draw(Main.BoundingBox, headRect, headRect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                spriteBatch.Draw(Main.BoundingBox, bodyRect, bodyRect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                spriteBatch.Draw(Main.BoundingBox, legsRect, legsRect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
            }

            Throwables.ForEach(thr => thr.Draw(spriteBatch));
        }

        public virtual void UpdateHand()
        {
            handPosition = Position - origin + handShoulderPosition;

            if (Controllable)
            {
                handRotation = MathAid.FindRotation(handPosition, Main.mouse.RealPosition) - MathHelper.ToRadians(90 - 3 * dir);
            }

            secondHandRotation = handRotation - MathHelper.ToRadians(30);
            weaponPosition = MathAid.ParentChildTransform(handPosition, weaponOffset, handRotation);
        }

        protected virtual void CheckForInput()
        {
            if (isOnGround)
            {
                verticalSpeed = 0f;
                if (!collidedWithGround)
                {
                    if (IsCollidingWithBlocks(rect))
                    {
                        UpdatePosition(Position - new Vector2(0, 1));
                    }
                    else
                    {
                        collidedWithGround = true;
                    }
                }
                if (Controllable)
                {
                    if (Scripts.KeyIsPressed(Keys.Space))
                    {
                        isOnGround = false;
                        verticalSpeed = -jumpStrength;
                        collidedWithGround = false;

                        if (Main.HasNetworking)
                        {
                            Networking.SendJump(this);
                        }
                    }
                }
            }
            else
            {
                if (verticalSpeed < 0)
                {
                    if (Main.keyboard.IsReleased(Keys.Space))
                    {
                        verticalSpeed /= 2;
                        if (Main.HasNetworking)
                        {
                            Networking.SendJump(this);
                        }
                    }
                }
            }
            if (Controllable)
            {
                if (!IsOnLadder)
                {
                    horizontalSpeed = (Convert.ToInt32(Scripts.KeyIsPressed(Keys.D)) - Convert.ToInt32(Scripts.KeyIsPressed(Keys.A))) * Speed;
                }

                dir = (int)Math.Sign(Main.mouse.RealPosition.X - Position.X);

                if (!IsInHelicopter)
                {
                    if (Main.mouse.LeftClick())
                    {
                        Attack();

                        if (Main.HasNetworking)
                        {
                            Networking.SendAttack(this);
                        }
                    }

                    if (throwablesCount > 0)
                    {
                        if (Main.mouse.RightClick())
                        {
                            Throw();

                            if (Main.HasNetworking)
                            {
                                Networking.SendThrowable(this);
                            }
                        }
                    }
                }
            }
        }

        public virtual void Attack()
        {
            Vector2 firePosition = MathAid.ParentChildTransform(weaponPosition, weaponFireOffset, handRotation);
            projectiles.Add(new Projectile(firePosition, handRotation, projType, this, 45f, Damage));
            weaponUseSound.Play();
        }

        protected virtual void HandleAnimations()
        {
            if (horizontalSpeed != 0)
            {
                if (isOnGround)
                {
                    if (!walkingAnimation.isAnimating)
                    {
                        walkingAnimation.ChangeAnimatingState(true);
                    }
                }
                else
                {
                    walkingAnimation.ChangeAnimatingState(false);
                }
            }
            else
            {
                walkingAnimation.ChangeAnimatingState(false);
                previousFoot = Foot.Left;
            }

            if (dir > 0)
            {
                effects = SpriteEffects.None;
                handOrigin.X = 6;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
                handOrigin.X = 14;
            }
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
        }

        public void Throw()
        {
            Vector2 firePosition = MathAid.ParentChildTransform(weaponPosition, weaponFireOffset, handRotation);
            Throwables.Add(new Throwable(firePosition, handRotation, 30, throwableType, this));

            throwablesCount--;
        }

        protected override void HitGround()
        {
            PlayFootSound();
            base.HitGround();
        }

        protected override bool IsWithinBoundary(Rectangle rectangle)
        {
            if (IsInHelicopter)
            {
                return true;
            }
            else
            {
                return base.IsWithinBoundary(rectangle);
            }
        }
    }
}