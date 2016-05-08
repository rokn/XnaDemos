using Artemis.Interface;

namespace EntityPong.Components
{
    public class DimensionComponent : IComponent
    {
        public int Width;
        public int Height;

        public DimensionComponent(int w, int h)
        {
            Width = w;
            Height = h;
        }
    }
}
