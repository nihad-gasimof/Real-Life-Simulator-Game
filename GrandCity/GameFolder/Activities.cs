using System;
using System.Threading;
using System.Linq;

namespace CityLifeGameV3
{
    // İş və səyahət fəaliyyətləri
    public static class Activities
    {
        // İşə getmə: maksimum 2 dəfə/gün
        public static void TryWork()
        {
            if (GameState.WorkCountPerDay >= 2)
            {
                UI.ShowMessage("Bu gün artıq maksimum iş limitinə çatdın. İstirahət etməlisən!", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            Console.WriteLine("Hansı növ işə getmək istəyirsən?");
            Console.WriteLine("1. Gündəlik İşlər (Ofis, Frilanser)");

            // Yaş və kilid yoxlaması
            if (GameState.Age >= 25 && GameState.UnlockedProJob)
            {
                Console.WriteLine("2. Mütəxəssis İşləri (Yüksək gəlir, Yaş > 25)");
            }
            else
            {
                Console.WriteLine("2. 🚫 Mütəxəssis İşləri (Yaşınız çatmır / {0} yaşda açılır)", 25);
            }

            Console.Write("Kateqoriya seç (1 və ya 2): ");
            string categoryChoice = Console.ReadLine() ?? "";

            switch (categoryChoice.Trim())
            {
                case "1": WorkMenu(isPro: false); break;
                case "2":
                    if (GameState.Age >= 25 && GameState.UnlockedProJob) WorkMenu(isPro: true);
                    else UI.ShowMessage("Bu iş kateqoriyası hələ açılmayıb. Yaşın 25 və ya yuxarı olmalıdır.", ConsoleColor.Red);
                    break;
                default: UI.ShowMessage("Yanlış kateqoriya seçimi.", ConsoleColor.Red); break;
            }
        }

        private static void WorkMenu(bool isPro)
        {
            Console.Clear();
            Console.WriteLine(isPro ? "--- Mütəxəssis İşləri (25+ Yaş) ---" : "--- Gündəlik İşlər ---");

            if (!isPro)
            {
                Console.WriteLine("1. 🏢 Ofis işi (orta gəlir, 8 saat)");
                Console.WriteLine("2. ✍️ Frilanser işi (qısa, 4 saat)");
                // Yaşdan asılı olmayaraq, konsol varsa, bu iş açıqdır.
                if (GameState.HasGameConsole) Console.WriteLine("3. 🎮 Oyun tərtibatçısı (yüksək gəlir, 8 saat)");
            }
            else // Mütəxəssis İşləri (Yaş 25+)
            {
                Console.WriteLine("1. 💼 Maliyyə Analitiki (çox yüksək gəlir, 8 saat)");
                Console.WriteLine("2. 💡 Layihə Meneceri (yüksək gəlir, 6 saat)");
            }

            Console.Write("Seçim: ");
            string pick = Console.ReadLine() ?? "";

            int earned = 0;
            int hours = 0;
            string jobTitle = "";
            bool validChoice = true;

            if (!isPro)
            {
                switch (pick.Trim())
                {
                    case "1":
                        hours = 8; earned = GameState.Rand.Next(100, 301); jobTitle = "Ofis işçisi"; UI.Animate("💼"); break;
                    case "2":
                        hours = 4; earned = GameState.Rand.Next(50, 161); jobTitle = "Frilanser"; UI.Animate("⌨️"); break;
                    case "3":
                        if (GameState.HasGameConsole) { hours = 8; earned = GameState.Rand.Next(300, 601); jobTitle = "Oyun Tərtibatçısı"; UI.Animate("🖥️"); }
                        else { validChoice = false; UI.ShowMessage("Bu iş üçün Oyun Konsolu lazımdır.", ConsoleColor.Red); }
                        break;
                    default: validChoice = false; UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); break;
                }
            }
            else // Mütəxəssis İşləri (Yaş 25+)
            {
                switch (pick.Trim())
                {
                    case "1": hours = 8; earned = GameState.Rand.Next(500, 1201); jobTitle = "Maliyyə Analitiki"; UI.Animate("📈"); break;
                    case "2": hours = 6; earned = GameState.Rand.Next(400, 901); jobTitle = "Layihə Meneceri"; UI.Animate("🗓️"); break;
                    default: validChoice = false; UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); break;
                }
            }

            if (validChoice)
            {
                GameState.Balance += earned;
                GameState.WorkCountPerDay++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Təbriklər, {GameState.Name}, {jobTitle} işindən {earned}$ qazandın! ({hours} saat)");
                Console.ForegroundColor = ConsoleColor.White;
                GameState.NextHour(hours);
            }
        }

        // Taksi ilə gəzinti
        public static void Taxi()
        {
            Console.Clear();
            Console.WriteLine("Taksi çağırılır...");
            UI.Animate("🚕");

            Console.WriteLine("Hara getmək istəyirsən? (Qiymət 15$-30$ arası)");
            Console.WriteLine("1. Park (🌳)");
            Console.WriteLine("2. Dənizkənarı (🌊)");
            Console.WriteLine("3. Ticarət Mərkəzi (🏢)");
            Console.WriteLine("4. Kazino (💸) - Yaş > 18 tələb olunur");
            Console.Write("Seçim: ");
            string s = Console.ReadLine() ?? "";

            string dest = s.Trim() switch
            {
                "1" => "Parka",
                "2" => "Dənizkənarına",
                "3" => "Ticarət Mərkəzinə",
                "4" => "Kazinoya",
                _ => "Bilinməyən"
            };

            if (dest == "Bilinməyən") { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }

            // Kazino seçilibsə yaş yoxlaması
            if (dest == "Kazinoya" && GameState.Age < 18)
            {
                UI.ShowMessage("Kazinoya getmək üçün 18 yaşın olmalıdır.", ConsoleColor.Red);
                return;
            }

            int cost = GameState.Rand.Next(15, 31);
            if (GameState.Balance < cost)
            {
                UI.ShowMessage($"Pulun çatmır. Taksi xərci: {cost}$, Balans: {GameState.Balance}$", ConsoleColor.Red);
                return;
            }

            GameState.Balance -= cost;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{dest} çatdın. Xərcləndi: {cost}$ — Qalıq: {GameState.Balance}$");
            UI.Animate("🛣️");
            GameState.NextHour(2);
        }
    }
}
