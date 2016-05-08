using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Wizards
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
        int deleteCount, pointerCount;
        public bool Loaded;
        public TextBox(Vector2 Position, short MaxChars, string defaultText="")
        {
            text = defaultText;
            maxChars = MaxChars;
            isActive = false;
            position = Position;
            pointer = "";
            deleteCount = 0;
            Loaded = false;
            rect = new Rectangle();
        }

        public void Load(ContentManager Content)
        {
            if (!Loaded)
            {
                texture = Scripts.LoadTexture("TextBox", Content);
                rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
                Loaded = true;
            }
        }

        public void Update()
        {
            if (delete)
            {
                if (deleteCount < 25)
                {
                    deleteCount++;
                }
                if (Game.keyboard.IsReleased(Keys.Back))
                {
                    delete = false;
                    deleteCount = 0;
                }
            }
            if (Game.mouse.LeftClick())
            {
                if (Scripts.CheckIfMouseIsOver(rect))
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
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Loaded)
            {
                spriteBatch.Draw(texture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.881f);
                spriteBatch.DrawString(Game.Font, text + pointer, new Vector2(position.X + 3, position.Y + 2), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.882f);
            }
        }
        #region Text Entry
        void TextEntry()
        {
            if (Game.keyboard.JustPressed(Keys.Enter))
            {
                isActive = false;
            }
            else if (Game.keyboard.JustPressed(Keys.Back))
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            else if (Game.keyboard.IsHeld(Keys.Back) && deleteCount == 25)
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            if (text.Length < maxChars)
            {
                if (Game.keyboard.JustPressed(Keys.Space))
                {
                    text += " ";
                }
                else if (Game.keyboard.IsHeld(Keys.LeftShift) || Game.keyboard.IsHeld(Keys.RightShift))
                {
                    #region Upper case Game.keyboard
                    if (Game.keyboard.JustPressed(Keys.A))
                    {
                        text += "A";
                    }
                    if (Game.keyboard.JustPressed(Keys.B))
                    {
                        text += "B";
                    }
                    if (Game.keyboard.JustPressed(Keys.C))
                    {
                        text += "C";
                    }
                    if (Game.keyboard.JustPressed(Keys.D))
                    {
                        text += "D";
                    }
                    if (Game.keyboard.JustPressed(Keys.E))
                    {
                        text += "E";
                    }
                    if (Game.keyboard.JustPressed(Keys.F))
                    {
                        text += "F";
                    }
                    if (Game.keyboard.JustPressed(Keys.G))
                    {
                        text += "G";
                    }
                    if (Game.keyboard.JustPressed(Keys.H))
                    {
                        text += "H";
                    }
                    if (Game.keyboard.JustPressed(Keys.I))
                    {
                        text += "I";
                    }
                    if (Game.keyboard.JustPressed(Keys.J))
                    {
                        text += "J";
                    }
                    if (Game.keyboard.JustPressed(Keys.K))
                    {
                        text += "K";
                    }
                    if (Game.keyboard.JustPressed(Keys.L))
                    {
                        text += "L";
                    }
                    if (Game.keyboard.JustPressed(Keys.M))
                    {
                        text += "M";
                    }
                    if (Game.keyboard.JustPressed(Keys.N))
                    {
                        text += "N";
                    }
                    if (Game.keyboard.JustPressed(Keys.O))
                    {
                        text += "O";
                    }
                    if (Game.keyboard.JustPressed(Keys.P))
                    {
                        text += "P";
                    }
                    if (Game.keyboard.JustPressed(Keys.Q))
                    {
                        text += "Q";
                    }
                    if (Game.keyboard.JustPressed(Keys.R))
                    {
                        text += "R";
                    }
                    if (Game.keyboard.JustPressed(Keys.S))
                    {
                        text += "S";
                    }
                    if (Game.keyboard.JustPressed(Keys.T))
                    {
                        text += "T";
                    }
                    if (Game.keyboard.JustPressed(Keys.U))
                    {
                        text += "U";
                    }
                    if (Game.keyboard.JustPressed(Keys.V))
                    {
                        text += "V";
                    }
                    if (Game.keyboard.JustPressed(Keys.W))
                    {
                        text += "W";
                    }
                    if (Game.keyboard.JustPressed(Keys.X))
                    {
                        text += "X";
                    }
                    if (Game.keyboard.JustPressed(Keys.Y))
                    {
                        text += "Y";
                    }
                    if (Game.keyboard.JustPressed(Keys.Z))
                    {
                        text += "Z";
                    }
                    #endregion
                }
                else
                {
                    #region Lower case Game.keyboard
                    if (Game.keyboard.JustPressed(Keys.Decimal))
                    {
                        text += ".";
                    }
                    if (Game.keyboard.JustPressed(Keys.OemPeriod))
                    {
                        text += ".";
                    }
                    if (Game.keyboard.JustPressed(Keys.D0))
                    {
                        text += "0";
                    }
                    if (Game.keyboard.JustPressed(Keys.D1))
                    {
                        text += "1";
                    }
                    if (Game.keyboard.JustPressed(Keys.D2))
                    {
                        text += "2";
                    }
                    if (Game.keyboard.JustPressed(Keys.D3))
                    {
                        text += "3";
                    }
                    if (Game.keyboard.JustPressed(Keys.D4))
                    {
                        text += "4";
                    }
                    if (Game.keyboard.JustPressed(Keys.D5))
                    {
                        text += "5";
                    }
                    if (Game.keyboard.JustPressed(Keys.D6))
                    {
                        text += "6";
                    }
                    if (Game.keyboard.JustPressed(Keys.D7))
                    {
                        text += "7";
                    }
                    if (Game.keyboard.JustPressed(Keys.D8))
                    {
                        text += "8";
                    }
                    if (Game.keyboard.JustPressed(Keys.D9))
                    {
                        text += "9";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad0))
                    {
                        text += "0";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad1))
                    {
                        text += "1";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad2))
                    {
                        text += "2";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad3))
                    {
                        text += "3";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad4))
                    {
                        text += "4";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad5))
                    {
                        text += "5";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad6))
                    {
                        text += "6";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad7))
                    {
                        text += "7";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad8))
                    {
                        text += "8";
                    }
                    if (Game.keyboard.JustPressed(Keys.NumPad9))
                    {
                        text += "9";
                    }
                    if (Game.keyboard.JustPressed(Keys.A))
                    {
                        text += "a";
                    }
                    if (Game.keyboard.JustPressed(Keys.B))
                    {
                        text += "b";
                    }
                    if (Game.keyboard.JustPressed(Keys.C))
                    {
                        text += "c";
                    }
                    if (Game.keyboard.JustPressed(Keys.D))
                    {
                        text += "d";
                    }
                    if (Game.keyboard.JustPressed(Keys.E))
                    {
                        text += "e";
                    }
                    if (Game.keyboard.JustPressed(Keys.F))
                    {
                        text += "f";
                    }
                    if (Game.keyboard.JustPressed(Keys.G))
                    {
                        text += "g";
                    }
                    if (Game.keyboard.JustPressed(Keys.H))
                    {
                        text += "h";
                    }
                    if (Game.keyboard.JustPressed(Keys.I))
                    {
                        text += "i";
                    }
                    if (Game.keyboard.JustPressed(Keys.J))
                    {
                        text += "j";
                    }
                    if (Game.keyboard.JustPressed(Keys.K))
                    {
                        text += "k";
                    }
                    if (Game.keyboard.JustPressed(Keys.L))
                    {
                        text += "l";
                    }
                    if (Game.keyboard.JustPressed(Keys.M))
                    {
                        text += "m";
                    }
                    if (Game.keyboard.JustPressed(Keys.N))
                    {
                        text += "n";
                    }
                    if (Game.keyboard.JustPressed(Keys.O))
                    {
                        text += "o";
                    }
                    if (Game.keyboard.JustPressed(Keys.P))
                    {
                        text += "p";
                    }
                    if (Game.keyboard.JustPressed(Keys.Q))
                    {
                        text += "q";
                    }
                    if (Game.keyboard.JustPressed(Keys.R))
                    {
                        text += "r";
                    }
                    if (Game.keyboard.JustPressed(Keys.S))
                    {
                        text += "s";
                    }
                    if (Game.keyboard.JustPressed(Keys.T))
                    {
                        text += "t";
                    }
                    if (Game.keyboard.JustPressed(Keys.U))
                    {
                        text += "u";
                    }
                    if (Game.keyboard.JustPressed(Keys.V))
                    {
                        text += "v";
                    }
                    if (Game.keyboard.JustPressed(Keys.W))
                    {
                        text += "w";
                    }
                    if (Game.keyboard.JustPressed(Keys.X))
                    {
                        text += "x";
                    }
                    if (Game.keyboard.JustPressed(Keys.Y))
                    {
                        text += "y";
                    }
                    if (Game.keyboard.JustPressed(Keys.Z))
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
