using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace TheRicoshield
{
    public class Player
    {
        private Vector2 position;
        private Texture2D player;
        private Texture2D shieldTexture;
        private float depth;
        private Rectangle rect;
        private float hitPartSize;

        public Player()
        {
            position = new Vector2(Game.width / 2 - 64, Game.height - 124);
            depth = 0.5f;
        }

        public Texture2D ShieldTexture
        {
            get
            {
                return shieldTexture;
            }
            private set
            {
                shieldTexture = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            private set
            {
                position = value;
            }
        }

        public Rectangle Rect
        {
            get
            {
                return rect;
            }
            private set
            {
                rect = value;
            }
        }

        public float HitPartSize
        {
            get
            {
                return hitPartSize;
            }
            private set
            {
                if(value<=0)
                {
                    throw new ArgumentOutOfRangeException("The hit part size can't be lower than 0");
                }
                hitPartSize = value;
            }
        }

        public void Load(ContentManager Content)
        {
            player = Scripts.LoadTexture(@"Player\Player",Content);
            shieldTexture = Scripts.LoadTexture(@"Player\WoodShield",Content);
            Rect = new Rectangle((int)position.X, (int)position.Y,shieldTexture.Width,shieldTexture.Height);
            hitPartSize = shieldTexture.Width / 20;
        }

        public void Update()
        {
            position.X = Game.mouse.Position.X-player.Width/2;
            rect = MathAid.UpdateRectViaVector(rect, position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shieldTexture,position,null,Color.White,0,new Vector2(),1f,SpriteEffects.None,depth);
            spriteBatch.Draw(player,position,null,Color.White,0,new Vector2(),1f,SpriteEffects.None,depth-0.00001f);
        }
    }
}
