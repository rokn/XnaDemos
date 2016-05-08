using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleships
{
    class TileMap
    {
        public List<List<Tile>> map;
        public TileMap(int x,int y,Texture2D Starting)
        {
            map = new List<List<Tile>>();
            for (int i = 0; i < x / 32; i++)
            {
                map.Add(new List<Tile>());
            }
            for (int i = 0; i < x/32; i++)
            {
                for (int b = 0; b < y/32; b++)
                {
                    map[i].Add( new Tile(Starting));
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int b = 0; b < map[0].Count; b++)
                {
                    spriteBatch.Draw(map[i][b].tile, new Vector2(i * 32, b * 32), Color.White);
                }
            }
        }
    }
}
