using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace CityLifeGameV3
{
    // Bütün oyun vəziyyətini (state) saxlayan və vaxtı idarə edən statik sinif.
    public static class GameState
    {
        // Oyun vəziyyəti dəyişənləri
        public static int Balance = 500;
        public static int Hour = 6;
        public static int Day = 1;
        public static string Name = "";
        public static int Age = 1; // Yaş limitini tətbiq etmək üçün
        public static Random Rand = new Random();

        public static int CurrentYear = 2024; // YENİ: Oyunda mövcud il
        public static bool IsDead = false; // YENİ: Ölüm vəziyyəti

        public static int DaysSinceBirthday = 0; // Yaş proqressi üçün sayğac

        // Gün ərzində iş sayını saxla (reset gündə)
        public static int WorkCountPerDay = 0;

        // Inventory & Documents
        public static List<string> Inventory = new List<string>();
        // Sənədlər: Adı, status (True = Var). Başlanğıcda ID almaq üçün hər şey False.
        public static Dictionary<string, bool> Documents = new Dictionary<string, bool>
        {
            { "Şəxsiyyət Vəsiqəsi (ID)", false }, // Proqram başlayanda verilir
            { "Pasport (Beynəlxalq)", false },
            { "Sürücülük Vəsiqəsi", false }
        };

        // Unlocks
        public static bool HasGameConsole = false;
        public static bool UnlockedProJob = false; // Yaşa görə açılan yeni iş

        // Mağaza Kataloqu (Item.cs tələb edir)
        public static Dictionary<string, Dictionary<string, List<Item>>> StoreCatalog = new Dictionary<string, Dictionary<string, List<Item>>>();

        // Oyunun cari statusunu konsola çıxarır (daha vizual formatda)
        public static void ShowStatus()
        {
            CheckAgeUnlocks(); // Hər status göstəriləndə kilidləri yoxla

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔═════════════════════════════════════════════════════╗");
            Console.WriteLine($"║ 👤 {Name}, {Age} yaş | 📅 Gün: {Day} | 🕒 Saat: {Hour:00}:00 ({36 - DaysSinceBirthday} günə ad günü) ║");
            Console.WriteLine($"║ 💰 Balans: {Balance}$ | İl: {CurrentYear} | İş limiti: {WorkCountPerDay}/2 | Əşyalar: {Inventory.Count} ║"); // CurrentYear əlavə edildi
            Console.WriteLine($"║ Status: ID: {(Documents["Şəxsiyyət Vəsiqəsi (ID)"] ? "✅" : "❌")} | Pro İş: {(UnlockedProJob ? "✅" : "❌")} | Konsol: {(HasGameConsole ? "✅" : "❌")} ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Yaşa görə funksiyaları açır
        public static void CheckAgeUnlocks()
        {
            // Mütəxəssis İşləri 25 yaşdan sonra açılır
            if (Age >= 25 && !UnlockedProJob)
            {
                UnlockedProJob = true;
                UI.ShowMessage($"TƏBRİKLƏR! {Age} yaşın tamam oldu. Yeni iş kateqoriyası açıldı: Mütəxəssis İşləri.", ConsoleColor.Magenta);
            }
        }

        // Yatış funksiyası (məcburi və ya könüllü)
        public static void ForceSleep(string reason)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"*** {reason} ***");
            Console.WriteLine($"{Name}, yuxu gəlir... Gecəniz xeyrə!");
            UI.Animate("🌙"); // UI sinifindən animasyon çağırılır

            // Gündəlik iş sayını sıfırla — yeni gün başlayır
            WorkCountPerDay = 0;
            Day++;
            Hour = 6;

            DaysSinceBirthday++;
            if (DaysSinceBirthday >= 36)
            {
                Age++;
                DaysSinceBirthday = 0;
                CurrentYear++; // YENİ: İl dəyişir
                UI.ShowMessage($"AD GÜNÜN MÜBARƏK! Artıq {Age} yaşın var. İl: {CurrentYear}", ConsoleColor.Green);
                CheckAgeUnlocks(); // Yaş dəyişəndə kilidləri yoxla
            }

            Console.WriteLine($"\n*** Səhər oldu! Yeni gün: {Day} | Yaş: {Age} | İl: {CurrentYear} ***");
            Thread.Sleep(2000);
        }

        // Saatı artır və gün keçidini idarə edir
        public static void NextHour(int add = 1)
        {
            Hour += add;
            if (Hour >= 24)
            {
                // Məcburi yatış 24:00-da
                ForceSleep("Vaxt başa çatdı");
            }

            // Gecə vaxtı xəbərdarlığı
            if (Hour >= 22 && Hour < 24)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\nSaat {Hour:00}:00 — artıq gecdir. Yatmağınız tövsiyə olunur.");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("\nDavam etmək üçün Enter düyməsini bas...");
            Console.ReadLine();
        }
    }
}
