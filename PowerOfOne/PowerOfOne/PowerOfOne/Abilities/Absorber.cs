using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PowerOfOne
{
    public class Absorber : Ability
    {
        private Texture2D laser;
        private bool absorbing;
        private Vector2 target;
        private Rectangle laserRect;
        private Vector2 Origin;
        private float laserRotation;

        public Absorber()
            : base()
        {
            absorbing = false;
        }

        public override void Initialize(Entity owner)
        {
            base.Initialize(owner);
        }

        public override void Load()
        {
            laser = Scripts.LoadTexture(@"Abilities\AbsorberLaser");
            Origin = new Vector2(0, laser.Height / 2);
        }

        public override void ActivateBasicAbility(Vector2 Target)
        {
            Owner.CanWalk = false;
            int LaserWidth = (int)Vector2.Distance(Owner.Position, Target);
            laserRect = Scripts.InitRectangle(Owner.Position, LaserWidth, laser.Height);
            target = Target;
            absorbing = true;
            laserRotation = MathAid.FindRotation(Owner.Position, Target);
        }

        public override void Update(GameTime gameTime)
        {
            if (absorbing)
            {
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (absorbing)
            {
                spriteBatch.Draw(laser, laserRect, null, Color.White, laserRotation, Origin, SpriteEffects.None, 0.19f);
            }
        }
    }
}