using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace GalacticCommander
{
    public static class Resources
    {
        public const int SpaceObjectsCount = 10;
        public const int PlanetsCount = 9;

        public static List<Texture2D> SpaceObjects;

        public static void Load()
        {
            SpaceObjects = new List<Texture2D>();
            for (int i = 0; i < SpaceObjectsCount; i++)
            {
                SpaceObjects.Add(Scripts.LoadTexture(@"SpaceObjects\Object_" + i.ToString()));
            }
        }
    }
}