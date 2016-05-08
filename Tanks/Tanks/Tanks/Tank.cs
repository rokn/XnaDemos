using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Tanks
{
    public enum Direction
    {
        Left = -1, Right = 1 , Up = -1, Down = 1
    }

    public class Tank
    {
        private static int defaultSpeed = 5;


        public Vector2 position;
        private Vector2 gunPositionOffset;
        private Vector2 origin;
        private Vector2 gunOrigin;
        private Texture2D baseTexture;
        private Texture2D gunTexture;
        private double firePower;
        private Color color;
        private int moveSpeed;
        private int height;
        private float gunAngle;
        private int fuel;
        private bool Controllable;

        public Tank(Vector2 startPos, Color tankColor,bool controllable)
        {
            this.position = startPos;
            this.color = tankColor;
            this.moveSpeed = defaultSpeed;
            this.height = Terrain.heightMap[(int)position.X];
            this.position.Y = height;
            this.gunAngle = 0f;
            this.Controllable = controllable;
        }

        public int Power
        {
            get
            {
                return (int)Math.Round(firePower*10);
            }
        }

        public void Load(ContentManager Content)
        {
            baseTexture = Scripts.LoadTexture(@"Tanks\Tank_Base", Content);
            gunTexture = Scripts.LoadTexture(@"Tanks\Tank_Gun",Content);
            gunPositionOffset = new Vector2(baseTexture.Width / 2, baseTexture.Height / 3-1);
            gunOrigin = new Vector2(0, gunTexture.Height / 2);
            origin = new Vector2(baseTexture.Width / 2, baseTexture.Height);
        }

        public void Update()
        {
            if(Controllable)
                CheckForInput();

            if(position.Y!=Terrain.heightMap[(int)position.X])
            {
                position.Y = Terrain.heightMap[(int)position.X];
                height = (int)position.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gunTexture, position + gunPositionOffset - origin, null, Color.White,gunAngle, gunOrigin, 1f, SpriteEffects.None, 0.49f);
            spriteBatch.Draw(baseTexture, position, null, color, 0, origin, 1f, SpriteEffects.None, 0.5f);
        }

        private void CheckForInput()
        {
            if (Scripts.KeyIsPressed(Keys.D))
            {
                Move(Direction.Right);
            }
            else if (Scripts.KeyIsPressed(Keys.A))
            {
                Move(Direction.Left);
            }
            if(Scripts.KeyIsPressed(Keys.W))
            {
                RotateGun(Direction.Up);
            }
            else if(Scripts.KeyIsPressed(Keys.S))
            {
                RotateGun(Direction.Down);
            }
            if(Game.keyboard.JustPressed(Keys.Space))
            {
                Shoot();
            }
            if (Scripts.KeyIsPressed(Keys.Up))
            {
                if (firePower < 10)
                {
                    firePower += 0.1;
                }
                else
                {
                    firePower = 10;
                }
            }
            else if (Scripts.KeyIsPressed(Keys.Down))
            {
                if (firePower > 0)
                {
                    firePower -= 0.1;
                }
                else
                {
                    firePower = 0;
                }
            }

        }

        public void Shoot()
        {
            Vector2 bulletPosition = position + gunPositionOffset - origin + MathAid.AngleToVector(gunAngle)*gunTexture.Width;
            Bullet bullet = new Bullet(bulletPosition, MathAid.AngleToVector(gunAngle) * (float)firePower);
            Game.AddBullet(bullet);
        }

        private void RotateGun(Direction direction)
        {
            float angleInDegrees = MathHelper.ToDegrees(gunAngle);
            angleInDegrees = MathHelper.Clamp(angleInDegrees + (int)direction, -180, 0);
            gunAngle = MathHelper.ToRadians(angleInDegrees);
        }

        private void Move(Direction direction)
        {
            for (int i = 0; i < moveSpeed; i++)
            {
                switch (direction)
                {
                    case Direction.Left:
                        if (position.X - origin.X < 1) return;
                        break;
                    case Direction.Right:
                        if (position.X - origin.X > Terrain.Width - 2 - baseTexture.Width) return;
                        break;
                }
                if (height - Terrain.heightMap[(int)position.X + (int)direction] < 30)
                {
                    position.X += (int)direction;
                    height = Terrain.heightMap[(int)position.X];
                    position.Y = height;
                }
                else
                {
                    break;
                }
            }
            Game.SendMessage(Packets.Move);
        }
    }
}