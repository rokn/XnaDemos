using Artemis.Interface;

namespace EntityPong.Components
{
    public class VelocityComponent : IComponent
    {
        public float Direction;
        public float Speed;

        public VelocityComponent(float direction)
            :this(direction, 0){}

        public VelocityComponent(float direction, int speed)
        {
            Direction = direction;
            Speed = speed;
        }
    }
}
