using System;

namespace EntityRPG
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (EntityRPG game = new EntityRPG())
            {
                game.Run();
            }
        }
    }
#endif
}

