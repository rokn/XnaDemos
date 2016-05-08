using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG
{
    [Serializable]
    public class Level
    {
        private static string SavePath = @"Levels\";

        public string ID;
        public List<Rectangle> blockRects;
        public TileMap tilemap;

        public Level(string id, int width, int height)
        {
            this.ID = id;
            tilemap = new TileMap(new Vector2(), width, height);
            blockRects = new List<Rectangle>();
        }

        public static Level LoadLevel(string filename)
        {
            string LevelName = SavePath + filename;

            Level level; 

            if (File.Exists(LevelName))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(LevelName, FileMode.Open, FileAccess.Read, FileShare.Read);

                level = (Level)formatter.Deserialize(stream);

                stream.Close();
            }
            else
            {
                level = new Level("First",100,60);
            }

            return level;
        }

        public static void SaveLevel(Level level)
        {
            string LevelName = SavePath + level.ID;

            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(LevelName, FileMode.Create, FileAccess.Write, FileShare.None);

            formatter.Serialize(stream, level);

            stream.Close();
        }

        public void DrawLevel(SpriteBatch spriteBatch)
        {
            tilemap.Draw(spriteBatch);

            if(Main.ShowBoundingBoxes)
            {
                foreach (var rect in blockRects)
                {
                    Scripts.ShowBoundingBox(spriteBatch, rect);
                }
            }
        }
    }
}
