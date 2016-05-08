using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Snake
{
    public class TileMap
    {
        public TileCell[,] tileMap { get; set; }

        private SpriteBatch sB;
        private RenderTarget2D renderedMap;
        private Texture2D texture;
        private bool IsRenderable;

        public TileMap(Vector2 pos, int width, int height, bool isRenderable)
        {
            this.Position = pos;
            this.tileMap = new TileCell[width, height];
            this.Width = width;
            this.Height = height;
            this.IsRenderable = isRenderable;
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
                }
            }

            if (IsRenderable)
            {
                renderedMap = new RenderTarget2D(Main.graphics.GraphicsDevice, Width * TileSet.TileWidth, Height * TileSet.TileHeight);
                texture = new Texture2D(Main.graphics.GraphicsDevice, Width * TileSet.TileWidth, Height * TileSet.TileHeight);
            }
        }

        public void RenderTileMap(SpriteBatch spriteBatch)
        {
            if (IsRenderable)
            {
                GraphicsDevice device = Main.graphics.GraphicsDevice;
                device.SetRenderTarget(renderedMap);
                device.Clear(Color.Pink);

                spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                DrawMap(spriteBatch, new Vector2());
                spriteBatch.End();

                texture = (Texture2D)renderedMap;
                device.SetRenderTarget(null);
            }
        }

        private void DrawMap(SpriteBatch spriteBatch, Vector2 position, float defaultDepth = Layer.TileDefault, float alpha = 1.0f)
        {            
            sB = spriteBatch;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float topDepth = defaultDepth + (y / 1000f);

                    Vector2 pos = position + new Vector2(TileSet.TileWidth * x, TileSet.TileHeight * y);

                    if (tileMap[x, y].hasBackTile)
                    {
                        DrawTile(tileMap[x, y].BackTile, pos, defaultDepth - 0.00001f, Color.Gray);
                    }

                    if (tileMap[x, y].hasTile)
                    {
                        DrawTile(tileMap[x, y].tile, pos, defaultDepth, Color.White * alpha);
                    }
                }
            }            
        }

        public void Draw(SpriteBatch spriteBatch, float Depth = Layer.TileDefault, float alpha = 1.0f)
        {

            if (IsRenderable)
            {
                if (renderedMap != null)
                {
                    spriteBatch.Draw(texture, Position, null, Color.White * alpha, 0.0f, new Vector2(), 1f, SpriteEffects.None, Depth);
                }
            }
            else
            {
                DrawMap(spriteBatch, Position, Depth, alpha);
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
                sB.Draw(TileSet.SpriteSheets[tile.tileSet], pos, TileSet.GetSourceRectangle(tile), color, 0, new Vector2(), 1f, SpriteEffects.None, depth);
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