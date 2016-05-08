using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public static class GUI
    {
        static Texture2D spellsCoolDownTexture;
        public static Animation spellsCoolDown;
        static Texture2D spellsBack;
        public static List<Spell> Spells = new List<Spell>();
        public static void Load(ContentManager Content)
        {
            Spells.Add(new Spell(0));
            foreach(Spell spell in Spells)
            {
                spell.Load(Content);
            }
            spellsCoolDownTexture = Content.Load<Texture2D>(@"GUI\SpellCD");
            spellsBack = Content.Load<Texture2D>(@"GUI\SpellBase");
            List<Rectangle> rects = new List<Rectangle>();
            for (int i = 0; i < spellsCoolDownTexture.Width/96-2; i++)
			{
                rects.Add(new Rectangle(i*96,0,96,96));
			}
            spellsCoolDown = new Animation(rects, spellsCoolDownTexture, 5,false);
        }
        public static void Update(GameTime gameTime)
        {
            spellsCoolDown.Update(new Vector2(700,950),0);
            foreach(Spell spell in Spells)
            {
                spell.Update();
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(spellsBack, new Vector2(700+i*100, 950), null, Color.White, 0, new Vector2(spellsBack.Width / 2, spellsBack.Height / 2), 1f, SpriteEffects.None, 0.69f);
            }
            spellsCoolDown.Draw(spriteBatch,0.68f);
            int x=700;
            foreach(Spell spell in Spells)
            {
                spell.Draw(spriteBatch, new Vector2(x, 950));
                x += 100;
            }
        }
    }
}
