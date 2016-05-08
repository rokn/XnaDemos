﻿using System;

namespace StealthOrNot
{
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