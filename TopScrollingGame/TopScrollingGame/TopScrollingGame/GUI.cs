using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TopScrollingGame
{
    public struct Message
    {
        public Message(string text, int milliSecondsToStay)
            : this()
        {
            this.Text = text;
            MilliSecondsToStay = milliSecondsToStay;
        }

        public string Text { get; set; }

        public int MilliSecondsToStay { get; set; }
    }

    public static class GUI
    {
        private static Vector2 DamageTextPos;
        private static Vector2 ShotSpeedTextPos;
        private static Vector2 AttackSpeedTextPos;
        private static Vector2 SpeedTextPos;
        private static Vector2 EffectsPos;

        private static Texture2D rightPart;
        private static Texture2D sideMyst;

        private static Vector2 rightPosition;
        private static Vector2 rightSize;
        private static Vector2 leftSize;

        public static Texture2D castleGate;
        public static Vector2 castlePosition;
        private static Vector2 castleSize;

        private static Vector2 mainHealthBarPosition;
        private static HealthBar mainHealthBar;

        private static HealthBar playerHealthBar;
        private static Vector2 playerHealthBarPosition;

        public static bool hasMessage;
        private static SpriteFont messageFont;
        private static Vector2 messagePosition;
        private static Texture2D messageTexture;
        private static Vector2 messageTextureOffset;
        private static TimeSpan messageTime;

        private static Queue<Message> Messages;
        private static Queue<Color> BackgroundColors;
        private static string MessageText;
        private static Color BackgroundColor;

        public static void Initialize()
        {
            rightPosition = new Vector2(Main.playingAreaX + Main.playingAreaWidth, 0);

            mainHealthBarPosition = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 160);
            mainHealthBar = new HealthBar(150, mainHealthBarPosition, Color.Blue, Color.Black);

            playerHealthBarPosition = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 45);
            playerHealthBar = new HealthBar(150, playerHealthBarPosition, Color.Green, Color.Red);

            messageTextureOffset = new Vector2(10, 5);

            EffectsPos = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 950);
            DamageTextPos = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 350);
            ShotSpeedTextPos = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 400, 350);
            AttackSpeedTextPos = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 400);
            SpeedTextPos = new Vector2(Main.playingAreaX + Main.playingAreaWidth + 400, 400);

            Messages = new Queue<Message>();
            BackgroundColors = new Queue<Color>();

            GUI.Load();
        }

        public static void Load()
        {
            messageFont = Main.content.Load<SpriteFont>("MessageFont");

            rightPart = Scripts.LoadTexture(@"Sprites\GUI\RightPart");
            castleGate = Scripts.LoadTexture(@"Sprites\GUI\CastleGate");
            sideMyst = Scripts.LoadTexture(@"Sprites\GUI\SideMyst");
            float rightSizeX = (Main.baseScreenSize.X - (Main.playingAreaX + Main.playingAreaWidth)) / (float)rightPart.Width;
            float leftSizeX = (Main.playingAreaX) / (float)rightPart.Width;
            float rightSizeY = Main.baseScreenSize.Y / (float)rightPart.Height;

            float castleSizeX = (float)Main.playingAreaWidth / (float)castleGate.Width;

            castleSize = new Vector2(castleSizeX, 1f);
            rightSize = new Vector2(rightSizeX, rightSizeY);
            leftSize = new Vector2(leftSizeX, rightSizeY);

            castlePosition = new Vector2(Main.playingAreaX, (int)Main.baseScreenSize.Y - castleGate.Height);

            mainHealthBar.Load(true, @"Sprites\GUI\");
            playerHealthBar.Load(true, @"Sprites\GUI\");
        }

        public static void Update(GameTime gameTime)
        {
            mainHealthBar.Update(mainHealthBarPosition, Main.Health, Main.MaxHealth);
            playerHealthBar.Update(playerHealthBarPosition, Main.heroRef.Health, Main.heroRef.MaxHealth);

            
        }

        public static void AddMessage(string message, int milliSecondsToStay, Color backGroundColor)
        {
            Messages.Enqueue(new Message(message, milliSecondsToStay));
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
            float posX = Main.playingAreaX + Main.playingAreaWidth / 2 - messageSize.X / 2;
            float posY = Main.baseScreenSize.Y / 2 - messageSize.Y / 2;
            messagePosition = new Vector2(posX, posY);
            messageTexture = Scripts.RoundRectangle((int)messageSize.X + 20, (int)messageSize.Y + 10, 4, Color.DimGray, Color.MintCream);
            BackgroundColor = BackgroundColors.Dequeue();
            hasMessage = true;
            messageTime = new TimeSpan(0, 0, 0, 0, message.MilliSecondsToStay);
        }

        private static Color GetDrawColor(float currentValue, float startingValue)
        {
            if (currentValue <= startingValue)
            {
                return Color.Black;
            }
            else
            {
                return Color.Green;
            }
        }

        private static void DrawStats(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Main.Font, String.Format("{0:0}/{1} Lady's HP", Main.heroRef.Health, Main.heroRef.MaxHealth), playerHealthBarPosition + new Vector2(220, -5), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            spriteBatch.DrawString(Main.Font, String.Format("{0}/{1} Castle's HP", Main.Health, Main.MaxHealth), mainHealthBarPosition + new Vector2(220, -5), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);

            spriteBatch.DrawString(Main.Font, "Main stats:", new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 240), Color.Black, 0f, new Vector2(), 2f, SpriteEffects.None, 0.82f);
            spriteBatch.DrawString(Main.Font, "Secondary stats:", new Vector2(Main.playingAreaX + Main.playingAreaWidth + 400, 240), Color.Black, 0f, new Vector2(), 2f, SpriteEffects.None, 0.82f);

            Color drawColor = GetDrawColor(Main.heroRef.Damage, Hero.startingDamage);
            spriteBatch.DrawString(Main.Font, "Damage : " + Main.heroRef.Damage, DamageTextPos, drawColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            drawColor = GetDrawColor(Main.heroRef.ShotSpeed, Hero.startingShotSpeed);
            spriteBatch.DrawString(Main.Font, "Shot Speed : " + Main.heroRef.ShotSpeed, ShotSpeedTextPos, drawColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            drawColor = GetDrawColor(Main.heroRef.AttackSpeed, Hero.startingAttackSpeed);
            spriteBatch.DrawString(Main.Font, "Atacks per second: " + Main.heroRef.AttackSpeed, AttackSpeedTextPos, drawColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            drawColor = GetDrawColor(Main.heroRef.Speed, Hero.startingSpeed);
            spriteBatch.DrawString(Main.Font, "Speed : " + Main.heroRef.Speed, SpeedTextPos, drawColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
        }

        private static void DrawWaveBonuses(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Main.Font, "Current Wave Bonus: " + WavesSystem.CurrentWaveBonus.ToString(), new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 500), Color.Blue, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);

            if (WavesSystem.CurrentWave >= 5)
            {
                spriteBatch.DrawString(Main.Font, "Current Negative Wave Bonus: " + WavesSystem.CurrentNegativeWaveBonus.ToString(), new Vector2(Main.playingAreaX + Main.playingAreaWidth + 50, 550), Color.Red, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rightPart, rightPosition, null, Color.White, 0, new Vector2(), rightSize, SpriteEffects.None, 0.8f);

            spriteBatch.Draw(rightPart, new Vector2(), null, Color.White, 0, new Vector2(), leftSize, SpriteEffects.None, 0.8f);

            spriteBatch.Draw(castleGate, castlePosition, null, Color.White, 0, new Vector2(), castleSize, SpriteEffects.None, 0.81f);

            if (WavesSystem.CurrentWave == 0)
            {
                spriteBatch.DrawString(Main.Font, "Press E to start the first wave!", new Vector2(400, 500), Color.Black, 0f, new Vector2(), 1f, SpriteEffects.None, 0.99f);
            }

            mainHealthBar.Draw(spriteBatch, 0.9f);
            playerHealthBar.Draw(spriteBatch, 0.9f);

            DrawStats(spriteBatch);
            DrawWaveBonuses(spriteBatch);
            spriteBatch.DrawString(Main.Font, "Current Wave: " + WavesSystem.CurrentWave, new Vector2(Main.playingAreaX + Main.playingAreaWidth + 200, 700), Color.MidnightBlue, 0f, new Vector2(), 2f, SpriteEffects.None, 0.82f);

            for (int i = 0; i < Main.heroRef.Effects.Count; i++)
            {
                Vector2 effectPosition = EffectsPos + new Vector2(60f * i, 0f);
                spriteBatch.Draw(MainHelper.PowerUpTextures[(int)Main.heroRef.Effects[i].Type], effectPosition, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
                spriteBatch.DrawString(Main.Font, String.Format("{0:0}s", Main.heroRef.Effects[i].Duration.TotalSeconds), effectPosition + new Vector2(0, 50), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            }

            if (hasMessage)
            {
                spriteBatch.Draw(messageTexture, messagePosition - messageTextureOffset, null, BackgroundColor, 0, new Vector2(), 1f, SpriteEffects.None, 0.89f);
                spriteBatch.DrawString(messageFont, MessageText, messagePosition, Color.Red, 0, new Vector2(), 1f, SpriteEffects.None, 0.891f);
            }
            if (WavesSystem.CurrentWave >= 11)
            {
                spriteBatch.Draw(sideMyst, new Vector2(Main.playingAreaX, 0), null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.82f);
            }
        }
    }
}