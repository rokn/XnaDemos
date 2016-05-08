using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace GalacticCommander
{
    public class Line
    {
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Length
        {
            get
            {
                return Vector2.Distance(Start, End);
            }
        }

        public bool IsIntersectingWith(Line otherLine)
        {
            if (Math.Round(otherLine.Start.X) == Math.Round(otherLine.End.X))
            {
                otherLine.End = new Vector2(otherLine.End.X - 1f, otherLine.End.Y);
            }

            if (Math.Round(Start.X) == Math.Round(End.X))
            {
                End = new Vector2(End.X + 1f, End.Y);
            }

            float denominator = ((End.X - Start.X) * (otherLine.End.Y - otherLine.Start.Y)) - ((End.Y - Start.Y) * (otherLine.End.X - otherLine.Start.X));
            float numerator1 = ((Start.Y - otherLine.Start.Y) * (otherLine.End.X - otherLine.Start.X)) - ((Start.X - otherLine.Start.X) * (otherLine.End.Y - otherLine.Start.Y));
            float numerator2 = ((Start.Y - otherLine.Start.Y) * (End.X - Start.X)) - ((Start.X - otherLine.Start.X) * (End.Y - Start.Y));

            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public Vector2 GetPositionOfInterection(Line otherLine)
        {
            Vector2 pointOfIntersect;
            float m1, b1, m2, b2;

            m1 = GetSlope(Start, End);
            b1 = GetYIntersect(Start, m1);

            m2 = GetSlope(otherLine.Start, otherLine.End);
            b2 = GetYIntersect(otherLine.Start, m2);
            pointOfIntersect.X = (b2 - b1) / (m1 - m2);
            pointOfIntersect.Y = m1 * pointOfIntersect.X + b1;

            return pointOfIntersect;
        }

        private float GetSlope(Vector2 Start, Vector2 End)
        {
            return (End.Y - Start.Y) / (End.X - Start.X);
        }

        private float GetYIntersect(Vector2 Start, float slope)
        {
            return Start.Y - Start.X * slope;
        }
    }
}