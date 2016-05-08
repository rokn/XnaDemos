using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public class Stealther : Player
    {
        private const float CrossbowSoundDistance = 1500f;
        private bool isStealthed;

        public Stealther(Vector2 pos, bool controllable, string name, NetConnection connection)
            : base(pos, controllable, name, connection)
        {
            projType = ProjectileType.CrossbowShot;

            Damage = 1;

            throwablesCount = 3;
            throwableType = ThrowableType.SmokeBomb;

            isStealthed = false;
            stepSoundDistance = 500f;

            weaponUseSound.SoundDistance = CrossbowSoundDistance;
            stepSound.SoundDistance = stepSoundDistance;

            IsInHelicopter = false;
        }

        public override void Load()
        {
            walking = Scripts.LoadTexture(BaseLoadDir + "Terrorist_Walk");

            base.Load();

            handTexture = Scripts.LoadTexture(BaseLoadDir + "Terrorist_Hand");
            secondHandTexture = Scripts.LoadTexture(BaseLoadDir + "Terrorist_Hand2");

            weaponTexture = Scripts.LoadTexture(BaseLoadDir + "Crossbow");
            weaponOffset = new Vector2(0, handTexture.Height - handOrigin.Y);
            weaponOrigin = new Vector2(weaponTexture.Width / 3, 20);
            weaponFireOffset = new Vector2(weaponTexture.Height / 2 - 15, weaponTexture.Width / 5);

            weaponUseSound.Load("CrossbowShot");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            StealthUpdate();
            UpdateSteps();

            if (Power < MaxPower)
            {
                Power += 0.5f;
            }
            else if (Power > MaxPower)
            {
                Power = MaxPower;
            }
        }

        private void UpdateSteps()
        {
            if (!playStepSound)
            {
                Power -= 1f;

                if (Power < 0)
                {
                    Power = 0;
                    ChangeSteps();
                }
            }
        }

        private void StealthUpdate()
        {
            if (isStealthed)
            {
                Power -= 1.5f;

                if (Power <= 0)
                {
                    Power = 0;
                    isStealthed = false;
                }

                float minValue = 0.0f;

                if (Main.MainPlayer is Stealther)
                {
                    minValue = 0.5f;
                }

                if (Alpha > minValue)
                {
                    Alpha -= 0.05f;
                }
            }
            else
            {
                if (Alpha < 1.0f)
                {
                    Alpha += 0.05f;
                }
                else if (Alpha > 1.0f)
                {
                    Alpha = 1.0f;
                }
            }
        }

        protected override void HandleAnimations()
        {
            base.HandleAnimations();

            if (dir > 0)
            {
                weaponOrigin.X = weaponTexture.Width / 3;
                weaponFireOffset.X = weaponTexture.Height / 2 - 15;
            }
            else
            {
                weaponOrigin.X = weaponTexture.Width - weaponTexture.Width / 3;
                weaponFireOffset.X = -(weaponTexture.Height / 2 - 15);
            }
        }

        protected override void CheckForInput()
        {
            base.CheckForInput();

            if (Controllable)
            {
                if (Main.keyboard.JustPressed(Keys.F))
                {
                    ChangeStealth();

                    if (Main.HasNetworking)
                    {
                        Networking.SendStealthChange(this);
                    }
                }

                if (Main.keyboard.JustPressed(Keys.LeftShift))
                {
                    ChangeSteps();

                    if (Main.HasNetworking)
                    {
                        Networking.SendStealthChange(this);
                    }
                }
            }
        }

        private void ChangeSteps()
        {
            playStepSound = !playStepSound;
        }

        public void ChangeStealth()
        {
            isStealthed = !isStealthed;
        }
    }
}