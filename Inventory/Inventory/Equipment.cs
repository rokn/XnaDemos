using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public class Equipment
    {
        public int typeEquip;
        List<string> Tooltip;
        Vector2 position;
        Texture2D texture,tooltipTexture;
        public Rectangle rect,itemRect;
        public Item item;
        float toolTipAlpha=0.8f;
        public bool free,selected;
        /// <summary>
        /// Create a new Equipment slot
        /// </summary>
        /// <param name="Texture">The texture that is used as a slot</param>
        /// <param name="Position">The position of the slot</param>
        /// <param name="type">The type of the item that can be placed inside</param>
        public Equipment(Texture2D Texture,Vector2 Position,int type)
        {
            position = Position;
            texture = Texture;
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            itemRect = new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
            item = null;
            free = true;
            typeEquip = type;
            Tooltip = new List<string>();
            tooltipTexture = null;
            selected = false;
        }
        public void Update()
        {
            if (rect.Contains(Rpg.mouse.clickRectangle))
            {
                selected = true;
                if (Rpg.mouse.LeftClicked())
                {
                    if (Rpg.hasItemAtMouse)
                    {
                        if (Rpg.mouseItem.equipType == typeEquip)
                        {
                            if (free)
                            {
                                item = Rpg.mouseItem;
                                Rpg.mouseItem = null;           //Place item in slot
                                Rpg.hasItemAtMouse = false;
                                free = false;
                            }
                            else
                            {
                                Item temp = item;
                                item = Rpg.mouseItem;         // Swap items
                                Rpg.mouseItem = temp;
                            }
                            Tooltip = Scripts.GenerateTooltip(item);
                            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
                        }
                    }
                    else
                    {
                        if (!free)
                        {
                            Rpg.mouseItem = item;
                            Rpg.hasItemAtMouse = true; // Take item from slot
                            item = null;
                            free = true;
                            ClearTooltip();
                        }
                    }
                }
            }
            else { selected = false; }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,position, null, Color.White,0, new Vector2(),1f, SpriteEffects.None, 0.861f);// Draw the back
            if(!free)
            {
                spriteBatch.Draw(item.texture, itemRect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.862f);// Draw tthe item
                if(selected)
                DrawTooltip(spriteBatch); // Draw the tooltip
            }
        }
        public void DrawTooltip(SpriteBatch spriteBatch)
        {
            if (Tooltip.Count > 0)
            {
                if (tooltipTexture != null)
                {
                    int i = 0;
                    spriteBatch.Draw(tooltipTexture, Rpg.mouse.Position, null, Color.White * toolTipAlpha, 0, new Vector2(), 1, SpriteEffects.None, 0.9f); // the back of the tooltip
                    foreach (string s in Tooltip)
                    {
                        spriteBatch.DrawString(Rpg.Font, s, Rpg.mouse.Position + new Vector2(9, i * 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f); // Thte text of the tooltip
                        i++;
                    }
                }
            }
        }
        


        internal void ClearTooltip()
        {
            Tooltip.Clear();
        }
    }
}
