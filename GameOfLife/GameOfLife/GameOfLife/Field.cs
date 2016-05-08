using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GameOfLife
{
    public class Field
    {
        private bool[,] data;
        private int width;
        private int heigth;
        private Texture2D cellTexture;
        private Vector2 position;
        private List<Cell> cells;
        private bool automaticPlay;
        private TimeSpan timeToStep;
        private int stepMilliseconds;
        private Point? selectedPosition;
        Random rand;

        public Field(int Width,int Height,int CellWidth, int CellHeigth, Vector2 Position)
        {
            rand = new Random();
            this.width = Width;
            this.heigth = Height;
            this.data = new bool[this.width, this.heigth];
            GenerateCellTexture(CellWidth, CellHeigth);
            this.position = Position;
            cells = new List<Cell>();
            automaticPlay = false;
            stepMilliseconds = 100;
            timeToStep = new TimeSpan(0, 0, 0, 0, stepMilliseconds);
        }

        public void AddCell(Point position, Color cellColor)
        {
            cells.Add(new Cell(position, cellColor));
            data[position.X, position.Y] = true;
        }

        public void RemoveCell(Point position)
        {
            foreach (var cell in cells)
            {
                if(cell.position.X == position.X && cell.position.Y == position.Y)
                {
                    cells.Remove(cell);
                    data[position.X, position.Y] = false;
                    return;
                }
            }

            throw new IndexOutOfRangeException("Cell not found at position " + position.ToString());
        }

        private void GenerateCellTexture(int cellWidth, int cellHeigth)
        {
            cellTexture = new Texture2D(Main.graphics.GraphicsDevice, cellWidth, cellHeigth);
            Color[] data = new Color[cellWidth * cellHeigth];
            data.Populate(Color.White);
            cellTexture.SetData(data);
        }

        public void Update(GameTime gameTime)
        {
            if (Main.keyboard.JustPressed(Keys.Space))
            {
                automaticPlay = !automaticPlay;
            }
            if (!automaticPlay)
            {
                if (Main.keyboard.JustPressed(Keys.S))
                {
                    StepFurther();
                }
            }
            else
            {
                timeToStep = timeToStep.Subtract(gameTime.ElapsedGameTime);

                if(timeToStep.Milliseconds<0)
                {
                    timeToStep = new TimeSpan(0, 0, 0, 0, stepMilliseconds);
                    StepFurther();
                }
            }

            UpdateSelectedPosition();

            if(Main.mouse.LeftHeld() && selectedPosition != null)
            {
                if(!data[selectedPosition.Value.X, selectedPosition.Value.Y])
                {
                    int x = rand.Next(50);
                    if(x < 25)
                        AddCell(selectedPosition.Value, Color.Red);
                    else
                        AddCell(selectedPosition.Value, Color.Orange);
                }
            }
            if (Main.mouse.RightHeld() && selectedPosition != null)
            {
                if(data[selectedPosition.Value.X, selectedPosition.Value.Y])
                {                    
                    RemoveCell(selectedPosition.Value);
                }
            }
            if(Main.keyboard.JustPressed(Keys.C))
            {
                Clear();
            }
        }

        private void Clear()
        {
            for (int i = cells.Count-1; i >= 0; i--)
            {
                RemoveCell(cells[i].position);
            }
        }

        private void UpdateSelectedPosition()
        {
            float x = Main.mouse.RealPosition.X;
            float y = Main.mouse.RealPosition.Y;
            x = x / (cellTexture.Width * Main.camera.Zoom + 1);
            y = y / (cellTexture.Height * Main.camera.Zoom + 1);

            if(IsWithinField((int)x,(int)y))
            {
                selectedPosition = new Point((int)x,(int)y);                
            }
            else
            {
                selectedPosition = null;
            }
        }

        private void StepFurther()
        {
            bool[,] newData = new bool[width, heigth];
            int counter = 0;
            foreach (var cell in cells)
            {
                for (int i = cell.position.X - 1; i < cell.position.X + 2; i++)
                {
                    for (int b = cell.position.Y - 1; b < cell.position.Y + 2; b++)
                    {
                        if (IsWithinField(i, b))
                        {
                            if (!(i == cell.position.X && b == cell.position.Y))
                            {
                                if (!data[i, b])
                                {
                                    newData[i, b] = CheckForReproduction(i, b);
                                }
                                else
                                {
                                    counter++;
                                }
                            }
                        }
                    }
                }

                if (counter >= 2 && counter <= 3)
                {
                    newData[cell.position.X, cell.position.Y] = true;
                }
                counter = 0;
            }
            cells.Clear();
            data = newData;

            for (int i = 0; i < width; i++)
            {
                for (int b = 0; b < heigth; b++)
                {
                    if (data[i, b])
                    {
                        Random rand = new Random();
                        int x = rand.Next(50);
                        if (x < 25)
                            AddCell(new Point(i,b), Color.Red);
                        else
                            AddCell(new Point(i, b), Color.Orange);
                    }
                }
            }
        }

        private bool IsWithinField(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < heigth;
        }

        private bool CheckForReproduction(int x, int y)
        {
            int counter = 0;
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int b = y - 1; b < y + 2; b++)
                {
                    if(IsWithinField(i,b))
                    {
                        if(data[i,b])
                        {
                            counter++;

                            if(counter>3)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return counter == 3;            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cellPos;
            foreach (var cell in cells)
            {
                cellPos = position + new Vector2(cell.position.X * (cellTexture.Width + 1), cell.position.Y * (cellTexture.Height + 1));
                spriteBatch.Draw(cellTexture, cellPos, null, cell.color, 0.0f, new Vector2(0), 1f, SpriteEffects.None, 0.8f);
            }
            if(selectedPosition != null)
            {
                Vector2 selPos = position + new Vector2(selectedPosition.Value.X * (cellTexture.Width + 1), selectedPosition.Value.Y * (cellTexture.Height + 1));
                spriteBatch.Draw(cellTexture, selPos, null, Color.Red * 0.4f, 0f, new Vector2(), 1f, SpriteEffects.None, 0.9f);
                
            }
        }
    }
}
