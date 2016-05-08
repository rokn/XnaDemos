using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Cars
{
    internal class Scripts
    {
        public static Texture2D LoadTexture(string asset, ContentManager Content)
        {
            try
            {
                return Content.Load<Texture2D>(asset) as Texture2D;
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Texture not found: " + asset);
                return null;
            }
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if (rect.Contains(Main.mouse.clickRectangle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool KeyIsPressed(Keys key)
        {
            return Main.keyboard.JustPressed(key) || Main.keyboard.IsHeld(key);
        }

        public static bool KeyIsReleased(Keys key)
        {
            return Main.keyboard.JustPressed(key) || (!Main.keyboard.JustPressed(key) && !Main.keyboard.IsHeld(key));
        }
    }
}