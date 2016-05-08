using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TopScrollingGame.Creatures.Enemies;

namespace TopScrollingGame
{
    public enum ProjectileType
    {
        Cat,
        Axe,
        Poison,
        Scythe
    }

    public class Projectile
    {
        public Projectile(Vector2 startPos, Vector2 startDir, float speed, ProjectileType projType, Creature owner, float damage, float angleVelocity, EffectType debuff, float rotation = 0, bool piercing = false)
        {
            this.ProjectileEffect = debuff;
            this.Type = projType;
            this.Position = startPos;
            this.Direction = startDir;
            this.Speed = speed;
            this.Rotation = rotation;
            Load();
            this.Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            this.Rect = Scripts.InitRectangle(Position - Origin, Texture);
            this.Owner = owner;
            this.Piercing = piercing;
            this.HitEnemies = new List<Creature>();
            this.Damage = damage;
            AngleVelocity = MathHelper.ToRadians(angleVelocity);
            TrailTicks = 3;
        }

        public EffectType ProjectileEffect { get; set; }

        private int TrailTicks { get; set; }

        public float AngleVelocity { get; set; }

        public float Damage { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Direction { get; set; }

        public float Speed { get; set; }

        public ProjectileType Type { get; set; }

        public Texture2D Texture { get; set; }

        public Vector2 Origin { get; set; }

        public Creature Owner { get; set; }

        public bool Piercing { get; set; }

        private List<Creature> HitEnemies { get; set; }

        public Rectangle Rect { get; set; }

        public void Load()
        {
            string asset = @"Sprites\";

            switch (Type)
            {
                case ProjectileType.Cat:
                    asset += @"Player\CatProjectile";
                    break;

                case ProjectileType.Axe:
                    asset += @"Enemies\Axe";
                    break;

                case ProjectileType.Poison:
                    asset += @"Enemies\Poison";
                    break;

                case ProjectileType.Scythe:
                    asset += @"Enemies\Scythe";
                    break;

                default:
                    break;
            }

            Texture = Scripts.LoadTexture(asset);
        }

        public void Update(GameTime gameTime)
        {
            Position += Direction * Speed;
            Rect = MathAid.UpdateRectViaVector(Rect, Position - Origin);
            Rotation += AngleVelocity;
            HandleCollision();

            if (Type == ProjectileType.Cat)
            {
                if ((--TrailTicks) <= 0)
                {
                    TrailTicks = 3;
                    Main.ParticleEngine.GenerateTrailEffect(Position, Texture, Rotation, 10, Color.White);
                }
            }
        }

        public bool AreTheyDifferenTypes(Creature creatureOne, Creature creatureTwo)
        {
            if (creatureOne is Enemy && creatureTwo is Hero || creatureOne is Hero && creatureTwo is Enemy)
            {
                return true;
            }

            return false;
        }

        private void HandleCollision()
        {
            List<Creature> colidedCreatures = Scripts.CheckForCollisionWithEnemies(Rect);
            bool hitCreature = false;

            if (colidedCreatures.Count > 0)
            {
                foreach (var creature in colidedCreatures)
                {
                    if (AreTheyDifferenTypes(Owner, creature) && !HitEnemies.Contains(creature))
                    {
                        creature.TakeDamage(Damage);
                        HitEnemies.Add(creature);
                        hitCreature = true;
                        if (ProjectileEffect != EffectType.None && AreTheyDifferenTypes(Owner, creature))
                        {
                            new DotEffect(ProjectileEffect, creature);
                        }
                    }
                }
            }

            if (!IsWithinBounds() || (hitCreature && !Piercing))
            {
                (Owner as IShooting).Projectiles.Remove(this);

                if (Main.temporaryProjectiles.Contains(this))
                {
                    Main.temporaryProjectiles.Remove(this);
                }
            }
        }

        private bool IsWithinBounds()
        {
            if (Position.X - Origin.X >= Main.playingAreaX + Main.playingAreaWidth)
            {
                return false;
            }

            if (Position.X + Origin.X <= Main.playingAreaX)
            {
                return false;
            }

            if (Position.Y + Origin.Y <= 0)
            {
                return false;
            }

            if (Position.Y - Origin.Y >= GUI.castlePosition.Y)
            {
                return false;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1, SpriteEffects.None, 0.61f);
        }
    }
}