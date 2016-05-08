using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityPong.Components
{
    public class SpriteComponent : IComponent
    {
        public Texture2D texture;
        public float Depth;
        public Vector2 Origin;
        public bool Visible;
        public Color color;
        public SpriteEffects Effects;

        public SpriteComponent()
            : this("", 0, new Vector2()) { }

        public SpriteComponent(string asset)
            : this(asset, 0, new Vector2()) { }

        public SpriteComponent(string asset, float depth)
            : this(asset, depth, new Vector2()) { }

        public SpriteComponent(string asset, float depth, Vector2 origin)
        {
            texture = Scripts.LoadTexture(asset);
            Depth = depth;
            Origin = origin;
            Visible = true;
            color = Color.White;
            Effects = SpriteEffects.None;
        }
    }
}
