using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TopScrollingGame
{
    public class Hero : Creature, IShooting
    {
        #region Variables

        public const int startingSpeed = 5;
        public const float startingDamage = 10f;
        public const int startingHealth = 100;
        public const int startingAttackSpeed = 2;
        private const bool piercingProjectilesAtStart = false;
        private const string heroFolder = @"Sprites\Player\";
        public const float startingShotSpeed = 15;

        private float attackSpeed;
        private SoundEffect kittenMeow;
        private TimeSpan lastAttack;
        private TimeSpan attackInterval;
        private bool canShoot;

        #endregion Variables

        public Hero(Vector2 position)
            : base(position)
        {
            ShotSpeed = startingShotSpeed;
            AttackSpeed = startingAttackSpeed;
            attackInterval = new TimeSpan(0, 0, 0, 0, (int)(1000 / AttackSpeed));
            canShoot = true;
            Health = startingHealth;
            MaxHealth = startingHealth;
            Damage = startingDamage;
            beginningSpeed = startingSpeed;
            Speed = startingSpeed;
            PiercingProjectiles = piercingProjectilesAtStart;
            Projectiles = new List<Projectile>();
            Effects = new List<Effect>();
            Load();
        }

        public float AttackSpeed
        {
            get
            {
                return attackSpeed;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    attackSpeed = value;
                    attackInterval = new TimeSpan(0, 0, 0, 0, (int)(1000 / attackSpeed));
                }
            }
        }

        public List<Projectile> Projectiles { get; set; }

        public EffectType ProjectileDebuff { get; set; }

        public bool PiercingProjectiles { get; set; }

        public float ShotSpeed { get; set; }

        public override void Load()
        {
            Texture = Scripts.LoadTexture(heroFolder + "Character");
            base.Load();
            WalkingAnimation[Direction.Up].ChangeAnimatingState(false);

            kittenMeow = Main.content.Load<SoundEffect>(@"Sounds\KittenMeow");
        }

        private void HandleInput(GameTime gameTime)
        {
            if (Scripts.KeyIsPressed(Keys.A))
            {
                if (Position.X - Origin.X > Main.playingAreaX)
                {
                    Position = new Vector2(Position.X - Speed, Position.Y);
                    CurrentDirection = Direction.Left;
                    EnableAnimation();
                }
                else
                {
                    CurrentDirection = Direction.Up;
                }
            }

            if (Scripts.KeyIsPressed(Keys.D))
            {
                if (Position.X + Origin.X < Main.playingAreaX + Main.playingAreaWidth)
                {
                    Position = new Vector2(Position.X + Speed, Position.Y);
                    CurrentDirection = Direction.Right;
                    EnableAnimation();
                }
                else
                {
                    CurrentDirection = Direction.Up;
                }
            }

            if (Scripts.KeyIsReleased(Keys.A) && Scripts.KeyIsReleased(Keys.D))
            {
                CurrentDirection = Direction.Up;
            }

            if (canShoot)
            {
                if (Main.mouse.LeftHeld())
                {
                    ShootCat(gameTime);
                }
            }
        }

        protected override void Die()
        {
            Health = MaxHealth;
            Main.RestartGame();
        }

        private void EnableAnimation()
        {
            if (!WalkingAnimation[CurrentDirection].isAnimating)
            {
                WalkingAnimation[CurrentDirection].ChangeAnimatingState(true);
            }
        }

        private void ShootCat(GameTime gameTime)
        {
            canShoot = false;
            lastAttack = gameTime.TotalGameTime;
            Vector2 projDirection = Vector2.Normalize(Main.mouse.Position - Position);
            Projectile proj = new Projectile(Position, projDirection, ShotSpeed, ProjectileType.Cat, this, Damage, 30f, ProjectileDebuff, 0, PiercingProjectiles);
            Projectiles.Add(proj);

            if (!Main.IsMuted)
            {
                kittenMeow.Play(0.1f, 0f, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            base.Update(gameTime);
            Projectiles.ForEach(pr => pr.Update(gameTime));
            CheckIfTimeToAttack(gameTime);
            HandlePowerUpCollision();
        }

        public override void ResetEffectStats()
        {
            ShotSpeed = Hero.startingShotSpeed;
            base.ResetEffectStats();
        }

        protected override void UpdateEffects(GameTime gameTime)
        {
            base.UpdateEffects(gameTime);
        }

        private void HandlePowerUpCollision()
        {
            bool resetEffectDuration = false;

            for (int i = 0; i < Main.powerUps.Count; i++)
            {
                if (rect.Intersects(Main.powerUps[i].rect))
                {
                    for (int b = 0; b < Effects.Count; b++)
                    {
                        if (Effects[b].Type == Main.powerUps[i].Type)
                        {
                            Effects[b].ResetDuration();
                            Main.powerUps.Remove(Main.powerUps[i]);
                            resetEffectDuration = true;
                            break;
                        }
                    }

                    if (!resetEffectDuration)
                    {
                        Main.powerUps[i].Collect();
                        Main.powerUps.Remove(Main.powerUps[i]);
                        i--;
                    }
                }
            }
        }

        private void CheckIfTimeToAttack(GameTime gameTime)
        {
            if (!canShoot)
            {
                if (lastAttack + attackInterval < gameTime.TotalGameTime)
                {
                    canShoot = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Projectiles.ForEach(projectile => projectile.Draw(spriteBatch));
        }
    }
}