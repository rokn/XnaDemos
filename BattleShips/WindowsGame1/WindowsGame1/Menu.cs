using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battleships
{
    class Menu
    {
        public List<Button> buttons;
        Texture2D background;
        Vector2 back_position;
        bool hasBack;
        public Menu()
        {
            buttons = new List<Button>();
        }
        public Menu(List<Button> Buttons)
        {
            buttons = Buttons;
        }
        public Menu(Texture2D Background,Vector2 Position)
        {
            background = Background;
            back_position = Position;
            hasBack = true;
        }
        public Menu(List<Button> Buttons, Texture2D Background, Vector2 Position)
        {
            buttons = Buttons;
            background = Background;
            back_position = Position;
            hasBack = true;
        }
        public int update()
        {
            int i = -1;
            foreach (Button butt in buttons)
            {
                i++;
                if (Game1.rectContains(butt.rect, Game1.mouse.Position))
                {
                    if(Game1.mouse.LeftClicked())
                    return i;
                }
            }
            return -1;
        }
        public void draw(SpriteBatch spriteBatch)
        {
            if (hasBack)
            {
                spriteBatch.Draw(background, back_position, Color.White);
            }
            foreach (Button butt in buttons)
            {
                butt.Draw(spriteBatch);
            }
        }
        public void Add(Vector2 Position,Texture2D Texture)
        {
            buttons.Add(new Button(Position, Texture));
        }
    }
}
