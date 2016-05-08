using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RPG
{
    public static class Layers
    {
        public const float TILES = 0.2f;
        public const float ENTITY = 0.5f;
        public const float PROJECTILES = 0.6f;
        public const float MELEEATACKS = 0.61f;
        public const float TOPTILES = 0.8f;
        public const float BOUNDINGBOXES = 0.9f;        
    }

    public static class Resources
    {
        public static List<SpriteFont> fonts;

        public static Texture2D WhitePixel;        

        public static void LoadFonts()
        {
            fonts = new List<SpriteFont>();
            SpriteFont font = Main.GetContentManager().Load<SpriteFont>(@"Fonts\Font");
            fonts.Add(font);
        }

        public static void LoadMisc()
        {
            WhitePixel = Scripts.GenerateWhitePixelTexture();
        }
    }
}
