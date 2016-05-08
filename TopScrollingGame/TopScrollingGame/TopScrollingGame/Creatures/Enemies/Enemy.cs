using Microsoft.Xna.Framework;
using System;

namespace TopScrollingGame.Creatures.Enemies
{
    public enum EnemyType
    {
        AxeMaster,
        Skeleton,
        Scorpion,
        Thief,
        Boss
    }

    public class Enemy : Creature
    {
        protected const string spriteFolder = @"Sprites\Enemies\";

        public const int powerUpPercent = 10;
        protected HealthBar healthBar;
        protected Vector2 healthBarOffset;
        protected bool showHealthBar;
        protected bool isWalking;
        protected EnemyType Type;

        public Enemy(Vector2 position, EnemyType enemyType)
            : base(position)
        {
            Type = enemyType;
            Damage = MainHelper.EnemiesStats[(int)Type].Damage;
            MaxHealth = MainHelper.EnemiesStats[(int)Type].MaxHealth;
            Speed = MainHelper.EnemiesStats[(int)Type].Speed;
            beginningSpeed = Speed;
            Health = MaxHealth;
            showHealthBar = false;
            CurrentDirection = Direction.Down;
            isWalking = true;
            TrailTicks = 3;
        }

        private int TrailTicks { get; set; }

        public override void Load()
        {
            Texture = Scripts.LoadTexture(spriteFolder + MainHelper.EnemiesStats[(int)Type].Asset);
            base.Load();
            healthBarOffset = new Vector2(34, Origin.Y + 18);
            healthBar = new HealthBar(60, Position - healthBarOffset, Color.Green, Color.Red);
            healthBar.Load(true, @"Sprites\Enemies\");
            WalkingAnimation[CurrentDirection].ChangeAnimatingState(true);
        }

        public override void TakeDamage(float damage, bool Bleed = true)
        {
            base.TakeDamage(damage, Bleed);

            if (!showHealthBar)
            {
                showHealthBar = true;
            }
        }

        protected override void Die()
        {
            base.Die();
            Main.ParticleEngine.GenerateDeathEffect(Position - Origin, WalkingAnimation[CurrentDirection].GetCurrentTexture());

            Random rand = new Random();

            if (rand.Next(101) < powerUpPercent)
            {
                Main.powerUps.Add(new PowerUp(Position + Origin, (EffectType)rand.Next(1, MainHelper.PowerUpsCount)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Health == MaxHealth && showHealthBar)
            {
                showHealthBar = false;
            }

            if (isWalking)
            {
                Position = new Vector2(Position.X, Position.Y + Speed);
            }

            if (showHealthBar)
            {
                healthBar.Update(Position - healthBarOffset, Health, MaxHealth);
            }
            if (isWalking)
            {
                if (Speed >= 6)
                {
                    if ((--TrailTicks) <= 0)
                    {
                        TrailTicks = 3;
                        Main.ParticleEngine.GenerateTrailEffect(Position, WalkingAnimation[CurrentDirection].GetCurrentTexture(), 0f, 10, Color.White);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (showHealthBar)
            {
                healthBar.Draw(spriteBatch, 0.9f);
            }

            base.Draw(spriteBatch);
        }
    }
}