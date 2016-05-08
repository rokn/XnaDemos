using System;

namespace Cars
{
#if WINDOWS || XBOX

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }

#endif
}