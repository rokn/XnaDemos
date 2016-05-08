using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace TopScrollingGame.Creatures.Enemies
{
    public class MeleeEnemy : Enemy
    {
        public MeleeEnemy(Vector2 position, EnemyType type)
            : base(position, type)
        {
            Load();
        }

        public bool HasHitHero { get; set; }

        public override void Load()
        {
            Texture = Scripts.LoadTexture(@"Sprites\Enemies\Skeleton");
            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleCollision();

            Hero hero = WavesSystem.Creatures.First(cr => cr is Hero) as Hero;

            if (rect.Intersects(hero.rect) && !HasHitHero)
            {
                hero.TakeDamage(Damage);
                HasHitHero = true;
            }
        }

        protected override void Die()
        {
            base.Die();
        }

        private void HandleCollision()
        {
            if (Position.Y > GUI.castlePosition.Y)
            {
                Main.camera.ShakeCamera(0.05f);
                Die();
                Main.Health -= Damage;
            }
        }
    }
}