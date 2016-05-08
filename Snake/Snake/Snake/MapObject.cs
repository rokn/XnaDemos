using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    public abstract class MapObject
    {
        private Point position;
        public Point Position
        {
            get 
            {
                return position;
            }
            set
            {
                position = value;

                if (value.X < 0)
                {
                    position.X = (int)Main.playingGroundDimensions.X;
                }
                else if (value.X > Main.playingGroundDimensions.X)
                {
                    position.X = 0;
                }

                if (value.Y < 0)
                {
                    position.Y = (int)Main.playingGroundDimensions.Y;
                }
                else if (value.Y > Main.playingGroundDimensions.Y)
                {
                    position.Y = 0;
                }
            }
        }

        public MapObject(Point pos)
        {
            Position = pos;
        }

        public abstract void Draw(SpriteBatch spriteBatch, Color color, float depth);
    }
}