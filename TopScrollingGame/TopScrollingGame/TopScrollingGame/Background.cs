using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopScrollingGame
{
    public class Background
    {
        private Vector2 position;
        private int repeatHorizontalTimes;
        private int repeatVerticalTimes;
        public Texture2D texture;
        public Texture2D baseTexture;

        public Background()
        {
            Load();
            this.position = new Vector2(Main.playingAreaX, -texture.Height);
            this.repeatHorizontalTimes = Main.playingAreaWidth / texture.Width;
            this.repeatVerticalTimes = (int)Main.baseScreenSize.Y / texture.Height + 2;
            //this.HorizontalSpeed = startingHorizontalSpeed;
        }

        //public int HorizontalSpeed { get; private set; }

        public void Load()
        {
            texture = Scripts.LoadTexture(@"Sprites\Background\Background");
            baseTexture = texture;
        }

        //public void Update()
        //{
        //    UpdatePosition();
        //}

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < repeatHorizontalTimes; i++)
            {
                for (int b = 0; b < repeatVerticalTimes; b++)
                {
                    Vector2 pos = position + new Vector2(i * texture.Width, b * texture.Height);
                    spriteBatch.Draw(texture, pos, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.2f);
                }
            }
        }

        //private void UpdatePosition()
        //{
        //    for (int i = 0; i < HorizontalSpeed; i++)
        //    {
        //        position.Y++;
        //        if(position.Y>=0)
        //        {
        //            GoBack();
        //        }

        //    }
        //}

        //private void GoBack()
        //{
        //    position.Y = -texture.Height;
        //}
    }
}