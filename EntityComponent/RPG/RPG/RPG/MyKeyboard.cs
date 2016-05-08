using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RPG
{
    public class MyKeyboard : DrawableGameComponent
    {
        private static KeyboardState currentKeyboard;
        private static KeyboardState oldKeyboard;

        public MyKeyboard(Main game)
            :base(game)
        {
            currentKeyboard = new KeyboardState();
            oldKeyboard = new KeyboardState();
        }

        public override void Update(GameTime gameTime)
        {
            oldKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
        }

        /// <summary>
        /// Use this to determine if the key is being held
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>returns true if the key is held.</returns>
        public static bool IsHeld(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Use this to check if the key was just released
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was released</returns>
        public static bool IsReleased(Keys key)
        {
            if (currentKeyboard.IsKeyUp(key) && oldKeyboard.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Use this to see if the key was just pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key was just pressed</returns>
        public static bool JustPressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
