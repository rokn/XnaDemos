using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GalacticCommander
{
    public class Ship
    {
        private bool Controlable;
        private float Deceleration;
        private float Acceleration;
        private float MaxThrust;
        private Vector2 Position;
        private Vector2 Origin;
        private Vector2 Direction;
        private float Rotation;
        private float thrust;
        private float angularVelocity;
        private float MaxAngularVelocity;
        private float AngularAcceleration;
        private float AngularDeceleration;

        private Texture2D texture;
        private float Depth;

        private Vector2 EngineSize;
        private Texture2D EngineTexture;
        private Vector2 EngineOffset;
        private Vector2 EnginePosition;
        private Vector2 EngineOrigin;

        public Ship(Vector2 position)
        {
            Controlable = true;
            Thrust = 0;
            Acceleration = 0.4f;
            Deceleration = 0.1f;
            MaxThrust = 12f;
            AngularAcceleration = 0.008f;
            AngularDeceleration = 0.005f;
            MaxAngularVelocity = 0.05f;
            Depth = 0.5f;

            Position = position;

            EngineSize = new Vector2();
            EngineSize.Y = 1.0f;
        }

        private float Thrust
        {
            get
            {
                return thrust;
            }
            set
            {
                thrust = value;

                if (thrust > MaxThrust)
                {
                    thrust = MaxThrust;
                }
            }
        }

        private float AngularThrust
        {
            get
            {
                return angularVelocity;
            }
            set
            {
                angularVelocity = value;

                if (angularVelocity > MaxAngularVelocity)
                {
                    angularVelocity = MaxAngularVelocity;
                }
            }
        }

        public void Load()
        {
            texture = Scripts.LoadTexture(@"Ships\Main");
            EngineTexture = Scripts.LoadTexture(@"Ships\Engine");
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            EngineOffset = new Vector2(-Origin.X, 0);
            EngineOrigin = new Vector2(EngineTexture.Width, EngineTexture.Height / 2);
        }

        public void Update()
        {
            UpdateForces();
            CheckForInput();
            ApplyForces();
            UpdateEngine();
            Main.camera.FollowTarget(Position);
        }

        private void UpdateEngine()
        {
            EnginePosition = MathAid.ParentChildTransform(Position, EngineOffset, Rotation);
            //EngineSize.X = MathHelper.Lerp(0.0f, MaxThrust / 2, Thrust / MaxThrust);
        }

        private void ApplyForces()
        {
            Rotation += AngularThrust;
            Direction = MathAid.AngleToVector(Rotation);

            for (int i = 0; i < (int)Thrust; i++)
            {
                Position += Direction;
                UpdateEngine();
                Main.particleEngine.GenerateEngineEffect(EnginePosition, Rotation);
            }

            //Position += Direction * (Thrust % 10f);
            UpdateEngine();
            Main.particleEngine.GenerateEngineEffect(EnginePosition, Rotation);
        }

        private void CheckForInput()
        {
            if (Controlable)
            {
                if (Scripts.KeyIsPressed(Keys.LeftShift))
                {
                    MaxThrust = 90;
                    Acceleration = 5f;
                    Deceleration = 2f;
                }
                else
                {
                    if (Thrust <= 12)
                    {
                        MaxThrust = 12f;
                        Deceleration = 0.1f;
                    }
                    else
                    {
                        Deceleration = 2f;
                        Thrust -= Acceleration + Deceleration;
                    }

                    Acceleration = 0.4f;
                }
                if (Scripts.KeyIsPressed(Keys.W))
                {
                    Thrust += Acceleration;
                }

                if (Scripts.KeyIsPressed(Keys.S))
                {
                    Thrust -= Acceleration / 2;
                }

                if (Scripts.KeyIsPressed(Keys.D))
                {
                    AngularThrust += AngularAcceleration;
                }

                if (Scripts.KeyIsPressed(Keys.A))
                {
                    AngularThrust -= AngularAcceleration;
                }
            }
        }

        private void UpdateForces()
        {
            UpdateValue(ref thrust, Deceleration, MaxThrust);
            UpdateValue(ref angularVelocity, AngularDeceleration, MaxAngularVelocity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, Rotation, Origin, 1f, SpriteEffects.None, Depth);
            //spriteBatch.Draw(EngineTexture, EnginePosition, null, Color.White, Rotation, EngineOrigin, EngineSize, SpriteEffects.None, Depth - 0.00001f);
        }

        private void UpdateValue(ref float Value, float DeValue, float MaxValue)
        {
            if (Value != 0)
            {
                int Sign = Math.Sign(Value);

                if (Sign > 0)
                {
                    Value = MathHelper.Clamp(Value - DeValue, 0.0f, MaxValue);
                }
                else
                {
                    Value = MathHelper.Clamp(Value + DeValue, -MaxValue, 0.0f);
                }
            }
        }
    }
}