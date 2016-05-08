using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheTimeDungeon
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        Up_left,
        Down_left,
        Up_right,
        Down_right
    }

    class Player
    {
        private Texture2D texture;
        private float depth;
        private Direction direction;
        private Vector2 position;
        private Vector2 Origin;
        private float Speed;
        private Dictionary<Direction, float> directionAngles;
        private bool isMoving;
        public Vector2 Position {
            get
            {
                return position;
            } 
            set
            {
                position = value;
            } 
        }

        public Player()
        {
            position = new Vector2();
            directionAngles = new Dictionary<Direction, float>();
            SetUpDirectionAngles();
            direction = Direction.Left;
            Speed = 4f;
            depth = 0.5f;
        }

        public void Load(ContentManager Content)
        {
            texture = Scripts.LoadTexture(@"Player\Player",Content);
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update()
        {
            CheckForInput();
            Movement();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, directionAngles[direction], Origin, 1f, SpriteEffects.None, depth);
        }
        
        private void SetUpDirectionAngles()
        {
            directionAngles.Add(Direction.Right, 0f);
            directionAngles.Add(Direction.Down, MathHelper.ToRadians(90f));
            directionAngles.Add(Direction.Left, MathHelper.ToRadians(180f));
            directionAngles.Add(Direction.Up, MathHelper.ToRadians(270f));
            directionAngles.Add(Direction.Down_right, MathHelper.ToRadians(45f));
            directionAngles.Add(Direction.Down_left, MathHelper.ToRadians(135f));
            directionAngles.Add(Direction.Up_left, MathHelper.ToRadians(225f));
            directionAngles.Add(Direction.Up_right, MathHelper.ToRadians(315f));

        }

        private void CheckForInput()
        {
            if (Scripts.KeyIsPressed(Keys.D))
            {
                isMoving = true;
                direction = Direction.Right;
            }

            if (Scripts.KeyIsPressed(Keys.A))
            {
                isMoving = true;
                direction = Direction.Left;
            }

            if (Scripts.KeyIsPressed(Keys.S))
            {
                isMoving = true;
                direction = Direction.Down;
            }

            if (Scripts.KeyIsPressed(Keys.W))
            {
                isMoving = true;
                direction = Direction.Up;
            }

            if (Scripts.KeyIsPressed(Keys.W) && Scripts.KeyIsPressed(Keys.A))
            {
                isMoving = true;
                direction = Direction.Up_left;
            }

            if (Scripts.KeyIsPressed(Keys.W) && Scripts.KeyIsPressed(Keys.D))
            {
                isMoving = true;
                direction = Direction.Up_right;
            }

            if (Scripts.KeyIsPressed(Keys.S) && Scripts.KeyIsPressed(Keys.A))
            {
                isMoving = true;
                direction = Direction.Down_left;
            }

            if (Scripts.KeyIsPressed(Keys.S) && Scripts.KeyIsPressed(Keys.D))
            {
                isMoving = true;
                direction = Direction.Down_right;
            }

            if (Scripts.KeyIsReleased(Keys.D))
            {
                if (isMoving)
                {
                    if (direction == Direction.Right)
                    {
                        isMoving = false;
                    }
                    else if (direction == Direction.Down_right)
                    {
                        direction = Direction.Down;
                    }
                    else if (direction == Direction.Up_right)
                    {
                        direction = Direction.Up;
                    }
                }
            }

            if (Scripts.KeyIsReleased(Keys.A))
            {
                if (isMoving)
                {
                    if (direction == Direction.Left)
                    {
                        isMoving = false;
                    }
                    else if (direction == Direction.Down_left)
                    {
                        direction = Direction.Down;
                    }
                    else if (direction == Direction.Up_left)
                    {
                        direction = Direction.Up;
                    }
                }
            }

            if (Scripts.KeyIsReleased(Keys.W))
            {
                if (isMoving)
                {
                    if (direction == Direction.Up)
                    {
                        isMoving = false;
                    }
                    else if (direction == Direction.Up_left)
                    {
                        direction = Direction.Left;
                    }
                    else if (direction == Direction.Up_right)
                    {
                        direction = Direction.Right;
                    }
                }
            }

            if (Scripts.KeyIsReleased(Keys.S))
            {
                if (isMoving)
                {
                    if (direction == Direction.Down)
                    {
                        isMoving = false;
                    }
                    else if (direction == Direction.Down_right)
                    {
                        direction = Direction.Right;
                    }
                    else if (direction == Direction.Down_left)
                    {
                        direction = Direction.Left;
                    }
                }
            }
        }

        private void Movement()
        {
            if (isMoving)
            {
                #region Direction
                switch (direction)
                {
                    case Direction.Right:
                        position.X += Speed;
                        break;
                    case Direction.Left:
                        position.X -= Speed;
                        break;
                    case Direction.Up:
                        position.Y -= Speed;
                        break;
                    case Direction.Down:
                        position.Y += Speed;
                        break;
                    case Direction.Up_left:
                        position.X -= Speed;
                        position.Y -= Speed;
                        break;
                    case Direction.Up_right:
                        position.X += Speed;
                        position.Y -= Speed;
                        break;
                    case Direction.Down_left:
                        position.X -= Speed;
                        position.Y += Speed;
                        break;
                    case Direction.Down_right:
                        position.X += Speed;
                        position.Y += Speed;
                        break;
                }
                #endregion
            }
        }

        

    }
}
