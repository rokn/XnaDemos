using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Rpg
{
    public class Inventory
    {
        private int width, height;
        private Texture2D cellTexture, sellectedTexture;
        private Vector2 Position;
        private int cellXOffset, cellYOffset;
        public bool opened;
        public Cell[,] cells;
        public Rectangle rect;

        public Inventory(int Width, int Height, Vector2 position, int xOffset = 0, int yOffset = 0)
        {
            opened = false;
            width = Width;
            height = Height;
            Position = position;
            cellXOffset = xOffset;
            cellYOffset = yOffset;
        }

        public void Load(ContentManager Content)
        {
            cellTexture = Scripts.LoadTexture(@"Inventory\CellTexture",Content);
            sellectedTexture = Scripts.LoadTexture(@"Inventory\SelectedCellTexture",Content);
            SetUpCells();
            rect = new Rectangle((int)Position.X, (int)Position.Y, (cellTexture.Width + cellXOffset) * width, (cellTexture.Height + cellYOffset) * height);
        }

        void SetUpCells()
        {
            cells = new Cell[width,height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int xPos, yPos;
                    xPos = (int)Position.X + cellTexture.Width * x + cellXOffset * x;
                    yPos = (int)Position.Y + cellTexture.Height * y + cellYOffset * y;
                    cells[x,y] = new Cell(new Rectangle(xPos, yPos, cellTexture.Width, cellTexture.Height), cellTexture, sellectedTexture);
                }
            }
        }

        public void Update()
        {
            if (opened)
            {
                foreach (Cell cell in cells)
                {
                    cell.Update();
                }
                if(Rpg.mouse.LeftClick())
                {
                    if (Scripts.CheckIfMouseIsOver(rect))
                    {
                        Cell cell = GetSelectedCell();
                        if (cell != null)
                        {
                            LeftClickHandle(cell);
                        }
                    }
                }
                if (Rpg.mouse.RightClick())
                {
                    if (Scripts.CheckIfMouseIsOver(rect))
                    {
                        Cell cell = GetSelectedCell();
                        if (cell != null)
                        {
                            RightClickHandle(cell);
                        }
                    }
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (opened)
            {
                foreach (Cell cell in cells)
                {
                    cell.Draw(spriteBatch);
                }
            }
        }

        public bool AddItem(Item item)
        {
            Cell cell = GetFreeCell(true);
            if (item.stats.Stackable)
            {
                Cell stackableCell;
                do
                {
                    stackableCell = GetStackableCell(item.Id);
                    if (stackableCell != null)
                    {
                        PlaceStackableItem(stackableCell, item);
                    }
                } while (stackableCell != null && item.Stack > 0);
                while (item.Stack > 0 && cell != null)
                {
                    if (item.Stack > item.stats.MaxStack)
                    {
                        cell.item = new Item(item.Id,item.Position,item.stats.MaxStack);
                        cell.free = false;
                        cell = GetFreeCell(true);
                        item.Stack -= item.stats.MaxStack;
                    }
                    else
                    {
                        cell.item = new Item(item.Id, item.Position, item.Stack);
                        cell.free = false;
                        item.Stack = 0;
                    }
                }
                if(item.Stack>0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (cell != null)
                {
                    cell.item = item;
                    cell.free = false;
                    cell.HandleTooltip();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private Cell GetSelectedCell()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(cells[x,y].selected)
                    {
                        return cells[x, y];
                    }
                }
            }
            return null;
        }

        private Cell GetStackableCell(int itemId)
        {
            foreach (Cell cell in cells)
            {
                if(!cell.free)
                {
                    if(cell.item.Id == itemId)
                    {
                        if (cell.item.Stack < cell.item.stats.MaxStack)
                        {
                            return cell;
                        }
                    }
                }
            }
            return null;
        }

        private Cell GetFreeCell(bool fromBack)
        {
            if (fromBack)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        if(cells[x,y].free)
                        {
                            return cells[x, y];
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (cells[x, y].free)
                        {
                            return cells[x, y];
                        }
                    }
                }
            }
            return null;
        }

        private void LeftClickHandle(Cell cell)
        {
            if(Rpg.mouseHasItem)
            {
                if(cell.free)
                {
                    PlaceItemFromMouse(cell);
                }
                else
                {
                    if (Rpg.mouseItem.stats.Stackable && Rpg.mouseItem.Id == cell.item.Id)
                    {
                        if(PlaceStackableItem(cell, Rpg.mouseItem))
                        {
                            Rpg.mouseItem = null;
                            Rpg.mouseHasItem = false;
                        }
                    }
                    else
                    {
                        SwapItemsWithMouse(cell);
                    }
                }
            }
            else
            {
                if(!cell.free)
                {
                    TakeItemInMouse(cell);
                }
            }
        }

        private void RightClickHandle(Cell cell)
        {
            if(!cell.free)
            {
                if(!Rpg.mouseHasItem)
                {
                    Rpg.mouseItem = new Item(cell.item.Id, cell.item.Position, 1);
                    Rpg.mouseHasItem = true;
                    cell.item.Stack--;
                    if(cell.item.Stack<=0)
                    {
                        cell.item = null;
                        cell.free = true;
                        cell.Tooltip.Clear();
                    }
                }
                else
                {
                    if(Rpg.mouseItem.Id==cell.item.Id)
                    {
                        cell.item.Stack--;
                        Rpg.mouseItem.Stack++;
                        if (cell.item.Stack <= 0)
                        {
                            cell.item = null;
                            cell.free = true;
                            cell.Tooltip.Clear();
                        }
                    }
                }
            }
        }

        private bool PlaceStackableItem(Cell cell,Item item)
        {
            cell.item.Stack += item.Stack;
            if(cell.item.Stack>cell.item.stats.MaxStack)
            {
                item.Stack = cell.item.Stack - item.stats.MaxStack;
                cell.item.Stack = cell.item.stats.MaxStack;
                return false;
            }
            else
            {
                item.Stack = 0;
            }
            return true;
        }

        private void TakeItemInMouse(Cell cell)
        {
            Rpg.mouseItem = cell.item;
            Rpg.mouseHasItem = true;
            cell.free = true;
            cell.item = null;
            cell.Tooltip.Clear();
        }

        private void SwapItemsWithMouse(Cell cell)
        {
            Item item = cell.item;
            cell.item = Rpg.mouseItem;
            Rpg.mouseItem = new Item(item.Id,item.Position,item.Stack);
            cell.HandleTooltip();
        }

        private void PlaceItemFromMouse(Cell cell)
        {
            cell.item = Rpg.mouseItem;
            cell.free = false;
            Rpg.mouseHasItem = false;
            Rpg.mouseItem = null;
            cell.HandleTooltip();
        }

    }
}
