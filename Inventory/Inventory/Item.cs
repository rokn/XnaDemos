using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Rpg
{
    public class Item
    {
        public bool onGround, consumable, stackable, isWeapon, isMelee, isRanged, isMagic, equipable,selected;
        public int? damage,stack,maxStack;
        public int Id,equipType;
        public string name,tooltip;
        public Texture2D texture,tooltipTexture;
        public Vector2 position;
        public List<string> Tooltip;
        public float toolTipAlpha = 0.8f,depth;
        #region Constructors
        /// <summary>
        /// Creates a item on the ground.
        /// </summary>
        /// <param name="Consumable">Is the item consumable</param>
        /// <param name="IsWeapon">Is the item a weapon</param>
        /// <param name="Type">What kind of item is it for ex. if weapon is it 0:melee,1:ranged,2:magic</param>
        /// <param name="stat">The damage/healing etc. of the item</param>
        /// <param name="id">The ID of the item</param>
        public Item(int id, Vector2 position)
        {
            Tooltip = new List<string>();
            onGround = true;
            consumable = Rpg.itemStats[id].Consumable;
            stackable = Rpg.itemStats[id].Stackable;
            isWeapon = Rpg.itemStats[id].IsWeapon;
            damage = Rpg.itemStats[id].Damage;
            isMelee = Rpg.itemStats[id].IsMelee;
            isRanged = Rpg.itemStats[id].IsRanged;
            isMagic = Rpg.itemStats[id].IsMagic;
            texture = Rpg.itemTextures[id];
            name = Rpg.itemStats[id].Name;
            tooltip = Rpg.itemStats[id].Tooltip;
            equipable = Rpg.itemStats[id].Equipable;
            equipType = Rpg.itemStats[id].equipType;
            selected = false;
            if (stackable)
            {
                maxStack = Rpg.itemStats[id].MaxStack;
                stack = 1;
            }
            else
            {
                maxStack = null;
                stack = null;
            }
            Id = id;
            this.position = position;
            GenerateTooltip();
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
        }
        public Item(int id, Vector2 position,int Stack)
        {
            Tooltip = new List<string>();
            onGround = true;
            consumable = Rpg.itemStats[id].Consumable;
            stackable = Rpg.itemStats[id].Stackable;
            isWeapon = Rpg.itemStats[id].IsWeapon;
            damage = Rpg.itemStats[id].Damage;
            isMelee = Rpg.itemStats[id].IsMelee;
            isRanged = Rpg.itemStats[id].IsRanged;
            isMagic = Rpg.itemStats[id].IsMagic;
            texture = Rpg.itemTextures[id];
            name = Rpg.itemStats[id].Name;
            tooltip = Rpg.itemStats[id].Tooltip;
            equipable = Rpg.itemStats[id].Equipable;
            equipType = Rpg.itemStats[id].equipType;
            selected = false;
            if (stackable)
            {
                maxStack = Rpg.itemStats[id].MaxStack;
                stack = Stack;
            }
            else
            {
                maxStack = null;
                stack = null;
            }
            Id = id;
            this.position = position;
            GenerateTooltip();
            tooltipTexture = Scripts.GenerateTooltipTexture(Tooltip);
        }
        #endregion
        public void Draw(SpriteBatch spriteBatch)
        {
            if (onGround)
            {
                spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(texture.Width/2,texture.Height/2),1, SpriteEffects.None, depth);
                if(selected)
                {
                    DrawTooltip(spriteBatch);
                }
            }
        }
        public void DrawTooltip(SpriteBatch spriteBatch)
        {
            if (Tooltip.Count > 0)
            {
                if (tooltipTexture != null)
                {
                    int i = 0;
                    spriteBatch.Draw(tooltipTexture, Rpg.mouse.Position, null, Color.White * toolTipAlpha, 0, new Vector2(), 1, SpriteEffects.None, 0.9f);
                    foreach (string s in Tooltip)
                    {
                        spriteBatch.DrawString(Rpg.Font, s, Rpg.mouse.Position + new Vector2(9, i * 18), Color.Black, 0, new Vector2(), 1, SpriteEffects.None, 1f);
                        i++;
                    }
                }
            }
        }
        public void GenerateTooltip()
        {
            Tooltip.Clear();
            Tooltip.Add(name);
            if (isWeapon)
            {
                if (isMelee) { Tooltip.Add(damage.ToString() + " melee damage"); }
                if (isRanged) { Tooltip.Add(damage.ToString() + " ranged damage"); }
                if (isMagic) { Tooltip.Add(damage.ToString() + " magic damage"); }
            }
            if (consumable) { Tooltip.Add("Consumable"); }
            if (tooltip.Length > 0) { Tooltip.Add(tooltip); }
        }


        internal void ClearTooltip()
        {
            Tooltip.Clear();
        }
        
    }
}
