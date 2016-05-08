using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheRicoshield
{
    class Scripts
    {
        public static Texture2D LoadTexture(string asset, ContentManager Content)
        {
            try
            {
                return Content.Load<Texture2D>(asset) as Texture2D;
            }
            catch (ContentLoadException)
            {
                throw new ContentLoadException("Texture not found: " + asset);
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

        public static bool CheckForPerfectCollision(Texture2D texture1,Texture2D texture2,Rectangle rect1,Rectangle rect2)
        {
            Texture2D shieldTexture = Game.player.ShieldTexture;
            Rectangle intersectRect = MathAid.GetIntersectingRectangle(rect1, rect2);

            Color[] textureData1 = new Color[texture1.Width * texture1.Height];
            texture1.GetData(textureData1);

            Color[] textureData2 = new Color[texture2.Width * texture2.Height];
            texture2.GetData(textureData2);

            Point startPos1 = new Point(intersectRect.X - rect1.X, intersectRect.Y - rect1.Y);
            Point startPos2 = new Point(intersectRect.X - rect2.X, intersectRect.Y - rect2.Y);

            for (int x = 0; x < intersectRect.Width; x++)
            {
                for (int y = 0; y < intersectRect.Height; y++)
                {
                    if (textureData1[(startPos1.X + x) + (startPos1.Y + y) * texture1.Width].A != 0)
                    {
                        if (textureData2[(startPos2.X + x) + (startPos2.Y + y) * texture2.Width].A != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
