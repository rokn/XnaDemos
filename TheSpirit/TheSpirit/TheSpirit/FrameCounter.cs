using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheSpirit
{
    public class FrameRateCounter : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        public static string fps;
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private SpriteFont Font;

        public FrameRateCounter(Main game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font = Main.content.Load<SpriteFont>(@"Fonts\Font");
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
            fps = string.Format("Fps: {0}", frameRate);

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(Font, fps, new Vector2(32, 32), Color.White);

            spriteBatch.End();
        }
    }
}