using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace StealthOrNot
{
    public class TileMap
    {
        public TileCell[,] tileMap { get; set; }

        private SpriteBatch sB;

        public TileMap(Vector2 pos, int width, int height)
        {
            this.Position = pos;
            this.tileMap = new TileCell[width, height];
            this.Width = width;
            this.Height = height;
            Initialize();
        }

        public Vector2 Position { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        private void Initialize()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tileMap[x, y] = new TileCell(0, 0);
                    tileMap[x, y].hasTile = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sB = spriteBatch;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float defaultDepth = 0.1f;
                    float topDepth = defaultDepth + (y / 1000f);

                    Vector2 pos = Position + new Vector2(TileSet.TileWidth * x, TileSet.TileHeight * y);

                    if (tileMap[x, y].hasBackTile)
                    {
                        DrawTile(tileMap[x, y].BackTile, pos, defaultDepth - 0.00001f, Color.Gray);
                    }

                    if (tileMap[x, y].hasTile)
                    {
                        DrawTile(tileMap[x, y].tile, pos, defaultDepth, Color.White);
                    }
                }
            }
        }

        private bool CheckIsTileInView(float x, float y)
        {
            bool left = x + TileSet.TileWidth > Main.camera.Position.X;
            bool top = y + TileSet.TileHeight > Main.camera.Position.Y;
            bool right = x < Main.camera.Position.X + Main.width;
            bool down = y < Main.camera.Position.Y + Main.height;
            return left && top && right && down;
        }

        private void DrawTile(Tile tile, Vector2 pos, float depth, Color color)
        {
            if (CheckIsTileInView(pos.X, pos.Y))
            {
                sB.Draw(TileSet.SpriteSheet[tile.tileSet], pos, TileSet.GetSourceRectangle(tile), color, 0, new Vector2(), 1f, SpriteEffects.None, depth);
            }
        }

        public void ChangeBackTile(int x, int y, int TileId, int tileSet)
        {
            tileMap[x, y].BackTile = new Tile(TileId, tileSet);
            if (!tileMap[x, y].hasBackTile)
            {
                tileMap[x, y].hasBackTile = true;
            }
        }

        public void ChangeBaseTile(int x, int y, int TileId, int tileSet)
        {
            if (!tileMap[x, y].hasTile)
            {
                tileMap[x, y].hasTile = true;
            }

            tileMap[x, y].tile = new Tile(TileId, tileSet);
        }

        public void RemoveMergeTile(int x, int y)
        {
            tileMap[x, y].hasBackTile = false;
        }

        public void RemoveBaseTile(int x, int y)
        {
            tileMap[x, y].hasTile = false;
        }

        public void ClearTileMap()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tileMap[x, y].hasTile = false;
                    tileMap[x, y].hasBackTile = false;
                }
            }
        }
    }
}