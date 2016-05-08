#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Rpg
{
    /// <summary>
    /// A class to animate 2D objects.  Can be used via
    /// Time or through index manually.
    /// </summary>
    public class Animation
    {
        #region Vars
        private int stepsPerFrame,Index,currFrameSteps;
        private List<Rectangle> sourceRectangles;
        private Texture2D sourceSpriteSheet,currentTexture;
        private List<Texture2D> texturesList;
        private bool isSpriteSheet;
        private Vector2 Position, Origin;
        private float rotation;
        private bool looping,isAnimating;
        #endregion
        #region Constructors
        public Animation(List<Rectangle> rectangleList, Texture2D spriteSheet, int StepsPerFrame,bool Looping)
        {
            sourceRectangles = rectangleList;
            sourceSpriteSheet = spriteSheet;
            stepsPerFrame = StepsPerFrame;
            Index = 0;
            currFrameSteps = 0;
            isSpriteSheet = true;
            Origin = new Vector2(rectangleList[0].Width/2,rectangleList[0].Height/2);
            looping = Looping;
            isAnimating = true;
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
            
        }
        #endregion
        public void GhangeAnimatingState(bool IsAnimating)
        {
            Index = 0;
            currFrameSteps = 0;
            isAnimating = IsAnimating;
        }
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }
        #region Update
        public void Update(Vector2 position,float Rotation)
        {
            if (isAnimating)
            {
                rotation = Rotation;
                Position = position;
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
        #endregion;
        public void Draw(SpriteBatch spriteBatch,float depth)
        {
            if (isAnimating)
            {
                if (isSpriteSheet)
                {
                    spriteBatch.Draw(sourceSpriteSheet, Position, sourceRectangles[Index], Color.White, rotation, Origin, 1f, SpriteEffects.None, depth);
                }
                else
                {
                    spriteBatch.Draw(currentTexture, Position, null, Color.White, rotation, Origin, 1f, SpriteEffects.None, depth);
                }
            }
        }
    }
}