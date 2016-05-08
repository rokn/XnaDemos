using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rpg
{
    class Particle
    {
        Vector2 position;
        public Vector2 speed;
        Texture2D texture;
        Color color;
        public TimeSpan lifeSpan;
        float depth;
        float size;
        public double numberOfTicks;
        public Particle(Vector2 Position, Vector2 Speed, Texture2D Texture, Color Color, TimeSpan LifeSpan,float Depth,float Size)
        {
            position = Position;
            speed = Speed;
            numberOfTicks = 0;
            texture = Texture;
            color = Color;
            lifeSpan = LifeSpan;
            depth = Depth;
            size = Size;
        }
        public void Update(GameTime gameTime)
        {
            numberOfTicks++;
            position.X += speed.X;
            position.Y = 250 * (float)Math.Sin(numberOfTicks * 0.5 * Math.PI);
            lifeSpan -= gameTime.ElapsedGameTime;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color , 0, new Vector2(), 1, SpriteEffects.None, depth);
        }
    }
}
