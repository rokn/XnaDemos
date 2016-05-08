using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StealthOrNot
{
    public static class Helicopter
    {
        private const int ladderSpeed = 15;

        private static Animation flyAnimation;
        private static bool isDroppingLadder;
        public static bool isMoving;
        private static float ladderEndPosition;
        private static float Speed;
        private static Sound sound;
        private static Texture2D ladderTexture;
        private static Vector2 Destination;
        private static Vector2 ladderStartPosition;
        private static Vector2 Origin;
        public static Vector2 Position;
        private static Vector2 Velocity;
        private static bool hasDroppedLadder;
        public static List<Rectangle> rects;

        public static void Initialize(Vector2 position, Vector2 destination)
        {
            sound = new Sound(4000, position, 1.0f);
            Load();
            Position = position;
            Destination = destination;

            sound.IsLooped = true;
            sound.Play();

            rects = new List<Rectangle>();

            Rectangle rect = Scripts.InitRectangle(Position - Origin, 440, 309);
            rects.Add(rect);
            rect = Scripts.InitRectangle(Position + new Vector2(-37, 144), 308, 11);
            rects.Add(rect);
            rect = Scripts.InitRectangle(Position + new Vector2(-37, -79), 308, 18);
            rects.Add(rect);
            rect = Scripts.InitRectangle(Position + new Vector2(270, -80), 80, 300);
            rects.Add(rect);

            Speed = 15;
            Velocity = Vector2.Normalize(Destination - Position) * Speed;
            isMoving = true;
            isDroppingLadder = false;
            hasDroppedLadder = false;
        }

        private static void Load()
        {
            int height = 309;

            Texture2D flyTexture = Scripts.LoadTexture("Helicopter");
            Origin = new Vector2(flyTexture.Width / 2, height / 2);
            Rectangle flyRect = Scripts.InitRectangle(new Vector2(), flyTexture.Width, height);
            List<Rectangle> walkingRects = Scripts.GetSourceRectangles(0, 2, flyRect.Width, flyRect.Height, flyTexture);
            flyAnimation = new Animation(walkingRects, flyTexture, 2, true);
            flyAnimation.ChangeAnimatingState(true);

            ladderTexture = Scripts.LoadTexture("RopeLadder");

            sound.Load("HelicopterSound");
        }

        public static void Update()
        {
            if (isMoving)
            {
                Velocity = Vector2.Normalize(Destination - Position) * Speed;
                Vector2 goTo = Position + Velocity;
                float distToTarget = Vector2.Distance(Position, Destination);

                if (distToTarget < Vector2.Distance(Position, goTo))
                {
                    Velocity = Destination - Position;
                    isMoving = false;
                }

                Position += Velocity;

                if (Main.MainPlayer is PoliceMan)
                {
                    UpdatePlayers(Main.mainPlayers);
                }
                else
                {
                    UpdatePlayers(Main.enemyPlayers);
                }

                UpdateRects();
            }
            else
            {
                if (!isDroppingLadder && !hasDroppedLadder)
                {
                    isDroppingLadder = true;
                    ladderStartPosition = Position + new Vector2(80, 145);
                    ladderEndPosition = ladderStartPosition.Y + 1;
                }

                if (isDroppingLadder)
                {
                    DropLadder();
                }
            }

            flyAnimation.Update(Position, 0.0f);
            sound.Update(Position);
        }

        private static void DropLadder()
        {
            for (int i = 0; i < ladderSpeed; i++)
            {
                ladderEndPosition++;

                foreach (Rectangle rect in Main.blockRects)
                {
                    if (rect.Contains(new Vector2(ladderStartPosition.X, ladderEndPosition)))
                    {
                        isDroppingLadder = false;
                        hasDroppedLadder = true;
                        return;
                    }
                }
            }
        }

        private static void UpdateRects()
        {
            for (int i = 0; i < rects.Count; i++)
            {
                rects[i] = Scripts.InitRectangle(rects[i].Location.ToVector() + Velocity, rects[i].Width, rects[i].Height);
            }
        }

        private static void UpdatePlayers(List<Player> players)
        {
            foreach (Player p in players)
            {
                if (p.IsInHelicopter)
                {
                    p.UpdatePosition(p.Position + Velocity);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            flyAnimation.Draw(spriteBatch, 1f, 0.4f, Color.White);

            if (!isMoving)
            {
                DrawLadder(spriteBatch);
            }

            if (Main.showBoundingBoxes)
            {
                foreach (var rect in rects)
                {
                    spriteBatch.Draw(Main.BoundingBox, rect, rect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                }
            }
        }

        private static void DrawLadder(SpriteBatch spriteBatch)
        {
            float y = ladderStartPosition.Y;
            int height = ladderTexture.Height;

            int ladderSize = (int)(ladderEndPosition - ladderStartPosition.Y);

            int remainder = ladderSize % height;

            if (remainder != 0)
            {
                Rectangle sourceRect = new Rectangle(0, 0, ladderTexture.Width, remainder);
                spriteBatch.Draw(ladderTexture, new Vector2(ladderStartPosition.X, y), sourceRect, Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.41f);
                y += remainder;
            }

            int ladderParts = (int)(ladderSize / height);

            for (int i = 0; i < ladderParts; i++)
            {
                spriteBatch.Draw(ladderTexture, new Vector2(ladderStartPosition.X, y), null, Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0.41f);
                y += height;
            }
        }
    }
}