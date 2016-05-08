using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;

namespace Rpg
{
    public enum Classes
    {
        Warrior,
        Ranger,
        Wizard
    }

    public enum States
    {
        Standing,
        Walking,
        SwordSwinging,
        SwordSwingingReverse,
        SwordCombo
    }
    public class Player : Entity
    {
        public Classes currClass;
        bool goingForItem, collectingAllItems;
        Item targetItem;
        public string name;
        public List<Spell> spells;
        int attackQueue,attackWait;
        int ComboNumber;
        private List<Vector2> swordPositions;
        private Vector2 weaponOrigin,weaponPosition;
        private Texture2D weaponTexture;
        private float weaponAngle;
        private bool basicAttack;

        public Player(Vector2 position,Effect eff): base(position,eff)
        {
            
            goingForItem = false;
            collectingAllItems = false;
            GraphicsLoaded = false;
            isStanding = true;
            basicAttack = false;
            spells = new List<Spell>();
            Health = 600;
            maxHealth = 600;
            Mana = 150;
            maxMana = 150;
            moveSpeed = 5;
            healthRegen = 0.2f;
            manaRegen = 0.4f;
            ComboNumber = 0;
            attackQueue = 0;
            attackWait = 0;
            swordPositions = new List<Vector2>();
            InitializeWeaponPositions();
        }

        public override void Load(ContentManager Content)
        {
            Standing = Scripts.LoadTexture(@"Character\Player_Standing",Content);
            animations.Add(States.Walking,
                new Animation(Scripts.GetSpriteSheetRects(64, 64, 7, 7),
                    Scripts.LoadTexture(@"Character\Player_Walk", Content),
                    15 - (int)moveSpeed, true));
            animations.Add(States.SwordSwinging,
                new Animation(Scripts.GetSpriteSheetRects(64, 64, 6, 6),
                    Scripts.LoadTexture(@"Character\Player_SwordSwing", Content),
                    3, false));
            animations.Add(States.SwordSwingingReverse,
                new Animation(Scripts.GetSpriteSheetRects(64, 64, 6, 6),
                    Scripts.LoadTexture(@"Character\Player_SwordSwingReverse", Content),
                    3, false));
            animations.Add(States.SwordCombo,
                new Animation(Scripts.GetSpriteSheetRects(64, 64, 3, 3),
                    Scripts.LoadTexture(@"Character\Player_SwordCombo", Content),
                    3, false));
            Hair = Content.Load<Texture2D>(@"Character\Hair_0");
            GraphicsLoaded = true;
            Origin = new Vector2(Standing.Width / 2, Standing.Height / 2);
            hairOrigin = new Vector2(Hair.Width / 2, Hair.Height / 2);
        }

        public override void Update()
        {
            if(spells.Count<1)
            {
                InitializeSpells();
            }
            if(Rpg.mouse.RightClick()||Rpg.mouse.RightHeld())
            {
                if (Rpg.CheckIfMouseIsOutsideInventorys())
                {
                    MoveToDestination(Rpg.mouse.Position);
                    if (collectingAllItems == true)
                    {
                        collectingAllItems = false;
                    }
                }
            }
            if(Rpg.mouse.LeftClick())
            {
                if (Rpg.CheckIfMouseIsOutsideInventorys())
                {
                    foreach (Item item in Rpg.itemsOnGround)
                    {
                        if (Scripts.CheckIfMouseIsOver(item.rect))
                        {
                            CollectItem(item);
                            break;
                        }
                    }
                    if(Rpg.keyboard.IsHeld(Keys.LeftShift))
                    {
                        if(!Rpg.equipmentSlots[EquipSlot.Weapon].free)
                        {
                            if (attackQueue < 2)
                            {
                                attackQueue++;
                            }
                        }
                    }
                }
            }
            
            if (Rpg.keyboard.IsHeld(Keys.Z)||Rpg.keyboard.JustPressed(Keys.Z))
            {
                if (Rpg.itemsOnGround.Count > 0)
                {
                    collectingAllItems = true;
                    FindClosestItem();
                }
            }
            UpdateAttacks();
            SpellsUpdate();
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (GraphicsLoaded)
            {
                if (state == States.SwordSwinging || state == States.SwordSwingingReverse || state == States.SwordCombo)
                {
                    spriteBatch.Draw(weaponTexture,weaponPosition, null, Color.White, MathHelper.ToRadians(weaponAngle), weaponOrigin, 1f, SpriteEffects.None, 0.5999f);
                }
            }
        }

        private void InitializeWeaponPositions()
        {
            swordPositions.Add(new Vector2(14, 24));
            swordPositions.Add(new Vector2(20, 13));
            swordPositions.Add(new Vector2(22, 5));
            swordPositions.Add(new Vector2(23, -5));
            swordPositions.Add(new Vector2(19, -14));
            swordPositions.Add(new Vector2(15, -18));
        }

