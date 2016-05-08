using System;
using System.Windows.Forms;

namespace StealthOrNot
{
#if WINDOWS || XBOX

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        [STAThread]
        private static void Main(string[] args)
        {
            using (Main game = new Main())
            {
                //try
                //{
                game.Run();
                //}
                //catch (Exception e)
                //{
                //    MessageBox.Show(e.Message, "The game throwed an exception!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
        }
    }

#endif
}