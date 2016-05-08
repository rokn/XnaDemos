using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StealthOrNot
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

        public override void Draw(GameTime dagameTime)
        {
            frameCounter++;
            fps = string.Format("fps: {0}", frameRate);
            //string Proj = string.Format("Projectiles : {0}", Main.player.projectiles.Count);

            spriteBatch.Begin();
            spriteBatch.DrawString(Main.Font, fps, new Vector2(32, 64), Color.White);
            //spriteBatch.DrawString(Main.Font, Proj, new Vector2(32, 64), Color.White);

            spriteBatch.End();
        }
    }
}