using System;
namespace api {
    public class ApiLogger
    {
        public static void Log(object message){
            Console.ForegroundColor = ConsoleColor.Green;
            Print(message);
        }
        public static void Warn(object message){
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Print(message);
        }
        public static void Error(object message){
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Print(message);
        }
        private static void Print(object message){
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}