using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Snake
{
    public enum Direction
    {
        Right,
        Down,
        Left,
        Up
    }

    public enum PartType
    {
        Head,
        Body,
        Tail
    }

    public class Snake
    {
        private List<SnakePart> parts;
        private Timer moveTimer;
        private bool AddPart;
        private bool RemoveFirstPart;
        private bool Dead;
        public bool RealDead;
        private bool check;

        public Snake()
        {
            check = false;
            parts = new List<SnakePart>();
            parts.Add(new SnakePart(new Point(6, 5), PartType.Head));

            for (int i = 5; i > 3; i--)
            {
                parts.Add(new SnakePart(new Point(i, 5), PartType.Body));
            }

            parts.Add(new SnakePart(new Point(3, 5), PartType.Tail));

            moveTimer = new Timer();

            moveTimer.Elapsed += new ElapsedEventHandler(MoveTimerElapsed);

            moveTimer.Interval = 200;
            moveTimer.Start();

            Dead = false;
            RealDead = false;
        }

        public void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!Dead)
            {
                Direction dir = parts[0].Direction;

                if (Main.keyboard.JustPressed(Keys.W))
                {
                    if (dir != Direction.Down && dir != Direction.Up)
                    {
                        dir = Direction.Up;
                        Move(dir);
                    }
                }

                if (Main.keyboard.JustPressed(Keys.A))
                {
                    if (dir != Direction.Right && dir != Direction.Left)
                    {
                        dir = Direction.Left;
                        Move(dir);
                    }
                }
                if (Main.keyboard.JustPressed(Keys.S))
                {
                    if (dir != Direction.Up && dir != Direction.Down)
                    {
                        dir = Direction.Down;
                        Move(dir);
                    }
                }
                if (Main.keyboard.JustPressed(Keys.D))
                {
                    if (dir != Direction.Left && dir != Direction.Right)
                    {
                        dir = Direction.Right;
                        Move(dir);
                    }
                }
            }
        }

        private void Move(Direction direction)
        {
            if (!Dead)
            {
                Direction dir = direction;
                Direction prevDir = direction;
                Direction nextDir;
                Point prevPos = new Point();

                for (int i = 0; i < parts.Count; i++)
                {
                    prevPos = parts[i].Position;
                    nextDir = parts[i].Direction;
                    parts[i].Move(dir);

                    if (parts[i].type == PartType.Body)
                    {
                        if (dir != prevDir)
                        {
                            parts[i].IsCurved = true;

                            #region DO NOT OPEN

                            switch (prevDir)
                            {
                                case Direction.Right:
                                    switch (dir)
                                    {
                                        case Direction.Down:
                                            parts[i].ChangeRotation(270f);
                                            break;

                                        case Direction.Up:
                                            parts[i].ChangeRotation(0f);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;

                                case Direction.Down:
                                    switch (dir)
                                    {
                                        case Direction.Right:
                                            parts[i].ChangeRotation(90f);
                                            break;

                                        case Direction.Left:
                                            parts[i].ChangeRotation(0f);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;

                                case Direction.Left:
                                    switch (dir)
                                    {
                                        case Direction.Down:
                                            parts[i].ChangeRotation(180f);
                                            break;

                                        case Direction.Up:
                                            parts[i].ChangeRotation(90f);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;

                                case Direction.Up:
                                    switch (dir)
                                    {
                                        case Direction.Right:
                                            parts[i].ChangeRotation(180f);
                                            break;

                                        case Direction.Left:
                                            parts[i].ChangeRotation(270f);
                                            break;

                                        default:
                                            break;
                                    }
                                    break;

                                default:
                                    break;
                            }

                            #endregion DO NOT OPEN
                        }
                        else
                        {
                            parts[i].IsCurved = false;
                        }
                    }
                    else if (parts[i].type == PartType.Head)
                    {
                        if (CheckHeadCollision(parts[i], prevPos))
                        {
                            parts[i].ChangeDirection(dir);
                            break;
                        }
                    }

                    prevDir = dir;
                    dir = nextDir;
                }

                if (AddPart)
                {
                    parts[parts.Count - 1].ChangeType(PartType.Body);
                    parts.Add(new SnakePart(prevPos, PartType.Tail));
                    AddPart = false;
                }

                parts[parts.Count - 1].ChangeDirection(parts[parts.Count - 2].Direction);
            }
            else
            {
                if (parts.Count > 0)
                {
                    Main.mapObjects.Remove(parts[0]);
                    parts.RemoveAt(0);
                }
                else if(!RealDead)
                {
                    if(Main.CheckHighScore() && !check)
                    {
                        check = true;
                        Main.GetNameForHighscore();
                        Main.UpdateHighscores();
                        Main.ShowHighscores();
                    }
                    else
                    {
                        Main.ShowGameOverMsg();
                    }

                    RealDead = true;
                }
            }

            if (RemoveFirstPart)
            {
                Main.mapObjects.Remove(parts[1]);
                parts.RemoveAt(1);
                RemoveFirstPart = false;
            }

            moveTimer.Stop();
            moveTimer.Start();
        }

        private bool CheckHeadCollision(SnakePart part, Point prevPos)
        {
            foreach (var obj in Main.mapObjects)
            {
                if (part != obj)
                {
                    if (part.Position.IsSameAs(obj.Position))
                    {
                        if (obj is Apple)
                        {
                            AddPart = true;
                            Main.RemoveApple(obj as Apple);
                            return false;
                        }
                        else if (obj is SnakePart)
                        {
                            Dead = true;
                            //RemoveFirstPart = true;
                            part.Position = prevPos;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void MoveTimerElapsed(object source, ElapsedEventArgs e)
        {
            if (!Dead)
            {
                Move(parts[0].Direction);
            }
            else
            {
                Move(Direction.Down);
            }
        }
    }





    public class SnakePart : MapObject
    {
        private Vector2 Origin;

        public PartType type;

        private Texture2D texture, curvedBody;
        private float rotation;
        public Direction Direction;
        public bool IsCurved;

        public SnakePart(Point pos, PartType partType)
            : base(pos)
        {
            IsCurved = false;
            ChangeType(partType);

            curvedBody = Resources.SnakeBodyCurved;
            Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            rotation = 0f;

            Main.mapObjects.Add(this);
        }

        public void ChangeType(PartType newType)
        {
            type = newType;

            switch (type)
            {
                case PartType.Head:
                    texture = Resources.SnakeHead;
                    break;

                case PartType.Body:
                    texture = Resources.SnakeBody;
                    break;

                case PartType.Tail:
                    texture = Resources.SnakeTail;
                    break;

                default:
                    throw new IndexOutOfRangeException("There is no texture for snake part:" + type.ToString());
            }
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    Position = new Point(Position.X, Position.Y - 1);
                    break;

                case Direction.Right:
                    Position = new Point(Position.X + 1, Position.Y);
                    break;

                case Direction.Left:
                    Position = new Point(Position.X - 1, Position.Y);
                    break;

                case Direction.Down:
                    Position = new Point(Position.X, Position.Y + 1);
                    break;

                default:
                    break;
            }

            ChangeDirection(dir);
        }

        public void ChangeDirection(Direction newDirection)
        {
            Direction = newDirection;
            rotation = MathHelper.ToRadians(90 * (int)newDirection);
        }

        public void ChangeRotation(Direction newDirection)
        {
            rotation = MathHelper.ToRadians(90 * (int)newDirection);
        }

        public void ChangeRotation(float angle)
        {
            rotation = MathHelper.ToRadians(angle);
        }

        public override void Draw(SpriteBatch spriteBatch, Color color, float depth)
        {
            if (!IsCurved)
            {
                spriteBatch.Draw(texture, Scripts.GetPositionOfGridPlace(Position) + Origin, null, color, rotation, Origin, 1.0f, SpriteEffects.None, depth);
            }
            else
            {
                spriteBatch.Draw(curvedBody, Scripts.GetPositionOfGridPlace(Position) + Origin, null, color, rotation, Origin, 1.0f, SpriteEffects.None, depth);
            }
        }
    }
}