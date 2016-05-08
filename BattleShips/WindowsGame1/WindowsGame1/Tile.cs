using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleships
{
    class Tile
    {
        public Texture2D tile;
        public Tile(Texture2D start)
        {
            tile = start;
        }
    }
}
