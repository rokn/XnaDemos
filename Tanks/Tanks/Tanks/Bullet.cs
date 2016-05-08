using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tanks
{
    public class Bullet
    {
        public static Texture2D Texture;
        public static Texture2D Explosion;
        public static Rectangle explosionRectangle;
        public static float gravityForce;

        private Vector2 position;
        private Vector2 speed;
        private Vector2 origin;
        private bool dead;
        private int deadTimer;
        private Rectangle ExplosionRectangle;

        public Bullet(Vector2 startPos,Vector2 startSpeed)
        {
            position = startPos;
            speed = startSpeed;
            origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            dead = false;
            deadTimer = 0;
        }

        public void Update()
        {
            if (!dead)
            {
                position += speed;
                speed.Y += gravityForce;
                if(position.X<0||position.X>Terrain.Width-1)
                {
                    dead = true;
                    return;
                }
                if (position.Y > Terrain.heightMap[(int)position.X])
                {
                    dead = true;
                    deadTimer = 15;
                    ExplosionRectangle = explosionRectangle;
                    ExplosionRectangle.X = (int)position.X - ExplosionRectangle.Width / 2;
                    ExplosionRectangle.Y = (int)position.Y - ExplosionRectangle.Height / 2;
                }
            }
            else
            {
                deadTimer--;
                if(deadTimer<=0)
                {
                    Terrain.DestroyTerrain(Explosion, ExplosionRectangle);
                    Game.RemoveBullet(this);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!dead)
            {
                spriteBatch.Draw(Texture, position, null, Color.White, 0, origin, 1f, SpriteEffects.None, 0.3f);
            }
            else
            {
                spriteBatch.Draw(Explosion, ExplosionRectangle, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.6f);
            }
        }
    }
}
