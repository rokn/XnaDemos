using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG
{
    public class MyMouse : DrawableGameComponent
    {
        private static MouseState currentmouse;
        private static MouseState oldmouse;
        private static Vector2 position;
        private static Color pointerColor;
        private static Color pointerDefaultColor;

        public static Vector2 Position { get { return position; } private set { position = value; } }
        public static Vector2 RealPosition { get { return Main.camera.GetRealMousePosition(); } }

        //This rectangle will be used to track
        //mouse click
        private static bool leftIsHeld, rightIsHeld;
        private static int screenwidth, screenheight;
        public static int scrollWheelValue;
        private static double time;
        private static GameTime gametime;
        private static bool clickedonce;
        private static float doubleclicktime;
        private static Texture2D cursorTexture;


        private SpriteBatch spriteBatch;

        /// <summary>
        /// A mouse class with standard functionality
        /// </summary>
        /// <param name="windowwidth">The screen width</param>
        /// <param name="windowheight">The screen height</param>
        /// <param name="doubleClickTime">The amount of time allowed
        /// between double clickeyboard</param>
        public MyMouse(Main game, Color defaultColor)
            : base(game)
        {
            currentmouse = new MouseState();
            oldmouse = new MouseState();
            position = new Vector2(Main.WindowWidth / 2, Main.WindowHeight / 2);
            leftIsHeld = false;
            rightIsHeld = false;
            screenwidth = Main.WindowWidth;
            screenheight = Main.WindowHeight;            
            doubleclicktime = 150f;
            pointerDefaultColor = defaultColor;
            pointerColor = defaultColor;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Main.GetGraphicsDevice());
            cursorTexture = Scripts.LoadTexture(@"Misc\Mouse");
        }

        #region Update Mouse
        /// <summary>
        /// This should be called every update cycle
        /// </summary>     
        public override void Update(GameTime gameTime)
        {
            #region Set Position
            oldmouse = currentmouse;
            currentmouse = Mouse.GetState();
            position.X = currentmouse.X;
            position.Y = currentmouse.Y;

            if (clickedonce && time < gametime.TotalGameTime.TotalSeconds)
            {
                clickedonce = false;
            }
            //This is to keep our mouse on the screen
            if (position.X < 0)
            {
                position.X = 0;
            }
            else if (position.X > screenwidth)
            {
                position.X = screenwidth;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            else if (position.Y > screenheight)
            {
                position.Y = screenheight;
            }
            gametime = gameTime;
            #endregion

            if (leftIsHeld == true)
            {
                if (currentmouse.LeftButton == ButtonState.Released)
                {
                    leftIsHeld = false;
                }
            }
            scrollWheelValue += currentmouse.ScrollWheelValue -
                oldmouse.ScrollWheelValue;
        }
        #endregion

        #region Left Click
        /// <summary>
        /// Use this to determine if the left button was clicked
        /// </summary>
        /// <returns>if the left button was clicked</returns>
        public static bool LeftClick()
        {
            if (currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Released)
            {
                clickedonce = true;
                time = gametime.TotalGameTime.TotalSeconds + doubleclicktime;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Left Held
        /// <summary>
        /// Use this to determine if the left button is being held
        /// The tricky part is figuring out the difference
        /// between a drag and a long click.
        ///
        /// What we're doing is testing if we already determined
        /// if it's already being held, or if it's being held and
        /// it's been moved.
        /// Else we assume it's a long click.
        /// </summary>
        /// <returns>If the button is being held</returns>
        public static bool LeftHeld()
        {
            if (currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Pressed
                && leftIsHeld == true)
            {
                return true;
            }
            else if ((currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Pressed))
            {
                leftIsHeld = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public static bool LeftReleased()
        {
            if (currentmouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        public static bool RightReleased()
        {
            if (currentmouse.RightButton == ButtonState.Released && oldmouse.RightButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        #region RightHeld
        public static bool RightHeld()
        {
            if (currentmouse.RightButton == ButtonState.Pressed
                && oldmouse.RightButton == ButtonState.Pressed
                && rightIsHeld == true)
            {
                return true;
            }
            else if ((currentmouse.RightButton == ButtonState.Pressed
                && oldmouse.RightButton == ButtonState.Pressed))
            {
                rightIsHeld = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Right Clicked
        /// <summary>
        /// Use this to determine if the right button is
        /// being held
        /// </summary>
        /// <returns>If the right button was clicked</returns>
        public static bool RightClick()
        {
            if (currentmouse.RightButton == ButtonState.Pressed
                && oldmouse.RightButton == ButtonState.Released)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        /// <summary>
        /// Use this to determine if the left button was
        /// double clicked
        /// </summary>
        /// <returns>If the left button was double clicked</returns>
        public static bool DoubleClick()
        {
            if (clickedonce)
            {

                if (currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Released)
                {
                    clickedonce = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void ChangeColor(Color newColor)
        {
            pointerColor = newColor;
        }

        public static void ChangeToDefaultColor()
        {
            pointerColor = pointerDefaultColor;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(cursorTexture, Position, null, pointerColor, 0, new Vector2(), 1f, SpriteEffects.None, 1f);
            spriteBatch.End();
        }
    }
}