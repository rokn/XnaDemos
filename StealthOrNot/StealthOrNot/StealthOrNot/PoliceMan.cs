using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public class PoliceMan : Player
    {
        private const float bulletSoundDistance = 3000f;
        private const float laserReactivateDistance = 100f;
        private const int ladderSpeed = 12;

        private Light flashLight;
        private Texture2D flashLightTexture;
        private Vector2 flashLightPosition;
        private Vector2 flashLightOrigin;
        private Vector2 flashLightOffset;

        private int lasersCount;

        private Texture2D climbDownTexture;

        public PoliceMan(Vector2 pos, bool controllable, string name, NetConnection connection)
            : base(pos, controllable, name, connection)
        {
            flashLightTexture = Scripts.LoadTexture(BaseLoadDir + "Flashlight");
            climbDownTexture = Scripts.LoadTexture(BaseLoadDir + "Police_climb_down");
            flashLightOffset = new Vector2(0, handTexture.Height - handOrigin.Y - 10);
            flashLightOrigin = new Vector2();
            flashLight = new Light(flashLightPosition, Color.White * 0.5f, 350, 360, true, 45, 0);

            projType = ProjectileType.GunShot;

            Damage = 4;

            throwablesCount = 3;
            throwableType = ThrowableType.Grenade;

            stepSoundDistance = 1000f;

            lasersCount = 2;

            weaponUseSound.SoundDistance = bulletSoundDistance;
            stepSound.SoundDistance = stepSoundDistance;

            IsInHelicopter = true;
            IsOnLadder = false;
        }

        public override void Load()
        {
            walking = Scripts.LoadTexture(BaseLoadDir + "Police_Walk");

            base.Load();

            handTexture = Scripts.LoadTexture(BaseLoadDir + "Police_Hand");
            secondHandTexture = Scripts.LoadTexture(BaseLoadDir + "Police_Hand2");

            weaponTexture = Scripts.LoadTexture(BaseLoadDir + "Police_Gun");
            weaponOffset = new Vector2(0, handTexture.Height - handOrigin.Y);
            weaponOrigin = new Vector2(weaponTexture.Width / 3, 20);
            weaponFireOffset = new Vector2(weaponTexture.Height / 2 - 6, weaponTexture.Width / 5);
            weaponUseSound.Load("GunShot");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (flashLight.IsOn)
            {
                if (IsOnLadder)
                {
                    flashLight.SwitchState();
                }

                if (!IsInHelicopter)
                {
                    Power -= 0.1f;
                }

                if (Power <= 0)
                {
                    Power = 0;
                    flashLight.SwitchState();
                }
            }

            if (IsOnLadder)
            {
                if (!IsCollidingWithBlocks(new Rectangle(rect.X, rect.Y + ladderSpeed, rect.Width, rect.Height)))
                {
                    UpdatePosition(Position + new Vector2(0, ladderSpeed));
                }
                else
                {
                    IsOnLadder = false;
                }
            }
        }

        public override void Attack()
        {
            if (!IsInHelicopter)
            {
                base.Attack();
            }
        }

        public override void UpdateHand()
        {
            base.UpdateHand();
            flashLightPosition = MathAid.ParentChildTransform(handPosition, flashLightOffset, secondHandRotation);

            flashLight.ChangePosition(MathAid.ParentChildTransform(flashLightPosition, new Vector2(-7, flashLightTexture.Height), secondHandRotation));
            flashLight.StartingAngle = (int)MathHelper.ToDegrees(secondHandRotation) + 90;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsOnLadder)
            {
                spriteBatch.Draw(flashLightTexture, flashLightPosition, null, Color.White, handRotation, flashLightOrigin, 1f, effects, 0.504f);
                base.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(climbDownTexture, Position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0.5f);
            }
        }

        protected override void CheckForInput()
        {
            base.CheckForInput();

            if (Controllable)
            {
                if (!IsInHelicopter)
                {
                    if (IsOnLadder)
                    {
                        if (Main.keyboard.JustPressed(Keys.Space))
                        {
                            IsOnLadder = false;
                        }
                    }
                    else
                    {
                        if (Main.keyboard.JustPressed(Keys.F))
                        {
                            flashLight.SwitchState();
                        }

                        if (Main.keyboard.JustPressed(Keys.Q))
                        {
                            if (isOnGround)
                            {
                                bool reactivatedLaser = false;

                                for (int i = 0; i < Main.lasers.Count; i++)
                                {
                                    if (!Main.lasers[i].IsActivated)
                                    {
                                        if (Vector2.Distance(Position, Main.lasers[i].Position) < laserReactivateDistance)
                                        {
                                            reactivatedLaser = true;
                                            Main.lasers[i].IsActivated = true;

                                            if (Main.HasNetworking)
                                            {
                                                Networking.ReactivateLaser(i, this);
                                            }

                                            break;
                                        }
                                    }
                                }

                                if (!reactivatedLaser)
                                {
                                    if (lasersCount > 0)
                                    {
                                        Vector2 laserPos = new Vector2(Position.X, Position.Y + origin.Y);
                                        Main.lasers.Add(new Laser(laserPos));

                                        if (Main.HasNetworking)
                                        {
                                            Networking.SendLaser(this, laserPos);
                                        }
                                        lasersCount--;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!Helicopter.isMoving)
                    {
                        if (Scripts.KeyIsPressed(Keys.S))
                        {
                            ClimbDown();

                            if (Main.HasNetworking)
                            {
                                Networking.SendClimbDown(this);
                            }
                        }
                    }
                }
            }
        }

        public void ClimbDown()
        {
            IsInHelicopter = false;
            IsOnLadder = true;
            UpdatePosition(Helicopter.Position + new Vector2(112, 145));
        }

        protected override void HandleAnimations()
        {
            base.HandleAnimations();

            if (dir > 0)
            {
                flashLightOffset.X = 0;
                weaponOrigin.X = weaponTexture.Width / 3;
                weaponFireOffset.X = weaponTexture.Height / 2 - 15;
            }
            else
            {
                flashLightOffset.X = -flashLightTexture.Width;
                weaponOrigin.X = weaponTexture.Width - weaponTexture.Width / 3;
                weaponFireOffset.X = -(weaponTexture.Height / 2 - 15);
            }
        }

        protected override bool IsCollidingWithBlocks(Rectangle rectangle)
        {
            bool collided = base.IsCollidingWithBlocks(rectangle);

            if (!collided && IsInHelicopter)
            {
                foreach (Rectangle rect in Helicopter.rects)
                {
                    if (rectangle.Intersects(rect))
                    {
                        return true;
                    }
                }
            }

            return collided;
        }
    }
}