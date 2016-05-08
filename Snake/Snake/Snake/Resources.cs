using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;

namespace Snake
{
    public static class Layer
    {
        public const float TileDefault = 0.1f;
        public const float Grid = 0.15f;
        public const float SnakeLayer = 0.8f;
    }

    public static class Resources
    {
        public static Texture2D SnakeHead, SnakeBody, SnakeBodyCurved, SnakeTail;
        public static Texture2D Apple;

        public static void Load()
        {
            SnakeHead = Scripts.LoadTexture(@"Snake\SnakeHead");
            SnakeBody = Scripts.LoadTexture(@"Snake\SnakeBody");
            SnakeTail = Scripts.LoadTexture(@"Snake\SnakeTail");
            SnakeBodyCurved = Scripts.LoadTexture(@"Snake\SnakeBodyCurved");
            Apple = Scripts.LoadTexture("Apple");
        }
    }
}