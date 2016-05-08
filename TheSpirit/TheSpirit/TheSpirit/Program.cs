using System;

namespace TheSpirit
{
#if WINDOWS || XBOX

    internal static class Program
    {
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