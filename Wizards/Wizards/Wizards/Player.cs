using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
namespace Wizards
{
    class Player
    {
        public Vector2 Position;
        public NetConnection Connection;
        public string Name;
        public Player(string name,Vector2 pos,NetConnection connection)
        {
            Name = name;
            Position = pos;
            Connection = connection;
        }
    }
}
