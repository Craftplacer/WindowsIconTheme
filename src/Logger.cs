using System;

namespace IconConv
{
    public static class Logger
    {
        private static void Log(string message) => Console.WriteLine(message);

        public static void Log(string message, ConsoleColor? color = null)
        {
            if (color.HasValue)
                Console.ForegroundColor = color.Value;
            else
                Console.ResetColor();
            
            Log(message);
        }

        public static void Warning(string message) => Log(message, ConsoleColor.Yellow);

        public static void Error(string message) => Log(message, ConsoleColor.Red);
    }
}