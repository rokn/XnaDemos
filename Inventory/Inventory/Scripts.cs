using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public static class Scripts
    {
        /// <summary>
        /// A nice method for making rounded rectangles
        /// </summary>
        /// <returns>Returns a texture that needs to be drawn later on</returns>
        public static Texture2D RoundRectangle(int width,int height,int borderThicknes,Color fillColor,Color borderColor)
        {
            Texture2D roundRect = new Texture2D(Rpg.graphics.GraphicsDevice,width,height);
            Color[] color = new Color[width*height];
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
        public static Texture2D MergeTextures(Texture2D bottom, Texture2D top)
        {
            Texture2D result = new Texture2D(bottom.GraphicsDevice, bottom.Width, bottom.Height);
            Color[] bottomData = new Color[bottom.Width * bottom.Height];
            Color[] topData = new Color[top.Width * top.Height];
            bottom.GetData(bottomData);
            top.GetData(topData);
            for (int y = 0; y < top.Height * top.Height; y++)
            {
                if (topData[y].A == 255)
                {
                    bottomData[y] = topData[y];
                }
                else
                {
                    Vector4 bV = bottomData[y].ToVector4();
                    Vector4 tV = topData[y].ToVector4();
                    bottomData[y] = new Color(tV + bV);
                }
            }
            result.SetData(bottomData);
            return result;
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
        public static Texture2D GenerateTooltipTexture(List<string> Tooltip)
        {
            if (Tooltip.Count > 0)
            {
                float longestWidth = 0;
                Vector2 wh = new Vector2();
                foreach (string str in Tooltip)
                {
                    if (longestWidth < Rpg.Font.MeasureString(str).X)
                    {
                        longestWidth = Rpg.Font.MeasureString(str).X;
                        wh = Rpg.Font.MeasureString(str);
                    }
                }
                if (longestWidth > 0)
                {
                    return Scripts.RoundRectangle((int)wh.X + 15, 6 + Tooltip.Count * 18, 3, Color.DeepSkyBlue, Color.Blue);
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
        public static List<string> GenerateTooltip(Item item)
        {
            List<string> Tooltip = new List<string>();
            Tooltip.Add(item.name);
            if (item.isWeapon)
            {
                if (item.isMelee) { Tooltip.Add(item.damage.ToString() + " melee damage"); }
                if (item.isRanged) { Tooltip.Add(item.damage.ToString() + " ranged damage"); }
                if (item.isMagic) { Tooltip.Add(item.damage.ToString() + " magic damage"); }
            }
            if (item.consumable) { Tooltip.Add("Consumable"); }
            if (item.tooltip.Length > 0) { Tooltip.Add(item.tooltip); }
            return Tooltip;
        }
    }
}
