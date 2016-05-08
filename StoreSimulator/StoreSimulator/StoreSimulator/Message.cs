using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StoreSimulator
{
    public class Message
    {
        private Vector2 Position;
        private Texture2D texture;
        private SpriteFont Font;
        public string Text;
        private Color backColor;
        private Point padding;

        /// <summary>
        /// Creates a new message on the screen
        /// </summary>
        /// <param name="text">The text of the message</param>
        /// <param name="font">The font used by the message</param>
        /// <param name="clr">The color of the background of the message</param>
        /// <param name="position">The left top position of the message</param>
        /// <param name="centered">If true will override the given position and will place the message in the center of the screen</param>
        public Message(string text, SpriteFont font, Color clr, Vector2 position, bool centered = false)
        {
            padding = new Point(10, 10);
            Text = text;
            Font = font;
            backColor = clr;
            CreateTexture();

            if (!centered)
            {
                Position = position;
            }
            else
            {
                Position = new Vector2();
                Position.X = Main.width / 2 - texture.Width / 2;
                Position.Y = Main.height / 2 - texture.Height / 2;
            }
        }

        public void CreateTexture()
        {
            Point textSize = Font.MeasureString(Text).ToPoint();
            texture = Scripts.RoundRectangle(textSize.X + padding.X * 2, textSize.Y + padding.Y * 2, 3, backColor, Color.Black);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, Layer.Messages);
            spriteBatch.DrawString(Font, Text, Position + padding.ToVector(), Color.Black, 0, new Vector2(), 1.0f, SpriteEffects.None, Layer.Messages + 0.00001f);
        }
    }
}