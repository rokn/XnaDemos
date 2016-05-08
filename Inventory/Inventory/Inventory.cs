using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rpg
{
    public class Inventory
    {
        int width, height;
        Texture2D cellTexture,sellectedTexture;
        Vector2 Position;
        int cellXOffset, cellYOffset;
        public bool opened;
        public List<List<Cell>> cells;
        public Rectangle rect;
        #region Constructors
        public Inventory(int Width, int Height, Texture2D CellTexture, Texture2D SellectedCellTexture, Vector2 position)
        {
            opened = false;
            width = Width;
            height = Height;
            cellTexture = CellTexture;
            sellectedTexture = SellectedCellTexture;
            Position = position;
            cellXOffset = 0;
            cellYOffset = 0;
            SetUpCells();
            rect = new Rectangle((int)position.X, (int)position.Y, cellTexture.Width * width, cellTexture.Height * height);
        }
        public Inventory(int Width, int Height, Texture2D CellTexture, Texture2D SellectedCellTexture, Vector2 position,int xOffset,int yOffset)
        {
            opened = false;
            width = Width;
            height = Height;
            cellTexture = CellTexture;
            sellectedTexture = SellectedCellTexture;
            Position = position;
            cellXOffset = xOffset;
            cellYOffset = yOffset;
            SetUpCells();
            rect = new Rectangle((int)position.X, (int)position.Y, cellTexture.Width * width + xOffset * (width + 1), cellTexture.Height * height + yOffset * (height - 1));
        }
        void SetUpCells()
        {
            cells = new List<List<Cell>>();
            for (int y = 0; y < height; y++)
            {
                cells.Add(new List<Cell>());
                for (int x = 0; x < width; x++)
                {
                    int xPos,yPos;
                    xPos = (int)Position.X + cellTexture.Width * x + cellXOffset * x;
                    yPos = (int)Position.Y + cellTexture.Height * y + cellYOffset * y;
                    cells[y].Add(new Cell(new Rectangle(xPos,yPos,cellTexture.Width,cellTexture.Height),cellTexture,sellectedTexture));
                }
            }
        }
        #endregion
        public void Update()
        {
            if (opened)
            {
                foreach (Equipment equip in Rpg.equipment)
                {
                    equip.Update();
                }
                foreach (List<Cell> list in cells)
                {
                    foreach (Cell cell in list)
                    {
                        cell.Update();
                        
                    }
                }
                if (Rpg.mouse.RightClick())
                {
                    if (rect.Contains(Rpg.mouse.clickRectangle))
                    {
                        foreach (List<Cell> list in cells)
                        {
                            foreach (Cell cell in list)
                            {
                                if (cell.selected && !cell.free)
                                {
                                    if (cell.item.stackable)
                                    {
                                        if (!Rpg.hasItemAtMouse)
                                        {
                                            Rpg.hasItemAtMouse = true;
                                            Rpg.mouseItem = new Item(cell.item.Id, new Vector2(0), 0);
                                            if (cell.item.stack > 1)
                                            {
                                                Rpg.mouseItem.stack++;
                                                cell.item.stack--;
                                            }
                                            else
                                            {
                                                Rpg.mouseItem.stack++;
                                                cell.item = null;
                                                cell.free = true;
                                                cell.ClearTooltip();
                                            }
                                        }
                                        else
                                        {
                                            if (cell.item.stack > 1)
                                            {
                                                Rpg.mouseItem.stack++;
                                                cell.item.stack--;
                                            }
                                            else
                                            {
                                                Rpg.mouseItem.stack++;
                                                cell.item = null;
                                                cell.free = true;
                                                cell.ClearTooltip();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Rpg.mouse.LeftClicked())
                {
                    if (rect.Contains(Rpg.mouse.clickRectangle))
                    {
                        foreach (List<Cell> list in cells)
                        {
                            foreach (Cell cell in list)
                            {
                                if (!Rpg.hasItemAtMouse)
                                {
                                    if (cell.selected && !cell.free)
                                    {
                                        cell.free = true;
                                        Rpg.mouseItem = cell.item;
                                        Rpg.hasItemAtMouse = true;
                                        cell.item = null;
                                        cell.ClearTooltip();
                                    }
                                }
                                else
                                {
                                    if (!Rpg.mouseItem.stackable)
                                    {
                                        if (cell.selected )
                                        {
                                            if (cell.free)
                                            {
                                                cell.free = false;
                                                cell.item = Rpg.mouseItem;
                                                Rpg.mouseItem = null;
                                                Rpg.hasItemAtMouse = false;
                                                cell.Tooltip = Scripts.GenerateTooltip(cell.item);
                                                cell.tooltipTexture = Scripts.GenerateTooltipTexture(cell.Tooltip);
                                            }
                                            else
                                            {
                                                Item temp = Rpg.mouseItem;
                                                Rpg.mouseItem = cell.item;
                                                cell.item = temp;
                                                cell.Tooltip = Scripts.GenerateTooltip(cell.item);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (cell.selected)
                                        {
                                            if (cell.free)
                                            {
                                                cell.free = false;
                                                cell.item = Rpg.mouseItem;
                                                Rpg.mouseItem = null;
                                                Rpg.hasItemAtMouse = false;
                                                cell.Tooltip = Scripts.GenerateTooltip(cell.item);
                                                cell.tooltipTexture = Scripts.GenerateTooltipTexture(cell.Tooltip);
                                            }
                                            else
                                            {
                                                if (cell.item.Id == Rpg.mouseItem.Id)
                                                {
                                                    cell.item.stack += Rpg.mouseItem.stack;
                                                    if (cell.item.stack > cell.item.maxStack)
                                                    {
                                                        Rpg.mouseItem.stack = cell.item.stack - cell.item.maxStack;
                                                        cell.item.stack = cell.item.maxStack;
                                                    }
                                                    else
                                                    {
                                                        Rpg.mouseItem = null;
                                                        Rpg.hasItemAtMouse = false;
                                                    }
                                                }
                                                else
                                                {
                                                    Item temp = Rpg.mouseItem;
                                                    Rpg.mouseItem = cell.item;
                                                    cell.item = temp;
                                                    cell.Tooltip = Scripts.GenerateTooltip(cell.item);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (opened)
            {
                foreach(Equipment equip in Rpg.equipment)
                {
                    equip.Draw(spriteBatch);
                }
                foreach (List<Cell> list in cells)
                {
                    foreach (Cell cell in list)
                    {
                        cell.Draw(spriteBatch);
                    }
                }
            }
        }
        public bool AddItem(Item item)
        {
            bool full = true;
            if (!item.stackable)
            {
                foreach (List<Cell> list in cells)
                {
                    foreach (Cell cell in list)
                    {
                        if (cell.free)
                        {
                            full = false;
                        }
                    }
                }
                if (!full)
                {
                    for (int y = cells.Count - 1; y >= 0; y--)
                    {
                        for (int x = cells[y].Count - 1; x >= 0; x--)
                        {
                            if (cells[y][x].free)
                            {
                                cells[y][x].item = item;
                                cells[y][x].free = false;
                                cells[y][x].Tooltip = Scripts.GenerateTooltip(cells[y][x].item);
                                goto End;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (List<Cell> list in cells)
                {
                    foreach (Cell cell in list)
                    {
                        if (item.stack > 0)
                        {
                            if (!cell.free)
                            {
                                if (cell.item.Id == item.Id)
                                {

                                    if (cell.item.stack < cell.item.maxStack)
                                    {
                                        cell.item.stack += item.stack;
                                        item.stack = 0;
                                    }
                                    if (cell.item.stack > cell.item.maxStack)
                                    {
                                        item.stack = cell.item.stack - cell.item.maxStack;
                                        cell.item.stack = cell.item.maxStack;
                                    }
                                }
                            }
                        }
                        else
                        {
                            full = false;
                            goto End;
                        }
                    }
                }
                if (item.stack > 0)
                {
                    foreach (List<Cell> list in cells)
                    {
                        foreach (Cell cell in list)
                        {
                            if (cell.free)
                            {
                                full = false;
                            }
                        }
                    }
                    if (!full)
                    {
                        for (int y = cells.Count - 1; y >= 0; y--)
                        {
                            for (int x = cells[y].Count - 1; x >= 0; x--)
                            {
                                if (cells[y][x].free)
                                {
                                    cells[y][x].item = item;
                                    cells[y][x].free = false;
                                    cells[y][x].Tooltip = Scripts.GenerateTooltip(cells[y][x].item);
                                    goto End;
                                }
                            }
                        }
                    }
                }
            }

            
            End: { }
            return !full;
        }
    }
}
