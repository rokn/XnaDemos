using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rpg
{
    public enum TileType
    {
        Stash
    }
    public class Tile
    {
        Texture2D texture;
        Vector2 Position;
        TileType Type;
        Rectangle rect;
        public Tile(TileType type, Vector2 pos)
        {
            Type = type;
            Position = pos;
            
        }
        public void Load(ContentManager Content)
        {
            texture = Scripts.LoadTexture(@"Tiles\" + Type.ToString(),Content);
            rect = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }
        public void Update()
        {
            if (Scripts.CheckIfMouseIsOver(rect))
            {
                if (Rpg.mouse.LeftClick())
                {
                    OnClick();
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.49f);
        }
        void OnClick()
        {
            switch (Type)
            {
                case TileType.Stash: OpenStash(); break;
            }
        }

        private void OpenStash()
        {
            Rpg.stash.opened = !Rpg.stash.opened;
            Rpg.inventory.opened = true;
        }
    }
}
