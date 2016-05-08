using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    public struct Message
    {
        public Message(string text, bool isPermanent, int milliSecondsToStay = 0)
            : this()
        {
            this.Text = text;
            IsPermanent = isPermanent;
            MilliSecondsToStay = milliSecondsToStay;
        }

        public string Text { get; set; }

        public int MilliSecondsToStay { get; set; }

        public bool IsPermanent;
    }

    public static class GUI
    {
        public static bool hasMessage;
        public static bool messageIsPermanent;
        private static SpriteFont messageFont;
        private static Vector2 messagePosition;
        private static Texture2D messageTexture;
        private static Vector2 messageTextureOffset;
        private static TimeSpan messageTime;

        private static Queue<Message> Messages;
        private static Queue<Color> BackgroundColors;
        private static string MessageText;
        private static Color BackgroundColor;

        //Score: 
        private static Vector2 scorePosition;
        private static string scoreText;

        public static void Initialize()
        {
            messageTextureOffset = new Vector2(10, 5);
            Messages = new Queue<Message>();
            BackgroundColors = new Queue<Color>();
            scorePosition = new Vector2(20, 20);
        }

        public static void Load()
        {
            messageFont = Main.content.Load<SpriteFont>("MessageFont");            
        }

        public static void Update(GameTime gameTime, int score)
        {
            scoreText = "Score: " + score;
            UpdateMessage(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (hasMessage)
            {
                spriteBatch.Draw(messageTexture, messagePosition - messageTextureOffset, null, BackgroundColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.89f);
                spriteBatch.DrawString(messageFont, MessageText, messagePosition, Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.891f);
            }

            spriteBatch.DrawString(messageFont, scoreText, scorePosition, Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.891f);
        }

        private static void UpdateMessage(GameTime gameTime)
        {
            if (hasMessage)
            {
                if (messageTime.TotalMilliseconds > 0)
                {
                    messageTime = messageTime.Subtract(gameTime.ElapsedGameTime);
                }
                if (messageTime.TotalMilliseconds <= 0)
                {
                    if (Messages.Count <= 0)
                    {
                        if (!messageIsPermanent)
                        {
                            hasMessage = false;
                        }
                    }
                    else
                    {
                        ShowNextMessage();
                    }
                }
            }
        }

        public static void AddMessage(string message, Color backGroundColor, bool isPermanent, int milliSecondsToStay = 0)
        {
            Messages.Enqueue(new Message(message, isPermanent, milliSecondsToStay));
            BackgroundColors.Enqueue(backGroundColor);

            if (!hasMessage)
            {
                ShowNextMessage();
            }
        }

        private static void ShowNextMessage()
        {
            Message message = Messages.Dequeue();
            MessageText = message.Text;
            Vector2 messageSize = messageFont.MeasureString(message.Text);
            float posX = Main.width / 2 - messageSize.X / 2;
            float posY = Main.height / 2 - messageSize.Y / 2;
            messagePosition = new Vector2(posX, posY);
            messageTexture = Scripts.RoundRectangle((int)messageSize.X + 20, (int)messageSize.Y + 10, 4, Color.DimGray, Color.MintCream);
            BackgroundColor = BackgroundColors.Dequeue();
            hasMessage = true;
            messageIsPermanent = message.IsPermanent;
            messageTime = new TimeSpan(0, 0, 0, 0, message.MilliSecondsToStay);
        }

        public static bool RemoveCurrentMessage()
        {
            if(hasMessage)
            {
                if (Messages.Count > 0)
                {
                    ShowNextMessage();
                }
                else
                {
                    hasMessage = false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}