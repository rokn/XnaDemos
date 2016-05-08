using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheRicoshield
{
    public class Ball
    {
        private Rectangle rect;
        private Texture2D texture;
        private Vector2 speed;
        private Vector2 position;
        private Vector2 origin;
        private float depth;
        private float moveSpeed;


        public Ball(Vector2 startPositon)
        {
            position = startPositon;
            speed = new Vector2();
            depth = 0.4f;
            moveSpeed = 10f;
        }

        public Vector2 Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            private set
            {
                position = value;
            }
        }

        public void Load(ContentManager Content)
        {
            texture = Scripts.LoadTexture(@"Balls\Armadillo_0", Content);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            rect = new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.X), texture.Width, texture.Height);

        }

        public void Update()
        {
            UpdatePosition();
            UpdateRectangle();
        }

        private void UpdateRectangle()
        {
            rect = MathAid.UpdateRectViaVector(rect, Position - origin);
        }

        private void UpdatePosition()
        {
            int xMovePixels = Math.Abs((int)Speed.X);
            int yMovePixels = Math.Abs((int)Speed.Y);
            float xLeftOverPixels = (Math.Abs((int)Speed.X) - xMovePixels) * Math.Sign(speed.X);
            float yLeftOverPixels = (Math.Abs((int)Speed.Y) - yMovePixels) * Math.Sign(speed.Y);
            for (int x = 0; x < xMovePixels; x++)
            {
                position.X += Math.Sign(Speed.X);
                rect.X += Math.Sign(Speed.X);
                CheckForCollision();
            }
            position.X += xLeftOverPixels * Math.Sign(Speed.X);
            CheckForCollision();
            for (int y = 0; y < yMovePixels; y++)
            {
                position.Y += Math.Sign(Speed.Y);
                rect.Y += Math.Sign(Speed.Y);
                CheckForCollision();
            }
            position.Y += yLeftOverPixels * Math.Sign(Speed.Y);
            CheckForCollision();
        }

        private void CheckForCollision()
        {
            for (int i = 0; i < Game.SolidObjects.Count; i++)
            {
                SolidObject obj = Game.SolidObjects[i];
                if (rect.Intersects(obj.Rect))
                {
                    obj.CollisonWithBall(this);
                    Rectangle intersectRect = MathAid.GetIntersectingRectangle(rect, obj.Rect);
                    if (intersectRect.Height > intersectRect.Width)
                    {
                        speed.X *= -1;
                        if (speed.X < 0)
                        {
                            position.X = obj.Rect.Left - origin.X - 1;
                        }
                        else
                        {
                            position.X = obj.Rect.Right + origin.X + 1;
                        }
                        UpdateRectangle();
                    }
                    else
                    {
                        speed.Y *= -1;
                        if (speed.Y < 0)
                        {
                            position.Y = obj.Rect.Top - origin.Y - 1;
                        }
                        else
                        {
                            position.Y = obj.Rect.Bottom + origin.Y + 1;
                        }
                        UpdateRectangle();
                    }
                }
            }
            if (rect.Intersects(Game.player.Rect))
            {
                if (Scripts.CheckForPerfectCollision(texture, Game.player.ShieldTexture, rect, Game.player.Rect))
                {
                    rect.Y--;
                    float positionFromPaddle = position.X - Game.player.Position.X;
                    int place = (int)(positionFromPaddle / Game.player.HitPartSize);
                    moveSpeed = Math.Abs(speed.X) + Math.Abs(speed.Y);
                    if(place>19)
                    {
                        place = 19;
                    }
                    float modifier;
                    float xModifier;
                    float yModifier;
                    
                    if (place < 9)
                    {
                        modifier = (float)((place + 1f) / 10f);
                        xModifier = modifier - 1;
                        yModifier = 1 + xModifier;

                    }
                    else if (place > 10)
                    {
                        modifier = (float)((20f-place) / 10f);
                        xModifier = 1 - modifier;
                        yModifier = 1 - xModifier;
                    }
                    else
                    {
                        modifier = 1;
                        xModifier = 1 - modifier;
                        yModifier = 1 - xModifier;
                    }

                    speed.X = moveSpeed * xModifier;
                    speed.Y = Math.Abs(moveSpeed * yModifier)*(-1f);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 1f, SpriteEffects.None, depth);
        }
    }
}
