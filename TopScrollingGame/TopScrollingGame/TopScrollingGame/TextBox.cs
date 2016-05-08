using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TopScrollingGame
{
    public class TextBox
    {
        public string text;
        private short maxChars;
        public bool isActive = true;
        public Rectangle rect;
        public Vector2 position;
        protected Texture2D texture;
        private bool delete = false;
        private string pointer;
        private int deleteCount, pointerCount;
        public bool Loaded;

        public TextBox(Vector2 Position, short MaxChars, string defaultText = "")
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

        public void Load()
        {
            if (!Loaded)
            {
                texture = Scripts.LoadTexture("TextBox");
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

                if (Main.keyboard.IsReleased(Keys.Back))
                {
                    delete = false;
                    deleteCount = 0;
                }
            }

            if (Main.mouse.LeftClick())
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
                spriteBatch.DrawString(Main.Font, text + pointer, new Vector2(position.X + 3, position.Y + 2), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.882f);
            }
        }

        #region Text Entry

        private void TextEntry()
        {
            if (Main.keyboard.JustPressed(Keys.Enter))
            {
                isActive = false;
            }
            else if (Main.keyboard.JustPressed(Keys.Back))
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            else if (Main.keyboard.IsHeld(Keys.Back) && deleteCount == 25)
            {
                if (text.Length > 0)
                {
                    text = text.Remove(text.Length - 1);
                }
                delete = true;
            }
            if (text.Length < maxChars)
            {
                if (Main.keyboard.JustPressed(Keys.Space))
                {
                    text += " ";
                }
                else if (Main.keyboard.IsHeld(Keys.LeftShift) || Main.keyboard.IsHeld(Keys.RightShift))
                {
                    #region Upper case Main.keyboard

                    if (Main.keyboard.JustPressed(Keys.A))
                    {
                        text += "A";
                    }
                    if (Main.keyboard.JustPressed(Keys.B))
                    {
                        text += "B";
                    }
                    if (Main.keyboard.JustPressed(Keys.C))
                    {
                        text += "C";
                    }
                    if (Main.keyboard.JustPressed(Keys.D))
                    {
                        text += "D";
                    }
                    if (Main.keyboard.JustPressed(Keys.E))
                    {
                        text += "E";
                    }
                    if (Main.keyboard.JustPressed(Keys.F))
                    {
                        text += "F";
                    }
                    if (Main.keyboard.JustPressed(Keys.G))
                    {
                        text += "G";
                    }
                    if (Main.keyboard.JustPressed(Keys.H))
                    {
                        text += "H";
                    }
                    if (Main.keyboard.JustPressed(Keys.I))
                    {
                        text += "I";
                    }
                    if (Main.keyboard.JustPressed(Keys.J))
                    {
                        text += "J";
                    }
                    if (Main.keyboard.JustPressed(Keys.K))
                    {
                        text += "K";
                    }
                    if (Main.keyboard.JustPressed(Keys.L))
                    {
                        text += "L";
                    }
                    if (Main.keyboard.JustPressed(Keys.M))
                    {
                        text += "M";
                    }
                    if (Main.keyboard.JustPressed(Keys.N))
                    {
                        text += "N";
                    }
                    if (Main.keyboard.JustPressed(Keys.O))
                    {
                        text += "O";
                    }
                    if (Main.keyboard.JustPressed(Keys.P))
                    {
                        text += "P";
                    }
                    if (Main.keyboard.JustPressed(Keys.Q))
                    {
                        text += "Q";
                    }
                    if (Main.keyboard.JustPressed(Keys.R))
                    {
                        text += "R";
                    }
                    if (Main.keyboard.JustPressed(Keys.S))
                    {
                        text += "S";
                    }
                    if (Main.keyboard.JustPressed(Keys.T))
                    {
                        text += "T";
                    }
                    if (Main.keyboard.JustPressed(Keys.U))
                    {
                        text += "U";
                    }
                    if (Main.keyboard.JustPressed(Keys.V))
                    {
                        text += "V";
                    }
                    if (Main.keyboard.JustPressed(Keys.W))
                    {
                        text += "W";
                    }
                    if (Main.keyboard.JustPressed(Keys.X))
                    {
                        text += "X";
                    }
                    if (Main.keyboard.JustPressed(Keys.Y))
                    {
                        text += "Y";
                    }
                    if (Main.keyboard.JustPressed(Keys.Z))
                    {
                        text += "Z";
                    }

                    #endregion Upper case Main.keyboard
                }
                else
                {
                    #region Lower case Main.keyboard

                    if (Main.keyboard.JustPressed(Keys.D0))
                    {
                        text += "0";
                    }
                    if (Main.keyboard.JustPressed(Keys.D1))
                    {
                        text += "1";
                    }
                    if (Main.keyboard.JustPressed(Keys.D2))
                    {
                        text += "2";
                    }
                    if (Main.keyboard.JustPressed(Keys.D3))
                    {
                        text += "3";
                    }
                    if (Main.keyboard.JustPressed(Keys.D4))
                    {
                        text += "4";
                    }
                    if (Main.keyboard.JustPressed(Keys.D5))
                    {
                        text += "5";
                    }
                    if (Main.keyboard.JustPressed(Keys.D6))
                    {
                        text += "6";
                    }
                    if (Main.keyboard.JustPressed(Keys.D7))
                    {
                        text += "7";
                    }
                    if (Main.keyboard.JustPressed(Keys.D8))
                    {
                        text += "8";
                    }
                    if (Main.keyboard.JustPressed(Keys.D9))
                    {
                        text += "9";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad0))
                    {
                        text += "0";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad1))
                    {
                        text += "1";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad2))
                    {
                        text += "2";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad3))
                    {
                        text += "3";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad4))
                    {
                        text += "4";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad5))
                    {
                        text += "5";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad6))
                    {
                        text += "6";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad7))
                    {
                        text += "7";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad8))
                    {
                        text += "8";
                    }
                    if (Main.keyboard.JustPressed(Keys.NumPad9))
                    {
                        text += "9";
                    }
                    if (Main.keyboard.JustPressed(Keys.A))
                    {
                        text += "a";
                    }
                    if (Main.keyboard.JustPressed(Keys.B))
                    {
                        text += "b";
                    }
                    if (Main.keyboard.JustPressed(Keys.C))
                    {
                        text += "c";
                    }
                    if (Main.keyboard.JustPressed(Keys.D))
                    {
                        text += "d";
                    }
                    if (Main.keyboard.JustPressed(Keys.E))
                    {
                        text += "e";
                    }
                    if (Main.keyboard.JustPressed(Keys.F))
                    {
                        text += "f";
                    }
                    if (Main.keyboard.JustPressed(Keys.G))
                    {
                        text += "g";
                    }
                    if (Main.keyboard.JustPressed(Keys.H))
                    {
                        text += "h";
                    }
                    if (Main.keyboard.JustPressed(Keys.I))
                    {
                        text += "i";
                    }
                    if (Main.keyboard.JustPressed(Keys.J))
                    {
                        text += "j";
                    }
                    if (Main.keyboard.JustPressed(Keys.K))
                    {
                        text += "k";
                    }
                    if (Main.keyboard.JustPressed(Keys.L))
                    {
                        text += "l";
                    }
                    if (Main.keyboard.JustPressed(Keys.M))
                    {
                        text += "m";
                    }
                    if (Main.keyboard.JustPressed(Keys.N))
                    {
                        text += "n";
                    }
                    if (Main.keyboard.JustPressed(Keys.O))
                    {
                        text += "o";
                    }
                    if (Main.keyboard.JustPressed(Keys.P))
                    {
                        text += "p";
                    }
                    if (Main.keyboard.JustPressed(Keys.Q))
                    {
                        text += "q";
                    }
                    if (Main.keyboard.JustPressed(Keys.R))
                    {
                        text += "r";
                    }
                    if (Main.keyboard.JustPressed(Keys.S))
                    {
                        text += "s";
                    }
                    if (Main.keyboard.JustPressed(Keys.T))
                    {
                        text += "t";
                    }
                    if (Main.keyboard.JustPressed(Keys.U))
                    {
                        text += "u";
                    }
                    if (Main.keyboard.JustPressed(Keys.V))
                    {
                        text += "v";
                    }
                    if (Main.keyboard.JustPressed(Keys.W))
                    {
                        text += "w";
                    }
                    if (Main.keyboard.JustPressed(Keys.X))
                    {
                        text += "x";
                    }
                    if (Main.keyboard.JustPressed(Keys.Y))
                    {
                        text += "y";
                    }
                    if (Main.keyboard.JustPressed(Keys.Z))
                    {
                        text += "z";
                    }
                }

                    #endregion Lower case Main.keyboard
            }
        }

        #endregion Text Entry
    }
}