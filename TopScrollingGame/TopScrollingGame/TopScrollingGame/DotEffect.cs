using Microsoft.Xna.Framework;
using System;

namespace TopScrollingGame
{
    public class DotEffect : Effect
    {
        private TimeSpan tickTime;

        public DotEffect(EffectType type, Creature owner)
            : base(type, owner)
        {
            tickTime = GetTickTime();
        }

        private TimeSpan GetTickTime()
        {
            int milliSeconds = 0;

            switch (Type)
            {
                case EffectType.Burning:
                    milliSeconds = 500;
                    break;

                case EffectType.Bleed:
                    milliSeconds = 1000;
                    break;

                default:
                    break;
            }

            return new TimeSpan(0, 0, 0, 0, milliSeconds);
        }

        protected override void ApplyEffect()
        {
            tickTime = tickTime.Subtract(Main.CurrentGameTime.ElapsedGameTime);

            if (tickTime.Milliseconds <= 0)
            {
                tickTime = GetTickTime();
                base.ApplyEffect();
            }
        }
    }
}