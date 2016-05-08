using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace EntityPong.Components
{
    public class TransformComponent : IComponent
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Scale;

        public TransformComponent()
            : this(new Vector2(), 0f, new Vector2(1f)) { }

        public TransformComponent(Vector2 position)
            : this(position, 0f, new Vector2(1f)) { }

        public TransformComponent(Vector2 position, float rotation)
            : this(position, rotation, new Vector2(1f)) { }

        public TransformComponent(Vector2 position, float rotation, Vector2 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
