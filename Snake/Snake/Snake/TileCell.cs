using System;
using System.Collections.Generic;

namespace Snake
{
    [Serializable]
    public class TileCell
    {
        public Tile tile { get; set; }

        public bool hasTile { get; set; }
        public bool hasBackTile { get; set; }

        public TileCell(int id, int tileSet)
        {
            this.tile = new Tile(id, tileSet);
            hasTile = true;
            hasBackTile = false;
        }

        public Tile BackTile { get; set; }
    }

    [Serializable]
    public struct Tile
    {
        public int Id;
        public int tileSet;

        public Tile(int id, int tileSet)
        {
            this.Id = id;
            this.tileSet = tileSet;
        }
    }
}