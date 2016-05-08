#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
#endregion

namespace Rpg
{
    public enum Menu
    {
        Main,
        CharacterSelect,
        CharacterCreate
    }
    public class Rpg : Game
    {
        #region Variables
        public const int itemCount = 5;
        public const int projectileCount = 3;

        public static GraphicsDeviceManager graphics;
        public static KeysInput keyboard;
        public static MouseCursor mouse;
        private Texture2D mouseTexture;
        private SpriteBatch spriteBatch;
        public static int height;
        public static int width;
        public static Menu menu;
        public static bool inGame,exit;
        private List<Button> mainMenu, characterCreate, characterSelect;
        public static int chosenCharacter;
        public static Classes chosenClass;
        public static SpriteFont Font;
        public static List<ItemStat> itemStats;
        public static List<Item> itemsOnGround;
        public static List<Texture2D> itemTextures;
        public static Item mouseItem;
        public static bool mouseHasItem;
        public static Inventory inventory;
        public static Inventory stash;
        public static Dictionary<EquipSlot,Equipment> equipmentSlots;
        public static Player player;
        public static TextBox textBox;
        public static Texture2D textBoxTexture, characterButtonTexture, characterButtonTextureGlow;
        public static List<Player> Characters;
        public static List<Rectangle> charactersButtonsRects;
        public static string saveFilePath;
        public static List<Tile> tiles;
        public static List<SpellStat> spellsStats;
        public static List<Texture2D> projectilesTextures;
        public static List<ProjectileStat> projectilesStats;
        public static List<Projectile> Projectiles;
        public static Texture2D lightningCenter, lightningArc;
        public static Effect effect;
        public static List<Enemy> enemies;
        public static Camera camera;
        #endregion

        #region Base Stuff

