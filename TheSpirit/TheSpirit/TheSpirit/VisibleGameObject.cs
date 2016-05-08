
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheSpirit
{
    public abstract class VisibleGameObject
    {
        private Vector2 position;
        private Vector2 origin;
        private Texture2D texture;
        private Rectangle mainRect;
        float rotation;

        public abstract void Load();
        public abstract void Draw();
    }
}
