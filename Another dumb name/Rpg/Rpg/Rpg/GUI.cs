using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public class GUI : DrawableGameComponent
    {
        private int spellsBaseX,spellsBaseY;
        ContentManager Content;
        private static HealthBar healthBar,manaBar;
        Effect effect;

        public GUI(Game game) : base(game)
        {
            spellsBaseX = 700;
            spellsBaseY = 950;
            Content = new ContentManager(game.Services);
            Content.RootDirectory = "Content";
            healthBar = new HealthBar(150, new Vector2(1200, 50), Color.Green, Color.Red);
            manaBar = new HealthBar(150, new Vector2(1450, 50), Color.Blue, Color.Purple);
        }

        protected override void LoadContent()
        {
            healthBar.Load(Content,0);
            manaBar.Load(Content,0);
            effect = Content.Load<Effect>(@"Spells\Effect");
        }

        public override void Update(GameTime gameTime)
        {
            if (Rpg.inGame)
            {
                if (Rpg.player.spells.Count > 0)
                {
                    if (!Rpg.player.spells[0].GraphicsLoaded)
                    {
                        LoadSpells();
                    }
                    UpdateSpells();
                }
                healthBar.Update(Rpg.player.Health, Rpg.player.maxHealth);
                manaBar.Update(Rpg.player.Mana, Rpg.player.maxMana);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Rpg.inGame)
            {
                healthBar.Draw(spriteBatch);
                manaBar.Draw(spriteBatch);
                DrawSpells(spriteBatch);
            }
        }

        private void UpdateSpells()
        {
            foreach (Spell spell in Rpg.player.spells)
            {
                spell.Update();
            }
        }

        private static void DrawSpells(SpriteBatch spriteBatch)
        {
            if (Rpg.inGame)
            {
                foreach (Spell spell in Rpg.player.spells)
                {
                    spell.Draw(spriteBatch);
                }
            }
        }

        private void LoadSpells()
        {
            int spellX = spellsBaseX;
            int spellY = spellsBaseY;
            foreach(Spell spell in Rpg.player.spells)
            {
                spell.Position = new Vector2(spellX, spellY);
                spellX += 100;
                spell.Load(Content,effect.Clone());
            }
        }
    }
}
