using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using EntityPong.Components;
using Microsoft.Xna.Framework.Graphics;

namespace EntityPong.Systems
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    internal class SpriteRenderSystem : EntityProcessingSystem<SpriteComponent,TransformComponent>
    {
        private SpriteBatch spriteBatch;

        public override void LoadContent()
        {
            spriteBatch = EntitySystem.BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
        }

        protected override void Process(Entity entity, SpriteComponent sprite, TransformComponent transform)
        {
            if(sprite.Visible)
            {
                spriteBatch.Draw(sprite.texture, transform.Position, null, sprite.color, transform.Rotation, sprite.Origin, transform.Scale, sprite.Effects, sprite.Depth);
            }
        }
    }
}
