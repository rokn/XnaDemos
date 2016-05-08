using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rpg
{
    public class Cell
    {
        public Rectangle rect,itemRect;
        public Texture2D texture,selTexture;
        public bool selected,free;
        public List<string> Tooltip;
        public Texture2D tooltipTexture;
        float toolTipAlpha = 0.8f;
        public Item item;
        public Cell(Rectangle Rect, Texture2D texture, Texture2D SelectedTexture)
        {
            item = null;
            free = true;
            rect = Rect;
            itemRect = new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
            this.texture = texture;
            selTexture = SelectedTexture;
            selected = false;
            Tooltip = new List<string>();
            tooltipTexture = null;
        }
        
        public  void Update()
        {
            if (!selected)
            {
                if (rect.Contains(Rpg.mouse.clickRectangle))
                {
                    selected = true;
                    tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
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
                spriteBatch.Draw(item.texture, itemRect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.87f);
                if (item.stackable)
                {
                    spriteBatch.DrawString(Rpg.Font, item.stack.ToString(), new Vector2(rect.X+5, rect.Y), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
                }
            }
            if(selected)
            {
                spriteBatch.Draw(selTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.88f);
                
                DrawTooltip(spriteBatch);
            }
        }
        public void DrawTooltip(SpriteBatch spriteBatch)
        {
            if(Tooltip.Count>0)
            {
                if (tooltipTexture != null)
                {
                    int i=0;
                    spriteBatch.Draw(tooltipTexture, Rpg.mouse.Position,null, Color.White*toolTipAlpha,0,new Vector2(),1,SpriteEffects.None,0.9f);
                    foreach(string s in Tooltip)
                    {
                        spriteBatch.DrawString(Rpg.Font, s,Rpg.mouse.Position + new Vector2(9, i * 18), Color.Black,0,new Vector2(),1,SpriteEffects.None,1f);
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
