using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StealthOrNot
{
    public enum ProjectileType
    {
        GunShot,
        CrossbowShot
    }

    public class Projectile : MoveableObject
    {
        private Texture2D texture;
        private ProjectileType type;
        private float rotation;
        private Player Owner;
        private FalseLight light;
        private Vector2 direction;
        private float Damage;

        public Projectile(Vector2 pos, float Rotation, ProjectileType projType, Player owner, float speed, float damage)
            : base(pos)
        {
            type = projType;
            Load();
            Speed = speed;
            rotation = Rotation;

            if (type == ProjectileType.GunShot)
            {
                light.Rotation = rotation;
            }

            direction = MathAid.AngleToVector(rotation + MathHelper.ToRadians(90));
            horizontalSpeed = direction.X * Speed;
            verticalSpeed = direction.Y * Speed;
            rectOffset = MathAid.ParentChildTransform(Position, new Vector2(0, -texture.Height / 2), rotation) - Position;
            rect = new Rectangle((int)(pos.X - rectOffset.X), (int)(pos.Y - rectOffset.Y), 3, 3);
            Owner = owner;
            Damage = damage;
        }

        public void Load()
        {
            texture = Scripts.LoadTexture(@"Projectiles\Projectile_" + (int)type);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            if (type == ProjectileType.GunShot)
            {
                Texture2D lightTexture = Scripts.LoadTexture(@"Player\BulletLight");
                light = new FalseLight(Position, Color.Yellow, new Vector2(lightTexture.Width / 2, lightTexture.Height / 2));
                light.SetTexture(lightTexture);
            }
        }

        public void Update()
        {
            UpdateMovement();
            rect = MathAid.UpdateRectViaVector(rect, Position - rectOffset);

            if (type == ProjectileType.GunShot)
            {
                light.ChangePosition(Position);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0.4f);

            if (Main.showBoundingBoxes)
            {
                spriteBatch.Draw(Main.BoundingBox, rect, rect, Color.Black * 0.8f, 0f, new Vector2(), SpriteEffects.None, 0.9f);
            }
        }

        protected override bool IsCollidingWithBlocks(Rectangle rectangle)
        {
            bool collided = base.IsCollidingWithBlocks(rectangle);

            if (Main.mainPlayers.Contains(Owner))
            {
                foreach (Player enemyPlayer in Main.enemyPlayers)
                {
                    if (rectangle.Intersects(enemyPlayer.headRect))
                    {
                        enemyPlayer.TakeDamage(Damage * 2);
                        this.Destroy();
                        return true;
                    }
                    else if (rectangle.Intersects(enemyPlayer.bodyRect))
                    {
                        enemyPlayer.TakeDamage(Damage);
                        this.Destroy();
                        return true;
                    }
                    else if (rectangle.Intersects(enemyPlayer.legsRect))
                    {
                        enemyPlayer.TakeDamage(Damage * 0.5f);
                        this.Destroy();
                        return true;
                    }
                }
            }
            else
            {
                foreach (Player player in Main.mainPlayers)
                {
                    if (rectangle.Intersects(player.headRect))
                    {
                        player.TakeDamage(Damage * 2);
                        this.Destroy();
                        return true;
                    }
                    else if (rectangle.Intersects(player.bodyRect))
                    {
                        player.TakeDamage(Damage);
                        this.Destroy();
                        return true;
                    }
                    else if (rectangle.Intersects(player.legsRect))
                    {
                        player.TakeDamage(Damage * 0.5f);
                        this.Destroy();
                        return true;
                    }
                }
            }

            if (type == ProjectileType.CrossbowShot)
            {
                foreach (Lamp lamp in Main.Lamps)
                {
                    if (rectangle.Intersects(lamp.rect))
                    {
                        lamp.Remove();
                        this.Destroy();
                        return true;
                    }
                }
            }

            if (collided)
            {
                this.Destroy();
            }

            return collided;
        }

        protected override bool IsWithinBoundary(Rectangle rectangle)
        {
            if (Position.X < -texture.Width || Position.X > Main.tilemap.Width * TileSet.TileWidth || Position.Y < -texture.Height || Position.Y > Main.tilemap.Height * TileSet.TileHeight)
            {
                this.Destroy();
            }
            return true;
        }

        private void Destroy()
        {
            Owner.projectiles.Remove(this);

            if (type == ProjectileType.GunShot)
            {
                Main.FalseLights.Remove(light);
            }
        }
    }
}