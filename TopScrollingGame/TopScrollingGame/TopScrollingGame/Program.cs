namespace TopScrollingGame
{
#if WINDOWS || XBOX

    internal static class Program
    {
        private static void Main()
        {
            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }

#endif
}