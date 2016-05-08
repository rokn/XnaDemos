using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StealthOrNot
{
    public static class Editor
    {
        #region Vars

        private static MouseCursor mouse;
        private static KeysInput keyboard;
        private static Texture2D pixelBox;
        private static Texture2D selectedTileTexture;
        private static Texture2D currSpriteSheet;
        private static Vector2 currTileSetPosition;
        private static Vector2 backgroundPosition;
        private static Vector2 markerPosition;
        private static Rectangle backgroundRectangle;
        private static Rectangle spriteSheetRectangle;
        private static int selectedTileId;
        private static int tileSetTilesCount;
        private static bool dragging;
        private static int currentTileX;
        private static int currentTileY;
        private static int multipleTileX;
        private static int multipleTileY;
        private static Rectangle markerRect;
        private static Rectangle defaultRect;
        private static bool multipleSelected;

        private static bool blocksMode;
        private static Rectangle rectToBeAdded;
        private static bool isDraggingRect;
        private static List<Rectangle> rectsToRemove;

        #endregion Vars

        public static void Initialize()
        {
            mouse = Main.mouse;
            keyboard = Main.keyboard;
            currSpriteSheet = TileSet.SpriteSheet[Main.currTileset];
            currTileSetPosition = new Vector2(Main.width - currSpriteSheet.Width, 50f);
            backgroundRectangle = new Rectangle(0, 0, Main.width - (int)currTileSetPosition.X, Main.height);
            backgroundPosition = new Vector2(currTileSetPosition.X, 0);
            spriteSheetRectangle = new Rectangle((int)currTileSetPosition.X, (int)currTileSetPosition.Y, currSpriteSheet.Width, currSpriteSheet.Height);
            selectedTileId = 0;
            markerPosition = currTileSetPosition;
            dragging = false;
            defaultRect = new Rectangle(0, 0, TileSet.TileWidth, TileSet.TileHeight);
            markerRect = defaultRect;
            blocksMode = false;
            isDraggingRect = false;
            rectsToRemove = new List<Rectangle>();
            tileSetTilesCount = (currSpriteSheet.Width / TileSet.TileWidth) * (currSpriteSheet.Height / TileSet.TileHeight);
        }

        public static void Load()
        {
            pixelBox = Scripts.LoadTexture("WhitePixel");
            selectedTileTexture = Scripts.LoadTexture(@"TileSets\TileMarker");
        }

        public static void Update()
        {
            if (mouse.LeftClick())
            {
                if (spriteSheetRectangle.Contains(mouse.clickRectangle))
                {
                    ChangeSelectedTile();
                }
            }
            if (mouse.LeftHeld())
            {
                if (spriteSheetRectangle.Contains(mouse.clickRectangle))
                {
                    dragging = true;
                }
            }

            if (mouse.HasScrolledUp())
            {
                if (selectedTileId > 0)
                {
                    ChangeSelectedTile(selectedTileId - 1);
                }
                else
                {
                    ChangeSelectedTile(tileSetTilesCount - 1);
                }
            }

            if (mouse.HasScrolledDown())
            {
                if (selectedTileId < tileSetTilesCount - 1)
                {
                    ChangeSelectedTile(selectedTileId + 1);
                }
                else
                {
                    ChangeSelectedTile(0);
                }
            }

            if (dragging)
            {
                if (MouseIsInWorkingArea())
                {
                    dragging = false;
                }
                int x = ((int)mouse.Position.X - (int)currTileSetPosition.X) / TileSet.TileWidth;
                int y = ((int)mouse.Position.Y - (int)currTileSetPosition.Y) / TileSet.TileHeight;

                markerRect.Width = TileSet.TileWidth * (x - currentTileX + 1);
                markerRect.Height = TileSet.TileHeight * (y - currentTileY + 1);
                if (markerRect.Width > defaultRect.Width || markerRect.Height > defaultRect.Height)
                {
                    multipleSelected = true;
                    multipleTileX = x - currentTileX;
                    multipleTileY = y - currentTileY;
                }
                if (mouse.LeftReleased())
                {
                    dragging = false;
                }
            }

            #region Check For Input

            if (keyboard.IsHeld(Keys.LeftControl) || keyboard.IsHeld(Keys.RightControl))
            {
                if (keyboard.JustPressed(Keys.S))
                {
                    Save();
                }
            }

            if (!blocksMode)
            {
                if (Scripts.KeyIsPressed(Keys.Space))
                {
                    if (mouse.LeftHeld() || mouse.LeftClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            ChangeTile();
                        }
                    }
                    if (mouse.RightHeld() || mouse.RightClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            RemoveTile();
                        }
                    }
                }
                else
                {
                    if (mouse.LeftClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            ChangeTile();
                        }
                    }
                    if (mouse.RightClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            RemoveTile();
                        }
                    }
                }
                if (keyboard.IsHeld(Keys.LeftControl) || keyboard.IsHeld(Keys.RightControl))
                {
                    if (keyboard.JustPressed(Keys.C))
                    {
                        Main.tilemap.ClearTileMap();
                    }
                }
            }
            else
            {
                foreach (Rectangle rect in rectsToRemove)
                {
                    Main.blockRects.Remove(rect);
                }

                if (keyboard.JustPressed(Keys.L))
                {
                    if (keyboard.IsHeld(Keys.LeftShift) || keyboard.IsHeld(Keys.RightShift))
                    {
                        Vector2 snappedPos = GetSnappedMousePosition();
                        Main.Lamps.Add(new Lamp(new Vector2(mouse.RealPosition.X, snappedPos.Y), Main.LampDefaultSize));
                    }
                    else if (!keyboard.IsHeld(Keys.LeftControl) || !keyboard.IsHeld(Keys.RightControl))
                    {
                        Main.Lamps.Add(new Lamp(mouse.RealPosition, Main.LampDefaultSize));
                    }
                }

                if (keyboard.IsHeld(Keys.LeftControl) || keyboard.IsHeld(Keys.RightControl))
                {
                    if (keyboard.JustPressed(Keys.C))
                    {
                        Main.blockRects.Clear();
                    }
                    if (keyboard.JustPressed(Keys.L))
                    {
                        for (int i = 0; i < Main.Lamps.Count; i++)
                        {
                            Main.Lamps[i].Remove();
                            i--;
                        }
                    }
                }
                if (MouseIsInWorkingArea())
                {
                    #region AddRects

                    if (mouse.LeftClick())
                    {
                        rectToBeAdded = new Rectangle((int)mouse.RealPosition.X, (int)mouse.RealPosition.Y, 0, 0);
                        isDraggingRect = true;
                        if (keyboard.IsHeld(Keys.LeftShift) || keyboard.IsHeld(Keys.RightShift))
                        {
                            Vector2 snappedPosition = GetSnappedMousePosition();
                            rectToBeAdded = MathAid.UpdateRectViaVector(rectToBeAdded, snappedPosition);
                        }
                    }
                    if (isDraggingRect)
                    {
                        if (keyboard.IsHeld(Keys.LeftShift) || keyboard.IsHeld(Keys.RightShift))
                        {
                            Vector2 snappedPosition = GetSnappedMousePosition();
                            rectToBeAdded.Width = (int)snappedPosition.X - rectToBeAdded.X + TileSet.TileWidth;
                            rectToBeAdded.Height = (int)snappedPosition.Y - rectToBeAdded.Y + TileSet.TileHeight;
                        }
                        else
                        {
                            rectToBeAdded.Width = (int)mouse.RealPosition.X - rectToBeAdded.X;
                            rectToBeAdded.Height = (int)mouse.RealPosition.Y - rectToBeAdded.Y;
                        }
                        if (mouse.LeftReleased())
                        {
                            AddRect();
                        }
                    }

                    #endregion AddRects

                    #region RemoveRects

                    if (Scripts.KeyIsPressed(Keys.Space))
                    {
                        if (mouse.RightClick() || mouse.RightHeld())
                        {
                            RemoveRects();
                            RemoveLamps();
                        }
                    }
                    else
                    {
                        if (mouse.RightClick())
                        {
                            RemoveRects();
                            RemoveLamps();
                        }
                    }

                    #endregion RemoveRects
                }
                else
                {
                    if (isDraggingRect)
                    {
                        isDraggingRect = false;
                    }
                }
            }
            if (keyboard.JustPressed(Keys.B))
            {
                blocksMode = !blocksMode;
            }

            #endregion Check For Input
        }

        private static void RemoveLamps()
        {
            for (int i = 0; i < Main.Lamps.Count; i++)
            {
                if (Main.Lamps[i].rect.Contains(mouse.RealPosition))
                {
                    Main.Lamps[i].Remove();
                    i--;
                }
            }
        }

        private static void AddRect()
        {
            isDraggingRect = false;
            Main.blockRects.Add(rectToBeAdded);
        }

        private static void RemoveRects()
        {
            foreach (Rectangle rect in Main.blockRects)
            {
                if (rect.Contains(mouse.RealPosition))
                {
                    rectsToRemove.Add(rect);
                }
            }
        }

        private static bool MouseIsInWorkingArea()
        {
            return mouse.Position.X < Main.width - currSpriteSheet.Width;
        }

        private static void ChangeTile()
        {
            int x = (int)Main.camera.GetRealMousePosition().X / TileSet.TileWidth;
            int y = (int)Main.camera.GetRealMousePosition().Y / TileSet.TileHeight;

            if (multipleSelected)
            {
                int tilesPerRow = currSpriteSheet.Width / TileSet.TileWidth;

                for (int i = 0; i < multipleTileX + 1; i++)
                {
                    for (int b = 1; b <= multipleTileY; b++)
                    {
                        int tileId = tilesPerRow * (currentTileY + multipleTileY - b) + currentTileX + i;

                        if (CheckIfInTileIsRange(x + i, y + b))
                            Main.tilemap.ChangeBaseTile(x + i, y + b, tileId, Main.currTileset);
                    }
                }
            }
            else if (keyboard.IsHeld(Keys.LeftShift) || keyboard.IsHeld(Keys.RightShift))
            {
                if (CheckIfInTileIsRange(x, y))
                    Main.tilemap.ChangeBackTile(x, y, selectedTileId, Main.currTileset);
            }
            else
            {
                if (CheckIfInTileIsRange(x, y))
                    Main.tilemap.ChangeBaseTile(x, y, selectedTileId, Main.currTileset);
            }
        }

        private static bool CheckIfInTileIsRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Main.tilemap.Width && y < Main.tilemap.Height;
        }

        private static void RemoveTile()
        {
            int x = (int)Main.camera.GetRealMousePosition().X / TileSet.TileWidth;
            int y = (int)Main.camera.GetRealMousePosition().Y / TileSet.TileHeight;

            if (!CheckIfInTileIsRange(x, y))
            {
                return;
            }

            if (keyboard.IsHeld(Keys.LeftShift) || keyboard.IsHeld(Keys.RightShift))
            {
                Main.tilemap.RemoveMergeTile(x, y);
            }
            else
            {
                Main.tilemap.RemoveBaseTile(x, y);
            }
        }

        private static void Save()
        {
            if (!Directory.Exists(Main.SavePath))
            {
                Directory.CreateDirectory(Main.SavePath);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Main.SaveName + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);

            for (int i = 0; i < Main.tilemap.Width; i++)
            {
                for (int b = 0; b < Main.tilemap.Height; b++)
                {
                    formatter.Serialize(stream, Main.tilemap.tileMap[i, b]);
                }
            }

            stream.Close();

            Stream rectStream = new FileStream(Main.SaveName + "Rects.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(rectStream, Main.blockRects);
            rectStream.Close();

            List<Vector2> lampPositions = new List<Vector2>(Main.Lamps.Count);
            Main.Lamps.ForEach(l => lampPositions.Add(l.Position));
            Stream lampStream = new FileStream(Main.SaveName + "Lamps.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(lampStream, lampPositions);
            lampStream.Close();
        }

        private static void ChangeSelectedTile()
        {
            int tilesPerRow = currSpriteSheet.Width / TileSet.TileWidth;
            int x = ((int)mouse.Position.X - (int)currTileSetPosition.X) / TileSet.TileWidth;
            int y = ((int)mouse.Position.Y - (int)currTileSetPosition.Y) / TileSet.TileHeight;
            currentTileX = x;
            currentTileY = y;
            selectedTileId = tilesPerRow * y + x;
            markerPosition = new Vector2(currTileSetPosition.X + x * TileSet.TileWidth, currTileSetPosition.Y + y * TileSet.TileHeight);
            markerRect = defaultRect;
            multipleSelected = false;
        }

        private static void ChangeSelectedTile(int newId)
        {
            int tilesPerRow = currSpriteSheet.Width / TileSet.TileWidth;
            selectedTileId = newId;//tilesPerRow * y + x;
            int y = (int)(selectedTileId / tilesPerRow);
            int x = (int)(selectedTileId % tilesPerRow);
            currentTileX = x;
            currentTileY = y;
            markerPosition = new Vector2(currTileSetPosition.X + x * TileSet.TileWidth, currTileSetPosition.Y + y * TileSet.TileHeight);
            markerRect = defaultRect;
            multipleSelected = false;
        }

        public static void Draw(SpriteBatch spriteBatch, bool drawingGui)
        {
            if (drawingGui)
            {
                spriteBatch.Draw(pixelBox, backgroundPosition, backgroundRectangle, Color.Beige, 0, new Vector2(), 1f, SpriteEffects.None, 0.6f);
                spriteBatch.Draw(currSpriteSheet, currTileSetPosition, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.61f);
                spriteBatch.DrawString(Main.Font, "CT: " + Main.currTileset.ToString(), backgroundPosition, Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Main.Font, "Blocks Mode : " + blocksMode.ToString(), new Vector2(), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Main.Font, "Mouse Position : " + mouse.Position, new Vector2(0, 30), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Main.Font, "Mouse Real Position : " + mouse.RealPosition, new Vector2(0, 60), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Main.Font, "Camera Position : " + (Main.camera.pos - Main.camera.zeroPos).ToString(), new Vector2(0, 90), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Main.Font, "Blocks : " + Main.blockRects.Count, new Vector2(0, 120), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.Draw(selectedTileTexture, markerPosition, markerRect, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.9999f);
            }
            else
            {
                if (blocksMode)
                {
                    if (isDraggingRect)
                    {
                        spriteBatch.Draw(pixelBox, rectToBeAdded, rectToBeAdded, Color.Black * 0.5f, 0, new Vector2(), SpriteEffects.None, 0.79f);
                    }
                }
                else
                {
                    if (mouse.Position.X < Main.width - currSpriteSheet.Width)
                    {
                        Vector2 mouseSelectedPosition = GetSnappedMousePosition();
                        spriteBatch.Draw(selectedTileTexture, Main.tilemap.Position + mouseSelectedPosition, markerRect, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.95f);
                    }
                }
            }
        }

        private static Vector2 GetSnappedMousePosition()
        {
            Vector2 snappedPosition;
            snappedPosition = new Vector2((int)(mouse.RealPosition.X / TileSet.TileWidth), (int)(mouse.RealPosition.Y / TileSet.TileHeight));
            snappedPosition.X *= TileSet.TileWidth;
            snappedPosition.Y *= TileSet.TileHeight;
            return snappedPosition;
        }
    }
}