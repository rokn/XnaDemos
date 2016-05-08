using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RPG
{
    public static class TileSet
    {
        public const int TileSetsCount = 43;

        public static List<Texture2D> SpriteSheet { get; set; }

        public static int tileWidth { get; set; }

        public static int tileHeight { get; set; }

        public static Rectangle GetSourceRectangle(Tile tile)
        {
            int tilesPerRow = SpriteSheet[tile.tileSet].Width / tileWidth;
            int sourceY = tile.Id / tilesPerRow;
            int sourceX = tile.Id - sourceY * tilesPerRow;
            Rectangle source = new Rectangle(sourceX * tileWidth, sourceY * tileHeight, tileWidth, tileHeight);
            return source;
        }

        public static void LoadTileSets()
        {
            SpriteSheet = new List<Texture2D>();

            tileWidth = 32;
            tileHeight = 32;

            for (int i = 0; i < TileSetsCount; i++)
            {
                Texture2D texture = Scripts.LoadTexture(@"TileSets\Tileset_ ("+i+")");

                SpriteSheet.Add(texture);
            }
        }
    }
}
