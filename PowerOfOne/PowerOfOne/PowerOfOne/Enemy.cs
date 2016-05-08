﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerOfOne
{
    public class Enemy : Entity
    {
        private const int MoveChance = 1;

        private HealthBar healthBar;
        private Vector2 healtBarPositionOffset;
        private bool hasBeenDamaged;
        private Queue<Direction> idleMovementSteps;
        private Random rand;
        private int id;
        private TimeSpan idleMoveTimer;
        private EnemyStat stats;
        private Ability ability;

        public bool IsSelected { get; set; }

        public Enemy(Vector2 pos, int id)
            : base(pos)
        {
            AbilityPower = 5;
            stats = Main.EnemyStats[id];
            health = stats.MaxHealth;
            maxHealth = stats.MaxHealth;
            moveSpeed = stats.MoveSpeed;
            ability = (Ability)Activator.CreateInstance(stats.Ability);
            ability.Initialize(this);
            idleMovementSteps = new Queue<Direction>();
            rand = Main.rand;
            this.id = id;
            idleMoveTimer = new TimeSpan();
            IsSelected = false;
        }

        protected override void Initialize()
        {
            int healthBarWidth = 60;
            healtBarPositionOffset = new Vector2(healthBarWidth / 2, EntityHeight / 2 + 12);
            Vector2 healthBarPos = Position - healtBarPositionOffset;
            healthBar = new HealthBar(healthBarWidth, healthBarPos, Color.Green, Color.Red);
            hasBeenDamaged = false;
            base.Initialize();
        }

        public override void Load()
        {
            walkSpriteSheet = Scripts.LoadTexture(@"Enemies\Enemy_" + id.ToString());

            EntityWidth = walkSpriteSheet.Width / 4;
            EntityHeight = walkSpriteSheet.Height / 4;

            this.Initialize();

            healthBar.Load(true, "Enemies");

            base.Load();

            if (ability is Passive)
            {
                (ability as Passive).ActivatePassive();
            }
            ability.Load();
        }

        public override void Update(GameTime gameTime)
        {
            healthBar.Update(Position - healtBarPositionOffset, health, maxHealth);
            ability.Update(gameTime);

            if (stats.Aggresive)
            {
                if (CheckForPlayer())
                {
                    if (AbilityIsActive())
                    {
                        ability.AiControl();
                    }
                }
                else
                {
                    IdleMovement(gameTime);
                }
            }
            else
            {
                if (hasBeenDamaged)
                {
                    if (CheckForPlayer())
                    {
                        RunFromPlayer();
                    }
                    else
                    {
                        IdleMovement(gameTime);
                    }
                }
                else
                {
                    IdleMovement(gameTime);
                }
            }

            base.Update(gameTime);
        }

        private bool AbilityIsActive()
        {
            return !(ability is Passive);
        }

        public bool GoToPlayer()
        {
            bool moved = false;

            Entity player = Main.Entities.First(ent => ent.GetType() == typeof(Player));
            Vector2 spaceBetween = new Vector2(EntityWidth / 2 + player.EntityWidth / 2, EntityHeight / 2 + player.EntityHeight / 2);

            if (Position.X - player.Position.X > spaceBetween.X)
            {
                Move(Direction.Left, moveSpeed);
                moved = true;
            }

            if (Position.Y - player.Position.Y > spaceBetween.Y)
            {
                Move(Direction.Up, moveSpeed);
                moved = true;
            }

            if (player.Position.X - Position.X > spaceBetween.X)
            {
                Move(Direction.Right, moveSpeed);
                moved = true;
            }

            if (player.Position.Y - Position.Y > spaceBetween.Y)
            {
                Move(Direction.Down, moveSpeed);
                moved = true;
            }

            return moved;
        }

        public bool RunFromPlayer()
        {
            bool moved = false;

            Entity player = Main.Entities.First(ent => ent.GetType() == typeof(Player));
            Vector2 spaceBetween = new Vector2(EntityWidth / 2 + player.EntityWidth / 2, EntityHeight / 2 + player.EntityHeight / 2);

            if (Position.X - player.Position.X > spaceBetween.X)
            {
                Move(Direction.Right, moveSpeed);
                moved = true;
            }

            if (Position.Y - player.Position.Y > spaceBetween.Y)
            {
                Move(Direction.Down, moveSpeed);
                moved = true;
            }

            if (player.Position.X - Position.X > spaceBetween.X)
            {
                Move(Direction.Left, moveSpeed);
                moved = true;
            }

            if (player.Position.Y - Position.Y > spaceBetween.Y)
            {
                Move(Direction.Up, moveSpeed);
                moved = true;
            }

            return moved;
        }

        private bool CheckForPlayer()
        {
            return Vector2.Distance(Position, Main.Entities.Find(ent => ent is Player).Position) <= stats.SightDistance;
        }

        private void IdleMovement(GameTime gameTime)
        {
            if (idleMoveTimer.TotalMilliseconds <= 0)
            {
                int moveSteps = rand.Next(5, 30);
                Direction directionToMove = (Direction)rand.Next(4);

                for (int i = 0; i < moveSteps; i++)
                {
                    idleMovementSteps.Enqueue(directionToMove);
                }

                idleMoveTimer = new TimeSpan(0, 0, 0, 0, moveSteps * 100);
            }
            else
            {
                idleMoveTimer = idleMoveTimer.Subtract(gameTime.ElapsedGameTime);
            }

            if (idleMovementSteps.Count > 0)
            {
                Move(idleMovementSteps.Dequeue(), moveSpeed);
            }
            else
            {
                walkingAnimation[currentDirection].ChangeAnimatingState(false);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hasBeenDamaged)
            {
                healthBar.Draw(spriteBatch, 0.9f);
            }

            ability.Draw(spriteBatch);

            if (IsSelected)
            {
                walkingAnimation[currentDirection].Draw(spriteBatch, size + .1f, baseDepth + 0.01f, Color.Yellow);
            }

            base.Draw(spriteBatch);
        }

        public override void TakeDamage(float damageToBeTaken)
        {
            hasBeenDamaged = true;
            base.TakeDamage(damageToBeTaken);
        }

        protected override void Destroy()
        {
            Main.ParticleEngine.GenerateDeathEffect(Position - origin, walkingAnimation[currentDirection].GetCurrentTexture());
            base.Destroy();
        }
    }
}