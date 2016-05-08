using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Rpg
{
    class ParticleSystem
    {
        List<Particle> particles;
        bool emiting;
        public ParticleSystem()
        {
            particles = new List<Particle>();
            emiting = false;
        }
        public void Update(GameTime gameTime)
        {
            if(emiting)
            {

            }
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                if(particles[i].lifeSpan.Seconds<=0&&particles[i].lifeSpan.Milliseconds<=0)
                {
                    particles.Remove(particles[i]);
                    i++;
                }
            }
            /*if(Rpg.mouse.LeftClicked())
            {
                emiting = true;
            }
            if(Rpg.mouse.LeftReleased())
            {
                emiting = false;
            }
            if(emiting)
            {
                EmittRedStars();
            }*/
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Particle part in particles)
            {
                part.Draw(spriteBatch);
            }
            spriteBatch.DrawString(Rpg.Font, particles.Count.ToString(), new Vector2(9, 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f);
        }
        public void EmittRedStars()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Particle part = new Particle(Rpg.mouse.Position, new Vector2((float)rand.NextDouble() * rand.Next(-2, 2), (float)rand.NextDouble() * rand.Next(-4, -1)), Rpg.partRedStar, Color.White, new TimeSpan(0, 0, rand.Next(1, 2)), 0.6f, 1);
                particles.Add(part);
            }
        }
    }
}
