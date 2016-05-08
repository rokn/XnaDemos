﻿#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#endregion Using Statements

namespace StealthOrNot
{
    /// <summary>
    /// A class to animate 2D objects.  Can be used via
    /// Time or through index manually.
    /// </summary>
    public class Animation
    {
        #region Vars

        public int FrameCount;
        public bool looping, isAnimating;
        public int stepsPerFrame, Index, currFrameSteps;
        public bool isSpriteSheet;
        private Vector2 Position, Origin;
        private float rotation;
        private List<Rectangle> sourceRectangles;
        private Texture2D sourceSpriteSheet, currentTexture;
        private List<Texture2D> texturesList;

        #endregion Vars

        #region Constructors

        public Animation(List<Rectangle> rectangleList, Texture2D spriteSheet, int StepsPerFrame, bool Looping)
        {
            sourceRectangles = rectangleList;
            sourceSpriteSheet = spriteSheet;
            stepsPerFrame = StepsPerFrame;
            Index = 0;
            currFrameSteps = 0;
            isSpriteSheet = true;
            Origin = new Vector2(rectangleList[0].Width / 2, rectangleList[0].Height / 2);
            looping = Looping;
            isAnimating = true;
            FrameCount = sourceRectangles.Count;
        }

        public Animation(List<Texture2D> texturesList, int StepsPerFrame, bool Looping)
        {
            this.texturesList = texturesList;
            stepsPerFrame = StepsPerFrame;
            Index = 0;
            currFrameSteps = 0;
            isSpriteSheet = false;
            Origin = new Vector2(texturesList[0].Width / 2, texturesList[0].Height / 2);
            isAnimating = true;
            currentTexture = texturesList[Index];
            looping = Looping;
            FrameCount = texturesList.Count;
        }

        #endregion Constructors

        public void ChangeAnimatingState(bool IsAnimating)
        {
            Index = 0;
            currFrameSteps = 0;
            isAnimating = IsAnimating;
        }

        public Texture2D GetCurrentTexture()
        {
            if (!isSpriteSheet)
            {
                return currentTexture;
            }
            else
            {
                Rectangle rect = sourceRectangles[Index];
                int width = rect.Width;
                int height = rect.Height;
                Texture2D texture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
                Color[] data = new Color[width * height];
                Color[] sheetData = new Color[sourceSpriteSheet.Width * sourceSpriteSheet.Height];
                sourceSpriteSheet.GetData(sheetData);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        data[x + y * width] = sheetData[(x + rect.X) + (y + rect.Y) * sourceSpriteSheet.Width];
                    }
                }
                texture.SetData(data);
                return texture;
            }
        }

        #region Update

        public void Update(Vector2 position, float Rotation)
        {
            rotation = Rotation;
            Position = position;

            if (isAnimating)
            {
                if (currFrameSteps < stepsPerFrame)
                {
                    currFrameSteps++;
                }
                else
                {
                    currFrameSteps = 0;

                    if (isSpriteSheet)
                    {
                        if (Index < sourceRectangles.Count - 1)
                        {
                            Index++;
                        }
                        else
                        {
                            if (looping)
                            {
                                Index = 0;
                            }
                            else
                            {
                                isAnimating = false;
                            }
                        }
                    }
                    else
                    {
                        currentTexture = texturesList[Index];

                        if (Index < texturesList.Count - 1)
                        {
                            Index++;
                        }
                        else
                        {
                            if (looping)
                            {
                                Index = 0;
                            }
                            else
                            {
                                isAnimating = false;
                            }
                        }
                    }
                }
            }
        }

        #endregion Update

        public void Draw(SpriteBatch spriteBatch, float size, float depth, Color color, SpriteEffects effects = SpriteEffects.None)
        {
            if (isSpriteSheet)
            {
                spriteBatch.Draw(sourceSpriteSheet, Position, sourceRectangles[Index], color, rotation, Origin, size, effects, depth);
            }
            else
            {
                spriteBatch.Draw(currentTexture, Position, null, color, rotation, Origin, size, effects, depth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth, Color color, Rectangle rect, SpriteEffects effects = SpriteEffects.None, float alpha = 1.0f)
        {
            if (isSpriteSheet)
            {
                spriteBatch.Draw(sourceSpriteSheet, rect, sourceRectangles[Index], color * alpha, rotation, new Vector2(), effects, depth);
            }
            else
            {
                spriteBatch.Draw(currentTexture, rect, null, color * alpha, rotation, new Vector2(), effects, depth);
            }
        }
    }
}