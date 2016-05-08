using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;

namespace TheTimeDungeon
{
    class Scripts
    {
        public static Texture2D LoadTexture(string asset, ContentManager Content)
        {
            try
            {
                return Content.Load<Texture2D>(asset) as Texture2D;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Texture not found: " + asset);
                return null;
            }
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if (rect.Contains(Game.mouse.clickRectangle))
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
            return Game.keyboard.JustPressed(key) || Game.keyboard.IsHeld(key);
        }
        public static bool KeyIsReleased(Keys key)
        {
            return Game.keyboard.JustPressed(key)||(!Game.keyboard.JustPressed(key) && !Game.keyboard.IsHeld(key));
        }
    }
}
