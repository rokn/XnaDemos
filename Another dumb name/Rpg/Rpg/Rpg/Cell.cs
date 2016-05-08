using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rpg
{
    public class Cell
    {
        public Rectangle rect, itemRect;
        public Texture2D texture, selTexture;
        public bool selected, free;
        public List<string> Tooltip;
        public Texture2D tooltipTexture;
        float toolTipAlpha = 0.8f;
        public Item item;
        public Cell(Rectangle Rect, Texture2D Texture, Texture2D SelectedTexture)
        {
            item = null;
            free = true;
            rect = Rect;
            itemRect = new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
            texture = Texture;
            selTexture = SelectedTexture;
            selected = false;
            Tooltip = new List<string>();
            tooltipTexture = null;
        }

        public void Update()
        {
            if (!selected)
            {
                if (rect.Contains(Rpg.mouse.clickRectangle))
                {
                    selected = true;
                }
            }
            else
            {
                if (!rect.Contains(Rpg.mouse.clickRectangle))
                {
                    selected = false;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.86f);
            if (!free)
            {
                spriteBatch.Draw(item.Texture, itemRect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.87f);
                if (item.stats.Stackable)
                {
                    spriteBatch.DrawString(Rpg.Font, item.Stack.ToString(), new Vector2(rect.X + 5, rect.Y), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
                }
            }
            if (selected)
            {
                spriteBatch.Draw(selTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.88f);
                if (!free)
                {
                    Scripts.DrawTooltip(Tooltip, tooltipTexture, toolTipAlpha, spriteBatch);
                }
            }
        }

        public void HandleTooltip()
        {
            Tooltip = Scripts.GenerateItemTooltip(item);
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
        }
    }
}
