using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace RPG
{
    public interface ISpellComponent
    {        
    }

    public interface ISpellSystem
    {
        bool Start();
        void Update(GameTime gameTime);
        void End();
    }

    public class Spell
    {
        Dictionary<ISpellComponent, Type> Components;

        
    }
}
