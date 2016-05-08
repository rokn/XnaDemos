using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Battleships
{
    class TextBox : Button
    {
        public string text,str;
        SpriteFont font;
        char pointer;
        int counter;
        short maxChars;
        public bool isActive,alreadyTyped;
        KeysInput input = new KeysInput();
        Texture2D Back;
        Vector2 pos;
        public TextBox(Vector2 Position, Texture2D Text, SpriteFont Font, short MaxChars,Texture2D Background,string Str,string Default)
            : base(new Vector2(Position.X+10,Position.Y+Background.Height-30), Text)
        {
            pos = Position;
            text = Default;
            font = Font;
            maxChars = MaxChars;
            isActive = true;
            Back = Background;
            str = Str;
            alreadyTyped = false;
            counter = 0;
            pointer = 'I';
        }
        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                TextEntry();
                input.Update(gameTime);
                if (counter < 50)
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                    if (pointer == 'I')
                    {
                        pointer = ' ';
                    }
                    else
                    {
                        pointer = 'I';
                    }
                }
                if (counter > 3)
                {
                    if (Game1.mouse.LeftClicked())
                    {
                        if (!Game1.rectContains(rect, Game1.mouse.Position))
                        {
                            isActive = false;
                            pointer = ' ';
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Back, pos, Color.White);
            spriteBatch.DrawString(font,str,pos+new Vector2(20,22),Color.White);
            base.Draw(spriteBatch);
            spriteBatch.DrawString(font, text+pointer, new Vector2(position.X + 3, position.Y - 4), Color.Black);
        }
        #region Text Entry
        void TextEntry()
        {
            if (input.IsReleased(Keys.Enter))
            {
                isActive = false;
                pointer = ' ';
            }
            else if (input.IsReleased(Keys.Back))
            {
                if (alreadyTyped)
                {
                    if (text.Length > 0)
                    {
                        text = text.Remove(text.Length - 1);
                    }
                }
                else
                {
                    text = "";
                    alreadyTyped = true;
                }
            }
            if (text.Length < maxChars)
            {
                if (input.IsReleased(Keys.Space))
                {
                    text += " ";
                }
                else if (input.IsHeld(Keys.LeftShift) || input.IsHeld(Keys.RightShift))
                {
                    #region Upper case input
                    if (input.IsReleased(Keys.A))
                    {
                        text += "A";
                    }
                    if (input.IsReleased(Keys.B))
                    {
                        text += "B";
                    }
                    if (input.IsReleased(Keys.C))
                    {
                        text += "C";
                    }
                    if (input.IsReleased(Keys.D))
                    {
                        text += "D";
                    }
                    if (input.IsReleased(Keys.E))
                    {
                        text += "E";
                    }
                    if (input.IsReleased(Keys.F))
                    {
                        text += "F";
                    }
                    if (input.IsReleased(Keys.G))
                    {
                        text += "G";
                    }
                    if (input.IsReleased(Keys.H))
                    {
                        text += "H";
                    }
                    if (input.IsReleased(Keys.I))
                    {
                        text += "I";
                    }
                    if (input.IsReleased(Keys.J))
                    {
                        text += "J";
                    }
                    if (input.IsReleased(Keys.K))
                    {
                        text += "K";
                    }
                    if (input.IsReleased(Keys.L))
                    {
                        text += "L";
                    }
                    if (input.IsReleased(Keys.M))
                    {
                        text += "M";
                    }
                    if (input.IsReleased(Keys.N))
                    {
                        text += "N";
                    }
                    if (input.IsReleased(Keys.O))
                    {
                        text += "O";
                    }
                    if (input.IsReleased(Keys.P))
                    {
                        text += "P";
                    }
                    if (input.IsReleased(Keys.Q))
                    {
                        text += "Q";
                    }
                    if (input.IsReleased(Keys.R))
                    {
                        text += "R";
                    }
                    if (input.IsReleased(Keys.S))
                    {
                        text += "S";
                    }
                    if (input.IsReleased(Keys.T))
                    {
                        text += "T";
                    }
                    if (input.IsReleased(Keys.U))
                    {
                        text += "U";
                    }
                    if (input.IsReleased(Keys.V))
                    {
                        text += "V";
                    }
                    if (input.IsReleased(Keys.W))
                    {
                        text += "W";
                    }
                    if (input.IsReleased(Keys.X))
                    {
                        text += "X";
                    }
                    if (input.IsReleased(Keys.Y))
                    {
                        text += "Y";
                    }
                    if (input.IsReleased(Keys.Z))
                    {
                        text += "Z";
                    }
                    #endregion
                }
                else
                {
                    #region Lower case input
                    if (input.IsReleased(Keys.NumPad0))
                    {
                        text += "0";
                    }
                    if (input.IsReleased(Keys.NumPad1))
                    {
                        text += "1";
                    }
                    if (input.IsReleased(Keys.NumPad2))
                    {
                        text += "2";
                    }
                    if (input.IsReleased(Keys.NumPad3))
                    {
                        text += "3";
                    }
                    if (input.IsReleased(Keys.NumPad4))
                    {
                        text += "4";
                    }
                    if (input.IsReleased(Keys.NumPad5))
                    {
                        text += "5";
                    }
                    if (input.IsReleased(Keys.NumPad6))
                    {
                        text += "6";
                    }
                    if (input.IsReleased(Keys.NumPad7))
                    {
                        text += "7";
                    }
                    if (input.IsReleased(Keys.NumPad8))
                    {
                        text += "8";
                    }
                    if (input.IsReleased(Keys.NumPad9))
                    {
                        text += "9";
                    }
                    if (input.IsReleased(Keys.OemPeriod))
                    {
                        text += ".";
                    }
                    if (input.IsReleased(Keys.Decimal))
                    {
                        text += ".";
                    }
                    if (input.IsReleased(Keys.D0))
                    {
                        text += "0";
                    }
                    if (input.IsReleased(Keys.D1))
                    {
                        text += "1";
                    }
                    if (input.IsReleased(Keys.D2))
                    {
                        text += "2";
                    }
                    if (input.IsReleased(Keys.D3))
                    {
                        text += "3";
                    }
                    if (input.IsReleased(Keys.D4))
                    {
                        text += "4";
                    }
                    if (input.IsReleased(Keys.D5))
                    {
                        text += "5";
                    }
                    if (input.IsReleased(Keys.D6))
                    {
                        text += "6";
                    }
                    if (input.IsReleased(Keys.D7))
                    {
                        text += "7";
                    }
                    if (input.IsReleased(Keys.D8))
                    {
                        text += "8";
                    }
                    if (input.IsReleased(Keys.D9))
                    {
                        text += "9";
                    }
                    if (input.IsReleased(Keys.A))
                    {
                        text += "a";
                    }
                    if (input.IsReleased(Keys.B))
                    {
                        text += "b";
                    }
                    if (input.IsReleased(Keys.C))
                    {
                        text += "c";
                    }
                    if (input.IsReleased(Keys.D))
                    {
                        text += "d";
                    }
                    if (input.IsReleased(Keys.E))
                    {
                        text += "e";
                    }
                    if (input.IsReleased(Keys.F))
                    {
                        text += "f";
                    }
                    if (input.IsReleased(Keys.G))
                    {
                        text += "g";
                    }
                    if (input.IsReleased(Keys.H))
                    {
                        text += "h";
                    }
                    if (input.IsReleased(Keys.I))
                    {
                        text += "i";
                    }
                    if (input.IsReleased(Keys.J))
                    {
                        text += "j";
                    }
                    if (input.IsReleased(Keys.K))
                    {
                        text += "k";
                    }
                    if (input.IsReleased(Keys.L))
                    {
                        text += "l";
                    }
                    if (input.IsReleased(Keys.M))
                    {
                        text += "m";
                    }
                    if (input.IsReleased(Keys.N))
                    {
                        text += "n";
                    }
                    if (input.IsReleased(Keys.O))
                    {
                        text += "o";
                    }
                    if (input.IsReleased(Keys.P))
                    {
                        text += "p";
                    }
                    if (input.IsReleased(Keys.Q))
                    {
                        text += "q";
                    }
                    if (input.IsReleased(Keys.R))
                    {
                        text += "r";
                    }
                    if (input.IsReleased(Keys.S))
                    {
                        text += "s";
                    }
                    if (input.IsReleased(Keys.T))
                    {
                        text += "t";
                    }
                    if (input.IsReleased(Keys.U))
                    {
                        text += "u";
                    }
                    if (input.IsReleased(Keys.V))
                    {
                        text += "v";
                    }
                    if (input.IsReleased(Keys.W))
                    {
                        text += "w";
                    }
                    if (input.IsReleased(Keys.X))
                    {
                        text += "x";
                    }
                    if (input.IsReleased(Keys.Y))
                    {
                        text += "y";
                    }
                    if (input.IsReleased(Keys.Z))
                    {
                        text += "z";
                    }
                }
                    #endregion
            }
        }
        #endregion
        public string Destroy()
        {
            return text;
        }
    }
}
