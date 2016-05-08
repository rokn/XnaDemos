using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cars
{

    class Player
    {
        private Texture2D bodyTexture, windowsTexture, tiresTexture;
        private float depth;
        private Vector2 position;
        private Vector2 Origin;
        private float Speed;
        private bool isMoving;
        private float Angle;
        private int Id;
        public Color color;
        private float TopSpeed, Acceleration,Friction,backTopSpeed;
        private bool isHandBrakeOn,isNitrousOn,isGoingBack;

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Player(int id)
        {
            position = new Vector2();
            Speed = 0f;
            TopSpeed = 6f;
            Acceleration = 0.2f;
            Friction = 0.1f;
            backTopSpeed = -2f;
            depth = 0.5f;
            Id = id;
            color = Color.Red;
            isHandBrakeOn = false;
            isNitrousOn = false;
        }

        public void Load(ContentManager Content)
        {
            bodyTexture = Scripts.LoadTexture(@"Cars\Car_Body_" + Id.ToString(), Content);
            tiresTexture = Scripts.LoadTexture(@"Cars\Car_Tires_" + Id.ToString(), Content);
            windowsTexture = Scripts.LoadTexture(@"Cars\Car_Windows_" + Id.ToString(), Content);
            Origin = new Vector2(bodyTexture.Width / 2, bodyTexture.Height / 2);
        }

        public void Update()
        {
            position = MathAid.RotatedVectorMovement(Position, Angle, Speed);
            CheckForInput();
            if (Speed > 0)
            {
                if (!isNitrousOn)
                {
                    Speed = MathHelper.Clamp(Speed, 0, TopSpeed);
                }
                else
                {
                    Speed = MathHelper.Clamp(Speed, 0, TopSpeed * 3);
                }
            }
            if(Speed<0&&isGoingBack)
            {
                Speed = MathHelper.Clamp(Speed, backTopSpeed, 0);
            }
            if(Speed ==0)
            {
                if(isMoving)
                {
                    isMoving = false;
                }
            }
            if (Speed != 0)
            {
                Speed -= Friction*Math.Sign(Speed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tiresTexture, position, null, Color.White, Angle, Origin, 1f, SpriteEffects.None, depth);
            spriteBatch.Draw(bodyTexture, position, null, color, Angle, Origin, 1f, SpriteEffects.None, depth + 0.00001f);
            spriteBatch.Draw(windowsTexture, position, null, Color.White, Angle, Origin, 1f, SpriteEffects.None, depth + 0.00002f);
        }
        private void CheckForInput()
        {

            if (Scripts.KeyIsPressed(Keys.W))
            {
                if (!isMoving)
                {
                    isMoving = true;
                }
                if (!isHandBrakeOn)
                {
                    if (Speed < TopSpeed)
                    {
                        Speed += Acceleration;
                    }
                }
            }
            if(Scripts.KeyIsPressed(Keys.P))
            {
                Console.WriteLine(Speed);
            }
            if (Scripts.KeyIsPressed(Keys.S))
            {
                if (!isMoving)
                {
                    isMoving = true;
                }
                isGoingBack = true;
                if (!isHandBrakeOn)
                {
                    if (Speed > backTopSpeed)
                    {
                        Speed -= Acceleration;
                    }
                }
            }
            if (Scripts.KeyIsReleased(Keys.S))
            {
                isGoingBack = false;
            }
            if(Scripts.KeyIsPressed(Keys.Space))
            {
                if (!isHandBrakeOn)
                {
                    isHandBrakeOn = true;
                }
                if(Speed>=Friction*2)
                {
                    Speed -= Friction * 2;
                }
                else
                {
                    if(Speed>0)
                    {
                        Speed = 0;
                    }
                }
            }
            if (isHandBrakeOn)
            {
                if (Scripts.KeyIsReleased(Keys.Space))
                {
                    isHandBrakeOn = false;
                }
            }
            if(Scripts.KeyIsPressed(Keys.LeftShift))
            {
                if(!isNitrousOn)
                {
                    isNitrousOn = true;
                }
                if (!isMoving)
                {
                    isMoving = true;
                }
                if (!isHandBrakeOn)
                {
                    if (Speed < TopSpeed*3)
                    {
                        Speed += Acceleration*3;
                    }
                }
                
            }
            if (isNitrousOn)
            {
                if (Scripts.KeyIsReleased(Keys.LeftShift))
                {
                    isNitrousOn = false;
                }
            }
            if (isMoving && Speed != 0)
            {
                if (Scripts.KeyIsPressed(Keys.A))
                {
                    Angle -= 0.015f;
                }
                if (Scripts.KeyIsPressed(Keys.D))
                {
                    Angle += 0.015f;
                }
            }
        }
    }
}
