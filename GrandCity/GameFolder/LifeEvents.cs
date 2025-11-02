using System;
using System.Threading;
// Silinmiş: using System.Net.Http;
// Silinmiş: using System.Text;
// Silinmiş: using System.Text.Json;
// Silinmiş: using System.Collections.Generic;

namespace CityLifeGameV3
{
    // Həyat hadisələri, təhlükə və ölüm funksiyaları
    public static class LifeEvents
    {
        // Əvvəlcədən təyin edilmiş proqnozların siyahısı
        private static readonly string[] DefaultPredictions = new string[]
        {
            "Gələcəyin parlayır. Balansın xeyli artacaq, lakin yeni iş yeri tapmaq səni çətinlikdə qoya bilər.",
            "Qarşıdakı 10 il həm maliyyə sabitliyi gətirəcək, həm də səhhətində ciddi risklərlə üzləşəcəksən. Ehtiyatlı ol!",
            "Böyük bir eşq səni gözləyir, ancaq bu, ailənin narazılığına səbəb ola bilər. Pul vəziyyətin sabit qalacaq.",
            "Şəxsi inkişaf sahəsində böyük uğurlar əldə edəcəksən. Bəzi köhnə dostluqlar isə sınaqdan keçəcək.",
            "Uğurlu bir karyera sıçrayışı olacaq, lakin bu stress səviyyəni artıracaq. Həyat tərzi dəyişikliyi vacibdir."
        };

        // Yeni default proqnoz funksiyası (API-siz)
        private static string GetDefaultFuturePrediction()
        {
            // GameState.Rand ilə təsadüfi proqnoz seçilir
            int index = GameState.Rand.Next(DefaultPredictions.Length);
            return DefaultPredictions[index];
        }

        // 10 illik zamanda səyahətə qərar vermə
        public static void TimeTravelDecision()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- ⏳ ZAMANDA SƏYAHƏT: 10 İL İRƏLİ ---");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Hazırki il: {GameState.CurrentYear}, Yaşın: {GameState.Age}");
            Console.WriteLine("Qərar versən, həyatında 10 il irəli sıçrayacaqsan.");
            Console.WriteLine($"Yeni yaşın: {GameState.Age + 10}, Yeni il: {GameState.CurrentYear + 10} olacaq.");
            Console.Write("Bu səyahətə getmək istəyirsən? (y/n): ");
            string ans = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (ans != "y")
            {
                UI.ShowMessage("Zamanda səyahətdən imtina edildi.", ConsoleColor.Red);
                return;
            }
            // Təhlükə proqnozu
            Console.WriteLine("\n*** 🔮 Gələcək proqnozu yoxlanılır... ***");
            UI.Animate("⏳");

            // Default proqnoz alınır
            string prediction = GetDefaultFuturePrediction();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n[PROQNOZ]: Gələcək proqnozlaşdırıcısı deyir:");
            Console.WriteLine(prediction);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nBu proqnozlara baxmayaraq, yenə də 10 il irəli getmək istəyirsən? (y/n): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (confirm == "y")
            {
                PerformTimeTravel(10);
                // Vaxt səyahətindən sonra Təhlükəli Hadisə riski yoxlanılır (5% ölüm şansı)
                CheckForImmediateDanger(afterTimeTravel: true);
            }
            else
            {
                UI.ShowMessage("Səyahət riskli göründü, ləğv edildi.", ConsoleColor.Red);
            }
            GameState.NextHour(1); // Vaxt səyahəti qərarı 1 saat aparır.
        }

        // Əsl zamanda səyahət
        private static void PerformTimeTravel(int years)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"*** 🚀 {years} İL İRƏLİ SÇRAYIŞ BAŞLAYIR! ***");
            UI.Animate("⚡");

            GameState.Age += years;
            GameState.CurrentYear += years;

            // Balans dəyişiklikləri: Zamanla əşyaların dəyəri azalır, pul yığımı artır
            int balanceIncrease = GameState.Rand.Next(500, 2000);
            GameState.Balance = (int)(GameState.Balance * 1.5) + balanceIncrease;

            GameState.DaysSinceBirthday = 0;
            GameState.Day = 1;
            GameState.Hour = 6;
            GameState.WorkCountPerDay = 0;
            // Sənədlər qüvvədə qalır.

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSəyahət tamamlandı!");
            Console.WriteLine($"Yeni Yaş: {GameState.Age}, Yeni İl: {GameState.CurrentYear}, Yeni Balans: {GameState.Balance}$");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(3000);
            GameState.CheckAgeUnlocks();
        }

        // Təhlükəli Hadisə yoxlaması
        public static void CheckForImmediateDanger(bool afterTimeTravel = false)
        {
            // Yoxlama yalnız oyunçu hələ ölməyibsə aparılır
            if (GameState.IsDead) return;

            // Təhlükə şansı (Adi gün: 1%, Səyahətdən sonra: 5%)
            double dangerChance = afterTimeTravel ? 0.05 : 0.01;
            if (GameState.Rand.NextDouble() < dangerChance)
            {
                string eventType = afterTimeTravel ? "Ciddi Xəstəlik" : "Avtomobil Qəzası";
                HandleDeath(eventType);
            }
        }

        // Ölüm funksiyası (oyunu bitirir)
        public static void HandleDeath(string cause)
        {
            GameState.IsDead = true;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("█████████████████████████████████████████████");
            Console.WriteLine($"*** 💀 TRAJEDİ: OYUN BİTDİ 💀 ***");
            Console.WriteLine($"*** {GameState.Name} ({GameState.Age} yaş) həyatını itirdi ***");
            Console.WriteLine($"Ölüm Səbəbi: {cause}");
            Console.WriteLine($"Hadisənin baş verdiyi il: {GameState.CurrentYear}");
            Console.WriteLine($"Son Balans: {GameState.Balance}$");
            Console.WriteLine("Həyat macərası burada sona çatır...");
            Console.WriteLine("█████████████████████████████████████████████");
            Console.ForegroundColor = ConsoleColor.White;
            // Əsas oyun dövrü bunu yoxlayıb proqramı bağlayacaq
            Thread.Sleep(5000);
        }
    }
}
