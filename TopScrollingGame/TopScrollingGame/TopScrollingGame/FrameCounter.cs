using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TopScrollingGame
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch spriteBatch;
        public static string fps;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;

        public FrameRateCounter(Main game)
            : base(game)
        {
            content = new ContentManager(game.Services);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            fps = string.Format("FPS: {0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(Main.Font, fps, new Vector2(32, 32), Color.Black);
            spriteBatch.DrawString(Main.Font, "Particles : " + Main.ParticleEngine.ParticleCount, new Vector2(32, 64), Color.Black);

            spriteBatch.End();
        }
    }
}