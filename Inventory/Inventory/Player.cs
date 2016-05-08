using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;

namespace Rpg
{
    public class Player : Entity
    {
        public Classes currClass;
        Animation walkingAnim;
        Texture2D Hair;
        List<Texture2D> animationList;
        bool goingForItem,forMouse,collectingItems;
        Item targetItem;
        public bool loaded;
        public string name;
        public Player(Vector2 position):base(position)
        {
            animationList = new List<Texture2D>();
            goingForItem = false;
            forMouse = false;
            collectingItems = false;
            loaded = false;
        }
        public override void Load(ContentManager Content)
        {
            standing = Content.Load<Texture2D>(@"Character\Player_0");
            for (int i = 0; i <= 6; i++)
            {
                animationList.Add(Content.Load<Texture2D>(@"Character\Player_" + i.ToString()));
            }
            walkingAnim = new Animation(animationList,15-(int)moveSpeed,true);
            Hair = Content.Load<Texture2D>(@"Character\Hair_0");
            loaded = true;
        }
        public override void Update(GameTime gameTime)
        {
            if(Rpg.mouse.RightHeld()||Rpg.mouse.RightClick())
            {
                if(collectingItems)
                {
                    collectingItems = !collectingItems;
                }
                Target = Rpg.mouse.Position;
                if(goingForItem)
                {
                    goingForItem = false;
                }
                if (Vector2.Distance(Position, Target) > moveSpeed+3)
                {
                    if (!Walking)
                    {
                        Walking = true;
                        walkingAnim.GhangeAnimatingState(true);
                        walkingAnim.SetPosition(Position);
                    }
                    angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, Target));
                }
            }
            if (Walking)
            {
                if (Vector2.Distance(Position, Target)<=moveSpeed+1)
                {
                    Walking = false;
                    walkingAnim.GhangeAnimatingState(false);
                    if(goingForItem)
                    {
                        if (!forMouse)
                        {
                            if(Rpg.inv.AddItem(targetItem))
                            {
                                Rpg.items.Remove(targetItem);
                            }
                            else
                            {
                                collectingItems = false;
                            }
                        }
                        else
                        {
                            if (Rpg.hasItemAtMouse == false)
                            {
                                Rpg.hasItemAtMouse = true;
                                Rpg.mouseItem = targetItem;
                                Rpg.items.Remove(targetItem);
                            }
                            forMouse = false;
                        }
                        goingForItem = false;
                        if (collectingItems)
                        {
                            if(Rpg.items.Count>0)
                            {
                                ChooseItem();
                            }
                            else
                            {
                                collectingItems = false;
                            }
                        }
                    }
                }
                Position.X += (float)Math.Cos(MathHelper.ToRadians(angle)) * moveSpeed;
                Position.Y += (float)Math.Sin(MathHelper.ToRadians(angle)) * moveSpeed;
                walkingAnim.Update(Position, MathHelper.ToRadians(angle));
            }
            if(Rpg.keyboard.IsHeld(Keys.LeftShift))
            {
                if (Rpg.keyboard.IsHeld(Keys.C))
                {
                    if(Rpg.items.Count>0)
                    {
                        collectingItems = true;
                        ChooseItem();
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Walking)
            {
                walkingAnim.Draw(spriteBatch,0.4f);
            }
            else
            {
                spriteBatch.Draw(standing,Position, null, Color.White,MathHelper.ToRadians(angle), new Vector2(standing.Width/2,standing.Height/2),1f, SpriteEffects.None, 0.4f);
            }
            spriteBatch.Draw(Hair,Position, null, Color.White,MathHelper.ToRadians(angle), new Vector2(Hair.Width/2,Hair.Height/2),1f, SpriteEffects.None, 0.400001f);
        }

        public void CollectItem(Item item)
        {
            Target = item.position;
            if (Vector2.Distance(Position, Target) > moveSpeed + 3)
            {
                if (!Walking)
                {
                    Walking = true;
                    walkingAnim.GhangeAnimatingState(true);
                    walkingAnim.SetPosition(Position);
                }
                angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, Target));
                targetItem = item;
                goingForItem = true;
            }
            else
            {
                if (Rpg.inv.AddItem(item))
                {
                    Rpg.items.Remove(item);
                }
                else
                {
                    collectingItems = false;
                }
            }
        }

        public void CollectItemToMouse(Item item)
        {
            Target = item.position;
            if (Vector2.Distance(Position, Target) > moveSpeed + 3)
            {
                if (!Walking)
                {
                    Walking = true;
                    walkingAnim.GhangeAnimatingState(true);
                    walkingAnim.SetPosition(Position);
                }
                angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, Target));
                targetItem = item;
                goingForItem = true;
                forMouse = true;
            }
            else
            {
                if (Rpg.hasItemAtMouse == false)
                {
                    Rpg.hasItemAtMouse = true;
                    Rpg.mouseItem = item;
                    Rpg.items.Remove(item);
                }
            }
        }

        void ChooseItem()
        {
            float minLength = int.MaxValue;
            int index = 0, i = 0;
            Item item;
            foreach (Item it in Rpg.items)
            {
                if (Vector2.Distance(Position, it.position) < minLength)
                {
                    minLength = Vector2.Distance(Position, it.position);
                    index = i;
                }
                i++;
            }
            item = Rpg.items[index];
            Target = item.position;
            if (Vector2.Distance(Position, Target)>moveSpeed+1)
            {
                if (!Walking)
                {
                    Walking = true;
                    walkingAnim.GhangeAnimatingState(true);
                    walkingAnim.SetPosition(Position);
                }
                angle = MathHelper.ToDegrees(MathAid.FindRotation(Position, Target));
                targetItem = item;
                goingForItem = true;
            }
            else
            {
                bool tr = Rpg.inv.AddItem(item);
                if(tr)
                {
                    Rpg.items.Remove(item);
                }
                else
                {
                    collectingItems = false;
                }
                if (collectingItems)
                {
                    if (Rpg.items.Count > 0)
                    {
                        ChooseItem();
                    }
                    else
                    {
                        collectingItems = false;
                    }
                }
            }
        }
    }
}
