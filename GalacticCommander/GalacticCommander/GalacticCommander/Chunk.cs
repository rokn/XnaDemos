using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticCommander
{
    public class Chunk
    {
        private List<SpaceObject> spaceObjects;
        public Vector2 Position;

        public Chunk(Vector2 position)
        {
            Position = position;
            spaceObjects = new List<SpaceObject>();
            SpawnSpaceObject();
        }

        private void SpawnSpaceObject()
        {
            for (int i = 0; i < Main.rand.Next(5500, 6500); i++)
            {
                spaceObjects.Add(new SpaceObject(SpaceObjectType.Star, this));
            }

            for (int i = 0; i < Main.rand.Next(2, 10); i++)
            {
                spaceObjects.Add(new SpaceObject(SpaceObjectType.Planet, this));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var obj in spaceObjects)
            {
                obj.Draw(spriteBatch);
            }
        }
    }
}