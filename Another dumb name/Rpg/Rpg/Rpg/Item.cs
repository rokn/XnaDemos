using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public enum EquipSlot
    {
        Helm,
        Chestplate,
        Gloves,
        Boots,
        Weapon,
        Shield
    }

    public enum WeaponType
    {
        Melee,
        Ranged,
        Magic,
        Throwable
    }

    public enum ArmorType
    {
        Cloth,
        Leather,
        Metal,
        Plate
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Misc,
        Potion
    }

    public struct ItemStat
    {
        public string Name;
        public string tooltip;
        public int Damage;
        public int Armor;
        public ItemType type;
        public WeaponType weaponType;
        public ArmorType armorType;
        public EquipSlot equipSlot;
        public float attackSpeed;
        public bool Stackable;
        public int MaxStack;
    }

    public class Item
    {
        public Texture2D tooltipTexture,Texture;
        public Vector2 Position;
        public List<string> Tooltip;
        public int Stack,Id;
        public float toolTipAlpha = 0.8f, depth;
        public ItemStat stats;
        public bool selected;
        public Rectangle rect;
        public Item(int id, Vector2 position,int stack = 1)
        {
            Tooltip = new List<string>();
            Position = position;
            Id =id;
            stats = Rpg.itemStats[id];
            Scripts.GenerateItemTooltip(this);
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
            Texture = Rpg.itemTextures[id];
            selected = false;
            depth = 0.5f;
            rect = new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y-Texture.Height / 2, Texture.Width, Texture.Height);
            Stack = stack;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            float drawDepth = depth + 0.0001f * Rpg.itemsOnGround.IndexOf(this);
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, new Vector2(Texture.Width / 2, Texture.Height / 2), 1, SpriteEffects.None, drawDepth);
            if (selected)
            {
                Scripts.DrawTooltip(Tooltip, tooltipTexture, toolTipAlpha, spriteBatch);
            }
        }
        public void RepositionRectangle()
        {
            rect.X = (int)Position.X - Texture.Width / 2;
            rect.Y = (int)Position.Y - Texture.Height / 2;
        }
    }
}
