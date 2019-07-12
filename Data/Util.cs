using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTestServerSide.TfsIterations
{
    public static class Util
    {
        public static void WriteLog(string text, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
