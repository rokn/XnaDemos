using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class ParticleEngine
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

        private Random random;
        private List<Particle> particles;
        private List<Texture2D> textures;

        public int ParticleCount
        {
            get
            {
                return particles.Count;
            }
            private set
            {
            }
        }

        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        public Vector2 EmitterLocation { get; set; }

        public void GenerateFireParticles(int count, Vector2 Direction, float speed)
        {
            for (int i = 0; i < count; i++)
            {
                Texture2D texture = textures[random.Next(textures.Count)];
                Vector2 position = EmitterLocation;
                float vAngle = MathHelper.ToDegrees((float)Math.Atan2(Direction.Y, Direction.X));
                vAngle += random.Next(-40, 40);
                Vector2 velocity = (Direction + MathAid.AngleToVector(MathHelper.ToRadians(vAngle))) * speed;
                float angle = 0;
                float angularVelocity = 0;
                Color fromColor = Color.Orange;
                Color toColor = Color.OrangeRed;
                float size = (float)random.NextDouble();
                int ttl = 5 + random.Next(15);

                particles.Add(new Particle(texture, position, velocity, angle, angularVelocity, fromColor, toColor, size, size, ttl, 0.5f));
            }
        }

        public void GenerateDeathEffect(Vector2 position, Texture2D texture)
        {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            Color[] particleData = new Color[1];
            particleData[0] = Color.White;

            Texture2D particleTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            particleTexture.SetData(particleData);

            for (int x = 0; x < texture.Width; x += 3)
            {
                for (int y = 0; y < texture.Height; y += 3)
                {
                    Color c = colorData[x + y * texture.Width];
                    Vector2 velocity = new Vector2(
                        2f * ((float)random.NextDouble() * 2f - 1f),
                        2f * ((float)random.NextDouble() * 2f - 1f));
                    Vector2 pos = position + new Vector2(x, y);
                    particles.Add(new Particle(particleTexture, pos, velocity, 0, 0, c, c, 3f, 1f, 30, 0.51f));
                }
            }
        }

        public void GenerateTrailEffect(Vector2 position, Texture2D texture, float rotation, int ticksToStay, Color color)
        {
            particles.Add(new Particle(texture, position, new Vector2(), rotation, 0f, color, color, 1f, 0.2f, ticksToStay, 0.4f));
        }

        //public void GenerateShinyTrailEffect(Texture2D texture, Vector2 position)
        //{
        //    Color[] colorData = new Color[texture.Width * texture.Height];
        //    texture.GetData(colorData);

        //    Color[] particleData = new Color[1];
        //    particleData[0] = Color.White;

        //    Texture2D particleTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
        //    particleTexture.SetData(particleData);

        //    for (int x = 0; x < texture.Width; x += 3)
        //    {
        //        for (int y = 0; y < texture.Height; y += 3)
        //        {
        //            Color c = new Color(random.Next(255), random.Next(255), random.Next(255));
        //            Vector2 velocity = new Vector2(
        //                1f * ((float)random.NextDouble() * 2f - 1f),
        //                1f * ((float)random.NextDouble() * 2f - 1f));
        //            Vector2 pos = new Vector2(x, y);
        //            particles.Add(new Particle(particleTexture, position + pos, velocity, 0, 0, c, c, 3f, 1f, 5));
        //        }
        //    }
        //}

        public void GenerateBloodEffect(Vector2 position, int count)
        {
            Color color = Color.Red;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = new Vector2(
                    3f * ((float)random.NextDouble() * 2f - 1f),
                    3f * ((float)random.NextDouble() * 2f - 1f));
                float size = (float)random.NextDouble();
                particles.Add(new Particle(textures[0], position, velocity, 0f, 0f, color, color, size, size, 10, 0.6f));
            }
        }

        public void GenerateExplosionEffect(Vector2 position, int explosionSize, int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                Color color = new Color(255, random.Next(150), 0);
                float speed = 30;
                float rotation = MathHelper.ToRadians(random.Next(360));
                Vector2 velocity = MathAid.AngleToVector(rotation) * (speed - random.Next((int)speed));
                float size = (float)random.NextDouble() * 4;
                int TTL = explosionSize / (int)speed;
                particles.Add(new Particle(textures[1], position, velocity, 0f, 0f, color, color, size, size, TTL, 0.6f));
            }
        }

        public void GenerateSmokeEffect(Vector2 position, int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                int colorValue = 100;
                Color color = new Color(colorValue, colorValue, colorValue);
                float speed = 6;
                float rotation = MathHelper.ToRadians(random.Next(-180, 0));
                Vector2 velocity = MathAid.AngleToVector(rotation) * (speed - random.Next((int)speed));
                float size = (float)random.NextDouble();
                int TTL = 60;
                particles.Add(new Particle(textures[1], position, velocity, 0f, 0f, color, color, size, size * 10, TTL, 0.9f));
            }
        }

        public void GenerateEngineEffect(Vector2 position, float Rotation)
        {
            Color color = Color.White;
            float speed = 0f;
            float rotation = Rotation;
            Vector2 velocity = MathAid.AngleToVector(rotation) * speed;
            float size = 1f;
            int TTL = 10;
            float depth = 0.49f - particles.Count / 10000f;
            particles.Add(new Particle(textures[1], position, velocity, rotation, 0f, color, color, size, size / 100, TTL, depth));
        }

        public void Update()
        {
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }
}