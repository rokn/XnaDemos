using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rpg
{
    public class TextBox
    {
        public string text;
        short maxChars;
        public bool isActive = true;
        public Rectangle rect;
        public Vector2 position;
        protected Texture2D texture;
        bool delete = false;
        string pointer;
        int deleteCount,pointerCount;
        public TextBox(Vector2 Position, Texture2D Text, short MaxChars)
        {
            text = "";
            maxChars = MaxChars;
            isActive = false;
            position = Position;
            texture = Text;
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            pointer = "";
            deleteCount = 0;
        }
        public void Update()
        {
            if(delete)
            {
                if(deleteCount<25)
                {
                    deleteCount++;
                }
                if(Rpg.keyboard.IsReleased(Keys.Back))
                {
                    delete = false;
                    deleteCount = 0;
                }
            }
            if (Rpg.mouse.LeftClicked())
            {
                if (rect.Contains(Rpg.mouse.clickRectangle))
                {
                    isActive = true;
                }

            }
            if (isActive)
            {
                TextEntry();
                if (pointerCount < 25)
                {
                    pointerCount++;
                }
                else
                {
                    pointerCount = 0;
                    if (pointer == "|")
                    {
                        pointer = "";
                    }
                    else
                    {
                        pointer = "|";
                    }
                }
            }
            else
            {
                pointer = "";
            }
            Console.WriteLine(isActive);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.881f);
            spriteBatch.DrawString(Rpg.Font, text+pointer, new Vector2(position.X +3, position.Y+2), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
        }
        #region Text Entry
        void TextEntry()
        {
            if (Rpg.keyboard.JustPressed(Keys.Enter))
            {
                isActive = false;
            }
            else if (Rpg.keyboard.JustPressed(Keys.Back))
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            else if (Rpg.keyboard.IsHeld(Keys.Back) && deleteCount == 25)
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            if (text.Length < maxChars)
            {
                if (Rpg.keyboard.JustPressed(Keys.Space))
                {
                    text += " ";
                }
                else if (Rpg.keyboard.IsHeld(Keys.LeftShift) || Rpg.keyboard.IsHeld(Keys.RightShift))
                {
                    #region Upper case Rpg.keyboard
                    if (Rpg.keyboard.JustPressed(Keys.A))
                    {
                        text += "A";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.B))
                    {
                        text += "B";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.C))
                    {
                        text += "C";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D))
                    {
                        text += "D";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.E))
                    {
                        text += "E";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.F))
                    {
                        text += "F";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.G))
                    {
                        text += "G";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.H))
                    {
                        text += "H";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.I))
                    {
                        text += "I";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.J))
                    {
                        text += "J";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.K))
                    {
                        text += "K";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.L))
                    {
                        text += "L";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.M))
                    {
                        text += "M";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.N))
                    {
                        text += "N";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.O))
                    {
                        text += "O";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.P))
                    {
                        text += "P";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Q))
                    {
                        text += "Q";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.R))
                    {
                        text += "R";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.S))
                    {
                        text += "S";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.T))
                    {
                        text += "T";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.U))
                    {
                        text += "U";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.V))
                    {
                        text += "V";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.W))
                    {
                        text += "W";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.X))
                    {
                        text += "X";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Y))
                    {
                        text += "Y";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Z))
                    {
                        text += "Z";
                    }
                    #endregion
                }
                else
                {
                    #region Lower case Rpg.keyboard
                    if (Rpg.keyboard.JustPressed(Keys.D0))
                    {
                        text += "0";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D1))
                    {
                        text += "1";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D2))
                    {
                        text += "2";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D3))
                    {
                        text += "3";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D4))
                    {
                        text += "4";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D5))
                    {
                        text += "5";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D6))
                    {
                        text += "6";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D7))
                    {
                        text += "7";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D8))
                    {
                        text += "8";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D9))
                    {
                        text += "9";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.A))
                    {
                        text += "a";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.B))
                    {
                        text += "b";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.C))
                    {
                        text += "c";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.D))
                    {
                        text += "d";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.E))
                    {
                        text += "e";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.F))
                    {
                        text += "f";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.G))
                    {
                        text += "g";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.H))
                    {
                        text += "h";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.I))
                    {
                        text += "i";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.J))
                    {
                        text += "j";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.K))
                    {
                        text += "k";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.L))
                    {
                        text += "l";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.M))
                    {
                        text += "m";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.N))
                    {
                        text += "n";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.O))
                    {
                        text += "o";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.P))
                    {
                        text += "p";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Q))
                    {
                        text += "q";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.R))
                    {
                        text += "r";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.S))
                    {
                        text += "s";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.T))
                    {
                        text += "t";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.U))
                    {
                        text += "u";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.V))
                    {
                        text += "v";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.W))
                    {
                        text += "w";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.X))
                    {
                        text += "x";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Y))
                    {
                        text += "y";
                    }
                    if (Rpg.keyboard.JustPressed(Keys.Z))
                    {
                        text += "z";
                    }
                }
                    #endregion
            }
        }
        #endregion
    }
}
