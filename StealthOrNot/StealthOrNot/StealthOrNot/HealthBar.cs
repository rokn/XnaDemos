using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StealthOrNot
{
    public class HealthBar
    {
        private Rectangle source;
        private int maxWidth;
        private Texture2D texture, backGroundTexture;
        private Vector2 Position, Origin, backgroundPosition, backOrigin;
        private Color color, colorFrom, colorTo;
        private bool hasBackground;
        public bool ShowText;
        public bool ShowTextInPercent;
        private string text;
        private Vector2 textPosition;

        public HealthBar(int MaxWidth, Vector2 position, Color from, Color to)
        {
            Position = position;
            maxWidth = MaxWidth;
            colorFrom = from;
            colorTo = to;
            color = colorFrom;
            ShowText = false;
            ShowTextInPercent = false;
        }

        public void Load(bool HasBackground, string Folder)
        {
            this.hasBackground = HasBackground;
            if (HasBackground)
            {
                texture = Scripts.LoadTexture(Folder + @"\HealthBar");
                backGroundTexture = Scripts.LoadTexture(Folder + @"\HealthBarBackground");
            }
            else if (!HasBackground)
            {
                texture = Scripts.LoadTexture(Folder + @"\HealthBar");
            }

            Origin = new Vector2(maxWidth / 2, texture.Height / 2);

            if (HasBackground)
            {
                backgroundPosition = Position + Origin;
                backOrigin = new Vector2(backGroundTexture.Width / 2, backGroundTexture.Height / 2);
            }

            source = new Rectangle(0, 0, maxWidth, texture.Height);
        }

        public void Update(Vector2 newPosition, float hp, float maxHp)
        {
            if (hp != maxHp)
            {
                color = Color.Lerp(colorTo, colorFrom, hp / maxHp);
            }
            else
            {
                color = colorFrom;
            }

            source.Width = (int)(maxWidth * (hp / maxHp));
            this.Position = newPosition;
            backgroundPosition = Position + Origin;

            if (ShowText)
            {
                if (!ShowTextInPercent)
                {
                    text = String.Format(Math.Round(hp, 1) + " / " + Math.Round(maxHp, 1));
                }
                else
                {
                    text = String.Format(Math.Round((hp / maxHp) * 100).ToString() + "%");
                }

                Vector2 textSize = Main.Font.MeasureString(text);
                textPosition = backgroundPosition - textSize / 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (texture != null)
            {
                if (hasBackground)
                {
                    spriteBatch.Draw(backGroundTexture, backgroundPosition, null, Color.White, 0, backOrigin, 1f, SpriteEffects.None, depth);
                }

                spriteBatch.Draw(texture, Position, source, color, 0, new Vector2(), 1f, SpriteEffects.None, depth + 0.0001f);

                if (ShowText && text != null)
                {
                    spriteBatch.DrawString(Main.Font, text, textPosition, Color.Black, 0f, new Vector2(), 1f, SpriteEffects.None, depth + 0.0002f);
                }
            }
        }
    }
}