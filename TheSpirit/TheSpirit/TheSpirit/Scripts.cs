﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TheSpirit
{
    public static class Scripts
    {
        public static Texture2D LoadTexture(string asset)
        {
            try
            {
                return Main.content.Load<Texture2D>(asset) as Texture2D;
            }
            catch (ContentLoadException)
            {
                Console.WriteLine("Texture not found: " + asset);
                return new Texture2D(Main.graphics.GraphicsDevice, 1, 1); // return a new empty image if specified is not found;
            }
        }

        public static bool CheckIfMouseIsOver(Rectangle rect)
        {
            if (rect.Contains(Main.Mouse.clickRectangle))
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

        public static bool CheckPixelPerfectCollision(Texture2D texture, Rectangle rect, Texture2D secondTexture, Rectangle secondRectangle)
        {
            Rectangle intersectRect = MathAid.GetIntersectingRectangle(secondRectangle, rect);

            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);

            Color[] destructionTextureData = new Color[secondTexture.Width * secondTexture.Height];
            secondTexture.GetData(destructionTextureData);

            Point startPos1 = new Point(intersectRect.X - secondRectangle.X, intersectRect.Y - secondRectangle.Y);
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

        //public static Dictionary<Direction, Animation> LoadEntityWalkAnimation(Texture2D spriteSheet)
        //{
        //    int frameWidth = spriteSheet.Width / 4;
        //    int frameHeight = spriteSheet.Height / 4;

        //    Dictionary<Direction, Animation> walkingAnimation = new Dictionary<Direction, Animation>();
        //    List<Rectangle> Down = Scripts.GetSourceRectangles(0, 3, frameWidth, frameHeight, spriteSheet);
        //    Animation animation = new Animation(Down, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Down, animation);

        //    List<Rectangle> Left = Scripts.GetSourceRectangles(4, 7, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Left, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Left, animation);

        //    List<Rectangle> Right = Scripts.GetSourceRectangles(8, 11, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Right, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Right, animation);

        //    List<Rectangle> Up = Scripts.GetSourceRectangles(12, 15, frameWidth, frameHeight, spriteSheet);
        //    animation = new Animation(Up, spriteSheet, 15, true);
        //    walkingAnimation.Add(Direction.Up, animation);

        //    return walkingAnimation;
        //}

        public static void VoidRemove<T>(this List<T> list, T item)
        {
            list.Remove(item);
        }

        public static Rectangle GetWalkingRect(Vector2 Position, int Width, int Height)
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Width, Height - 24);
        }

        //internal static Vector2 GetWalkingOrigin(int EntityWidth, int EntityHeight)
        //{
        //    return new Vector2(EntityWidth / 2, EntityHeight / 2 - Math.Min(TileSet.tileHeight, EntityHeight / 2));
        //}
    }
}