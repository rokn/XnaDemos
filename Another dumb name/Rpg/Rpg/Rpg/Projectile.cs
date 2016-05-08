using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public struct ProjectileStat
    {
        public string Name;
        public int textureFrames, textureColumns,frameWidth,frameHeight;
        public bool isAnimation,isFrosty;
        public float rotationSpeed;
    }

    public class Projectile
    {
        float depth = 0.4f;
        Vector2 Position,Speed,Target;
        float moveSpeed;
        Texture2D texture;
        Animation animation;
        public ProjectileStat stats;
        public int Id, AiID;
        float angle;
        private float? baseAngle;
        private bool isGoingLeft;
        public Rectangle rect;
        public float baseDamage;

        public Projectile(int id,int aiID,Vector2 target,Vector2 position,float speed,float Damage,bool arcMiss = true)
        {
            Id = id;
            Target = target;
            Position = position;
            Speed = target - position;
            Speed.Normalize();
            angle = MathAid.FindRotation(Position, Target);
            moveSpeed = speed;
            Speed *= moveSpeed;
            stats = Rpg.projectilesStats[Id];
            if(stats.isAnimation)
            {
                List<Rectangle> rects = Scripts.GetSpriteSheetRects(stats.frameWidth, stats.frameHeight, stats.textureColumns, stats.textureFrames);
                animation = new Animation(rects,
                    Rpg.projectilesTextures[id],
                    15  - (int)moveSpeed,
                    true);
                rect = rects[0];
            }
            else
            {
                texture = Rpg.projectilesTextures[Id];
                rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
            AiID = aiID;
            baseAngle = null;
            isGoingLeft = arcMiss;
            baseDamage = Damage;
            
        }

        public void Update()
        {
            ProjectileAi();
            Position += Speed;
            rect.X = (int)Position.X;
            rect.Y = (int)Position.Y;
            if (stats.isAnimation)
            {
                animation.Update(Position, angle);
            }
            if(stats.rotationSpeed!=0)
            {
                angle += stats.rotationSpeed;
            }
        }

        private void ProjectileAi()
        {
            switch(AiID)
            {
                case 1:
                    if(baseAngle == null)
                    {
                        baseAngle = angle;
                    }
                    ArcaneMissilesUpdate();
                    break;
            }
        }

        private void ArcaneMissilesUpdate()
        {
            if (isGoingLeft)
            {
                angle -= 0.05f;
                if (angle < baseAngle - 0.5)
                {
                    isGoingLeft = false;
                }
            }
            else
            {
                angle += 0.05f;
                if (angle > baseAngle + 0.5)
                {
                    isGoingLeft = true;
                }
            }
            Speed = MathAid.AngleToVector(angle) * moveSpeed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(stats.isAnimation)
            {
                animation.Draw(spriteBatch, depth);
            }
            else
            {
                spriteBatch.Draw(texture, Position, null, Color.White, angle, new Vector2(texture.Width / 2, texture.Height / 2), 1, SpriteEffects.None, depth);
            }
        }

    }
}
