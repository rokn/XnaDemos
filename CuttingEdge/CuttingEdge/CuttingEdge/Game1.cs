using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CuttingEdge
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
	    private Texture2D _line;
	    private int _windowWidth;
	    private int _windowHeight;
	    private float[] _sizes;

		public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
			GoFullscreenBorderless();
			IsMouseVisible = true;
        }

        protected override void Initialize()
        {
			Input.Initialize(_windowWidth,_windowHeight,100f);
			_sizes = new float[720];
	        for (int i = 0; i < 720; i++)
	        {
		        _sizes[i] = 1.0f;
	        }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
	        _line = Content.Load<Texture2D>("Line");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
			Input.Update(gameTime);
	        HandleInput();
            base.Update(gameTime);
        }

		private void GoFullscreenBorderless()
		{
			IntPtr hWnd = this.Window.Handle;
			var control = System.Windows.Forms.Control.FromHandle(hWnd);
			var form = control.FindForm();
			if(form != null)
			{
				form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			}
			_windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			_windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
		}

		protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
	        _spriteBatch.Begin();

			int b = 0;
	        for (float i = 0; i < 360; i+=0.5f)
	        {
		        _spriteBatch.Draw(_line, new Vector2(250, 250), null, Color.Brown, (float)Math.PI/180.0f*i, new Vector2(), new Vector2(_sizes[b++],1f), SpriteEffects.None, 1.0f);
			}

	        _spriteBatch.End();
            base.Draw(gameTime);
        }

	    private void HandleInput()
	    {
		    if (Input.LeftHeld())
		    {
			    float angle = MathAid.FindRotation(new Vector2(250, 250), Input.MousePosition)*180.0f/(float) Math.PI + 90;
			    _sizes[(int) angle] = Vector2.Distance(new Vector2(250, 250), Input.MousePosition) / 100.0f;
		    }
	    }

	}
}