        public Rpg(): base()
        {
            Components.Add(new FrameRateCounter(this));
            Components.Add(new Lightnings(this));
            Components.Add(new GUI(this));
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            InitializeInput();
            chosenCharacter = 0;
            saveFilePath = @"C:\Rpg\";
            camera = new Camera();
            inventory = new Inventory(10, 6, new Vector2(20, 20),5,5);
            stash = new Inventory(10, 6, new Vector2(20, 450), 5, 5);
            mainMenu = new List<Button>();
            characterCreate = new List<Button>();
            characterSelect = new List<Button>();
            itemStats = new List<ItemStat>();
            itemTextures = new List<Texture2D>();
            itemsOnGround = new List<Item>();
            equipmentSlots = new Dictionary<EquipSlot,Equipment>();
            Characters = new List<Player>();
            charactersButtonsRects = new List<Rectangle>();
            tiles = new List<Tile>();
            projectilesTextures = new List<Texture2D>();
            projectilesStats = new List<ProjectileStat>();
            Projectiles = new List<Projectile>();
            spellsStats = new List<SpellStat>();
            enemies = new List<Enemy>();
            inGame = false;
            mouseHasItem = false;
            InitializeButtons();
            InitializeItems();
            InitializeEquipment();
            InitTiles();
            InitializeProjectiles();
            InitializeSpells();
            base.Initialize();
        }                       // INIT

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse",Content);
            Font = Content.Load<SpriteFont>("Font");
            lightningArc = Scripts.LoadTexture(@"Spells\LightningArc",Content);
            lightningCenter = Scripts.LoadTexture(@"Spells\LightningCenter",Content);
            effect = Content.Load<Effect>(@"Spells\Effect");
            enemies.Add(new Enemy(0, new Vector2(500, 500), effect.Clone()));
            enemies[0].Load(Content);
            inventory.Load(Content);
            stash.Load(Content);
            LoadItemTextures();
            MenusLoad();
            LoadTiles();
            LoadEquipment();
            LoadProjectiles();
        }                      //LOAD

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                if (keyboard.IsHeld(Keys.Left))
                {
                    camera.Move(new Vector2(-1, 0));
                }
                if (keyboard.IsHeld(Keys.Right))
                {
                    camera.Move(new Vector2(1, 0));
                }
                if (keyboard.IsHeld(Keys.Up))
                {
                    camera.Move(new Vector2(0, -1));
                }
                if (keyboard.IsHeld(Keys.Down))
                {
                    camera.Move(new Vector2(0, 1));
                }
                UpdateInput(gameTime);
                if(!inGame)
                {
                    MenusUpdate();
                }
                else
                {
                    enemies[0].Update();
                    if(!player.GraphicsLoaded)
                    {
                        player.Load(Content);
                    }
                    player.Update();
                    UpdateInventorys();
                    UpdateItems();
                    UpdateEquipment();
                    UpdateTiles();
                    UpdateProjectiles();
                    if(keyboard.JustPressed(Keys.I))
                    {
                        ChangeInventoryState();
                    }
                    if (inventory.opened)
                    {
                        if (mouseHasItem)
                        {
                            if (mouse.LeftClick())
                            {
                                if (CheckIfMouseIsOutsideInventorys())
                                {
                                    DropItem();
                                }
                            }
                        }
                    }
                    if (keyboard.JustPressed(Keys.P))
                    {
                        Random rand = new Random();
                        itemsOnGround.Add(new Item(rand.Next(itemCount), mouse.Position,1));
                    }
                }
                if(exit||keyboard.JustPressed(Keys.Escape))
                {
                    this.Exit();
                }
            }
            base.Update(gameTime);
        }          //UPDATE

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, "Camera pos:" + camera.pos, new Vector2(20, 50), Color.Black);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive,null,null,null,null,camera.GetTransformation(GraphicsDevice));
            Lightnings.Draw(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend,null,null,null,null,camera.GetTransformation(GraphicsDevice));
            
            if(!inGame)
            {
                MenusDraw();
            }
            else
            {
                enemies[0].Draw(spriteBatch);
                DrawInventorys();
                DrawItems();
                DrawMouseItem();
                player.Draw(spriteBatch);
                DrawTiles();
                DrawEquipment();
                DrawProjectiles();
                GUI.Draw(spriteBatch);
            }
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(), 1, SpriteEffects.None, 0.99f);
            spriteBatch.End();
            base.Draw(gameTime);
        }            //DRAW

        #endregion

        #region Initalize Methods

        private void InitializeEquipment()
        {
            equipmentSlots.Add(EquipSlot.Weapon,new Equipment(new Vector2(800, 100), EquipSlot.Weapon));
            equipmentSlots.Add(EquipSlot.Chestplate,new Equipment(new Vector2(900, 100), EquipSlot.Chestplate));
        }

        void InitializeInput()
        {
            keyboard = new KeysInput();
            mouse = new MouseCursor(width, height, 150);
        }

        private void InitializeButtons()
        {
            mainMenu.Add(new Button(new Vector2(800, 200), ButtonType.Menu, ButtonName.Play));
            mainMenu.Add(new Button(new Vector2(800, 600), ButtonType.Menu, ButtonName.Quit));
            characterCreate.Add(new Button(new Vector2(100, 100), ButtonType.Class, ButtonName.WarriorClass));
            characterCreate.Add(new Button(new Vector2(100, 300), ButtonType.Class, ButtonName.RangerClass));
            characterCreate.Add(new Button(new Vector2(100, 500), ButtonType.Class, ButtonName.WizardClass));
            characterCreate.Add(new Button(new Vector2(1200, 910), ButtonType.Menu, ButtonName.Create));
            characterCreate.Add(new Button(new Vector2(200, 910), ButtonType.Menu, ButtonName.Back));
            characterSelect.Add(new Button(new Vector2(1200, 910), ButtonType.Menu, ButtonName.Create));
            characterSelect.Add(new Button(new Vector2(200, 910), ButtonType.Menu, ButtonName.Back));
            characterSelect.Add(new Button(new Vector2(200, 810), ButtonType.Menu, ButtonName.Delete));
            characterSelect.Add(new Button(new Vector2(825, 910), ButtonType.Menu, ButtonName.Play));
        }

        private void InitializeItems()
        {
            ItemStat stat = new ItemStat();
            #region Weapons

            stat.Name = "Sword";
            stat.tooltip = "A shiny sword";
            stat.Stackable = false;
            stat.attackSpeed = 10;
            stat.Damage = 10;
            stat.type = ItemType.Weapon;
            stat.equipSlot = EquipSlot.Weapon;
            stat.weaponType = WeaponType.Melee;
            itemStats.Add(stat);
            stat = new ItemStat();

            stat.Name = "Shuriken";
            stat.tooltip = "Some ninja left it behind";
            stat.Stackable = true;
            stat.MaxStack = 10;
            stat.attackSpeed = 5;
            stat.Damage = 4;
            stat.type = ItemType.Weapon;
            stat.equipSlot = EquipSlot.Weapon;
            stat.weaponType = WeaponType.Ranged;
            itemStats.Add(stat);
            stat = new ItemStat();

            stat.Name = "Muramasa";
            stat.tooltip = "Fastest sword ever";
            stat.Stackable = false;
            stat.attackSpeed = 2;
            stat.Damage = 60;
            stat.type = ItemType.Weapon;
            stat.equipSlot = EquipSlot.Weapon;
            stat.weaponType = WeaponType.Melee;
            itemStats.Add(stat);
            stat = new ItemStat();

            stat.Name = "The Defiler";
            stat.tooltip = "Once belonged to a legendary warrior";
            stat.Stackable = false;
            stat.attackSpeed = 20;
            stat.Damage = 150;
            stat.type = ItemType.Weapon;
            stat.equipSlot = EquipSlot.Weapon;
            stat.weaponType = WeaponType.Melee;
            itemStats.Add(stat);
            stat = new ItemStat();

            stat.Name = "The Wind";
            stat.tooltip = "You can break the sound barrier with this";
            stat.Stackable = false;
            stat.attackSpeed = 25;
            stat.Damage = 110;
            stat.type = ItemType.Weapon;
            stat.equipSlot = EquipSlot.Weapon;
            stat.weaponType = WeaponType.Ranged;
            itemStats.Add(stat);
            stat = new ItemStat();
            #endregion
        }

        private void InitTiles()
        {
            tiles.Add(new Tile(TileType.Stash, new Vector2(900)));
        }

        private void InitializeProjectiles()
        {
            ProjectileStat stat = new ProjectileStat();

            stat.Name = "Fireball";
            stat.textureColumns = 3;
            stat.textureFrames = 3;
            stat.frameWidth = 32;
            stat.frameHeight = 13;
            stat.isAnimation = true;
            projectilesStats.Add(stat);
            stat = new ProjectileStat();

            stat.Name = "Arcane Missle";
            stat.isAnimation = false;
            projectilesStats.Add(stat);
            stat = new ProjectileStat();

            stat.Name = "IceBolt";
            stat.isAnimation = false;
            stat.rotationSpeed = 0.3f;
            stat.isFrosty = true;
            projectilesStats.Add(stat);
            stat = new ProjectileStat();
        }

        private void InitializeSpells()
        {
            SpellStat stat = new SpellStat();

            stat.Name = "Fireball";
            stat.asset = @"Spells\Fireball";
            stat.baseDamage = 10;
            stat.cooldown = 120;
            stat.manaCost = 10;
            stat.toolTip = "Conjure a fiery ball";
            spellsStats.Add(stat);
            stat = new SpellStat();

            stat.Name = "Arcane Missiles";
            stat.asset = @"Spells\ArcaneMissiles";
            stat.baseDamage = 4;
            stat.cooldown = 150;
            stat.manaCost = 15;
            stat.toolTip = "Summon 3 arcane missiles that move strangely";
            spellsStats.Add(stat);
            stat = new SpellStat();

            stat.Name = "Lightning";
            stat.asset = @"Spells\Ligthning";
            stat.baseDamage = 30;
            stat.cooldown = 100;
            stat.manaCost = 30;
            stat.toolTip = "Shoot a mighty lightning bolt at ana enemy";
            spellsStats.Add(stat);
            stat = new SpellStat();

            stat.Name = "Ice Strike";
            stat.asset = @"Spells\IceStrike";
            stat.baseDamage = 10;
            stat.cooldown = 200;
            stat.manaCost = 25;
            stat.toolTip = "You can freeze anything with this";
            spellsStats.Add(stat);
            stat = new SpellStat();

            stat.Name = "Teleport";
            stat.asset = @"Spells\Teleport";
            stat.cooldown = 600;
            stat.manaCost = 30;
            stat.toolTip = "Phase through space and time and go forward";
            spellsStats.Add(stat);
            stat = new SpellStat();


        }

        void GoFullscreenBorderless()
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen;
            graphics.PreferredBackBufferWidth = screen.Bounds.Width;
            graphics.PreferredBackBufferHeight = screen.Bounds.Height;
            width = screen.Bounds.Width;
            height = screen.Bounds.Height;
            graphics.IsFullScreen = true;
        }

        public static void CreateCharacter()
        {
            player = new Player(new Vector2(300),effect.Clone());
            player.currClass = chosenClass;
            player.name = textBox.text;

            StreamWriter file = File.AppendText(saveFilePath + "Characters.txt");
            file.WriteLine(player.name);
            file.Close();

            StreamWriter save = File.AppendText(saveFilePath + textBox.text + ".txt");
            save.WriteLine(Scripts.EncryptData(((byte)(chosenClass)).ToString()));
            save.WriteLine(Scripts.EncryptData(player.Position.X.ToString()));
            save.WriteLine(Scripts.EncryptData(player.Position.Y.ToString()));
            for (int i = 0; i < inventory.cells.Length; i++)
            {
                save.WriteLine(Scripts.EncryptData("0"));
                save.WriteLine(Scripts.EncryptData("0"));
            }
            save.Close();
            textBox.text = "";
        }

        #endregion

        #region Load Methods

        private void MenusLoad()
        {
            foreach (Button button in mainMenu)
            {
                button.Load(Content);
            }
            foreach (Button button in characterCreate)
            {
                button.Load(Content);
            }
            foreach (Button button in characterSelect)
            {
                button.Load(Content);
            }
        }

        private void LoadItemTextures()
        {
            for (int i = 0; i < itemCount; i++)
            {
                itemTextures.Add(Scripts.LoadTexture(@"Items\Item_" + i.ToString(), Content));
            }
        }

        private void LoadTiles()
        {
            foreach (Tile tile in tiles)
            {
                tile.Load(Content);
            }
        }

        private void LoadEquipment()
        {
            foreach (KeyValuePair<EquipSlot,Equipment> pair in equipmentSlots)
            {
                pair.Value.Load(Content);
            }
        }

        private void LoadProjectiles()
        {
            for (int i = 0; i < projectileCount; i++)
            {
                projectilesTextures.Add(Scripts.LoadTexture(@"Projectiles\Proj_" + i.ToString(),Content));
            }
        }

        #endregion

        #region Update Methods

        private void UpdateItems()
        {
            bool isSelectedItem=false;
            foreach (Item item in itemsOnGround)
            {
                if(item.selected)
                {
                    isSelectedItem = true;
                }
                if (Scripts.CheckIfMouseIsOver(item.rect))
                {
                    if (!isSelectedItem)
                    {
                        item.selected = true;
                        isSelectedItem = true;
                    }
                }
                else
                {
                    item.selected = false;
                }
            }
        }

        void UpdateInput(GameTime gameTime)
        {
            mouse.UpdateMouse(gameTime);
            keyboard.Update(gameTime);
        }

        private void MenusUpdate()
        {
            switch (menu)
            {
                case Menu.Main: UpdateMenu(mainMenu);
                    break;
                case Menu.CharacterCreate: UpdateMenu(characterCreate);
                    break;
                case Menu.CharacterSelect: UpdateMenu(characterSelect);
                    break;
            }
        }

        private void UpdateMenu(List<Button> updateMenu)
        {
            foreach (Button button in updateMenu)
            {
                button.Update();
            }
            if (menu == Menu.CharacterSelect)
            {
                CheckForCharacterPress();
            }
            if (menu == Menu.CharacterCreate)
            {
                if (!textBox.Loaded)
                {
                    textBox.Load(Content);
                }
                textBox.Update();
            }
        }

        private void CheckForCharacterPress()
        {
            if (mouse.LeftClick())
            {
                foreach (Rectangle rect in charactersButtonsRects)
                {
                    if (Scripts.CheckIfMouseIsOver(rect))
                    {
                        chosenCharacter = charactersButtonsRects.IndexOf(rect);
                    }
                }
            }
        }

        private void ChangeInventoryState()
        {
            inventory.opened = !inventory.opened;
            if (stash.opened)
            {
                stash.opened = false;
            }
        }

        public static bool CheckIfMouseIsOutsideInventorys()
        {
            if (inventory.opened)
            {
                if (stash.opened)
                {
                    if (Scripts.CheckIfMouseIsOver(stash.rect))
                    {
                        return false;
                    }
                }
                if (Scripts.CheckIfMouseIsOver(inventory.rect))
                {
                    return false;
                }
                foreach (KeyValuePair<EquipSlot, Equipment> pair in equipmentSlots)
                {
                    if (Scripts.CheckIfMouseIsOver(pair.Value.rect))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void UpdateInventorys()
        {
            if (inventory.opened)
            {
                inventory.Update();
            }
            if (stash.opened)
            {
                stash.Update();
            }
        }

        private void UpdateEquipment()
        {
            if (inventory.opened)
            {
                foreach (KeyValuePair<EquipSlot, Equipment> pair in equipmentSlots)
                {
                    pair.Value.Update();
                }
            }
        }

        private void UpdateTiles()
        {
            foreach (Tile tile in tiles)
            {
                tile.Update();
            }
        }
        
        private void UpdateProjectiles()
        {
            foreach(Projectile proj in Projectiles)
            {
                proj.Update();
            }
        }

        private void DropItem()
        {
            itemsOnGround.Add(new Item(mouseItem.Id,player.Position,mouseItem.Stack));
            mouseItem = null;
            mouseHasItem = false;
        }

        #endregion

        #region Draw Methods

        private void DrawMouseItem()
        {
            if (inventory.opened)
            {
                if (mouseHasItem)
                {
                    spriteBatch.Draw(mouseItem.Texture, mouse.Position, null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0.98f);
                    if(mouseItem.stats.Stackable)
                    {
                        spriteBatch.DrawString(Rpg.Font, mouseItem.Stack.ToString(), mouse.Position- new Vector2(8,2), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 0.98f);
                    }
                }
            }
        }

        private void MenusDraw()
        {
            switch (menu)
            {
                case Menu.Main: DrawMenu(mainMenu);
                    break;
                case Menu.CharacterCreate:
                    foreach (Button button in characterCreate)
                    {
                        if (button.Name.ToString().Contains(chosenClass.ToString()))
                        {
                            button.isSelected = true;
                        }
                    }
                    DrawMenu(characterCreate);
                    textBox.Draw(spriteBatch);
                    break;
                case Menu.CharacterSelect: DrawMenu(characterSelect);
                    DrawCharacters();
                    break;
            }
        }

        private void DrawCharacters()
        {
            int y;
            int x = y = 150;
            int i = 0;
            foreach (Player p in Characters)
            {
                spriteBatch.Draw(characterButtonTexture, new Vector2(x, y * (i + 1)), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0.97f);
                if (i == chosenCharacter)
                {
                    spriteBatch.Draw(characterButtonTextureGlow, new Vector2(x - 3, y * (i + 1) - 3), null, Color.White, 0, new Vector2(), 1, SpriteEffects.None, 0.96f);
                }
                spriteBatch.DrawString(Font, p.name, new Vector2(x + 6, y + 6), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.98f);
                spriteBatch.DrawString(Font, p.currClass.ToString(), new Vector2(x + 100, y + 20), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0.98f);
            }
        }

        private void DrawProjectiles()
        {
            foreach(Projectile proj in Projectiles)
            {
                proj.Draw(spriteBatch);
            }
        }

        private void DrawMenu(List<Button> updateMenu)
        {
            foreach (Button button in updateMenu)
            {
                button.Draw(spriteBatch);
            }
        }

        private void DrawEquipment()
        {
            if (inventory.opened)
            {
                foreach (KeyValuePair<EquipSlot,Equipment> pair in equipmentSlots)
                {
                    pair.Value.Draw(spriteBatch);
                }
            }
        }

        private void DrawTiles()
        {
            foreach (Tile tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
        }

        private void DrawInventorys()
        {
            if (inventory.opened)
            {
                inventory.Draw(spriteBatch);
            }
            if (stash.opened)
            {
                stash.Draw(spriteBatch);
            }
        }

        private void DrawItems()
        {
            foreach(Item item in itemsOnGround)
            {
                item.Draw(spriteBatch);
            }
        }

        #endregion


    }
}
