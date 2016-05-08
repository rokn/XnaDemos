using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Rpg
{
    public class Enemy : Entity
    {
        public int Id;
        Rectangle rect;
        HealthBar healthBar;
        
        public Enemy(int id,Vector2 Position,Effect eff) : base(Position,eff)
        {
            Id = id;
            Health = 100;
            maxHealth = 100;
        }

        public override void Load(ContentManager Content)
        {
            if (!GraphicsLoaded)
            {
                Standing = Scripts.LoadTexture(@"Enemies\Enemy_" + Id.ToString(), Content);
                rect = new Rectangle((int)Position.X, (int)Position.Y, Standing.Width, Standing.Height);
                healthBar = new HealthBar(80, Position - new Vector2(Standing.Width/2+10,Standing.Height/2+10),Color.Green,Color.Red);
                healthBar.Load(Content, 1);
                GraphicsLoaded = true;
            }
        }

        public override void Update()
        {
            healthBar.Update(Health, maxHealth);
            try
            {
                foreach (Projectile proj in Rpg.Projectiles)
                {
                    if (rect.Intersects(proj.rect))
                    {
                        Health -= proj.baseDamage;
                        Rpg.Projectiles.Remove(proj);
                        if(proj.stats.isFrosty)
                        {
                            isFrozen = true;
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            healthBar.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
}
