using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Rpg
{
    public static class ScreenFix
    {
        public static void Fix(Rpg game)
        {
            var screen = Screen.PrimaryScreen;
            game.Window.IsBorderless = true;
            game.Window.Position = new Point(screen.Bounds.X, screen.Bounds.Y);
            Rpg.graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            Rpg.graphics.PreferredBackBufferHeight = screen.Bounds.Height;
            Rpg.width = screen.Bounds.Width;
            Rpg.height = screen.Bounds.Height;
        }
    }
}
