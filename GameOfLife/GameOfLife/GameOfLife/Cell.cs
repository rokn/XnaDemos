using Microsoft.Xna.Framework;
using System;

namespace GameOfLife
{
    public class Cell
    {
        public Point position{get; private set;}
        public Color color{get; private set;}

        public Cell(Point Position, Color cellColor)
        {
            this.position = Position;
            this.color = cellColor;
        }

        public bool Update(bool[,] data)
        {
            return true;
        }
    }
}
