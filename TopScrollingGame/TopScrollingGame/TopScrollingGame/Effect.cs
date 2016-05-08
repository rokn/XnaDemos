using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TopScrollingGame.Creatures.Enemies;

namespace TopScrollingGame
{
    public enum EffectType
    {
        None,
        ScratchPost,
        RayGrass,
        Burning,
        Bleed
    }

    public class Effect
    {
        public TimeSpan Duration;
        public Creature Owner;

        public Effect(EffectType type, Creature owner)
        {
            Type = type;
            Duration = GetDuration();
            HasExpired = false;
            Owner = owner;
            GiveTo(owner);
        }

        private TimeSpan GetDuration()
        {
            int milliSeconds = 0;

            switch (Type)
            {
                case EffectType.ScratchPost:
                    milliSeconds = 10000;
                    break;

                case EffectType.RayGrass:
                    milliSeconds = 10000;
                    break;

                case EffectType.Burning:
                    milliSeconds = 3000;
                    break;

                case EffectType.Bleed:
                    milliSeconds = 10000;
                    break;

                default:
                    break;
            }

            return new TimeSpan(0, 0, 0, 0, milliSeconds);
        }

        public bool HasExpired { get; private set; }

        public EffectType Type { get; private set; }

        public void RemoveEffect()
        {
            Owner.Effects.Remove(this);
        }

        public void Update(GameTime gameTime)
        {
            if (Owner.Effects.Find(eff => eff.Type == EffectType.RayGrass) != null && Type == EffectType.ScratchPost)
            {

            }
            if (!HasExpired)
            {
                ApplyEffect();
                Duration = Duration.Subtract(gameTime.ElapsedGameTime);

                if (Duration.TotalMilliseconds <= 0)
                {
                    HasExpired = true;
                }

                switch (Type)
                {
                    case EffectType.Burning:
                        Owner.fireAnimation.Update(Owner.Position, 0f);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                RemoveEffect();
            }
        }

        public void GiveTo(Creature creature)
        {
            Effect sameTypeEffect = creature.Effects.FirstOrDefault(eff => eff.Type == this.Type);
            if (sameTypeEffect != null) //if there already is a effect of this type
            {
                sameTypeEffect.ResetDuration();
            }
            else
            {
                creature.Effects.Add(this);
            }
        }

        private int GetEffectStrength()
        {
            switch (Type)
            {
                case EffectType.ScratchPost:
                    return 3;

                case EffectType.RayGrass:
                    return 10;

                case EffectType.Burning:
                    return WavesSystem.CurrentWave;

                case EffectType.Bleed:
                    return 3;

                default:
                    return 10;
            }
        }

        protected virtual void ApplyEffect()
        {
            switch (Type)
            {
                case EffectType.ScratchPost:
                    Owner.Speed += GetEffectStrength();
                    break;

                case EffectType.RayGrass:
                    (Owner as Hero).ShotSpeed += GetEffectStrength();
                    break;

                case EffectType.Burning:
                    Owner.TakeDamage(GetEffectStrength(), false);
                    break;

                case EffectType.Bleed:
                    Owner.TakeDamage(GetEffectStrength());
                    break;

                default:
                    break;
            }
        }

        internal void ResetDuration()
        {
            Duration = GetDuration();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (Type)
            {
                case EffectType.Burning:
                    Owner.fireAnimation.Draw(spriteBatch, 0.51f, Color.White * 0.8f, Owner.rect);
                    break;

                default:
                    break;
            }
        }
    }
}