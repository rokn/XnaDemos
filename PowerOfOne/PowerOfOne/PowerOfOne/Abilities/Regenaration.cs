using Microsoft.Xna.Framework;

namespace PowerOfOne
{
    public class Regenaration : Passive
    {
        private const int baseRegeneration = 1;

        public Regenaration()
            : base() { }

        public override void Update(GameTime gameTime)
        {
            if (Activated)
            {
                if (Owner.Health < Owner.MaxHealth)
                {
                    Owner.Health += ((Owner.MaxHealth * (Owner.AbilityPower + baseRegeneration)) / 700);
                }
            }
            base.Update(gameTime);
        }
    }
}