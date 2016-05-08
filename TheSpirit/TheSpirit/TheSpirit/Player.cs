using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheSpirit
{
    public class Player
    {
        private Vector2 position;
        private Vector2 origin;
        private Texture2D texture;
        private Rectangle mainRect;
        float rotation;

        private Texture2D trail;
        private Vector2 trailOrigin;
        private Rectangle trailRect;
        private float trailRotation;
        private float trailAlpha;
        private bool showTrailRect;

        private Vector2 velocity;
        private float currSpeed;
        private float maxSpeed;
        private Vector2 direction;
        private float acceleration;


        public Player()
        {
            position = new Vector2(Main.WindowWidth / 2, Main.WindowHeight / 2);
            acceleration = 600;
            maxSpeed = 1200;
            CurrSpeed = 0;
            direction = new Vector2(0, 0);
            velocity = new Vector2(0, 0.00001f);
            trailAlpha = 0.0f;
            rotation = 0;
        }

        public void Load()
        {
            texture = Scripts.LoadTexture("Player");
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            trail = Scripts.LoadTexture("Trail");
            trailOrigin = new Vector2(0, trail.Height / 2);
            trailRect = new Rectangle(0, 0, trail.Width, trail.Height);
            mainRect = new Rectangle(0, 0, texture.Width, texture.Height);
            mainRect = MathAid.UpdateRectViaVector(mainRect, position);
        }

        public float CurrSpeed
        {
            get
            {
                return currSpeed;
            }

            protected set
            {
                if (value <= 0)
                {
                    currSpeed = 0;
                }
                else
                {
                    if (value > maxSpeed)
                    {
                        currSpeed = maxSpeed;
                    }
                    else
                    {
                        currSpeed = value;
                    }
                }
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }

            protected set
            {
                velocity = value;
                if(velocity.Length() > maxSpeed/2)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed/2;
                }
                //if (Math.Abs(value.X) > maxSpeed)
                //{
                //    velocity.X = maxSpeed * Math.Sign(value.X);
                //}
                //else
                //{
                //    velocity.X = value.X;
                //}

                //if (Math.Abs(value.Y) > maxSpeed)
                //{
                //    velocity.Y = maxSpeed * Math.Sign(value.Y);
                //}
                //else
                //{
                //    velocity.Y = value.Y;
                //}
            }
        }

        public void Update(float deltaTime)
        {

            //if(showTrailRect)
            //{
            //    showTrailRect = false;
            //}
            trailAlpha *= 0.80f;
            HandleInput(deltaTime);
            Velocity += direction * CurrSpeed * deltaTime;
            position += Velocity * deltaTime;
            mainRect = MathAid.UpdateRectViaVector(mainRect, position);
            mainRect.Height = (int)MathHelper.Lerp(texture.Height, 10, Velocity.Length() / maxSpeed);
            rotation = MathAid.VectorToAngle(velocity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, mainRect, null, Color.Red, rotation, origin, SpriteEffects.None, 0.1f);

            if(showTrailRect)
            {
                spriteBatch.Draw(trail, trailRect, null, Color.Red * trailAlpha, trailRotation, trailOrigin, SpriteEffects.None, 0.01f);
            }
        }

        private void HandleInput(float deltaTime)
        {
            if(Main.Mouse.LeftHeld())
            {
                CurrSpeed += acceleration * deltaTime;
                direction = Main.Mouse.RealPosition - position;
                direction.Normalize();
            }
            else
            {
                CurrSpeed *= 0.90f;
                velocity *= 0.90f;
            }
            if(Main.Mouse.RightClick())
            {
                currSpeed = 100;
                direction = Main.Mouse.RealPosition - position;
                direction.Normalize();
                trailRect = MathAid.UpdateRectViaVector(trailRect, position);
                trailRotation = MathAid.FindRotation(position,Main.Mouse.RealPosition);
                position += direction * currSpeed;
                trailRect.Width = 100;
                trailAlpha = 1.0f;
                Velocity = direction * Velocity.Length();
                showTrailRect = true;
            }
        }
    }
}