﻿using System;

namespace MonoGameCross_PlatformDesktopApplication1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Game1();
            game.Run();
        }
    }
}