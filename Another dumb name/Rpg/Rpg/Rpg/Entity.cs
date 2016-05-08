using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public struct DamageRect
    {
        private Rectangle rect;
        private int damage;
        private int lifeTime;
        private bool isAlive;
        public int Damage { get { return damage; } }
        public bool IsAlive { get { return isAlive; } }

        public DamageRect(int dmg,Rectangle Rect,int LifeTime)
        {
            damage = dmg;
            rect = Rect;
            isAlive = true;
            lifeTime = LifeTime;
        }
        
        public void Update() { 
            lifeTime--;
            if (lifeTime <= 0) { isAlive = false; }
            }

        public bool Intersects(Rectangle intersectRect)
        {
            return rect.Intersects(intersectRect);
        }

        public bool ContainsVector(Vector2 point)
        {
            return rect.Contains((int)point.X, (int)point.Y);
        }
    }
    public class Entity
    {
        public float Health, maxHealth, Damage, Mana, maxMana, moveSpeed,Angle,manaRegen,healthRegen;
        public Vector2 Position,Speed, movementTargetPosition,Origin;
        public States state;
        protected Dictionary<States, Animation> animations;
        public bool GraphicsLoaded, isStanding;
        protected Texture2D Standing,Hair;
        protected Vector2 hairOrigin;
        protected bool isFrozen;
        protected Effect effect;

        public Entity (Vector2 position,Effect eff)
        {
            Position = position;
            state = States.Standing;
            animations = new Dictionary<States, Animation>();
            isStanding = true;
            isFrozen = false;
            effect = eff;
        }
        public virtual void Load(ContentManager Content) { }

        public virtual void Update() 
        {
            if (isFrozen && state != States.Standing)
            {
                state = States.Standing;
            }
            RegenHealth();
            RegenMana();
            if (state != States.Standing)
            {
                animations[state].Update(Position, MathHelper.ToRadians(Angle));
            }
            if (state == States.Walking)
            {
                if (DistanceToTarget() <= moveSpeed + 3)
                {
                    StopWalking();
                }
                Position += Speed;
            }
            
        }

        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            if (GraphicsLoaded)
            {
                if(isFrozen)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Rpg.camera.GetTransformation(Rpg.graphics.GraphicsDevice));
                }
                DrawEntity(spriteBatch);
                if(isFrozen)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Rpg.camera.GetTransformation(Rpg.graphics.GraphicsDevice));
                }
            }
        }

        private void DrawEntity(SpriteBatch spriteBatch)
        {
            if (state != States.Standing)
            {
                animations[state].SetPosition(Position);
                animations[state].Draw(spriteBatch, 0.6f);
            }
            else
            {
                spriteBatch.Draw(Standing, Position, null, Color.White, MathHelper.ToRadians(Angle), Origin, 1f, SpriteEffects.None, 0.6f);
            }
            if(Hair != null)
            spriteBatch.Draw(Hair, Position, null, Color.White, MathHelper.ToRadians(Angle), hairOrigin, 1f, SpriteEffects.None, 0.600001f);
        }

        private void RegenHealth()
        {
            if (Health < maxHealth)
            {
                Health += healthRegen;
            }
            if (Health > maxHealth)
            {
                Health = maxHealth;
            }
        }

        private void RegenMana()
        {
            if (Mana < maxMana)
            {
                Mana += manaRegen;
            }
            if (Mana > maxMana)
            {
                Mana = maxMana;
            }
        }

        protected virtual void MoveToDestination(Vector2 target)
        {
            movementTargetPosition = target;
            isStanding = false;
            if (state != States.Walking)
            {
                state = States.Walking;
                animations[state].GhangeAnimatingState(true);
                animations[state].SetPosition(Position);
            }
            Angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, movementTargetPosition));
            Speed = target - Position;
            Speed.Normalize();
            Speed *= moveSpeed;
            animations[state].Update(Position, MathHelper.ToRadians(Angle));
        }

        protected float DistanceToTarget()
        {
            return Vector2.Distance(Position, movementTargetPosition);
        }

        public virtual void StopWalking()
        {
            Speed = new Vector2();
        }

        public void ShootProjectile(int projId, int projAi, Vector2 target, float speed,float dmg, bool type = true)
        {
            Rpg.Projectiles.Add(new Projectile(projId, projAi, target, Position, speed,dmg, type));
        }
    }
}
