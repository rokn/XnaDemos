using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public struct SpellStat
    {
        public string Name, toolTip;
        public string asset;
        public int cooldown;
        public int baseDamage;
        public int manaCost;
        public float projSpeed;
    }
    public class Spell
    {
        SpellStat stats;
        int Id;
        private bool isOnCooldown;
        private int coolDown;
        private Texture2D texture,tooltipTexture;
        public bool GraphicsLoaded;
        private bool isSelected;
        private Rectangle rect;
        public Vector2 Position,Origin;
        private float depth;
        private List<string> Tooltip;
        private float cooldownEffectRate;
        private float cooldownEffectPosition;
        Effect effect;

        public Spell(int id)
        {
            Id = id;
            stats = Rpg.spellsStats[id];
            GraphicsLoaded = false;
            isSelected = false;
            depth = 0.989f;
            Tooltip = new List<string>();
            GenerateTooltip();
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
            cooldownEffectRate = (float)1 / stats.cooldown;
            cooldownEffectPosition = 0f;
        }

        public void Load(ContentManager Content,Effect eff)
        {
            if (!GraphicsLoaded)
            {
                texture = Scripts.LoadTexture(stats.asset, Content);
                Origin = new Vector2(texture.Width / 2, texture.Height / 2);
                effect = eff;
                rect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
                GraphicsLoaded = true;
            }
        }

        public void Update()
        {
            if (isOnCooldown)
            {
                if (coolDown > 0)
                {
                    coolDown--;
                    cooldownEffectPosition += cooldownEffectRate;
                }
                else
                {
                    coolDown = 0;
                    isOnCooldown = false;
                    cooldownEffectPosition = 0;
                }
            }
            if (!isSelected)
            {
                if (Scripts.CheckIfMouseIsOver(rect))
                {
                    isSelected = true;
                    Console.WriteLine(isSelected);
                }
            }
            else
            {
                if (!Scripts.CheckIfMouseIsOver(rect))
                {
                    isSelected = false;
                    Console.WriteLine(isSelected);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (GraphicsLoaded)
            {
                if(isOnCooldown)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Rpg.camera.GetTransformation(Rpg.graphics.GraphicsDevice));
                    effect.CurrentTechnique.Passes[0].Apply();
                    effect.Parameters["param"].SetValue(cooldownEffectPosition);
                    spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, depth);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, Rpg.camera.GetTransformation(Rpg.graphics.GraphicsDevice));
                }
                else
                {
                    spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, depth); 
                }
                if(isSelected)
                {
                    Scripts.DrawTooltip(Tooltip, tooltipTexture, 0.8f, spriteBatch);
                }
            }
        }

        public void Activate(Player player)
        {
            if (!isOnCooldown)
            {
                if (player.Mana >= stats.manaCost)
                {
                    switch (Id)
                    {
                        case 0:
                            Fireball(player);
                            break;
                        case 1:
                            ArcaneMissile(player);
                            break;
                        case 2:
                            Lightnings.ShootLightning(player.Position, Rpg.mouse.Position, Color.Red);
                            player.TurnTowardsMouse();
                            break;
                        case 3:
                            IceStrike(player);
                            break;
                        case 4:
                            Teleport(player);
                            break;
                    }
                    isOnCooldown = true;
                    coolDown = stats.cooldown;
                    cooldownEffectPosition = 0;
                    player.Mana -= stats.manaCost;
                }
            }

        }

        private void ArcaneMissile(Player player)
        {
            player.ShootProjectile(1, 1, Rpg.mouse.Position, 12,stats.baseDamage, true);
            player.ShootProjectile(1, 1, Rpg.mouse.Position, 12, stats.baseDamage, false);
            player.ShootProjectile(1, 0, Rpg.mouse.Position, 11.5f, stats.baseDamage);
            player.TurnTowardsMouse();
        }

        private void Fireball(Player player)
        {
            player.ShootProjectile(0, 0, Rpg.mouse.Position, 10, stats.baseDamage);
            player.TurnTowardsMouse();

        }

        private void IceStrike(Player player)
        {
            player.ShootProjectile(2, 0, Rpg.mouse.Position, 9, stats.baseDamage);
            player.TurnTowardsMouse();

        }

        private void Teleport(Player player)
        {
            Vector2 direction = Rpg.mouse.Position-player.Position;
            direction.Normalize();
            direction *= 250;
            player.Position += direction;
            player.StopWalking();
            player.TurnTowardsMouse();
        }

        private void GenerateTooltip()
        {
            Tooltip.Add(stats.Name);
            Tooltip.Add("Damage: "+stats.baseDamage.ToString());
            Tooltip.Add("Cooldown: "+stats.cooldown.ToString());
            Tooltip.Add("Mana Cost: " + stats.manaCost.ToString());
        }
    }
}
