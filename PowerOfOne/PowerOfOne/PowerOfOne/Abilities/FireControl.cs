using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace PowerOfOne
{
    public class Fire
    {
        private const int FireWidth = 100;
        private const int FireHeight = 100;

        private Vector2 Position;
        private Animation fireAnimation;
        public Rectangle rect;

        public Fire(Vector2 position)
        {
            Position = position;
            Load();
        }

        public void Load()
        {
            Texture2D fireSpriteSheet = Scripts.LoadTexture(@"Abilities\FireControl\Fire");
            List<Rectangle> fireRects = Scripts.GetSourceRectangles(0, 15, FireWidth, FireHeight, fireSpriteSheet);
            fireAnimation = new Animation(fireRects, fireSpriteSheet, 3, true);
            Vector2 Origin = new Vector2(FireWidth / 2, FireHeight / 2);
            rect = Scripts.InitRectangle(Position - Origin, FireWidth, FireHeight);
        }

        public void Update()
        {
            fireAnimation.Update(Position, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            fireAnimation.Draw(spriteBatch, 1f, 0.15f, Color.White);
        }
    }

    public class FireControl : Ability
    {
        private const int baseBreathDamage = 1;
        private const int fireFrames = 6;
        private const int baseFireDamage = 3;
        private SpriteBatch sB;
        private ParticleEngine particleSystem;
        private float damage;
        private Vector2 fireDirection;
        private Vector2 inscribedCircleCenter;
        private List<Texture2D> particles;
        private float hitDistance;
        private Texture2D phoenixSpriteSheet;
        private Dictionary<Direction, Animation> ownerAnimation;
        private Dictionary<Direction, Animation> phoenixAnimation;
        private bool IsPhoenix;
        private int DefaultEntityWidth;
        private int DefaultEntityHeight;
        private bool mouseReleased;
        private int PhoenixWidth;
        private int PhoenixHeight;
        private Rectangle phoenixWalkingRect;
        private Vector2 phoenixWalkingOrigin;
        private Rectangle ownerWalkingRect;
        private Vector2 ownerWalkingOrigin;
        private float particleSpeed;
        private int currFireTime;
        private float fireDamage;
        private List<Fire> phoenixFires;

        public FireControl()
            : base()
        {
        }

        public override void Initialize(Entity owner)
        {
            base.Initialize(owner);
            IsPhoenix = false;
            damage = baseBreathDamage * Owner.AbilityPower;
            fireDamage = (float)(baseFireDamage * Owner.AbilityPower) / 5f;
            particleSpeed = Owner.AbilityPower;
            hitDistance = particleSpeed * 20f;
            currFireTime = 0;
            mouseReleased = true;
            phoenixFires = new List<Fire>();
            ownerAnimation = new Dictionary<Direction, Animation>();
        }

        public override void Load()
        {
            base.Load();
            phoenixSpriteSheet = Scripts.LoadTexture(@"Abilities\FireControl\Phoenix");
            phoenixAnimation = Scripts.LoadEntityWalkAnimation(phoenixSpriteSheet);
            sB = new SpriteBatch(Main.graphics.GraphicsDevice);
            PhoenixWidth = phoenixSpriteSheet.Width / 4;
            PhoenixHeight = phoenixSpriteSheet.Height / 4;
            phoenixWalkingOrigin = Scripts.GetWalkingOrigin(PhoenixWidth, PhoenixHeight);
            phoenixWalkingRect = Scripts.GetWalkingRect(new Vector2(), PhoenixWidth, PhoenixHeight);

            particles = new List<Texture2D>() { Scripts.LoadTexture(@"Abilities\Circle") };
            particleSystem = new ParticleEngine(particles, Owner.Position);

            foreach (KeyValuePair<Direction, Animation> kvp in Owner.walkingAnimation)
            {
                ownerAnimation.Add(kvp.Key, kvp.Value);
            }

            DefaultEntityWidth = Owner.EntityWidth;
            DefaultEntityHeight = Owner.EntityHeight;
            ownerWalkingOrigin = Owner.WalkingOrigin;
            ownerWalkingRect = Owner.WalkingRect;
        }

        public override void Update(GameTime gameTime)
        {
            particleSystem.Update();
            if (!mouseReleased)
            {
                if (Main.mouse.RightReleased())
                {
                    mouseReleased = true;
                }
            }

            if (IsPhoenix)
            {
                currFireTime++;

                if (currFireTime == fireFrames)
                {
                    currFireTime = 0;
                    phoenixFires.Add(new Fire(Owner.Position));

                    if (phoenixFires.Count > 5)
                    {
                        phoenixFires.Remove(phoenixFires[0]);
                    }
                }
            }
            else
            {
                if (phoenixFires.Count > 0)
                {
                    currFireTime++;

                    if (currFireTime == fireFrames)
                    {
                        currFireTime = 0;
                        phoenixFires.Remove(phoenixFires[0]);
                    }
                }
            }

            UpdateFires();

            base.Update(gameTime);
        }

        private void UpdateFires()
        {
            foreach (var fire in phoenixFires)
            {
                fire.Update();
                foreach (Entity ent in Main.Entities)
                {
                    if (ent != Owner)
                    {
                        if (fire.rect.Intersects(ent.rect))
                        {
                            ent.TakeDamage(fireDamage);
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sB.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, null, null, null, null, Main.camera.GetTransformation(Main.graphics.GraphicsDevice));
            particleSystem.Draw(sB);

            sB.End();
            phoenixFires.ForEach(fire => fire.Draw(spriteBatch));
            base.Draw(spriteBatch);
        }

        public override void ActivateBasicAbility(Vector2 Target)
        {
            damage = baseBreathDamage * Owner.AbilityPower / 2;
            fireDirection = Vector2.Normalize(Target - Owner.Position);
            particleSystem.EmitterLocation = Owner.Position + fireDirection * (Owner.EntityHeight / 2 + 8);
            particleSystem.GenerateFireParticles(55, fireDirection, 10 - 5);
            particleSpeed = Owner.AbilityPower;
            hitDistance = particleSpeed * 20f;
            inscribedCircleCenter = Owner.Position + fireDirection * hitDistance;

            FireHitEnemies();

            base.ActivateBasicAbility(Target);
        }

        public override void ActivateSecondaryAbility()
        {
            if (mouseReleased)
            {
                if (!IsPhoenix)
                {
                    fireDamage = (float)(baseFireDamage * Owner.AbilityPower) / 5f;
                    Owner.walkingAnimation = phoenixAnimation;
                    Owner.EntityNoClip = true;
                    Owner.EntityWidth = phoenixSpriteSheet.Width / 4;
                    Owner.EntityHeight = phoenixSpriteSheet.Height / 4;
                    Owner.WalkingRect = phoenixWalkingRect;
                    Owner.WalkingOrigin = phoenixWalkingOrigin;
                    IsPhoenix = true;
                }
                else
                {
                    Owner.walkingAnimation = ownerAnimation;
                    Owner.EntityNoClip = false;
                    Owner.EntityWidth = DefaultEntityWidth;
                    Owner.EntityHeight = DefaultEntityHeight;
                    Owner.WalkingRect = ownerWalkingRect;
                    Owner.WalkingOrigin = ownerWalkingOrigin;
                    IsPhoenix = false;
                }
                mouseReleased = false;
            }
            base.ActivateSecondaryAbility();
        }

        private void FireHitEnemies()
        {
            var EntitiesHit =
                from entity in Main.Entities
                where entity != Owner
                where Vector2.Distance(inscribedCircleCenter, entity.Position) <= hitDistance
                select entity;

            EntitiesHit.ForEach(entity => entity.TakeDamage(damage));
        }

        public override void AiControl()
        {
            Entity player = Main.Entities.Find(ent => ent is Player);
            if (Vector2.Distance(Owner.Position, player.Position) > hitDistance + 5)
            {
                (Owner as Enemy).GoToPlayer();
            }
            else
            {
                Owner.StopAnimation(Owner.walkingAnimation);
                ActivateBasicAbility(player.Position);
                Owner.DirectTowardsRotation(MathHelper.ToDegrees(MathAid.FindRotation(Owner.Position, player.Position)));
            }
        }
    }
}