using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG
{
    public class HealthBar
    {
        private Rectangle stretchRect;
        private int maxWidth;
        private Texture2D texture, backgroundTexture;
        private Vector2 position, origin, backOrigin;
        private Color currentColor, colorFrom, colorTo;
        bool hasBackground;

        public HealthBar(int width, int height, Vector2 Position, Color from, Color to)
        {
            position = Position;
            maxWidth = width;
            colorFrom = from;
            colorTo = to;
            currentColor = colorFrom;
            GenerateMainTexture(height);
            origin = new Vector2(maxWidth / 2, height / 2);
            stretchRect = new Rectangle(0, 0, maxWidth, height);
        }        

        public void SetBackground(string backgroundAsset)
        {           
            hasBackground = true;
            backgroundTexture = Scripts.LoadTexture(backgroundAsset);
            //backgroundPosition = position + origin;
            backOrigin = new Vector2(backgroundTexture.Width/2, backgroundTexture.Height/2);
        }

        public void UpdatePosition(Vector2 newPosition)
        {            
            this.position = newPosition;
            //backgroundPosition = position + origin;
        }

        public void UpdateHealth(float hp, float maxHp)
        {
            if (hp != maxHp)
            {
                currentColor = Color.Lerp(colorTo, colorFrom, hp / maxHp);
            }
            else
            {
                currentColor = colorFrom;
            }

            stretchRect.Width = (int)(maxWidth * (hp / maxHp));
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (hasBackground)
            {
                spriteBatch.Draw(backgroundTexture, position, null, Color.White, 0, backOrigin, 1f, SpriteEffects.None, depth);
            }

            spriteBatch.Draw(texture, position, stretchRect, currentColor, 0, origin, 1f, SpriteEffects.None, depth + 0.0001f);
        }

        private void GenerateMainTexture(int height)
        {
            texture = new Texture2D(Main.GetGraphicsDevice(), 1, height);
            Color[] data = new Color[height];

            for (int i = 0; i < height; i++)
            {
                data[i] = Color.White;
            }

            texture.SetData(data);
        }
    }
}
