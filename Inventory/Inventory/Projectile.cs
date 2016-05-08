using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg
{
    class Projectile
    {
        Vector2 Position,Target;
        Texture2D Texture;
        public Rectangle rect;
        public float speed;
        public float angle;
        public int damage;
        public string Name;
        public Projectile(Vector2 position,Vector2 target,int id)
        {

        }
    }
}
