using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rpg
{
    public class Tile
    {
        Texture2D texture;
        Vector2 Position;
        int iD;
        Rectangle rect;
        public Tile(int Id, Vector2 pos)
        {
            iD = Id;
            texture = Rpg.tileTextures[iD];
            Position = pos;
            rect = new Rectangle((int)pos.X,(int)pos.Y,texture.Width,texture.Height);
        }
        public void Update()
        {
            if(rect.Contains(Rpg.mouse.clickRectangle))
            {
                if (Rpg.mouse.LeftClicked())
                {
                    OnClick();
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.50f); 
        }
        void OnClick()
        {
            switch(iD)
            {
                case 0: OpenStash(); break;
            }
        }

        private void OpenStash()
        {
            Rpg.stash.opened = !Rpg.stash.opened;
            Rpg.inv.opened = true;
        }
    }
}
