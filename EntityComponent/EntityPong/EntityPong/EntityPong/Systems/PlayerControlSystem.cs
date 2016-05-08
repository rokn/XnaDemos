using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using EntityPong.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EntityPong.Systems
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    internal class PlayerControlSystem : TagSystem
    {
        int windowHeight;

        public PlayerControlSystem()
            : base("PLAYER") { }

        public override void LoadContent()
        {
            windowHeight = EntitySystem.BlackBoard.GetEntry<int>("WindowHeight");
        }

        public override void Process(Entity entity)
        {
            int down = Convert.ToInt32(MyKeyboard.IsHeld(Keys.Down) || MyKeyboard.IsHeld(Keys.S));
            int up = Convert.ToInt32(MyKeyboard.IsHeld(Keys.Up) || MyKeyboard.IsHeld(Keys.W));
            float ms = (float)TimeSpan.FromTicks(this.EntityWorld.Delta).Milliseconds;
            int playerHeight = entity.GetComponent<DimensionComponent>().Height;

            TransformComponent transform = entity.GetComponent<TransformComponent>();
            transform.Position.Y += (down - up) * 0.4f * ms;


            if (transform.Position.Y < playerHeight / 2)
            {
                transform.Position.Y = playerHeight / 2;
            }
            else if (transform.Position.Y > windowHeight - playerHeight / 2)
            {
                transform.Position.Y = windowHeight - playerHeight / 2;
            }
        }
    }
}