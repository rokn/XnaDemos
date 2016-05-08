using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CuttingEdge
{
	public static class Input
	{
		#region Fields

		private static KeyboardState _currentKeyboard;
		private static KeyboardState _oldKeyboard;
		private static float _doubleclicktime;
		private static int _screenheight;
		private static int _screenwidth;
		private static MouseState _currentmouse;
		private static bool _leftIsHeld, _rightIsHeld;
		private static MouseState _oldmouse;
		private static Vector2 _position;
		private static bool _clickedonce;
		private static GameTime _gametime;
		private static double _time;
		public static int ScrollWheelValue { get; private set; }

		#endregion

		#region Initialize

		/// <summary>
		///     A input class with standard functionality
		/// </summary>
		/// <param name="windowwidth">The screen width</param>
		/// <param name="windowheight">The screen height</param>
		/// <param name="doubleClickTime">
		///     The amount of time allowed
		///     between double click
		/// </param>
		public static void Initialize(int windowwidth, int windowheight, float doubleClickTime)
		{
			_currentmouse = new MouseState();
			_oldmouse = new MouseState();
			_position = new Vector2(windowwidth / 2, windowheight / 2);
			ClickRectangle = new Rectangle((int)_position.X,
				(int)_position.Y, 3, 3);
			_leftIsHeld = false;
			_rightIsHeld = false;
			_screenwidth = windowwidth;
			_screenheight = windowheight;
			_doubleclicktime = doubleClickTime;

			_currentKeyboard = new KeyboardState();
			_oldKeyboard = new KeyboardState();
		}

		#endregion

		#region Properties

		public static Vector2 MousePosition => _position;

		//This rectangle will be used to track
		//mouse click
		public static Rectangle ClickRectangle { get; private set; }

		#endregion

		#region Update

		/// <summary>
		///     This should be called every update cycle
		/// </summary>
		public static void Update(GameTime gameTime)
		{
			#region Set Position

			_oldmouse = _currentmouse;
			_currentmouse = Mouse.GetState();
			_position.X = _currentmouse.X;
			_position.Y = _currentmouse.Y;

			if(_clickedonce && _time < _gametime.TotalGameTime.TotalSeconds)
			{
				_clickedonce = false;
			}
			//This is to keep our mouse on the screen
			if(_position.X < 0)
			{
				_position.X = 0;
			}
			else if(_position.X > _screenwidth)
			{
				_position.X = _screenwidth;
			}
			if(_position.Y < 0)
			{
				_position.Y = 0;
			}
			else if(_position.Y > _screenheight)
			{
				_position.Y = _screenheight;
			}
			_gametime = gameTime;

			ClickRectangle = new Rectangle((int)_position.X,(int)_position.Y,ClickRectangle.Width,ClickRectangle.Height);
			

			#endregion Set Position

			if(_leftIsHeld)
			{
				if(_currentmouse.LeftButton == ButtonState.Released)
				{
					_leftIsHeld = false;
				}
			}
			ScrollWheelValue += _currentmouse.ScrollWheelValue -
								_oldmouse.ScrollWheelValue;

			_oldKeyboard = _currentKeyboard;
			_currentKeyboard = Keyboard.GetState();
		}

		#endregion Update

		#region Left Click

		/// <summary>
		///     Use this to determine if the left button was clicked
		/// </summary>
		/// <returns>if the left button was clicked</returns>
		public static bool LeftClick()
		{
			if(_currentmouse.LeftButton != ButtonState.Pressed || _oldmouse.LeftButton != ButtonState.Released)
				return false;

			_clickedonce = true;
			_time = _gametime.TotalGameTime.TotalSeconds + _doubleclicktime;
			return true;
		}

		#endregion Left Click

		#region Left Held

		/// <summary>
		///     Use this to determine if the left button is being held
		///     The tricky part is figuring out the difference
		///     between a drag and a long click.
		///     What we're doing is testing if we already determined
		///     if it's already being held, or if it's being held and
		///     it's been moved.
		///     Else we assume it's a long click.
		/// </summary>
		/// <returns>If the button is being held</returns>
		public static bool LeftHeld()
		{
			if(_currentmouse.LeftButton == ButtonState.Pressed
				&& _oldmouse.LeftButton == ButtonState.Pressed
				&& _leftIsHeld)
			{
				return true;
			}

			if((_currentmouse.LeftButton != ButtonState.Pressed || _oldmouse.LeftButton != ButtonState.Pressed))
				return false;


			_leftIsHeld = true;
			return true;
		}

		#endregion Left Held

		#region LeftReleased

		public static bool LeftReleased()
		{
			return _currentmouse.LeftButton == ButtonState.Released && _oldmouse.LeftButton == ButtonState.Pressed;
		}

		#endregion

		#region RightReleased

		public static bool RightReleased()
		{
			return _currentmouse.RightButton == ButtonState.Released && _oldmouse.RightButton == ButtonState.Pressed;
		}

		#endregion

		#region RightHeld

		public static bool RightHeld()
		{
			if(_currentmouse.RightButton == ButtonState.Pressed
				&& _oldmouse.RightButton == ButtonState.Pressed
				&& _rightIsHeld)
			{
				return true;
			}

			if((_currentmouse.RightButton != ButtonState.Pressed || _oldmouse.RightButton != ButtonState.Pressed))
				return false;


			_rightIsHeld = true;
			return true;
		}

		#endregion RightHeld

		#region Right Clicked

		/// <summary>
		///     Use this to determine if the right button is
		///     being held
		/// </summary>
		/// <returns>If the right button was clicked</returns>
		public static bool RightClick()
		{
			return _currentmouse.RightButton == ButtonState.Pressed
				   && _oldmouse.RightButton == ButtonState.Released;
		}

		#endregion Right Clicked

		#region DoubleClick
		/// <summary>
		///     Use this to determine if the left button was
		///     double clicked
		/// </summary>
		/// <returns>If the left button was double clicked</returns>
		public static bool DoubleClick()
		{
			if(!_clickedonce)
				return false;

			if(_currentmouse.LeftButton != ButtonState.Pressed || _oldmouse.LeftButton != ButtonState.Released)
				return false;


			_clickedonce = false;
			return true;
		}

		#endregion

		#region HasScrolledUp

		public static bool HasScrolledUp()
		{
			return _oldmouse.ScrollWheelValue < _currentmouse.ScrollWheelValue;
		}

		#endregion

		#region HasScrolledDown

		public static bool HasScrolledDown()
		{
			return _oldmouse.ScrollWheelValue > _currentmouse.ScrollWheelValue;
		}

		#endregion

		#region KeyIsHeld

		/// <summary>
		/// Use this to determine if the key is being held
		/// </summary>
		/// <param name="key">The key to check</param>
		/// <returns>returns true if the key is held.</returns>
		public static bool KeyIsHeld(Keys key)
		{
			return _currentKeyboard.IsKeyDown(key);
		}

		#endregion

		#region KeyIsReleased

		/// <summary>
		/// Use this to check if the key was just released
		/// </summary>
		/// <param name="key">The key to check</param>
		/// <returns>Returns true if the key was released</returns>
		public static bool KeyIsReleased(Keys key)
		{
			return _currentKeyboard.IsKeyUp(key) && _oldKeyboard.IsKeyDown(key);
		}

		#endregion

		#region KeyJustPressed

		/// <summary>
		/// Use this to see if the key was just pressed
		/// </summary>
		/// <param name="key">The key to check</param>
		/// <returns>Returns true if the key was just pressed</returns>
		public static bool KeyJustPressed(Keys key)
		{
			return _currentKeyboard.IsKeyDown(key) && _oldKeyboard.IsKeyUp(key);
		}

		#endregion

	}
}