using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Wizards
{

    enum ButtonType
    {
        Menu
    }

    enum ButtonName
    {
        Play,
        Quit,
        Create,
        Join
    }

    class Button
    {
        Vector2 position,textPosition;
        Texture2D texture, selectedTexture, pressedTexture,topTexture;
        public bool isHeld, isSelected;
        ButtonType Type;
        public ButtonName Name;
        Rectangle rect;

        public Button(Vector2 Position,ButtonType type,ButtonName name)
        {
            position = Position;
            isHeld = false;
            isSelected = false;
            Type = type;
            Name = name;
        }

        public void Load(ContentManager Content)
        {
            string folderName = @"Buttons\";
            string textureAsset = folderName + Type.ToString() + "Button";
            string selectedAsset = folderName + "Selected" + Type.ToString() + "Button";
            string pressedAsset = folderName + "Pressed" + Type.ToString() + "Button";

            texture = Scripts.LoadTexture(textureAsset,Content);
            selectedTexture = Scripts.LoadTexture(selectedAsset, Content);
            pressedTexture = Scripts.LoadTexture(pressedAsset, Content);
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            string name = Name.ToString();
            Vector2 nameSize = Game.ButtonFont.MeasureString(name);
            textPosition = position + new Vector2(texture.Width / 2 - nameSize.X / 2, texture.Height / 2 - nameSize.Y / 2);
        }

        public void Update()
        {
            if(!isSelected)
            {
                if(Scripts.CheckIfMouseIsOver(rect))
                {
                    isSelected = true;
                }
                else
                {
                    if (isHeld && Game.mouse.LeftReleased())
                    {
                        isHeld = false; //Release if mouse is released and was not over
                    }
                }
            }
            else
            {
                if (!Scripts.CheckIfMouseIsOver(rect))
                {
                    isSelected = false;
                }
                else
                {
                    if (Game.mouse.LeftClick())
                    {
                        isHeld = true;
                    }

                    if (isHeld && Game.mouse.LeftReleased())
                    {
                        PressButton();
                        isHeld = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isHeld)
            {
                spriteBatch.Draw(pressedTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.98f);
            }
            else if(isSelected)
            {
                spriteBatch.Draw(selectedTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.98f);
            }
            else
            {
                spriteBatch.Draw(texture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.98f);
            }

            spriteBatch.DrawString(Game.ButtonFont, Name.ToString(), textPosition, Color.Black);
        }

        public virtual void PressButton()
        {
            switch(Name)
            {
                case ButtonName.Play:
                    Game.currMenu = Menu.Server;
                    break;
                case ButtonName.Quit:
                    Game.exit = true;
                    break;
                case ButtonName.Join:
                    Game.JoinToServer();
                    break;
            }
        }
    }
}
