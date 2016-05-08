using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public class Equipment
    {
        public EquipSlot equipType;
        List<string> Tooltip;
        Vector2 position;
        Texture2D texture, tooltipTexture,baseTexture;
        public Rectangle rect, itemRect;
        public Item item;
        float toolTipAlpha = 0.8f;
        public bool free, selected;
        /// <summary>
        /// Create a new Equipment slot
        /// </summary>
        /// <param name="Texture">The texture that is used as a slot</param>
        /// <param name="Position">The position of the slot</param>
        /// <param name="type">The type of the item that can be placed inside</param>
        public Equipment(Vector2 Position, EquipSlot type)
        {
            position = Position;
            item = null;
            free = true;
            equipType = type;
            Tooltip = new List<string>();
            tooltipTexture = null;
            selected = false;
        }
        public void Load(ContentManager Content)
        {
            texture = Scripts.LoadTexture(@"Inventory\Equipment_" + equipType.ToString(), Content);
            baseTexture = Scripts.LoadTexture(@"Inventory\Equipment_Base", Content);
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            itemRect = new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
        }
        public void Update()
        {
            if (Scripts.CheckIfMouseIsOver(rect))
            {
                selected = true;
                if (Rpg.mouse.LeftClick())
                {
                    if (Rpg.mouseHasItem)
                    {
                        if (Rpg.mouseItem.stats.equipSlot == equipType)
                        {
                            if (free)
                            {
                                PlaceItemInSlot();
                            }
                            else
                            {
                                SwapItems();   
                            }
                            Tooltip = Scripts.GenerateItemTooltip(item);
                            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
                        }
                    }
                    else
                    {
                        if (!free)
                        {
                            TakeItemFromSlot();
                        }
                    }
                }
            }
            else { selected = false; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            if (!free)
            {
                spriteBatch.Draw(baseTexture, position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.861f);// Draw the back
                spriteBatch.Draw(item.Texture, itemRect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.862f);// Draw the item
                if (selected)
                {
                    Scripts.DrawTooltip(Tooltip,tooltipTexture,toolTipAlpha,spriteBatch); // Draw the tooltip
                }
            }
            else
            {
                spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.861f);
            }
        }

        private void SwapItems()
        {
            Item temp = item;
            item = Rpg.mouseItem;
            Rpg.mouseItem = temp;
        }

        private void TakeItemFromSlot()
        {
            Rpg.mouseItem = item;
            Rpg.mouseHasItem = true;
            item = null;
            free = true;
            Tooltip.Clear();
        }

        private void PlaceItemInSlot()
        {
            item = Rpg.mouseItem;
            Rpg.mouseItem = null;
            Rpg.mouseHasItem = false;
            free = false;
        }
    }
}
