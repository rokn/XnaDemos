using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    public class Entity
    {
        public Vector2 Position, Target;
        protected Texture2D standing;
        protected float angle;
        public int health, maxhealth;
        protected float moveSpeed;
        protected bool Walking;
        public Entity(Vector2 position)
        {
            angle = 0f;
            Position = position;
            Walking = false;
            moveSpeed = 4;
            Target = new Vector2();
        }
        public virtual void Load(ContentManager Content)
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
