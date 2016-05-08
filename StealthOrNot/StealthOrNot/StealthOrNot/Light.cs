using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StealthOrNot
{
    public class Light
    {
        public static GraphicsDevice Device;
        public List<Vector2> interSections;
        public int RaysCount;
        public int Angle;
        public int StartingAngle;
        public bool Dynamic;
        private bool UpdatedOnce;

        public Light(Vector2 position, Color clr, float size, int raysCount, bool dynamic, int angle = 360, int startingAngle = 0)
        {
            Position = position;
            color = clr;
            interSections = new List<Vector2>();
            Size = size;
            RaysCount = raysCount;
            Angle = angle;
            StartingAngle = startingAngle;
            IsOn = true;
            Dynamic = dynamic;

            if (!Dynamic)
            {
                UpdatedOnce = false;
            }

            Main.Lights.Add(this);
        }

        public Color color { get; set; }
        public bool IsOn { get; private set; }
        public Vector2 Position { get; private set; }
        public float Size { get; set; }

        public void SwitchState()
        {
            IsOn = !IsOn;
        }

        public void ChangePosition(Vector2 newPosition)
        {
            Position = newPosition;
        }

        public void Update()
        {
            if (!Dynamic && UpdatedOnce)
            {
                return;
            }

            if (IsOn && (IsWithinCamera() || !Dynamic))
            {
                if (!UpdatedOnce)
                {
                    UpdatedOnce = true;
                }

                interSections.Clear();
                foreach (Rectangle rect in Main.blockRects)
                {
                    if (rect.Contains(Position))
                    {
                        return;
                    }
                }

                CastRays();
            }
        }

        private void CastRays()
        {
            float k = (float)RaysCount / 360f;

            for (int i = (int)(StartingAngle * k); i < StartingAngle * k + Angle * k; i++)
            {
                Line ln = new Line(Position, Position + MathAid.AngleToVector(MathHelper.ToRadians(i * (360 / RaysCount))) * Size);
                interSections.Add(GetIntersection(ln));
                Main.raysCast++;
            }
        }

        public void Draw(PrimitiveBatch primitiveBatch, GraphicsDevice graphicsDevice)
        {
            if (IsOn && IsWithinCamera())
            {
                primitiveBatch.Begin(PrimitiveType.TriangleList, Main.camera.GetTransformation(graphicsDevice));

                Random rand = new Random();

                interSections.Sort((x, y) => GetDirectionTo(x).CompareTo(GetDirectionTo(y)));

                int count = interSections.Count;

                for (int i = 0; i < count; i++)
                {
                    Vector2 firstVector;
                    Vector2 secondVector;

                    if (i < count - 1)
                    {
                        firstVector = interSections[i];
                        secondVector = interSections[i + 1];
                    }
                    else
                    {
                        firstVector = interSections[i];
                        secondVector = interSections[0];
                    }

                    primitiveBatch.AddVertex(firstVector, color);
                    primitiveBatch.AddVertex(secondVector, color);
                    primitiveBatch.AddVertex(Position, color);
                }

                primitiveBatch.End();
            }
        }

        private bool IsWithinCamera()
        {
            if (Position.Y + Size < Main.camera.Position.Y)
            {
                return false;
            }

            if (Position.Y - Size > Main.camera.Position.Y + Main.height)
            {
                return false;
            }

            if (Position.X + Size < Main.camera.Position.X)
            {
                return false;
            }

            if (Position.X - Size > Main.camera.Position.X + Main.width)
            {
                return false;
            }
            return true;
        }

        private float GetDirectionTo(Vector2 p)
        {
            return MathAid.FindRotation(Position, p);
        }

        private Vector2 GetIntersection(Line mainLine)
        {
            float minDistance = Vector2.Distance(mainLine.Start, mainLine.End);

            Vector2 intersection = mainLine.End;

            foreach (var rect in Main.blockRects)
            {
                List<Line> placeHolder = GetLines(rect);
                List<Line> lines = new List<Line>(placeHolder.Count + Main.lines.Count);
                lines.AddRange(placeHolder);
                lines.AddRange(Main.lines);

                foreach (Line line in lines)
                {
                    if (mainLine.IsIntersectingWith(line))
                    {
                        Vector2 interSection = mainLine.GetPositionOfInterection(line);
                        float distance = Vector2.Distance(interSection, mainLine.Start);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            intersection = interSection;
                        }
                    }
                }
            }

            return intersection;
        }

        private List<Line> GetLines(Rectangle rect)
        {
            List<Line> lines = new List<Line>();

            Vector2 p1 = new Vector2(rect.X + 1, rect.Y + 3);
            Vector2 p2 = new Vector2(rect.X + rect.Width - 3, rect.Y + 3);
            Vector2 p3 = new Vector2(rect.X + rect.Width - 3, rect.Y + rect.Height - 3);
            Vector2 p4 = new Vector2(rect.X + 2, rect.Y + rect.Height - 3);

            if (Math.Abs(Position.Y - p1.Y) <= Size)
            {
                lines.Add(new Line(p1, p2));
            }

            if (Math.Abs(Position.X - p2.X) <= Size)
            {
                lines.Add(new Line(p2, p3));
            }

            if (Math.Abs(Position.Y - p3.Y) <= Size)
            {
                lines.Add(new Line(p3, p4));
            }

            if (Math.Abs(Position.X - p1.X) <= Size)
            {
                lines.Add(new Line(p4, p1));
            }

            return lines;
        }
    }
}