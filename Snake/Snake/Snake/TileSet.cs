using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Snake
{
    public static class TileSet
    {
        public static List<Texture2D> SpriteSheets { get; set; }

        public static int TileWidth { get; set; }

        public static int TileHeight { get; set; }

        public static Rectangle GetSourceRectangle(Tile tile)
        {
            int tilesPerRow = SpriteSheets[tile.tileSet].Width / TileWidth;
            int sourceY = tile.Id / tilesPerRow;
            int sourceX = tile.Id - sourceY * tilesPerRow;
            Rectangle source = new Rectangle(sourceX * TileWidth, sourceY * TileHeight, TileWidth, TileHeight);
            return source;
        }
    }
}