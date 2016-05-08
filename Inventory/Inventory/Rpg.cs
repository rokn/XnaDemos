#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endregion

namespace Rpg
{
    public enum Classes
    {
        Warrior,
        Ranger,
        Mage
    }
    public struct ItemStat
    {
        public string Name, Tooltip;
        public bool OnGround, Consumable, Stackable, IsWeapon, IsMelee, IsRanged, IsMagic, Equipable;
        public int? Damage, Stack, MaxStack;
        public int iD, equipType;
    }
    public struct SpellStat
    {
        public string Name, Description;
        public int Damage;
        public float Cooldown;
        public int iD;
        public string Asset { get; set; }
    }
    public struct ProjectileStat
    {
        public string Name;
        public int Damage;
        public int iD;
        public float Speed;
        public string Asset { get; set; }
    }
    public class Rpg : Microsoft.Xna.Framework.Game
    {
        #region Vars
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int itemCount = 5, tileCount = 1;
        public static KeysInput keyboard;
        public static MouseCursor mouse;
        public static int width, height;
        Texture2D cellTexture, selectTexture, mouseTexture;
        public static Inventory inv, stash;
        public static List<ItemStat> itemStats;
        public static List<SpellStat> spellsStats;
        public static SpriteFont Font;
        public static List<Texture2D> itemTextures;
        public static List<Texture2D> tileTextures;
        public static List<Item> items;
        public static List<Tile> tiles;
        public static Item mouseItem;
        public static bool hasItemAtMouse;
        public static List<Equipment> equipment;
        public static Texture2D partRedStar;
        ParticleSystem sys = new ParticleSystem();
        List<Button> mainMenu, characterCreate,characterChose;
        public static Player player;
        public static TextBox textBox;
        public static bool exit = false;
        public static bool atMainMenu = true, atCreateCharacter = false, atCharacherSelect = false;
        public static Texture2D textbox;
        public static Classes chosenClass;
        public static Texture2D characterChoseBack,highlightCharacterBack;
        public static List<Player> Characters;
        public static List<Rectangle> charactersRectangles;
        public static int chosenCharacter;
        public static List<Texture2D> ProjectilesTextures;
        public static List<ProjectileStat> ProjectileStats;
        #endregion
        #region Constructor
        public Rpg()
        {
            Components.Add(new FrameRateCounter(this));
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ScreenFix.Fix(this);
        }
        #endregion
        #region Initialize
        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 100);
            keyboard = new KeysInput();
            itemTextures = new List<Texture2D>();
            tileTextures = new List<Texture2D>();
            items = new List<Item>();
            itemStats = new List<ItemStat>();
            spellsStats = new List<SpellStat>();
            tiles = new List<Tile>();
            hasItemAtMouse = false;
            mouseItem = null;
            equipment = new List<Equipment>();
            mainMenu = new List<Button>();
            characterCreate = new List<Button>();
            chosenClass = Classes.Warrior;
            Characters = new List<Player>();
            charactersRectangles = new List<Rectangle>();
            characterChose = new List<Button>();
            chosenCharacter = 0;
            ProjectileStats = new List<ProjectileStat>();
            ProjectilesTextures = new List<Texture2D>();
            base.Initialize();
        }
        #endregion
        #region Load
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            cellTexture = Content.Load<Texture2D>(@"Inventory\CellTexture");
            selectTexture = Content.Load<Texture2D>(@"Inventory\SelectedCellTexture");
            textbox = Content.Load<Texture2D>("TextBox");
            Font = Content.Load<SpriteFont>("Font");
            for (int i = 0; i < itemCount; i++)
            {
                try
                {
                    itemTextures.Add(Content.Load<Texture2D>(@"Items\Item_" + i));
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
            for (int i = 0; i < tileCount; i++)
            {
                try
                {
                    tileTextures.Add(Content.Load<Texture2D>(@"Tiles\Tile_" + i));
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
            mouseTexture = Content.Load<Texture2D>("Mouse");
            inv = new Inventory(10, 6, cellTexture, selectTexture, new Vector2(20, 20), 5, 5);
            stash = new Inventory(5, 5, cellTexture, selectTexture, new Vector2(500, 500), 5, 5);
            tiles.Add(new Tile(0, new Vector2(1000, 800)));
            equipment.Add(new Equipment(Content.Load<Texture2D>(@"Inventory\Equipment_0"), new Vector2(800, 100), 1));
            equipment.Add(new Equipment(Content.Load<Texture2D>(@"Inventory\Equipment_1"), new Vector2(900, 100), 2));
            partRedStar = Content.Load<Texture2D>(@"Particles\RedStar");
            mainMenu.Add(CreateButton(new Vector2(800, 200), 1, "Menu", "Play", Content));
            mainMenu.Add(CreateButton(new Vector2(800, 600), 2, "Menu", "Quit", Content));
            characterCreate.Add(CreateButton(new Vector2(100, 100), 3, "Class", "WarriorClass", Content));
            characterCreate.Add(CreateButton(new Vector2(100, 300), 4, "Class", "RangerClass", Content));
            characterCreate.Add(CreateButton(new Vector2(100, 500), 5, "Class", "MageClass", Content));
            characterCreate.Add(CreateButton(new Vector2(1200, 910), 6, "Menu", "Create", Content));
            characterChose.Add(CreateButton(new Vector2(1200, 910), 7, "Menu", "Create", Content));
            characterCreate.Add(CreateButton(new Vector2(200, 910), 8, "Menu", "Back", Content));
            characterChose.Add(CreateButton(new Vector2(200, 910), 9, "Menu", "Back", Content));
            characterChose.Add(CreateButton(new Vector2(200, 810), 10, "Menu", "Delete", Content));
            characterChose.Add(CreateButton(new Vector2(825, 910), 11, "Menu", "Play", Content));
            //ProjectilesTextures.Add(Content.Load<Texture2D>("FireballAnimation"));
            SetUpItems();
            SetUpSpells();
            GUI.Load(Content);
        }
        #endregion
        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (keyboard.JustPressed(Keys.Escape))
                this.Exit();
            sys.Update(gameTime);
            if (this.IsActive)
            {
                keyboard.Update(gameTime);
                mouse.UpdateMouse(gameTime);
            }
            if (atMainMenu)
            {
                foreach (Button button in mainMenu)
                {
                    button.Update();
                }
            }
            else if (atCreateCharacter)
            {
                foreach (Button button in characterCreate)
                {
                    button.Update();
                }
                if (textBox != null)
                {
                    textBox.Update();
                }
                else
                {
                    textBox = new TextBox(new Vector2(600, 950), textbox, 15);
                }
                foreach (Button button in characterCreate)
                {
                    if (button.id == (byte)chosenClass + 3)
                    {
                        if (!button.isSelected)
                        {
                            button.isSelected = true;
                        }
                    }
                }
            }
            else if (atCharacherSelect)
            {
                int index = 0;
                foreach (Rectangle rect in charactersRectangles)
                {
                    if (rect.Contains(mouse.clickRectangle))
                    {
                        if (mouse.LeftClicked())
                        {
                            chosenCharacter = index;
                        }
                    }
                    index++;
                }
                foreach(Button b in characterChose)
                {
                    b.Update();
                }
            }
            else
            {
                if (!player.loaded) { player.Load(Content); }
                player.Update(gameTime);
                if (!inv.opened)
                {
                    if (Rpg.mouse.RightClick())
                    {
                        Random rand = new Random();
                        int numb = rand.Next(Rpg.itemTextures.Count);
                        Rpg.items.Add(new Item(numb, Rpg.mouse.Position, 1));
                    }
                }

                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    Vector2 subtracted = item.position - mouse.Position;
                    if (Math.Abs(subtracted.X) <= item.texture.Width / 2 && Math.Abs(subtracted.Y) <= item.texture.Height / 2)
                    {
                        if (!item.selected)
                        {
                            foreach (Item it in items)
                            {
                                if (it.selected)
                                {
                                    goto next;
                                }
                            }
                            item.selected = true;
                        }
                    next:
                        {
                            if (mouse.LeftClicked())
                            {
                                if (!inv.opened)
                                {
                                    player.CollectItem(item);
                                }
                                else
                                {
                                    if (!hasItemAtMouse)
                                    {
                                        player.CollectItemToMouse(item);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (item.selected)
                        {
                            item.selected = false;
                        }
                    }
                }
                foreach (Tile tile in tiles)
                {
                    tile.Update();
                }
                inv.Update();
                stash.Update();
                if (Rpg.keyboard.JustPressed(Keys.I))
                {
                    inv.opened = !inv.opened;
                    stash.opened = false;
                }
                if (mouse.LeftClicked())
                {
                    bool Over = false;
                    foreach (Equipment equip in equipment)
                    {
                        if (equip.rect.Contains(mouse.clickRectangle))
                        {
                            Over = true;
                        }
                    }
                    if (!Over)
                    {
                        if (stash.opened)
                        {
                            if (!inv.rect.Contains(mouse.clickRectangle) && !stash.rect.Contains(mouse.clickRectangle))
                            {
                                if (hasItemAtMouse)
                                {
                                    mouseItem.position = player.Position;
                                    mouseItem.onGround = true;
                                    items.Add(mouseItem);
                                    hasItemAtMouse = false;
                                    mouseItem = null;
                                }
                            }
                        }

                        else if (inv.opened)
                        {
                            if (!inv.rect.Contains(mouse.clickRectangle))
                            {
                                if (hasItemAtMouse)
                                {
                                    mouseItem.position = player.Position;
                                    mouseItem.onGround = true;
                                    items.Add(mouseItem);
                                    hasItemAtMouse = false;
                                    mouseItem = null;
                                }
                            }
                        }
                    }
                }
                GUI.Update(gameTime);
                if(mouse.LeftClicked())
                {
                    GUI.spellsCoolDown.GhangeAnimatingState(true);
                }
            }
            if (exit)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }
        #endregion
        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            if (atMainMenu)
            {
                foreach (Button button in mainMenu)
                {
                    button.Draw(spriteBatch);
                }
            }
            else if (atCreateCharacter)
            {
                foreach (Button button in characterCreate)
                {
                    button.Draw(spriteBatch);
                }
                if (textBox != null)
                {
                    spriteBatch.DrawString(Rpg.Font, "Name must be no longer than 15 characters", new Vector2(textBox.position.X, textBox.position.Y + 30), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
                    spriteBatch.DrawString(Rpg.Font, " and longer than 2 characters.", new Vector2(textBox.position.X, textBox.position.Y + 46), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
                    spriteBatch.DrawString(Rpg.Font, "Name:", new Vector2(textBox.position.X-65, textBox.position.Y +2), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.889f);
                    textBox.Draw(spriteBatch);
                }
            }
            else if (atCharacherSelect)
            {
                float y = 150;
                int index = 0;
                foreach (Player p in Characters)
                {
                    spriteBatch.Draw(characterChoseBack, new Vector2(150, y), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0.88f);
                    spriteBatch.DrawString(Rpg.Font, p.name, new Vector2(170, y + 4), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.881f);
                    spriteBatch.DrawString(Rpg.Font, p.currClass.ToString(), new Vector2(250, y + 22), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.881f);
                    if (index == chosenCharacter) { spriteBatch.Draw(highlightCharacterBack, new Vector2(148, y-3), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0.879f); }
                    y += characterChoseBack.Height + 50;
                    index++;
                }
                foreach (Button b in characterChose)
                {
                    b.Draw(spriteBatch);
                }
            }
            else
            {
                GUI.Draw(spriteBatch);
                if (hasItemAtMouse)
                {
                    spriteBatch.Draw(mouseItem.texture, mouse.Position, null, Color.White, 0, new Vector2(), 1f, SpriteEffects.None, 0.89f);
                    if (mouseItem.stackable)
                    {
                        spriteBatch.DrawString(Font, mouseItem.stack.ToString(), mouse.Position - new Vector2(11, 3), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f);
                    }
                }
                foreach (Tile tile in tiles)
                {
                    tile.Draw(spriteBatch);
                }
                inv.Draw(spriteBatch);
                stash.Draw(spriteBatch);
                int i = 0;
                foreach (Item item in items)
                {
                    item.depth = 0.2f + 0.00001f * i;
                    item.Draw(spriteBatch);
                    i++;
                }
                if (!player.loaded) { player.Load(Content); }
                player.Draw(spriteBatch);
                sys.Draw(spriteBatch);
            }
            
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(), 1, SpriteEffects.None, 0.89f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
        #region Generate Items
        void SetUpItems()
        {
            ItemStat stat = new ItemStat();
            #region Weapons
            stat.Name = "Sword";
            stat.Tooltip = "A shiny sword";
            stat.Consumable = false;
            stat.Stackable = false;
            stat.IsWeapon = true;
            stat.IsMelee = true;
            stat.IsRanged = false;
            stat.IsMagic = false;
            stat.Equipable = true;
            stat.equipType = 1;
            stat.Damage = 10;
            stat.iD = 0;
            stat.MaxStack = 1;
            itemStats.Add(stat);

            stat.Name = "Shuriken";
            stat.Tooltip = "Some ninja left it behind";
            stat.Consumable = true;
            stat.Stackable = true;
            stat.IsWeapon = true;
            stat.IsMelee = false;
            stat.IsRanged = true;
            stat.IsMagic = false;
            stat.Equipable = true;
            stat.equipType = 1;
            stat.Damage = 5;
            stat.iD = 1;
            stat.MaxStack = 20;
            itemStats.Add(stat);

            stat.Name = "Muramasa";
            stat.Tooltip = "Fastest sword ever!";
            stat.Consumable = false;
            stat.Stackable = false;
            stat.IsWeapon = true;
            stat.IsMelee = true;
            stat.IsRanged = false;
            stat.IsMagic = false;
            stat.Equipable = true;
            stat.equipType = 1;
            stat.Damage = 60;
            stat.iD = 2;
            stat.MaxStack = 1;
            itemStats.Add(stat);

            stat.Name = "The Defiler";
            stat.Tooltip = "Once belonged to a legendary warrior.";
            stat.Consumable = false;
            stat.Stackable = false;
            stat.IsWeapon = true;
            stat.IsMelee = true;
            stat.IsRanged = false;
            stat.IsMagic = false;
            stat.Equipable = true;
            stat.equipType = 1;
            stat.Damage = 150;
            stat.iD = 3;
            stat.MaxStack = 1;
            itemStats.Add(stat);

            stat.Name = "The Wind";
            stat.Tooltip = "You can break the sound barrier with this!";
            stat.Consumable = false;
            stat.Stackable = false;
            stat.IsWeapon = true;
            stat.IsMelee = false;
            stat.IsRanged = true;
            stat.IsMagic = false;
            stat.Equipable = true;
            stat.equipType = 1;
            stat.Damage = 110;
            stat.iD = 4;
            stat.MaxStack = 1;
            itemStats.Add(stat);
            #endregion
        }
        void SetUpSpells()
        {
            SpellStat stat = new SpellStat();
            #region Mage
            stat.Name = "Fireball";
            stat.Damage = 10;
            stat.Cooldown = 60;
            stat.Asset = @"Spells\Fireball";
            stat.Description = "Burn through your enemies";
            stat.iD = 0;
            spellsStats.Add(stat);
            #endregion
        }
        #endregion
        #region scripts
        Button CreateButton(Vector2 Position, int id, string name, string topName, ContentManager Content)
        {
            Texture2D texture = Content.Load<Texture2D>(@"Buttons\" + name + "Button");
            Texture2D prestexture = Content.Load<Texture2D>(@"Buttons\" + "Pressed" + name + "Button");
            Texture2D seltexture = Content.Load<Texture2D>(@"Buttons\" + "Selected" + name + "Button");
            Texture2D topTexture = Content.Load<Texture2D>(@"Buttons\" + topName);
            return new Button(Position, texture, seltexture, prestexture, topTexture, id);
        }

        public static void CreateCharacter()
        {
            StreamWriter file = File.AppendText(@"C:\Rpg\Characters.txt");
            file.WriteLine(textBox.text);
            file.Close();
            player = new Player(new Vector2(300));
            player.currClass = chosenClass;
            player.name = textBox.text;
            StreamWriter save = File.AppendText(@"C:\Rpg\" + textBox.text + ".txt");
            save.WriteLine(Scripts.EncryptData(((byte)(chosenClass)).ToString()));
            save.WriteLine(Scripts.EncryptData(player.Position.X.ToString()));
            save.WriteLine(Scripts.EncryptData(player.Position.Y.ToString()));
            for (int i = 0; i < inv.cells.Count * inv.cells[0].Count; i++)
            {
                save.WriteLine(Scripts.EncryptData("0"));
            }
            save.Close();
            textBox.text = "";
        }
        #endregion
    }
}