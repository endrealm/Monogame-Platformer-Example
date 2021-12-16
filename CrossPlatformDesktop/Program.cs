using System;

namespace CrossPlatformDesktop
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new DesktopGame();
            game.Run();
        }
    }
}