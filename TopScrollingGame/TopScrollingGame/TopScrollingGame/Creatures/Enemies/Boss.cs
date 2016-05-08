using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TopScrollingGame.Creatures.Enemies
{
    public class Boss : RangedEnemy
    {
        private Vector2 Target;
        private Vector2 pentagramOrigin;
        private Texture2D pentagram;
        private bool FiringScythe;
        private int ScytheThrows;
        private TimeSpan ScytheTime;
        private bool IsFiringLazor;
        private Texture2D LazorTexture;
        private TimeSpan LazorTime;
        private Vector2 LazorPosition;
        private Vector2 LazorOrigin;
        private Rectangle LazorRect;
        private int LastAction;
        private bool isHandAttacking;
        private TimeSpan pentagramTime;
        private TimeSpan handTime;
        private Vector2 leftHandPosition;
        private Vector2 rightHandPosition;
        private Texture2D leftHandTexture;
        private Texture2D rightHandTexture;
        private Rectangle leftHandRectangle;
        private Rectangle rightHandRectangle;
        private bool pentagramIsOn;
        private bool handIsOn;

        public Boss(Vector2 position)
            : base(position, EnemyType.Boss)
        {
            CurrentDirection = Direction.Down;
            isWalking = true;
            FiringScythe = false;
            ScytheThrows = 0;
            ScytheTime = new TimeSpan(0, 0, 0, 1);
            Target = Main.heroRef.Position;
            LastAction = 1;
            pentagramTime = new TimeSpan(0, 0, 0, 0, 1500);
            handTime = new TimeSpan(0, 0, 0, 0, 500);
            isHandAttacking = false;
            pentagramIsOn = false;
            handIsOn = false;
        }

        public override void Load()
        {
            base.Load();
            pentagram = Scripts.LoadTexture(spriteFolder + "Pentagram");
            pentagramOrigin = new Vector2(pentagram.Width / 2, pentagram.Height / 2);
            WalkingAnimation[CurrentDirection].ChangeAnimatingState(false);
            WalkingAnimation[Direction.Left].ChangeAnimatingState(false);
            WalkingAnimation[CurrentDirection].Index = 2;
            WalkingAnimation[Direction.Left].Index = 2;
            LazorTexture = Scripts.LoadTexture(spriteFolder + "Lazor");
            LazorOrigin = new Vector2(LazorTexture.Width / 2, 0f);
            LazorRect = new Rectangle(0, 0, LazorTexture.Width, LazorTexture.Height);
            leftHandTexture = WalkingAnimation[Direction.Up].GetCurrentTexture();
            rightHandTexture = WalkingAnimation[Direction.Right].GetCurrentTexture();
        }

        public override void Update(GameTime gameTime)
        {
            if (isWalking)
            {
                Position = new Vector2(Position.X + Speed, Position.Y);
            }

            healthBar.Update(Position - healthBarOffset, Health, MaxHealth);
            rect = MathAid.UpdateRectViaVector(rect, Position - Origin);
            WalkingAnimation[CurrentDirection].Update(Position, 0);
            WalkingAnimation[Direction.Left].Update(Position, 0);
            UpdateEffects(gameTime);

            if (Health <= 0)
            {
                Die();
            }

            Projectiles.ForEach(pr => pr.Update(gameTime));

            AI(gameTime);
        }

        private void AI(GameTime gameTime)
        {
            if (FiringScythe)
            {
                ScytheTime = ScytheTime.Subtract(gameTime.ElapsedGameTime);
                if (ScytheTime.TotalMilliseconds <= 0)
                {
                    FireScythes();
                    ScytheThrows++;
                    ScytheTime = new TimeSpan(0, 0, 0, 1);
                    if (ScytheThrows >= 5)
                    {
                        FiringScythe = false;
                        ScytheThrows = 0;
                        ScytheTime = new TimeSpan(0, 0, 0, 1);
                    }
                }
            }
            else if (isWalking)
            {
                Speed = beginningSpeed * Math.Sign(Target.X - Position.X);

                if (Math.Abs(Main.heroRef.Position.X - Position.X) <= beginningSpeed * 3)
                {
                    isWalking = false;
                }
            }
            else if (IsFiringLazor)
            {
                LazorTime = LazorTime.Subtract(gameTime.ElapsedGameTime);

                if (LazorRect.Intersects(Main.heroRef.rect))
                {
                    Main.heroRef.TakeDamage(Damage / 50, false);
                }

                LazorPosition = Position + new Vector2(0, 20);
                if (LazorTime.TotalMilliseconds <= 0)
                {
                    IsFiringLazor = false;
                    WalkingAnimation[CurrentDirection].Index = 2;
                }
            }
            else if (isHandAttacking)
            {
                if (pentagramIsOn)
                {
                    pentagramTime = pentagramTime.Subtract(gameTime.ElapsedGameTime);

                    if (pentagramTime.TotalMilliseconds <= 0)
                    {
                        pentagramIsOn = false;
                        handIsOn = true;

                        if (leftHandRectangle.Intersects(Main.heroRef.rect))
                        {
                            Main.heroRef.TakeDamage(Damage * 1.2f);
                        }

                        if (rightHandRectangle.Intersects(Main.heroRef.rect))
                        {
                            Main.heroRef.TakeDamage(Damage * 1.2f);
                        }
                    }
                }
                if (handIsOn)
                {
                    handTime = handTime.Subtract(gameTime.ElapsedGameTime);

                    if (handTime.TotalMilliseconds <= 0)
                    {
                        handIsOn = false;
                        isHandAttacking = false;
                        CurrentDirection = Direction.Down;
                    }
                }
            }
            else
            {
                Random rand = new Random();
                int action;

                do
                {
                    action = rand.Next(4);
                } while (action == LastAction);

                switch (action)
                {
                    case 0:
                        FiringScythe = true;
                        break;

                    case 1:
                        isWalking = true;
                        Target = Main.heroRef.Position;
                        break;

                    case 2:
                        FireLazor();
                        break;

                    case 3:
                        StartHandAttack();
                        break;
                }

                LastAction = action;
            }
        }

        private void StartHandAttack()
        {
            isHandAttacking = true;
            Random rand = new Random();
            leftHandPosition = new Vector2(rand.Next(Main.playingAreaX, Main.playingAreaX + Main.playingAreaWidth), Main.heroRef.Position.Y);
            rightHandPosition = new Vector2(rand.Next(Main.playingAreaX, Main.playingAreaX + Main.playingAreaWidth), Main.heroRef.Position.Y);

            leftHandRectangle = Scripts.InitRectangle(leftHandPosition - pentagramOrigin, pentagram);
            rightHandRectangle = Scripts.InitRectangle(rightHandPosition - pentagramOrigin, pentagram);

            pentagramIsOn = true;
            handIsOn = false;
            pentagramTime = new TimeSpan(0, 0, 0, 0, 1000);
            handTime = new TimeSpan(0, 0, 0, 0, 500);
            CurrentDirection = Direction.Left;
        }

        private void FireLazor()
        {
            LazorPosition = Position + new Vector2(0, 60);
            LazorRect = MathAid.UpdateRectViaVector(LazorRect, LazorPosition - LazorOrigin);
            LazorTime = new TimeSpan(0, 0, 0, 3);
            IsFiringLazor = true;
            WalkingAnimation[CurrentDirection].Index = 1;
        }

        private void FireScythes()
        {
            Vector2 offset = new Vector2(60, 25);
            Vector2 firstPosition = new Vector2(Position.X - offset.X, Position.Y + offset.Y);
            Vector2 secondPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
            Vector2 heroPos = Main.heroRef.Position;
            Vector2 direction = Vector2.Normalize(heroPos - firstPosition);

            Projectiles.Add(new Projectile(firstPosition, direction, projectileSpeed, projectileType, this, Damage, projectileAngleVelocity, ProjectileDebuff));

            direction = Vector2.Normalize(heroPos - secondPosition);

            Projectiles.Add(new Projectile(secondPosition, direction, projectileSpeed, projectileType, this, Damage, projectileAngleVelocity, ProjectileDebuff));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsFiringLazor)
            {
                spriteBatch.Draw(LazorTexture, LazorRect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.85f);
            }

            if (pentagramIsOn)
            {
                spriteBatch.Draw(pentagram, leftHandPosition, null, Color.White, 0, pentagramOrigin, 1f, SpriteEffects.None, 0.4f);

                spriteBatch.Draw(pentagram, rightHandPosition, null, Color.White, 0, pentagramOrigin, 1f, SpriteEffects.None, 0.41f);
            }

            if (handIsOn)
            {
                Vector2 Origin = WalkingAnimation[Direction.Right].Origin;
                Origin.Y *= 2;

                spriteBatch.Draw(leftHandTexture, leftHandPosition, null, Color.White, 0, Origin, 1f, SpriteEffects.None, 0.4f);

                spriteBatch.Draw(rightHandTexture, rightHandPosition, null, Color.White, 0, Origin, 1f, SpriteEffects.None, 0.41f);
            }
        }
    }
}