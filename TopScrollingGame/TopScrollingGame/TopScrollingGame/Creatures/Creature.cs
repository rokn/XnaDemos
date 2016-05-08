using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TopScrollingGame
{
    public abstract class Creature
    {
        public Rectangle rect;
        private float health;
        public float beginningSpeed;
        private float maxHealth;
        public Animation fireAnimation;

        private Dictionary<Direction, Animation> walkingAnimation;

        public Creature(Vector2 position)
        {
            Position = position;
            Effects = new List<Effect>();
        }

        public Dictionary<Direction, Animation> WalkingAnimation
        {
            get
            {
                return walkingAnimation;
            }
            protected set
            {
                walkingAnimation = value;
            }
        }

        public List<Effect> Effects { get; set; }

        public int CreatureWidth { get; set; }

        public int CreatureHeight { get; set; }

        public Direction CurrentDirection { get; set; }

        public Texture2D Texture { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position { get; set; }

        public float Speed { get; set; }

        public float Damage { get; set; }

        public float Health
        {
            get
            {
                return health;
            }
            protected set
            {
                health = value;

                if (health > MaxHealth)
                {
                    health = MaxHealth;
                }
            }
        }

        public float MaxHealth
        {
            get
            {
                return maxHealth;
            }
            set
            {
                health += value - maxHealth;
                maxHealth = value;
            }
        }

        public void Regenerate()
        {
            Health += (float)WavesSystem.CurrentWave / 50;
        }

        public virtual void Load()
        {
            CreatureWidth = Texture.Width / 4;
            CreatureHeight = Texture.Height / 4;
            Origin = GetOrigin(CreatureWidth, CreatureHeight);
            rect = InitRect();
            Texture2D FireTexture = Scripts.LoadTexture(@"Sprites\Fire");
            List<Rectangle> FireRects = Scripts.GetSourceRectangles(0, 15, FireTexture.Width / 4, FireTexture.Height / 4, FireTexture);
            fireAnimation = new Animation(FireRects, FireTexture, 3, true);
            walkingAnimation = Scripts.LoadCreatureWalkAnimation(Texture);
            foreach (KeyValuePair<Direction, Animation> kvp in walkingAnimation)
            {
                kvp.Value.stepsPerFrame = 15 - (int)Speed;
                kvp.Value.ChangeAnimatingState(false);
            }
        }

        protected Rectangle InitRect()
        {
            return new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, CreatureWidth, CreatureHeight);
        }

        protected Vector2 GetOrigin(int textureWidth, int textureHeight)
        {
            return new Vector2(textureWidth / 2, textureHeight / 2);
        }

        public virtual void TakeDamage(float damage, bool Bleed = true)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Die();
            }

            if (Bleed)
            {
                Main.ParticleEngine.GenerateBloodEffect(Position, 20);
            }
        }

        protected virtual void Die()
        {
            WavesSystem.Creatures.Remove(this);
        }

        public virtual void ResetEffectStats()
        {
            Speed = beginningSpeed;
        }

        protected virtual void UpdateEffects(GameTime gameTime)
        {
            ResetEffectStats();
            Effects.ForEach(eff => eff.Update(gameTime));
        }

        public virtual void Update(GameTime gameTime)
        {
            rect = MathAid.UpdateRectViaVector(rect, Position - Origin);
            WalkingAnimation[CurrentDirection].Update(Position, 0);
            UpdateEffects(gameTime);

            if (Health <= 0)
            {
                Die();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            WalkingAnimation[CurrentDirection].Draw(spriteBatch, 0.5f, Color.White);

            Effects.ForEach(eff => eff.Draw(spriteBatch));
        }
    }
}