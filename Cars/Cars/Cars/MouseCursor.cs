using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cars
{
    public class MouseCursor
    {
        public MouseState currentmouse;
        public MouseState oldmouse;
        public Vector2 Position;
        //This rectangle will be used to track
        //mouse clickeyboard
        public Rectangle clickRectangle;
        public bool leftIsHeld, rightIsHeld;
        private int screenwidth, screenheight;
        public int scrollWheelValue;
        private double time;
        private GameTime gametime;
        public bool clickedonce;
        private float doubleclicktime;

        /// <summary>
        /// A mouse class with standard functionality
        /// </summary>
        /// <param name="windowwidth">The screen width</param>
        /// <param name="windowheight">The screen height</param>
        /// <param name="doubleClickTime">The amount of time allowed
        /// between double clickeyboard</param>
        public MouseCursor(int windowwidth, int windowheight, float doubleClickTime)
        {
            currentmouse = new MouseState();
            oldmouse = new MouseState();
            Position = new Vector2(windowwidth / 2, windowheight / 2);
            clickRectangle = new Rectangle((int)Position.X,
                (int)Position.Y, 3, 3);
            leftIsHeld = false;
            rightIsHeld = false;
            screenwidth = windowwidth;
            screenheight = windowheight;
            doubleclicktime = doubleClickTime;

        }
        #region Update Mouse
        /// <summary>
        /// This should be called every update cycle
        /// </summary>     
        public void UpdateMouse(GameTime gameTime)
        {
            #region Set Position
            oldmouse = currentmouse;
            currentmouse = Mouse.GetState();
            Position.X = currentmouse.X;
            Position.Y = currentmouse.Y;

            if (clickedonce && time < gametime.TotalGameTime.TotalSeconds)
            {
                clickedonce = false;
            }
            //This is to keep our mouse on the screen
            if (Position.X < 0)
            {
                Position.X = 0;
            }
            else if (Position.X > screenwidth)
            {
                Position.X = screenwidth;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
            }
            else if (Position.Y > screenheight)
            {
                Position.Y = screenheight;
            }
            gametime = gameTime;
            clickRectangle.X = (int)Position.X;
            clickRectangle.Y = (int)Position.Y;
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
        public bool LeftClick()
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
        public bool LeftHeld()
        {
            if (currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Pressed
                && leftIsHeld == true)
            {
                return true;
            }
            else if ((currentmouse.LeftButton == ButtonState.Pressed
                && oldmouse.LeftButton == ButtonState.Pressed) &&
                (currentmouse.X != oldmouse.X || currentmouse.Y != oldmouse.Y))
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

        public bool LeftReleased()
        {
            if (currentmouse.LeftButton == ButtonState.Released && oldmouse.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        public bool RightReleased()
        {
            if (currentmouse.RightButton == ButtonState.Released && oldmouse.RightButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        #region RightHeld
        public bool RightHeld()
        {
            if (currentmouse.RightButton == ButtonState.Pressed
                && oldmouse.RightButton == ButtonState.Pressed
                && rightIsHeld == true)
            {
                return true;
            }
            else if ((currentmouse.RightButton == ButtonState.Pressed
                && oldmouse.RightButton == ButtonState.Pressed) &&
                (currentmouse.X != oldmouse.X || currentmouse.Y != oldmouse.Y))
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
        public bool RightClick()
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
        public bool DoubleClick()
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
    }
}