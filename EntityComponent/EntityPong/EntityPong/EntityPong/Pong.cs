using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Artemis;
using Artemis.System;
using EntityPong.Components;

namespace EntityPong
{
    public class Pong : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private EntityWorld world;
        private int WindowWidth;
        private int WindowHeight;

        public Pong()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WindowWidth = graphics.PreferredBackBufferWidth;
            WindowHeight = graphics.PreferredBackBufferHeight;

            world = new EntityWorld();

            EntitySystem.BlackBoard.SetEntry("Content", Content);
            EntitySystem.BlackBoard.SetEntry("GraphicsDevice", GraphicsDevice);
            EntitySystem.BlackBoard.SetEntry("WindowWidth", WindowWidth);
            EntitySystem.BlackBoard.SetEntry("WindowHeight", WindowHeight);
            EntitySystem.BlackBoard.SetEntry("SpriteBatch", spriteBatch);

            this.Components.Add(new MyMouse(this, Color.Red));
            this.Components.Add(new MyKeyboard(this));

            world.InitializeAll(true);

            CreatePaddle(new Vector2(20, WindowHeight/2)).Tag = Tags.PLAYER;
            CreatePaddle(new Vector2(WindowWidth - 20, WindowHeight/2)).Tag = Tags.ENEMY;

            base.Initialize();
        }

        protected override void LoadContent()
        {            
        }

        protected override void UnloadContent()
        {
            world.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            world.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            world.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Entity CreatePaddle(Vector2 position)
        {
            Entity entity = world.CreateEntity();
            SpriteComponent sprite = new SpriteComponent("Paddle");
            sprite.Origin = new Vector2(sprite.texture.Width / 2, sprite.texture.Height / 2);

            entity.AddComponent(new TransformComponent(position));
            entity.AddComponent(sprite);
            entity.AddComponent(new DimensionComponent(sprite.texture.Width, sprite.texture.Height));

            return entity;
        }

        private Entity CreateBall(Vector2 position)
        {

        }
    }
}
