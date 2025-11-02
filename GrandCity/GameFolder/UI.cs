using System;
using System.Threading;

namespace CityLifeGameV3
{
    // Konsol vizualizasiyası və animasyon köməkçisi
    public static class UI
    {
        // Simvol əsasında sadə konsol animasyası göstərir
        public static void Animate(string symbol)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Fəaliyyət davam edir: ");
            for (int i = 0; i < 15; i++)
            {
                Console.Write(symbol + " ");
                Thread.Sleep(50); // Daha sürətli animasyon
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ShowMessage(string message, ConsoleColor color = ConsoleColor.Red)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n[INFO] {message}");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1500);
        }
    }
}
