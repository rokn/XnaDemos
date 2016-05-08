using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG
{
    public enum Classes
    {
        Warrior,
        Ranger,
        Wizard
    }
    public class Player : Entity
    {
        private bool isMain;
        private Classes heroClass;

        public Player(bool IsMain, Classes HeroClass)
            : base()
        {            
            position = new Vector2(500, 500);
            moveDirection = new Vector2();
            animationDirection = Direction.Down;            
            this.isMain = IsMain;
            SetClass(HeroClass);
            Health = maxHealth;
            
            if (isMain)
            {
                position.X += 100;
            }
        }

        public override void Load()
        {            
            SetWalkingAnimation(@"Player\" + heroClass.ToString() + "Walk");
            basicAttackTexture = Scripts.LoadTexture(@"BasicAttacks\" + heroClass.ToString() + "Basic");
            basicAttackOrigin = new Vector2(basicAttackTexture.Width/2, basicAttackTexture.Height/2);
            UpdateAnimationSpeed();
            healthBar = new HealthBar(42, 10, new Vector2(), Color.Green, Color.Red);
            healthBar.SetBackground(@"Misc\PlayerHealthBarBack");
            UpdateHealthBarPosition();
            //SetMoveTarget(new Vector2(500, 3000));
        }

        public override void Update(GameTime gameTime)
        {
            if (isMain & !Main.InEditMode)
            {
                if (!isMoving && !isAtacking)
                {
                    DirectTowardsMouse();
                }

                HandleInput();

                Main.camera.FollowTarget(this.position);
            }

            base.Update(gameTime);

            UpdateHealthBarPosition();
        }

        private void UpdateHealthBarPosition()
        {
            healthBar.UpdatePosition(new Vector2(position.X, position.Y - height / 2 - 35));
        }

        private void HandleInput()
        {
            Entity mouseOverEntity = GetMouseOverEntity();

            if (mouseOverEntity != null)
            {
                MyMouse.ChangeColor(Color.Red);
            }
            else
            {
                MyMouse.ChangeToDefaultColor();
            }

            if (MyMouse.RightClick() || MyMouse.RightHeld())
            {
                if (!collisionRect.Contains(MyMouse.RealPosition) && GetMouseOverEntity() == null)
                {
                    Move(MyMouse.RealPosition);
                }
            }

            if (MyMouse.RightClick())
            {
                if (mouseOverEntity != null)
                {
                    BasicAttack(mouseOverEntity);
                }
            }
        }

        private Entity GetMouseOverEntity()
        {
            foreach (Entity entity in Main.Entities)
            {
                if (entity != this)
                {
                    if (entity.GetCollisionRect().Contains(MyMouse.RealPosition))
                    {
                        return entity;
                    }
                }
            }

            return null;
        }

        private void SetClass(Classes HeroClass)
        {
            heroClass = HeroClass;

            switch (heroClass)
            {
                #region Warrior
                case Classes.Warrior:
                    moveSpeed = 92;
                    isRanged = false;
                    basicAttackRange = 10;
                    basicAttackSpeed = 0.7f;
                    basicAttackDamage = 80;
                    maxHealth = 680;
                    armor = 30;
                    break;
                #endregion

                #region Ranger
                case Classes.Ranger:
                    moveSpeed = 110;
                    isRanged = true;
                    basicAttackRange = 280;
                    basicAttackSpeed = 0.8f;
                    basicAttackDamage = 72;
                    maxHealth = 600;
                    armor = 15;
                    break;
                #endregion

                #region Wizard
                case Classes.Wizard:
                    moveSpeed = 90;
                    isRanged = true;
                    basicAttackRange = 320;
                    basicAttackSpeed = 0.6f;
                    basicAttackDamage = 65;
                    maxHealth = 550;
                    armor = 10;
                    break;
                #endregion

                default:
                    break;
            }
        }
    }
}