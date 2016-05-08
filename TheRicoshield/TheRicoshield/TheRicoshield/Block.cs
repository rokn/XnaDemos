using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheRicoshield
{
    public class Block : SolidObject
    {
        private Texture2D texture;
        private List<Vector2> path;
        private int pathIndex;
        private float movespeed;
        private bool moveAble;

        public Block(Vector2 position,bool moveable=false)
        {
            this.Position = position;
            if(moveable)
            {
                path = new List<Vector2>();
                path.Add(new Vector2(position.X, position.Y));
                path.Add(new Vector2(position.X + 100, position.Y+100));
                path.Add(new Vector2(position.X + 200, position.Y + 275));
                path.Add(new Vector2(position.X+10, position.Y + 455));
                pathIndex = 0;
                movespeed = 5f;
            }
            moveAble = moveable;

        }

        public override void Load(ContentManager Content)
        {
            this.texture = Scripts.LoadTexture(@"Blocks\Block", Content);
            this.Rect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        public override void Update()
        {
            if (moveAble)
            {
                if (MoveToTarget(path[pathIndex]))
                {
                    if (pathIndex < path.Count - 1)
                    {
                        pathIndex++;
                    }
                    else
                    {
                        pathIndex = 0;
                    }
                }
            }
        }

        private bool MoveToTarget(Vector2 target)
        {
            if (Position == target) return true;

            Vector2 direction = Vector2.Normalize(target - Position);

            Position += direction * movespeed;

            Rect = MathAid.UpdateRectViaVector(Rect, Position);

            if(Math.Abs(Vector2.Dot(direction,Vector2.Normalize(target-Position)))<0.1f)
            {
                Position = target;
            }
            return Position == target;
        }

        public override void CollisonWithBall(Ball ball)
        {
            Game.SolidObjects.Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.Position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.3f);
        }
    }
}
