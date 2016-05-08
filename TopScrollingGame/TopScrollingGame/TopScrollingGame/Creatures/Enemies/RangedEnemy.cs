using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TopScrollingGame.Creatures.Enemies
{
    public class RangedEnemy : Enemy, IShooting
    {
        private static TimeSpan attackInterval = TimeSpan.FromMilliseconds(2000);
        private TimeSpan lastAttack;
        protected float projectileSpeed;
        protected float projectileAngleVelocity;
        protected float startingProjectileSpeed;
        protected ProjectileType projectileType;

        public RangedEnemy(Vector2 position, EnemyType type)
            : base(position, type)
        {
            Projectiles = new List<Projectile>();
            lastAttack = Main.CurrentGameTime.TotalGameTime;
            Range = MainHelper.EnemiesStats[(int)type].Range;
            startingProjectileSpeed = MainHelper.EnemiesStats[(int)type].ProjectileSpeed;
            projectileSpeed = startingProjectileSpeed;
            projectileAngleVelocity = MainHelper.EnemiesStats[(int)type].ProjectileAngleVelocity;
            projectileType = MainHelper.EnemiesStats[(int)type].projectileType;
            ProjectileDebuff = MainHelper.EnemiesStats[(int)type].projectileDebuff;
            Load();
        }

        public EffectType ProjectileDebuff { get; set; }

        public float Range { get; set; }

        public override void Load()
        {
            base.Load();
        }

        private void Attack()
        {
            var hero = (Hero)WavesSystem.Creatures.FirstOrDefault(cr => cr is Hero);
            Vector2 projectileDirection = Vector2.Normalize(hero.Position - Position);
            float projectileRotation = MathAid.FindRotation(Position, hero.Position);
            Projectile projectile = new Projectile(Position, projectileDirection, projectileSpeed, projectileType, this, Damage, projectileAngleVelocity, ProjectileDebuff, projectileRotation);
            Projectiles.Add(projectile);
        }

        public List<Projectile> Projectiles { get; set; }

        private void CheckAttack(GameTime gameTime)
        {
            if (lastAttack + attackInterval < gameTime.TotalGameTime)
            {
                Attack();
                lastAttack = gameTime.TotalGameTime;
            }
        }

        private void MakeProjectilesTemporary()
        {
            foreach (var projectile in Projectiles)
            {
                Main.temporaryProjectiles.Add(projectile);
            }
        }

        protected override void Die()
        {
            base.Die();
            MakeProjectilesTemporary();
        }

        public override void ResetEffectStats()
        {
            projectileSpeed = startingProjectileSpeed;
            base.ResetEffectStats();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Projectiles.ForEach(pr => pr.Update(gameTime));
            if (IsWithinRange())
            {
                if (isWalking)
                {
                    if (!IsColidingWithOtherEnemy())
                    {
                        isWalking = false;
                        WalkingAnimation[CurrentDirection].ChangeAnimatingState(false);
                    }
                }
                else
                {
                    CheckAttack(gameTime);
                }
            }
            else
            {
                if (!isWalking)
                {
                    isWalking = true;
                    WalkingAnimation[CurrentDirection].ChangeAnimatingState(true);
                }
            }
        }

        private bool IsColidingWithOtherEnemy()
        {
            foreach (var ent in WavesSystem.Creatures)
            {
                if (ent is RangedEnemy && ent != this)
                {
                    if (rect.Intersects(ent.rect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsWithinRange()
        {
            var hero = (Hero)WavesSystem.Creatures.FirstOrDefault(cr => cr is Hero);
            return hero.Position.Y - Position.Y <= Range;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Projectiles.ForEach(pr => pr.Draw(spriteBatch));

            base.Draw(spriteBatch);
        }
    }
}