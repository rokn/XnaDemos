using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG
{
    public static class Editor
    {
        #region Vars
        public static int currTileset;

        private const float cameraSpeed = 5f;

        private static Texture2D pixelBox;
        private static Texture2D selectedTileTexture;
        private static Texture2D currSpriteSheet;
        private static Vector2 currTileSetPosition;
        private static Vector2 backgroundPosition;
        private static Vector2 markerPosition;
        private static Rectangle backgroundRectangle;
        private static Rectangle spriteSheetRectangle;
        private static int selectedTileId;
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
        private static Stack<TileMap> undoTileMaps;

        #endregion

        public static void Initialize(int newTilesetId)
        {
            currTileset = newTilesetId;
            currSpriteSheet = TileSet.SpriteSheet[currTileset];
            currTileSetPosition = new Vector2(Main.WindowWidth - currSpriteSheet.Width, 50f);
            backgroundRectangle = new Rectangle(0, 0, Main.WindowWidth - (int)currTileSetPosition.X, Main.WindowHeight);
            backgroundPosition = new Vector2(currTileSetPosition.X, 0);
            spriteSheetRectangle = new Rectangle((int)currTileSetPosition.X, (int)currTileSetPosition.Y, currSpriteSheet.Width, currSpriteSheet.Height);
            selectedTileId = 0;
            markerPosition = currTileSetPosition;
            dragging = false;
            defaultRect = new Rectangle(0, 0, TileSet.tileWidth, TileSet.tileHeight);
            markerRect = defaultRect;
            blocksMode = false;
            isDraggingRect = false;
            rectsToRemove = new List<Rectangle>();

            if(undoTileMaps == null)
            {
                undoTileMaps = new Stack<TileMap>();
            }
        }

        public static void Load()
        {
            pixelBox = Scripts.GenerateWhitePixelTexture();
            selectedTileTexture = Scripts.LoadTexture(@"TileSets\TileMarker");
        }

        public static void Update()
        {
            CheckForInput();

            if (MyMouse.LeftClick())
            {
                if (spriteSheetRectangle.Contains(MyMouse.Position))
                {
                    ChangeSelectedTile();
                }
            }

            if (MyMouse.LeftHeld())
            {
                if (spriteSheetRectangle.Contains(MyMouse.Position))
                {
                    dragging = true;
                }
            }
            if (dragging)
            {
                if (MouseIsInWorkingArea())
                {
                    dragging = false;
                }
                int x = ((int)MyMouse.Position.X - (int)currTileSetPosition.X) / TileSet.tileWidth;
                int y = ((int)MyMouse.Position.Y - (int)currTileSetPosition.Y) / TileSet.tileHeight;

                markerRect.Width = TileSet.tileWidth * (x - currentTileX + 1);
                markerRect.Height = TileSet.tileHeight * (y - currentTileY + 1);
                if (markerRect.Width > defaultRect.Width || markerRect.Height > defaultRect.Height)
                {
                    multipleSelected = true;
                    multipleTileX = x - currentTileX;
                    multipleTileY = y - currentTileY;
                }
                if (MyMouse.LeftReleased())
                {
                    dragging = false;
                }
            }

            #region Check For Input
            if (MyKeyboard.IsHeld(Keys.LeftControl))
            {
                if (MyKeyboard.JustPressed(Keys.S))
                {
                    System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
                    saveDialog.Filter = "Level Files (*.level)|*.level";
                    saveDialog.FilterIndex = 1;

                    if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Main.currentLevel.ID = Path.GetFileName(saveDialog.FileName);
                    }

                    Level.SaveLevel(Main.currentLevel);
                }
                else if (MyKeyboard.JustPressed(Keys.L))
                {
                    string filename = "";

                    System.Windows.Forms.OpenFileDialog saveDialog = new System.Windows.Forms.OpenFileDialog();
                    saveDialog.Filter = "Level Files (*.level)|*.level";
                    saveDialog.FilterIndex = 1;

                    if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        filename = saveDialog.SafeFileName;
                        Main.currentLevel = Level.LoadLevel(filename);
                    }
                }

                if (MyKeyboard.JustPressed(Keys.Z))
                {
                    //Main.currentLevel.tilemap = undoTileMaps.Pop();
                }
            }

            if (MyKeyboard.JustPressed(Keys.F5))
            {                
                Level.SaveLevel(Main.currentLevel);
            }
            else if (MyKeyboard.JustPressed(Keys.F6))
            {
                Main.currentLevel = Level.LoadLevel(Main.currentLevel.ID);
            }



            if (!blocksMode)
            {
                if (Scripts.KeyIsPressed(Keys.Space))
                {
                    if (MyMouse.LeftHeld() || MyMouse.LeftClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            ChangeTile();                            
                        }
                    }
                    if (MyMouse.RightHeld() || MyMouse.RightClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            RemoveTile();
                        }
                    }
                }
                else
                {
                    if (MyMouse.LeftClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            ChangeTile();
                        }
                    }
                    if (MyMouse.RightClick())
                    {
                        if (MouseIsInWorkingArea())
                        {
                            RemoveTile();
                        }
                    }
                }
                if (MyKeyboard.IsHeld(Keys.LeftControl))
                {
                    if (MyKeyboard.JustPressed(Keys.C))
                    {
                        Main.currentLevel.tilemap.ClearTileMap();
                    }
                }
            }
            else
            {
                foreach (Rectangle rect in rectsToRemove)
                {
                    Main.currentLevel.blockRects.Remove(rect);
                }

                if (MyKeyboard.IsHeld(Keys.LeftControl))
                {
                    if (MyKeyboard.JustPressed(Keys.C))
                    {
                        Main.currentLevel.blockRects.Clear();
                    }
                }
                if (MouseIsInWorkingArea())
                {
                    #region AddRects
                    if (MyMouse.LeftClick())
                    {
                        rectToBeAdded = new Rectangle((int)MyMouse.RealPosition.X, (int)MyMouse.RealPosition.Y, 0, 0);
                        isDraggingRect = true;
                        if (MyKeyboard.IsHeld(Keys.LeftShift))
                        {
                            Vector2 snappedPosition = GetSnappedMousePosition();
                            rectToBeAdded = MathAid.UpdateRectViaVector(rectToBeAdded, snappedPosition);
                        }
                    }
                    if (isDraggingRect)
                    {
                        if (MyKeyboard.IsHeld(Keys.LeftShift))
                        {
                            Vector2 snappedPosition = GetSnappedMousePosition();
                            rectToBeAdded.Width = (int)snappedPosition.X - rectToBeAdded.X + TileSet.tileWidth;
                            rectToBeAdded.Height = (int)snappedPosition.Y - rectToBeAdded.Y + TileSet.tileHeight;
                        }
                        else
                        {
                            rectToBeAdded.Width = (int)MyMouse.RealPosition.X - rectToBeAdded.X;
                            rectToBeAdded.Height = (int)MyMouse.RealPosition.Y - rectToBeAdded.Y;
                        }
                        if (MyMouse.LeftReleased())
                        {
                            AddRect();
                        }
                    }
                    #endregion

                    #region RemoveRects
                    if (Scripts.KeyIsPressed(Keys.Space))
                    {
                        if (MyMouse.RightClick() || MyMouse.RightHeld())
                        {
                            RemoveRects();
                        }
                    }
                    else
                    {
                        if (MyMouse.RightClick())
                        {
                            RemoveRects();
                        }
                    }
                    #endregion
                }
                else
                {
                    if (isDraggingRect)
                    {
                        isDraggingRect = false;
                    }
                }
            }
            if (MyKeyboard.JustPressed(Keys.B))
            {
                blocksMode = !blocksMode;
            }
            #endregion
        }

        private static void CheckForInput()
        {
            if(MyKeyboard.JustPressed(Keys.Left))
            {
                if(currTileset<=0)
                {
                    Editor.Initialize(TileSet.TileSetsCount - 1);
                }
                else
                {
                    Editor.Initialize(currTileset - 1);
                }
            }
            else if (MyKeyboard.JustPressed(Keys.Right))
            {
                if (currTileset >= TileSet.TileSetsCount - 1)
                {
                    Editor.Initialize(0);
                }
                else
                {
                    Editor.Initialize(currTileset + 1);
                }
            }

            Vector2 cameraMovement = Vector2.Zero;

            if (MyKeyboard.IsHeld(Keys.A))
            {
                cameraMovement.X -= cameraSpeed;
            }
            else if (MyKeyboard.IsHeld(Keys.D))
            {
                cameraMovement.X += cameraSpeed;
            }

            if (MyKeyboard.IsHeld(Keys.W))
            {
                cameraMovement.Y -= cameraSpeed;
            }
            else if (MyKeyboard.IsHeld(Keys.S))
            {
                cameraMovement.Y += cameraSpeed;
            }

            if (MyKeyboard.IsHeld(Keys.LeftShift))
            {
                cameraMovement *= 3;
            }

            Main.camera.Move(cameraMovement);
        }

        private static void AddRect()
        {
            isDraggingRect = false;
            Main.currentLevel.blockRects.Add(rectToBeAdded);
        }

        private static void RemoveRects()
        {
            foreach (Rectangle rect in Main.currentLevel.blockRects)
            {
                if (rect.Contains(MyMouse.RealPosition))
                {
                    rectsToRemove.Add(rect);
                }
            }
        }

        private static bool MouseIsInWorkingArea()
        {
            return MyMouse.Position.X < Main.WindowWidth - currSpriteSheet.Width;
        }

        private static void ChangeTile()
        {
            //undoTileMaps.Push(Main.currentLevel.tilemap.Clone<TileMap>());

            int x = (int)MyMouse.RealPosition.X / TileSet.tileWidth;
            int y = (int)MyMouse.RealPosition.Y / TileSet.tileHeight;

            if (multipleSelected)
            {
                int tilesPerRow = currSpriteSheet.Width / TileSet.tileWidth;

                for (int i = 0; i < multipleTileX + 1; i++)
                {
                    int tileId = tilesPerRow * (currentTileY + multipleTileY) + currentTileX + i;

                    if(CheckIfInTileIsRange(x + i, y + multipleTileY))
                        Main.currentLevel.tilemap.AddMergeTile(x + i, y + multipleTileY, tileId, currTileset);

                    for (int b = 1; b <= multipleTileY; b++)
                    {
                        int topTileId = tilesPerRow * (currentTileY + multipleTileY - b) + currentTileX + i;

                        if(CheckIfInTileIsRange(x + i, y + multipleTileY))
                            Main.currentLevel.tilemap.AddTopTile(x + i, y + multipleTileY, topTileId, currTileset);
                    }
                }
            }

            else if (MyKeyboard.IsHeld(Keys.LeftControl))
            {
                if(CheckIfInTileIsRange(x,y))
                    Main.currentLevel.tilemap.AddMergeTile(x, y, selectedTileId, currTileset);
            }

            else if (MyKeyboard.IsHeld(Keys.LeftShift))
            {
                if (CheckIfInTileIsRange(x, y))
                    Main.currentLevel.tilemap.AddTopTile(x, y, selectedTileId, currTileset);
            }

            else
            {
                if (CheckIfInTileIsRange(x, y))
                    Main.currentLevel.tilemap.ChangeBaseTile(x, y, selectedTileId, currTileset);
            }
        }

        private static bool CheckIfInTileIsRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Main.currentLevel.tilemap.Width && y < Main.currentLevel.tilemap.Height;
        }

        private static void RemoveTile()
        {
            int x = (int)MyMouse.RealPosition.X / TileSet.tileWidth;
            int y = (int)MyMouse.RealPosition.Y / TileSet.tileHeight;

            if (MyKeyboard.IsHeld(Keys.LeftControl))
            {
                Main.currentLevel.tilemap.RemoveMergeTile(x, y);
            }
            else if (MyKeyboard.IsHeld(Keys.LeftShift))
            {
                Main.currentLevel.tilemap.RemoveTopTile(x, y);
            }
            else
            {
                Main.currentLevel.tilemap.RemoveBaseTile(x, y);
            }
        }        

        private static void ChangeSelectedTile()
        {
            int tilesPerRow = currSpriteSheet.Width / TileSet.tileWidth;
            int x = ((int)MyMouse.Position.X - (int)currTileSetPosition.X) / TileSet.tileWidth;
            int y = ((int)MyMouse.Position.Y - (int)currTileSetPosition.Y) / TileSet.tileHeight;
            currentTileX = x;
            currentTileY = y;
            selectedTileId = tilesPerRow * y + x;
            markerPosition = new Vector2(currTileSetPosition.X + x * TileSet.tileWidth, currTileSetPosition.Y + y * TileSet.tileHeight);
            markerRect = defaultRect;
            multipleSelected = false;
        }

        public static void Draw(SpriteBatch spriteBatch, bool drawingGui)
        {
            if (drawingGui)
            {
                spriteBatch.Draw(pixelBox, backgroundPosition, backgroundRectangle, Color.Beige, 0, new Vector2(), 1f, SpriteEffects.None, 0.6f);
                spriteBatch.Draw(currSpriteSheet, currTileSetPosition, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.61f);
                spriteBatch.DrawString(Resources.fonts[0], "Blocks Mode : " + blocksMode.ToString(), new Vector2(30), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Resources.fonts[0], "Mouse Position : " + MyMouse.Position, new Vector2(30, 60), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Resources.fonts[0], "Mouse Real Position : " + MyMouse.RealPosition, new Vector2(30, 90), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Resources.fonts[0], "Camera Position : " + (Main.camera.pos - Main.camera.zeroPos).ToString(), new Vector2(30, 120), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.DrawString(Resources.fonts[0], "CT: " + currTileset.ToString(), new Vector2(30, 150), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.8f);
                spriteBatch.Draw(selectedTileTexture, markerPosition, markerRect, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.9999f);
            }
            else
            {
                if (blocksMode)
                {

                    if (isDraggingRect)
                    {
                        spriteBatch.Draw(pixelBox, rectToBeAdded, rectToBeAdded, Color.Black * 0.5f, 0, new Vector2(), SpriteEffects.None, Layers.BOUNDINGBOXES);
                    }

                }
                else
                {

                    if (MyMouse.Position.X < Main.WindowWidth - currSpriteSheet.Width)
                    {                        
                        Vector2 MyMouseSelectedPosition = GetSnappedMousePosition();
                        spriteBatch.Draw(selectedTileTexture, Main.currentLevel.tilemap.Position + MyMouseSelectedPosition, markerRect, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.95f);
                    }

                }
            }
        }

        private static Vector2 GetSnappedMousePosition()
        {
            Vector2 snappedPosition;
            snappedPosition = new Vector2((int)(MyMouse.RealPosition.X / TileSet.tileWidth), (int)(MyMouse.RealPosition.Y / TileSet.tileHeight));
            snappedPosition.X *= TileSet.tileWidth;
            snappedPosition.Y *= TileSet.tileHeight;
            return snappedPosition;
        }
    }
}