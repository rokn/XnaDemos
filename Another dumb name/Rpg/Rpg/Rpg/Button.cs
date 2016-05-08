using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rpg
{

    enum ButtonType
    {
        Menu,
        Class
    }

    enum ButtonName
    {
        Back,
        Create,
        Delete,
        WizardClass,
        Play,
        Quit,
        RangerClass,
        WarriorClass
    }

    class Button
    {
        Vector2 position;
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
            string topAsset = folderName + Name.ToString();

            texture = Scripts.LoadTexture(textureAsset,Content);
            selectedTexture = Scripts.LoadTexture(selectedAsset, Content);
            pressedTexture = Scripts.LoadTexture(pressedAsset, Content);
            topTexture = Scripts.LoadTexture(topAsset, Content);
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
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
                    if (isHeld && Rpg.mouse.LeftReleased())
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
                    if(Rpg.mouse.LeftClick())
                    {
                        isHeld = true;
                    }

                    if(isHeld&&Rpg.mouse.LeftReleased())
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

            spriteBatch.Draw(topTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.981f);
        }

        public virtual void PressButton()
        {
            switch (Name)
            {
                case ButtonName.Play:
                    if (Rpg.menu == Menu.Main)
                    {
                        if(!CheckForDirectoryAndFile())
                        {
                            Rpg.menu = Menu.CharacterCreate;
                            Rpg.textBox = new TextBox(new Vector2(600, 900), 15, "Enter name here");
                        }
                        else
                        {
                            StreamReader file = new StreamReader(Rpg.saveFilePath+"Characters.txt");
                            if (file.EndOfStream)
                            {
                                Rpg.menu = Menu.CharacterCreate;
                                Rpg.textBox = new TextBox(new Vector2(600, 900), 15, "Enter name here");
                                file.Close();
                            }
                            else
                            {
                                file.Close();
                                SwitchToCharacterSelect();
                            }
                        }
                    }
                    else if(Rpg.menu == Menu.CharacterSelect)
                    {
                        InitializeGame();
                        Rpg.menu = Menu.Main;
                        Rpg.inGame = true;
                    }
                    break;

                case ButtonName.Quit:
                    Rpg.exit = true;
                    break;

                case ButtonName.WarriorClass: Rpg.chosenClass = Classes.Warrior; break;
                case ButtonName.RangerClass: Rpg.chosenClass = Classes.Ranger; break;
                case ButtonName.WizardClass: Rpg.chosenClass = Classes.Wizard; break;
                case ButtonName.Create:
                    if (Rpg.menu == Menu.CharacterCreate)
                    {
                        if (Rpg.textBox.text.Length >= 3)
                        {
                            Rpg.CreateCharacter();
                            SwitchToCharacterSelect();
                        }
                    }
                    else
                    {
                        if(Rpg.menu == Menu.CharacterSelect)
                        {
                            Rpg.menu = Menu.CharacterCreate;
                            Rpg.textBox = new TextBox(new Vector2(600, 900), 15, "Enter name here");
                        }
                    }
                    break;

                case ButtonName.Back:
                    GoBack();
                    break;

                case ButtonName.Delete:
                    DeleteCharacter();
                    SwitchToCharacterSelect();
                    break;
            }
        }

        private void InitializeGame()
        {
            Rpg.player = Rpg.Characters[Rpg.chosenCharacter];
            StreamReader sr = new StreamReader(Rpg.saveFilePath + Rpg.player.name + ".txt");
            sr.ReadLine(); // read the first line which is the player class
            Rpg.player.Position.X = float.Parse(Scripts.DecryptData(sr.ReadLine()));
            Rpg.player.Position.Y = float.Parse(Scripts.DecryptData(sr.ReadLine()));
            for (int i = 0; i < Rpg.inventory.cells.Length; i++)
            {
                int Id = int.Parse(Scripts.DecryptData(sr.ReadLine()));
                int stack = int.Parse(Scripts.DecryptData(sr.ReadLine()));
                if (Id > 0)
                {
                    Rpg.inventory.AddItem(new Item(Id, new Vector2(),stack));
                }
            }
            sr.Close();
        }

        private bool CheckForDirectoryAndFile()
        {
            if (!Directory.Exists(Rpg.saveFilePath))
            {
                Directory.CreateDirectory(Rpg.saveFilePath);
            }
            if (!File.Exists(Rpg.saveFilePath+"Characters.txt"))
            {
                StreamWriter sw = File.CreateText(Rpg.saveFilePath+"Characters.txt");
                sw.Close();
                return false;
            }
            return true;
        }

        private void SwitchToCharacterSelect()
        {
            Rpg.charactersButtonsRects.Clear();
            Rpg.Characters.Clear();
            Rpg.menu = Menu.CharacterSelect;
            if (Rpg.characterButtonTexture == null)
            {
                Rpg.characterButtonTexture = Scripts.RoundRectangle(15 * 15, 50, 4, Color.GreenYellow, Color.ForestGreen);
                Rpg.characterButtonTextureGlow = Scripts.RoundRectangle(15 * 15 + 6, 50 + 6, 4, Color.Yellow, Color.Yellow);
            }
            List<string> Characters = new List<string>();
            StreamReader sr = new StreamReader(Rpg.saveFilePath+"Characters.txt");
            while (!sr.EndOfStream)
            {
                Characters.Add(sr.ReadLine());
            }
            sr.Close();
            int x = 150;
            int y = 150;
            foreach (string name in Characters)
            {
                Player p = new Player(new Vector2(),Rpg.effect.Clone());
                sr = new StreamReader(@"C:\Rpg\" + name + ".txt");
                p.currClass = (Classes)(byte.Parse(Scripts.DecryptData(sr.ReadLine())));
                p.name = name;
                Rpg.Characters.Add(p);
                sr.Close();
                Rpg.charactersButtonsRects.Add(new Rectangle(x, y, 15 * 15, 50));
                y += 100;
            }
        }

        private void GoBack()
        {
            switch(Rpg.menu)
            {
                case Menu.CharacterCreate:
                    SwitchToCharacterSelect();
                    break;
                case Menu.CharacterSelect:
                    Rpg.menu = Menu.Main;
                    break;
            }
        }

        private void DeleteCharacter()
        {
            List<string> lines = new List<string>();
            StreamReader str = new StreamReader(Rpg.saveFilePath+"Characters.txt");
            while (!str.EndOfStream)
            {
                string line = str.ReadLine();
                if (line != Rpg.Characters[Rpg.chosenCharacter].name)
                {
                    lines.Add(line);
                }
            }
            str.Close();
            StreamWriter stw = new StreamWriter(Rpg.saveFilePath+"Characters.txt");
            foreach (string s in lines)
            {
                stw.WriteLine(s);
            }
            stw.Close();
            File.Delete(Rpg.saveFilePath + Rpg.Characters[Rpg.chosenCharacter].name + ".txt");
        }
    }
}
