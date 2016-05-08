using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rpg
{
    class Button
    {
        public Vector2 position;
        public Texture2D texture, selectedTexture, pressedTexture,topTexture;
        public bool isHeld, isSelected;
        public int id;
        Rectangle rect;
        public Button(Vector2 Position,Texture2D Texture,Texture2D SelectedTexture,Texture2D PressedTexture,Texture2D TopTexture,int Id)
        {
            texture = Texture;
            selectedTexture = SelectedTexture;
            pressedTexture = PressedTexture;
            id = Id;
            position = Position;
            rect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            isHeld = false;
            isSelected = false;
            topTexture = TopTexture;
        }
        public void Update()
        {
            if(!isSelected)
            {
                if(rect.Contains(Rpg.mouse.clickRectangle))
                {
                    isSelected = true;
                }
                else
                {
                    if (isHeld && Rpg.mouse.LeftReleased())
                    {
                        isHeld = false;
                    }
                }
            }
            else
            {
                if (!rect.Contains(Rpg.mouse.clickRectangle))
                {
                    isSelected = false;
                }
                else
                {
                    if(Rpg.mouse.LeftClicked())
                    {
                        isHeld = true;
                    }
                    if(isHeld&&Rpg.mouse.LeftReleased())
                    {
                        OnPress();
                        isHeld = false;
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isHeld)
            {
                spriteBatch.Draw(pressedTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.88f);
            }
            else if(isSelected)
            {
                spriteBatch.Draw(selectedTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.88f);
            }
            else
            {
                spriteBatch.Draw(texture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.88f);
            }
            spriteBatch.Draw(topTexture, rect, null, Color.White, 0, new Vector2(), SpriteEffects.None, 0.882f);
        }
        public virtual void OnPress()
        {
            switch (id)
            {
                case 1:
                    Rpg.atMainMenu = false;
                    if (!Directory.Exists(@"C:\Rpg"))
                    {
                        Directory.CreateDirectory(@"C:\Rpg");
                    }
                    if (!File.Exists(@"C:\Rpg\Characters.txt"))
                    {
                        StreamWriter sw = File.CreateText(@"C:\Rpg\Characters.txt");
                        sw.Close();
                        Rpg.atCreateCharacter = true;
                        Rpg.textBox = new TextBox(new Vector2(600, 900), Rpg.textbox, 15);
                    }
                    else
                    {
                        StreamReader file = new StreamReader(@"C:\Rpg\Characters.txt");
                        if (file.EndOfStream)
                        {
                            Rpg.atCreateCharacter = true;
                            file.Close();
                        }
                        else
                        {
                            SwitchToCharacterSelect();
                            file.Close();
                        }
                    }
                    break;
                case 2:
                    Rpg.exit = true;
                    break;
                case 3:
                case 4:
                case 5:
                    Rpg.chosenClass = (Classes)id - 3;
                    break;
                case 6:
                    if (Rpg.textBox.text.Length >= 3)
                    {
                        Rpg.atCreateCharacter = false;
                        Rpg.CreateCharacter();
                        SwitchToCharacterSelect();
                    }
                    break;
                case 7: Rpg.atCreateCharacter = true;
                    break;
                case 8: Rpg.atCreateCharacter = false;
                    Rpg.textBox.text = "";
                    SwitchToCharacterSelect();
                    break;
                case 9: Rpg.atCharacherSelect = false;
                    Rpg.atMainMenu = true;
                    break;
                case 10:
                    List<string> lines = new List<string>();
                    StreamReader str = new StreamReader(@"C:\Rpg\Characters.txt");
                    while(!str.EndOfStream)
                    {
                        string line = str.ReadLine();
                        if (line != Rpg.Characters[Rpg.chosenCharacter].name)
                        {
                            lines.Add(line);
                        }
                    }
                    str.Close();
                    StreamWriter stw = new StreamWriter(@"C:\Rpg\Characters.txt");
                    foreach(string s in lines)
                    {
                        stw.WriteLine(s);
                    }
                    stw.Close();
                    File.Delete(@"C:\Rpg\" + Rpg.Characters[Rpg.chosenCharacter].name + ".txt");
                    SwitchToCharacterSelect();
                    break;
                case 11:
                    Rpg.player = Rpg.Characters[Rpg.chosenCharacter];
                    StreamReader sr = new StreamReader(@"C:\Rpg\" + Rpg.player.name + ".txt");
                    sr.ReadLine();
                    Rpg.player.Position.X = float.Parse(Scripts.DecryptData(sr.ReadLine()));
                    Rpg.player.Position.Y = float.Parse(Scripts.DecryptData(sr.ReadLine()));
                    for (int i = 0; i < Rpg.inv.cells.Count * Rpg.inv.cells[0].Count; i++)
                    {
                        int Id = int.Parse(Scripts.DecryptData(sr.ReadLine()));
                        if (Id > 0)
                        {
                            Rpg.inv.AddItem(new Item(Id, new Vector2()));
                        }
                    }
                    sr.Close();
                    Rpg.atCharacherSelect = false;
                    break;
            }
        }
        void SwitchToCharacterSelect()
        {
            Rpg.charactersRectangles.Clear();
            Rpg.Characters.Clear();
            Rpg.atCharacherSelect = true;
            Rpg.characterChoseBack = Scripts.RoundRectangle(15 * 15, 50, 4, Color.GreenYellow, Color.ForestGreen);
            Rpg.highlightCharacterBack = Scripts.RoundRectangle(15 * 15+6, 50+6, 4, Color.Yellow, Color.Yellow);
            List<string> Characters = new List<string>();
            StreamReader sr = new StreamReader(@"C:\Rpg\Characters.txt");
            while (!sr.EndOfStream)
            {
                Characters.Add(sr.ReadLine());
            }
            sr.Close();
            int y = 150;
            foreach (string name in Characters)
            {
                Player p = new Player(new Vector2());
                sr = new StreamReader(@"C:\Rpg\" + name + ".txt");
                p.currClass = (Classes)(byte.Parse(Scripts.DecryptData(sr.ReadLine())));
                p.name = name;
                Rpg.Characters.Add(p);
                sr.Close();
                Rpg.charactersRectangles.Add(new Rectangle(150, y, 15 * 15, 50));
                y += 100;
            }
        }
    }
}
