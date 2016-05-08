using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Rpg
{
    public static class Scripts
    {
        public static Texture2D LoadTexture(string asset,ContentManager Content)
        {
            try
            {
                return Content.Load<Texture2D>(asset) as Texture2D;
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("Texture not found: "+asset);
                return null;
            }
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if(rect.Contains(Rpg.mouse.clickRectangle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<string> GenerateItemTooltip(Item item)
        {
            item.Tooltip.Clear();
            item.Tooltip.Add(item.stats.Name);
            switch (item.stats.type)
            {
                case ItemType.Weapon:
                    item.Tooltip.Add("Equip slot: " + item.stats.equipSlot.ToString());
                    item.Tooltip.Add(item.stats.Damage + " " +item.stats.weaponType.ToString() + " damage");
                    break;
                case ItemType.Armor:
                    item.Tooltip.Add("Equip slot: " + item.stats.equipSlot.ToString());
                    item.Tooltip.Add(item.stats.armorType.ToString() + " armor");
                    item.Tooltip.Add(item.stats.Armor + " defense");
                    break;
                case ItemType.Potion:
                    item.Tooltip.Add("Drink to restore health");
                    item.Tooltip.Add("Restores " + item.stats.Damage + " health");
                    break;
            }
            if (item.stats.Stackable)
            {
                item.Tooltip.Add("Stackable");
            }
            if (item.stats.tooltip.Length > 0)
            {
                item.Tooltip.Add(item.stats.tooltip);
            }
            return item.Tooltip;
        }

        public static Texture2D GenerateTooltipTexture(List<string> Tooltip)
        {
            if (Tooltip.Count > 0)
            {
                float longestWidth = 0;
                Vector2 LongestWidth = new Vector2();
                foreach (string str in Tooltip)
                {
                    if (longestWidth < Rpg.Font.MeasureString(str).X)
                    {
                        longestWidth = Rpg.Font.MeasureString(str).X;
                        LongestWidth = Rpg.Font.MeasureString(str);
                    }
                }
                if (longestWidth > 0)
                {
                    return Scripts.RoundRectangle((int)LongestWidth.X + 15, 6 + (Tooltip.Count-1) * (int)(LongestWidth.Y), 3, Color.DeepSkyBlue, Color.Blue);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static void DrawTooltip(List<string> Tooltip,Texture2D tooltipTexture,float toolTipAlpha,SpriteBatch spriteBatch)
        {
            if (Tooltip.Count > 0)
            {
                if (tooltipTexture != null)
                {
                    int i = 0;
                    if (Rpg.mouse.Position.Y + tooltipTexture.Height <= Rpg.height)
                    {
                        spriteBatch.Draw(tooltipTexture, Rpg.mouse.Position, null, Color.White * toolTipAlpha, 0, new Vector2(), 1, SpriteEffects.None, 0.991f);
                        foreach (string s in Tooltip)
                        {
                            spriteBatch.DrawString(Rpg.Font, s, Rpg.mouse.Position + new Vector2(9, i * 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f);
                            i++;
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(tooltipTexture, new Vector2(Rpg.mouse.Position.X,Rpg.mouse.Position.Y-tooltipTexture.Height), null, Color.White * toolTipAlpha, 0, new Vector2(), 1, SpriteEffects.None, 0.991f);
                        foreach (string s in Tooltip)
                        {
                            spriteBatch.DrawString(Rpg.Font, s, new Vector2(Rpg.mouse.Position.X,Rpg.mouse.Position.Y-tooltipTexture.Height) + new Vector2(9, i * 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f);
                            i++;
                        }
                    }
                }
            }
        }

        public static string EncryptData(string Data)
        {
            string encrypted = "";
            string keyword = "trampoline";
            for (int i = 0; i < Data.Length; i++)
            {
                int numb1 = 0, numb2 = 0;
                if ((int)Data[i] <= 126)
                {
                    numb1 = (int)(((int)Data[i] - 27) / 10);
                    numb2 = ((int)Data[i] - 27) % 10;
                }
                encrypted += keyword[numb1];
                encrypted += keyword[numb2];
            }
            return encrypted;
        }

        public static string DecryptData(string Data)
        {
            string decrypted = "";
            string keyword = "trampoline";
            for (int i = 0; i < Data.Length; i += 2)
            {
                int numb1, numb2;
                numb1 = keyword.IndexOf(Data[i]);
                numb2 = keyword.IndexOf(Data[i + 1]);
                numb1 *= 10;
                numb1 += numb2;
                decrypted += (char)(numb1 + 27);
            }
            return decrypted;
        }
        
        public static Texture2D RoundRectangle(int width, int height, int borderThicknes, Color fillColor, Color borderColor)
        {
            Texture2D roundRect = new Texture2D(Rpg.graphics.GraphicsDevice, width, height);
            Color[] color = new Color[width * height];
            for (int x = 0; x < width; x++)
            {

                for (int y = 0; y < height; y++)
                {
                    bool empty = true;
                    Vector2 point = new Vector2(x, y);
                    Vector2 Center = Vector2.Zero;
                    if (y < borderThicknes)
                    {
                        if (x < borderThicknes)
                        {
                            Center = new Vector2(borderThicknes, borderThicknes);
                        }
                        else if (x > width - (borderThicknes))
                        {
                            Center = new Vector2(width - (borderThicknes), borderThicknes);
                        }
                        else Center = new Vector2(x, borderThicknes);
                    }
                    else if (y > height - (borderThicknes))
                    {
                        if (x < borderThicknes)
                        {
                            Center = new Vector2(borderThicknes, height - (borderThicknes));
                        }
                        else if (x > width - (borderThicknes))
                        {
                            Center = new Vector2(width - (borderThicknes), height - (borderThicknes));
                        }
                        else Center = new Vector2(x, height - (borderThicknes));
                    }
                    else
                    {
                        if (x < borderThicknes)
                            Center = new Vector2(borderThicknes, y);
                        else if (x > width - (borderThicknes))
                            Center = new Vector2(width - (borderThicknes), y);
                    }
                    if (Vector2.Distance(Center, point) > borderThicknes)
                    {
                        empty = false;
                    }
                    if (empty)
                    {
                        color[x + width * y] = borderColor;
                    }
                    if (x > borderThicknes - 1 && x < width - (borderThicknes - 1) && y > (borderThicknes - 1) && y < height - (borderThicknes - 1))
                    {
                        color[x + width * y] = fillColor;
                    }
                }
            }

            roundRect.SetData(color);
            return roundRect;
        }

        public static List<Rectangle> GetSpriteSheetRects(int frameWidth, int frameHeight,int columns, int frames)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int x = 0;
            int y = 0;
            for (int i = 0; i < frames; i++)
            {
                rects.Add(new Rectangle(x*frameWidth,y*frameHeight,frameWidth,frameHeight));
                x++;
                if (i > 0)
                {
                    if (i % columns == 0)
                    {
                        x = 0;
                        y++;
                    }
                }
            }
            return rects;
            
        }
    }
}