        private void UpdateAttacks()
        {
            if (state == States.Standing || state == States.Walking)
            {
                if (attackQueue > 0)
                {
                    BasicAttack(Rpg.equipmentSlots[EquipSlot.Weapon].item.stats.weaponType);
                }
            }
            else
            {
                if (!animations[state].isAnimating)
                {
                    if (attackQueue > 0)
                    {
                        BasicAttack(Rpg.equipmentSlots[EquipSlot.Weapon].item.stats.weaponType);
                    }
                    else
                    {
                        if (attackWait < 3)
                        {
                            attackWait++;
                        }
                        else
                        {
                            state = States.Standing;
                            ComboNumber = 0;
                            attackWait = 0;
                            basicAttack = false;
                        }
                    }
                }
            }
            if(state==States.SwordSwinging)
            {
                int index = animations[state].Index;
                Vector2 temp = swordPositions[index];
                weaponPosition = MathAid.ParentChildTransform(Position, temp, MathHelper.ToRadians(Angle));
                weaponAngle = Angle+(((animations[state].FrameCount/2)-index)*10);
            }
            else if (state == States.SwordSwingingReverse)
            {
                int index = (swordPositions.Count - 1) - animations[state].Index;
                Vector2 temp = swordPositions[index];
                weaponPosition = MathAid.ParentChildTransform(Position, temp, MathHelper.ToRadians(Angle));
                weaponAngle = Angle + (((animations[state].FrameCount / 2) - index) * 10);
            }
            else if(state == States.SwordCombo)
            {
                Vector2 temp = swordPositions[2];
                weaponPosition = MathAid.ParentChildTransform(Position, temp, MathHelper.ToRadians(Angle));
                weaponAngle = Angle;
            }
        }

        protected override void MoveToDestination(Vector2 target)
        {
            if (!basicAttack)
            {
                base.MoveToDestination(target);
            }
        }

        public void CollectItem(Item item)
        {
            MoveToDestination(item.Position);
            if (DistanceToTarget() > moveSpeed + 3)
            {
                targetItem = item;
                goingForItem = true;
            }
            else
            {
                PickUpItem(item);
            }
        }

        private bool PickUpItem(Item item)
        {
            if (Rpg.inventory.AddItem(item))
            {
                Rpg.itemsOnGround.Remove(item);
                return true;
            }
            return false;
        }

        public override void StopWalking()
        {
            base.StopWalking();
            if(goingForItem)
            {
                goingForItem = false;
                PickUpItem(targetItem);
            }
            if(collectingAllItems)
            {
                if (Rpg.itemsOnGround.Count > 0)
                {
                    FindClosestItem();
                }
                else
                {
                    collectingAllItems = false;
                    state = States.Standing;
                }
            }
            else
            {
                state = States.Standing;
            }
        }

        void FindClosestItem()
        {
            float minLength = int.MaxValue;
            int index = 0, i = 0;
            Item item;
            foreach (Item it in Rpg.itemsOnGround)
            {
                if (Vector2.Distance(Position, it.Position) < minLength)
                {
                    minLength = Vector2.Distance(Position, it.Position);
                    index = i;
                }
                i++;
            }
            item = Rpg.itemsOnGround[index];
            MoveToDestination(item.Position);
            if (Vector2.Distance(Position, movementTargetPosition) > moveSpeed + 3)
            {
                targetItem = item;
                goingForItem = true;
            }
            else
            {
                if(!PickUpItem(item))
                {
                    collectingAllItems = false;
                }
                if (collectingAllItems)
                {
                    if (Rpg.itemsOnGround.Count > 0)
                    {
                        FindClosestItem();
                    }
                    else
                    {
                        collectingAllItems = false;
                    }
                }
            }
        }        

        private void InitializeSpells()
        {
            switch(currClass)
            {
                case Classes.Wizard:
                    spells.Add(new Spell(0));
                    spells.Add(new Spell(1));
                    spells.Add(new Spell(2));
                    spells.Add(new Spell(3));
                    spells.Add(new Spell(4));
                    break;
            }
        }

        private void SpellsUpdate()
        {
            if (Rpg.keyboard.JustPressed(Keys.Q))
            {
                spells[0].Activate(this);
            }
            if (Rpg.keyboard.JustPressed(Keys.W))
            {
                spells[1].Activate(this);
            }
            if (Rpg.keyboard.JustPressed(Keys.E))
            {
                spells[2].Activate(this);
            }
            if (Rpg.keyboard.JustPressed(Keys.R))
            {
                spells[3].Activate(this);
            }
            if (Rpg.keyboard.JustPressed(Keys.Space))
            {
                spells[4].Activate(this);
            }
        }

        public void TurnTowardsMouse()
        {
            Angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, Rpg.mouse.Position));
        }

        private void BasicAttack(WeaponType weaponType)
        {
            attackQueue--;
            basicAttack = true;
            TurnTowardsMouse();
            switch(weaponType)
            {
                case WeaponType.Melee:
                    weaponOrigin = new Vector2(3, Rpg.equipmentSlots[EquipSlot.Weapon].item.Texture.Height / 2);
                    weaponTexture = Rpg.equipmentSlots[EquipSlot.Weapon].item.Texture;
                    switch(ComboNumber)
                    {
                        case 0:
                            state = States.SwordSwinging;
                            ComboNumber++;
                            break;
                        case 1:
                            state = States.SwordSwingingReverse;
                            ComboNumber++;
                            break;
                        case 2:
                            state = States.SwordCombo;
                            ComboNumber=0;
                            break;
                    }
                    animations[state].GhangeAnimatingState(true);
                    animations[state].SetPosition(Position);
                    break;
            }
        }
    }
}
