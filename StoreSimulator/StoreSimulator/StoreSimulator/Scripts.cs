using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StoreSimulator
{
    public static class Scripts
    {
        public static Texture2D LoadTexture(string asset)
        {
            return Main.content.Load<Texture2D>(asset) as Texture2D;
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if (rect.Contains(Main.mouse.clickRectangle))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool KeyIsPressed(Keys key)
        {
            return Main.keyboard.JustPressed(key) || Main.keyboard.IsHeld(key);
        }

        public static bool KeyIsReleased(Keys key)
        {
            return Main.keyboard.IsReleased(key) || (!Main.keyboard.JustPressed(key) && !Main.keyboard.IsHeld(key));
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static Rectangle InitRectangle(Vector2 Position, int Width, int Height)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static bool CheckPixelPerfectCollision(Texture2D texture, Rectangle rect, Texture2D secondTexture, Rectangle secondRect)
        {
            Rectangle intersectRect = MathAid.GetIntersectingRectangle(secondRect, rect);

            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            Color[] destructionTextureData = new Color[secondTexture.Width * secondTexture.Height];
            secondTexture.GetData(destructionTextureData);

            Point startPos1 = new Point(intersectRect.X - secondRect.X, intersectRect.Y - secondRect.Y);
            Point startPos2 = new Point(intersectRect.X - rect.X, intersectRect.Y - rect.Y);

            for (int x = 0; x < intersectRect.Width; x++)
            {
                for (int y = 0; y < intersectRect.Height; y++)
                {
                    if (destructionTextureData[(startPos1.X + x) + (startPos1.Y + y) * secondTexture.Width].A != 0)
                    {
                        int X = startPos2.X + x;
                        int Y = startPos2.Y + y;
                        if (colorData[X + Y * texture.Width].A != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool Contains(this Rectangle rect, Vector2 point)
        {
            float left = rect.X;
            float top = rect.Y;
            float right = rect.Right;
            float bottom = rect.Bottom;

            if (point.X > left && point.X < right && point.Y > top && point.Y < bottom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Rectangle> GetSourceRectangles(int startingId, int endId, int rectWidth, int rectHeight, Texture2D texture)
        {
            int rectsPerRow = texture.Width / rectWidth;
            List<Rectangle> result = new List<Rectangle>();

            for (int i = startingId; i <= endId; i++)
            {
                int sourceY = i / rectsPerRow;
                int sourceX = i - sourceY * rectsPerRow;
                result.Add(new Rectangle(sourceX * rectWidth, sourceY * rectHeight, rectWidth, rectHeight));
            }

            return result;
        }

        public static void VoidRemove<T>(this List<T> list, T item)
        {
            list.Remove(item);
        }

        public static Rectangle GetWalkingRect(Vector2 Position, int Width, int Height)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height - 24);
        }

        public static Texture2D RoundRectangle(int width, int height, int borderThicknes, Color fillColor, Color borderColor)
        {
            Texture2D roundRect = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            Color[] color = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool empty = true;
                    Vector2 point = new Vector2(x, y);
                    Vector2 Center = Vector2.Zero;
                    if (y < borderThicknes)
                    {
                        if (x < borderThicknes)
                        {
                            Center = new Vector2(borderThicknes, borderThicknes);
                        }
                        else if (x > width - (borderThicknes))
                        {
                            Center = new Vector2(width - (borderThicknes), borderThicknes);
                        }
                        else Center = new Vector2(x, borderThicknes);
                    }
                    else if (y > height - (borderThicknes))
                    {
                        if (x < borderThicknes)
                        {
                            Center = new Vector2(borderThicknes, height - (borderThicknes));
                        }
                        else if (x > width - (borderThicknes))
                        {
                            Center = new Vector2(width - (borderThicknes), height - (borderThicknes));
                        }
                        else Center = new Vector2(x, height - (borderThicknes));
                    }
                    else
                    {
                        if (x < borderThicknes)
                            Center = new Vector2(borderThicknes, y);
                        else if (x > width - (borderThicknes))
                            Center = new Vector2(width - (borderThicknes), y);
                    }
                    if (Vector2.Distance(Center, point) > borderThicknes)
                    {
                        empty = false;
                    }
                    if (empty)
                    {
                        color[x + width * y] = borderColor;
                    }
                    if (x > borderThicknes - 1 && x < width - (borderThicknes - 1) && y > (borderThicknes - 1) && y < height - (borderThicknes - 1))
                    {
                        color[x + width * y] = fillColor;
                    }
                }
            }

            roundRect.SetData(color);
            return roundRect;
        }

        //public static void Write(this NetOutgoingMessage outMsg, Vector2 vector)
        //{
        //    outMsg.Write(vector.X);
        //    outMsg.Write(vector.Y);
        //}

        //public static Vector2 ReadVector(this NetIncomingMessage incMsg)
        //{
        //    Vector2 vector = new Vector2();
        //    vector.X = incMsg.ReadFloat();
        //    vector.Y = incMsg.ReadFloat();
        //    return vector;
        //}

        public static Point ToPoint(this Vector2 vector)
        {
            Point point = new Point((int)vector.X, (int)vector.Y);
            return point;
        }

        public static Vector2 ToVector(this Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y);
            return vector;
        }

        //public static List<Line> GetLines(this Rectangle rect, Vector2 Position, float Distance)
        //{
        //    List<Line> lines = new List<Line>();

        //    Vector2 p1 = new Vector2(rect.X + 1, rect.Y + 3);
        //    Vector2 p2 = new Vector2(rect.X + rect.Width - 3, rect.Y + 3);
        //    Vector2 p3 = new Vector2(rect.X + rect.Width - 3, rect.Y + rect.Height - 3);
        //    Vector2 p4 = new Vector2(rect.X + 2, rect.Y + rect.Height - 3);

        //    if (Math.Abs(Position.Y - p1.Y) <= Distance)
        //    {
        //        lines.Add(new Line(p1, p2));
        //    }

        //    if (Math.Abs(Position.X - p2.X) <= Distance)
        //    {
        //        lines.Add(new Line(p2, p3));
        //    }

        //    if (Math.Abs(Position.Y - p3.Y) <= Distance)
        //    {
        //        lines.Add(new Line(p3, p4));
        //    }

        //    if (Math.Abs(Position.X - p1.X) <= Distance)
        //    {
        //        lines.Add(new Line(p4, p1));
        //    }

        //    return lines;
        //}
    }
}