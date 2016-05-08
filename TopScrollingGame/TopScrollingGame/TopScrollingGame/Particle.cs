using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopScrollingGame
{
    public class Particle
    {
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color fromColor, Color toColor, float startingSize, float endSize, int ttl, float depth)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Angle = angle;
            AngularVelocity = angularVelocity;
            FromColor = fromColor;
            RealColor = fromColor;
            ToColor = toColor;
            TTL = ttl;
            TotalTTL = ttl;
            StartSize = startingSize;
            Size = startingSize;
            EndSize = endSize;
            Depth = depth;
        }

        private float StartSize { get; set; }

        private float EndSize { get; set; }

        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the particle

        public Vector2 Position { get; set; }        // The current position of the particle

        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance

        public Vector2 Origin { get; set; }

        public float Angle { get; set; }            // The current angle of rotation of the particle

        public float AngularVelocity { get; set; }    // The speed that the angle is changing

        public Color FromColor { get; set; }            // The color of the particle

        public Color ToColor { get; set; }            // The color of the particle

        public Color RealColor { get; set; }            // The color of the particle

        public float Size { get; set; }                // The size of the particle

        public int TTL { get; set; }                // The 'time to live' of the particle

        public int TotalTTL { get; set; }                // The 'time to live' of the particle

        public float Depth { get; set; }                // Depth of the particle

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
            RealColor = Color.Lerp(ToColor, FromColor, (float)((float)TTL / (float)TotalTTL));
            Size = MathHelper.Lerp(EndSize, StartSize, (float)((float)TTL / (float)TotalTTL));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, RealColor, Angle, Origin, Size, SpriteEffects.None, Depth);
        }
    }
}