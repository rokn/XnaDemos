using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StealthOrNot
{
    public class Laser
    {
        public Vector2 Position;
        private Rectangle laserRect;
        public Rectangle rect;
        private Texture2D baseTexture;
        private Texture2D laserTexture;
        private Texture2D alarmTexture;
        private Vector2 Origin;
        private Vector2 LaserOrigin;
        private Vector2 AlarmOrigin;
        public bool IsActivated;
        public bool AlarmIsActive;
        private float alarmSize;

        public Laser(Vector2 position)
        {
            Position = position;
            IsActivated = true;
            AlarmIsActive = false;
            alarmSize = 0.1f;
            Load();
        }

        public void Load()
        {
            baseTexture = Scripts.LoadTexture("LaserBase");
            laserTexture = Scripts.LoadTexture("Laser");
            alarmTexture = Scripts.LoadTexture(@"Misc\LaserAlarm");
            Origin = new Vector2(baseTexture.Width / 2, baseTexture.Height);
            AlarmOrigin = new Vector2(alarmTexture.Width / 2, alarmTexture.Height / 2);
            LaserOrigin = new Vector2(0, laserTexture.Height / 2);
            InitLaserRect();
        }

        public void Update()
        {
            UpdateAlarm();

            if (!AlarmIsActive && IsActivated)
            {
                if (Main.MainPlayer is PoliceMan)
                {
                    CheckForActivation(Main.enemyPlayers);
                }
                else
                {
                    CheckForActivation(Main.mainPlayers);
                }
            }
        }

        private void UpdateAlarm()
        {
            if (alarmSize > 8f)
            {
                AlarmIsActive = false;
                alarmSize = 0.1f;
            }

            if (AlarmIsActive)
            {
                alarmSize += 0.2f;
            }
        }

        private void CheckForActivation(List<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.rect.Intersects(rect))
                {
                    IsActivated = false;
                    AlarmIsActive = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Main.MainPlayer is PoliceMan && IsActivated)
            {
                spriteBatch.Draw(laserTexture, Position, laserRect, Color.White, -MathHelper.PiOver2, LaserOrigin, 1f, SpriteEffects.None, 0.49f);
            }

            spriteBatch.Draw(baseTexture, Position, null, Color.White, 0, Origin, 1f, SpriteEffects.None, 0.5f);
        }

        public void InitLaserRect()
        {
            float minDistance = float.MaxValue;
            List<Line> lines = new List<Line>();
            Line MainLine = new Line(Position, Position - new Vector2(0, Main.width));
            Vector2 finalCollisionPoint = Position - new Vector2(0, Main.width);

            foreach (Rectangle rectangle in Main.blockRects)
            {
                Vector2 p1 = new Vector2(rectangle.X + 1, rectangle.Y + 3);
                Vector2 p2 = new Vector2(rectangle.X + rectangle.Width - 3, rectangle.Y + 3);
                Vector2 p3 = new Vector2(rectangle.X + rectangle.Width - 3, rectangle.Y + rectangle.Height - 3);
                Vector2 p4 = new Vector2(rectangle.X + 2, rectangle.Y + rectangle.Height - 3);

                lines.Add(new Line(p1, p2));
                lines.Add(new Line(p2, p3));
                lines.Add(new Line(p3, p4));
                lines.Add(new Line(p4, p1));
            }

            foreach (Line line in lines)
            {
                if (MainLine.IsIntersectingWith(line))
                {
                    Vector2 collisionPoint = MainLine.GetPositionOfInterection(line);
                    float distance = Vector2.Distance(collisionPoint, Position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        finalCollisionPoint = collisionPoint;
                    }
                }
            }

            minDistance = Vector2.Distance(finalCollisionPoint, Position);
            laserRect = new Rectangle(0, 0, (int)Math.Round(minDistance), laserTexture.Height);
            rect = Scripts.InitRectangle(finalCollisionPoint - LaserOrigin, laserRect.Height, laserRect.Width);
        }

        public void AlarmDraw(SpriteBatch spriteBatch)
        {
            if (AlarmIsActive)
            {
                spriteBatch.Draw(alarmTexture, Position, null, Color.White, 0, AlarmOrigin, alarmSize, SpriteEffects.None, 0.8f);
            }
        }
    }
}