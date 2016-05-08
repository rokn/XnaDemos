using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public enum ThrowableType
    {
        Grenade,
        SmokeBomb
    }

    public class Throwable : MoveableObject
    {
        private const int grenadeTimeToExplode = 2000;
        private const int grenadeTimeToDoStuff = 200;

        private const int smokeTimeToExplode = 0;
        private const int smokeTimeToDoStuff = 5000;

        private float BlastRadius;
        private float Damage;
        private Texture2D texture;
        private float Rotation;
        private TimeSpan timeToExplode;
        private Player Owner;
        private bool doingStuff;
        private TimeSpan timeToDoStuff;
        private FalseLight light;
        private ThrowableType type;

        public Throwable(Vector2 startPos, float rotation, float speed, ThrowableType throwableType, Player owner)
            : base(startPos)
        {
            type = throwableType;
            Load();
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            rectOffset = origin;
            rect = Scripts.InitRectangle(Position, texture.Width, texture.Height);
            Rotation = rotation;
            Speed = speed;
            Vector2 dir = MathAid.AngleToVector(rotation + MathHelper.ToRadians(90));
            horizontalSpeed = dir.X * Speed;
            verticalSpeed = dir.Y * Speed;
            gravityForce = Main.GravityForce;

            Owner = owner;
            doingStuff = false;

            if (type == ThrowableType.Grenade)
            {
                BlastRadius = 600;
                Damage = 30;
                timeToExplode = new TimeSpan(0, 0, 0, 0, grenadeTimeToExplode);
                timeToDoStuff = new TimeSpan(0, 0, 0, 0, grenadeTimeToDoStuff);
                light.Size = 1.0f - (float)timeToDoStuff.TotalMilliseconds / 1000f;
            }
            else
            {
                timeToExplode = new TimeSpan(0, 0, 0, 0, smokeTimeToExplode);
                timeToDoStuff = new TimeSpan(0, 0, 0, 0, smokeTimeToDoStuff);
            }
        }

        private void Load()
        {
            if (type == ThrowableType.Grenade)
            {
                texture = Scripts.LoadTexture(@"Projectiles\Grenade");
                Texture2D lightTexture = Scripts.LoadTexture(@"Projectiles\GrenadeLight");
                light = new FalseLight(Position, Color.White, new Vector2(lightTexture.Width / 2, lightTexture.Height / 2));
                light.SetTexture(lightTexture);
                light.SwitchState();
            }
            else
            {
                texture = Scripts.LoadTexture(@"Projectiles\SmokeBomb");
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!CheckIsOnGround())
            {
                verticalSpeed += gravityForce;
            }
            else if ((int)verticalSpeed == 0)
            {
                horizontalSpeed = 0;
            }

            UpdateMovement();

            Position = rectOffset + rect.Location.ToVector();

            Rotation = ((float)Math.Abs(horizontalSpeed) + (float)Math.Abs(verticalSpeed)) / 10 + (float)MathHelper.PiOver2;

            if (doingStuff)
            {
                timeToDoStuff = timeToDoStuff.Subtract(gameTime.ElapsedGameTime);

                if (type == ThrowableType.SmokeBomb)
                {
                    Main.particleEngine.GenerateSmokeEffect(Position + new Vector2(0, 10), 10);
                }

                if (type == ThrowableType.Grenade)
                {
                    light.Size = 1.0f - (float)timeToDoStuff.TotalMilliseconds / 1000f;
                }

                if (timeToDoStuff.TotalMilliseconds <= 0)
                {
                    if (type == ThrowableType.Grenade)
                    {
                        Main.FalseLights.Remove(light);
                    }

                    Owner.Throwables.Remove(this);
                }
            }

            timeToExplode = timeToExplode.Subtract(gameTime.ElapsedGameTime);

            if (!doingStuff)
            {
                if (timeToExplode.TotalMilliseconds <= 0)
                {
                    Explode();
                }
            }
        }

        private void Explode()
        {
            doingStuff = true;

            if (type == ThrowableType.Grenade)
            {
                Main.particleEngine.GenerateExplosionEffect(Position, (int)BlastRadius, 2000);
                light.SwitchState();
                light.ChangePosition(Position);

                foreach (Player p in Main.mainPlayers)
                {
                    if (Vector2.Distance(p.Position, Position) <= BlastRadius)
                    {
                        p.TakeDamage(Damage);
                    }
                }

                foreach (Player p in Main.enemyPlayers)
                {
                    if (Vector2.Distance(p.Position, Position) <= BlastRadius)
                    {
                        p.TakeDamage(Damage);
                    }
                }
            }
            else
            {
                Main.particleEngine.GenerateSmokeEffect(Position + new Vector2(0, 10), 10);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (type == ThrowableType.Grenade && doingStuff)
            {
                return;
            }

            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, origin, 1f, SpriteEffects.None, 0.4f);

            if (Main.showBoundingBoxes)
            {
                spriteBatch.Draw(Main.BoundingBox, rect, rect, Color.Black * 0.8f, 0f, new Vector2(), SpriteEffects.None, 0.9f);
            }
        }

        protected override bool IsCollidingWithBlocks(Rectangle rectangle)
        {
            return base.IsCollidingWithBlocks(rectangle);
        }

        protected override void HorizontalCollision()
        {
            verticalSpeed /= 2;
            horizontalSpeed /= -2;
        }

        protected override void VerticalCollision()
        {
            horizontalSpeed /= 2;
            verticalSpeed /= -2;
        }
    }
}