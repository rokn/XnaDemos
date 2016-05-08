using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public class Spell
    {
        private string Name;
        private int id;
        private float Damage;
        private float Cooldown;
        string description,asset;
        Texture2D tooltipTexture, texture;
        private List<string> Tooltip;
        private bool selected;
        private Rectangle rect;
        float toolTipAlpha = 0.8f;
        public Spell(int Id)
        {
            id = Id;
            Name = Rpg.spellsStats[id].Name;
            Damage = Rpg.spellsStats[id].Damage;
            Cooldown = Rpg.spellsStats[id].Cooldown;
            description = Rpg.spellsStats[id].Description;
            asset = Rpg.spellsStats[id].Asset;
            Tooltip = new List<string>();
            Tooltip.Add(Name);
            Tooltip.Add("Damage: "+ Damage.ToString());
            Tooltip.Add("Cooldown: "+ Cooldown.ToString());
            Tooltip.Add(description);
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
            selected = false;
        }
        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(asset);
            rect = new Rectangle(0, 0, texture.Width, texture.Height);
        }
        public void Update()
        {
            if(!selected)
            {
                if(rect.Contains(Rpg.mouse.clickRectangle))
                {
                    selected = true;
                    Console.WriteLine(selected);
                }
            }
            else
            {
                if (!rect.Contains(Rpg.mouse.clickRectangle))
                {
                    selected = false;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch,Vector2 Position)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0.65f); 
            rect.X = (int)Position.X - texture.Width / 2;
            rect.Y = (int)Position.Y - texture.Height / 2;
            if(selected)
            {
                DrawTooltip(spriteBatch);
            }
        }
        public void DrawTooltip(SpriteBatch spriteBatch)
        {
            if (Tooltip.Count > 0)
            {
                if (tooltipTexture != null)
                {
                    int i = 0;
                    spriteBatch.Draw(tooltipTexture, Rpg.mouse.Position, null, Color.White * toolTipAlpha, 0, new Vector2(), 1, SpriteEffects.None, 0.9f); // the back of the tooltip
                    foreach (string s in Tooltip)
                    {
                        spriteBatch.DrawString(Rpg.Font, s, Rpg.mouse.Position + new Vector2(9, i * 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f); // Thte text of the tooltip
                        i++;
                    }
                }
            }
        }
    }
}
