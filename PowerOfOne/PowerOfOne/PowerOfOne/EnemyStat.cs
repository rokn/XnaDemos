using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerOfOne
{
    public struct EnemyStat
    {
        public int MaxHealth { get; set; }
        public int MoveSpeed { get; set; }
        public int SightDistance { get; set; }
        public Type Ability { get; set; }
        public bool Aggresive { get; set; }
    }
}
