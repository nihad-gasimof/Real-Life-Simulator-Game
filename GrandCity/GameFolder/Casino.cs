using System;
using System.Threading;

namespace CityLifeGameV3
{
    // Kazino və qumar fəaliyyətləri
    public static class Casino
    {
        // Casino menyusu
        public static void OpenCasinoMenu()
        {
            // Yaş yoxlaması
            if (GameState.Age < 18)
            {
                UI.ShowMessage("Kazino yalnız 18 yaşından yuxarı şəxslər üçündür.", ConsoleColor.Red);
                return;
            }

            // Sənəd yoxlaması
            if (!GameState.Documents["Şəxsiyyət Vəsiqəsi (ID)"])
            {
                UI.ShowMessage("Kazino giriş üçün Şəxsiyyət Vəsiqəsi (ID) tələb edir. Sənədlər menyusundan almalısan.", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            Console.WriteLine("💸 Kazinoya xoş gəlmisən! Bəxtini sına!");
            Console.WriteLine($"Cari Balansın: {GameState.Balance}$");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("1. Sadə mərc (55% qazanma şansı)");
            Console.WriteLine("2. Riskli mərc (40% qazanma şansı, daha böyük qazanc)");
            Console.WriteLine("3. Kart oyunu (Blackjack)");
            Console.WriteLine("4. Geri");
            Console.Write("Seçim: ");
            string s = Console.ReadLine() ?? "";

            switch (s.Trim())
            {
                case "1": SimpleBet(risk: false); break;
                case "2": SimpleBet(risk: true); break;
                case "3": PlayBlackjack(); break;
                default: break;
            }
        }

        // Sadə mərc
        private static void SimpleBet(bool risk)
        {
            Console.Clear();
            Console.WriteLine(risk ? "🔥 Riskli mərc seçildi" : "🍀 Sadə mərc seçildi");
            int stake = AskForStake();
            if (stake <= 0) return;

            UI.Animate("🎲");
            double winChance = risk ? 0.4 : 0.55;

            if (GameState.Rand.NextDouble() < winChance)
            {
                double multiplier = risk ? (1.5 + GameState.Rand.NextDouble() * 0.7) : (0.5 + GameState.Rand.NextDouble() * 0.5);
                int win = (int)(stake * multiplier);
                GameState.Balance += win;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"🎉 Qazandın! Məbləğ: +{win}$  Yeni balans: {GameState.Balance}$");
            }
            else
            {
                GameState.Balance -= stake;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"💀 Uduzdun: -{stake}$  Yeni balans: {GameState.Balance}$");
            }
            Console.ForegroundColor = ConsoleColor.White;
            GameState.NextHour(1);
        }

        // Mərc miqdarını soruş
        public static int AskForStake()
        {
            int stake = 0;
            while (true)
            {
                Console.Write($"Mərc qoymaq istədiyin məbləği daxil et ($) - (Cari: {GameState.Balance}$) (0 = imtina): ");
                string s = Console.ReadLine() ?? "";

                if (!int.TryParse(s, out stake)) { UI.ShowMessage("Zəhmət olmasa rəqəm daxil et.", ConsoleColor.Red); continue; }
                if (stake == 0) return 0;
                if (stake < 10) { UI.ShowMessage("Minimum mərc 10$ olmalıdır.", ConsoleColor.Red); continue; }
                if (stake > GameState.Balance) { UI.ShowMessage($"Məbləğ balansından böyükdür. Balans: {GameState.Balance}$", ConsoleColor.Red); continue; }

                return stake;
            }
        }

        // Sadə Blackjack oyunu
        public static void PlayBlackjack()
        {
            // Yaş yoxlaması
            if (GameState.Age < 18)
            {
                UI.ShowMessage("Kart oyunu (Blackjack) yalnız 18 yaşından yuxarı şəxslər üçündür.", ConsoleColor.Red);
                return;
            }

            // Sənəd yoxlaması
            if (!GameState.Documents["Şəxsiyyət Vəsiqəsi (ID)"])
            {
                UI.ShowMessage("Blackjack masası üçün Şəxsiyyət Vəsiqəsi (ID) tələb olunur.", ConsoleColor.Red);
                return;
            }

            Console.Clear();
            Console.WriteLine("♠️ Blackjack Oyunu Başlayır!");
            int stake = AskForStake();
            if (stake <= 0) return;

            int playerTotal = 0;
            int dealerTotal = 0;

            // Kartları payla
            playerTotal += DrawCard(); playerTotal += DrawCard();
            dealerTotal += DrawCard(); dealerTotal += DrawCard();

            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Sənin başlanğıc kart cəmin: {playerTotal}");
            // Burada DrawCardPreview-in neçə çıxdığını bilmək çətin olduğu üçün, sadəcə açıq kartı göstərək:
            Console.WriteLine($"Dilerin açıq kartı: {dealerTotal / 2} + (Gizli Kart)");

            // Oyunçunun növbəsi
            bool playerBust = false;
            while (true)
            {
                Console.Write("HIT (h) yoxsa STAND (s)? ");
                string action = (Console.ReadLine() ?? "").Trim().ToLower();
                if (action == "h")
                {
                    int c = DrawCard();
                    playerTotal += c;
                    Console.WriteLine($"Yeni Kart: {c}  — Ümumi Cəm: {playerTotal}");
                    if (playerTotal > 21) { playerBust = true; break; }
                }
                else if (action == "s") break;
                else Console.WriteLine("h və ya s yaz.");
            }

            // Dilerin növbəsi
            while (dealerTotal < 17 && !playerBust)
            {
                int c = DrawCard();
                dealerTotal += c;
                Thread.Sleep(500);
            }

            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Sənin son cəmin: {playerTotal}");
            Console.WriteLine($"Dilerin son cəmi: {dealerTotal}");
            Console.WriteLine("-----------------------------------");

            // Nəticəni müəyyən et
            if (playerBust)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sən 21-i keçdin (Bust)! Mərc itirildi.");
                GameState.Balance -= stake;
            }
            else
            {
                if (dealerTotal > 21 || playerTotal > dealerTotal)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Qazandın! Balansına +{0}$ əlavə edildi.", stake);
                    GameState.Balance += stake;
                }
                else if (playerTotal == dealerTotal)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Heç-heçə (Push). Mərc geri qaytarıldı.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Diler qalib gəldi. Uduzdun.");
                    GameState.Balance -= stake;
                }
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Yeni balans: {GameState.Balance}$");
            Console.ForegroundColor = ConsoleColor.White;
            GameState.NextHour(2); // Blackjack 2 saat vaxt aparır
        }

        // Sadə kart çəkmə (1-11)
        private static int DrawCard() => GameState.Rand.Next(1, 12);
    }
}
