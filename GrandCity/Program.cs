using System;
using System.Threading;
using System.Linq;

namespace CityLifeGameV3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Kodlaşdırma tənzimləmələri
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "🌆 Şəhər Həyatı RPG — Genişləndirilmiş V3";

            Store.BuildStoreCatalog(); // Mağaza məlumatlarını yüklə

            WelcomeAndSetup();
            GameLoop();
        }

        // Xoş gəlmə və ilkin qurulum
        static void WelcomeAndSetup()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("***********************************");
            Console.WriteLine("    🌆 Şəhər Həyatı — RPG Başlayır    ");
            Console.WriteLine("***********************************");
            Console.ForegroundColor = ConsoleColor.White;

            while (string.IsNullOrWhiteSpace(GameState.Name))
            {
                Console.Write("Adını daxil et: ");
                GameState.Name = Console.ReadLine() ?? "";
            }

            while (true)
            {
                Console.Write("Yaşını daxil et (İstədiyiniz yaşı seçə bilərsiniz): ");
                string s = Console.ReadLine() ?? "";
                // Yaş limitini qaldırırıq, yalnız rəqəm olmasını yoxlayırıq.
                if (int.TryParse(s, out GameState.Age) && GameState.Age > 0 && GameState.Age < 200) break;
                Console.WriteLine("Düzgün yaş daxil et (məsələn: 15, 25).");
            }

            // Başlanğıc ilini təyin et
            GameState.CurrentYear = DateTime.Now.Year;

            // Şəxsiyyət Vəsiqəsi pulsuz və məlumat verildikdən sonra avtomatik verilir.
            GameState.Documents["Şəxsiyyət Vəsiqəsi (ID)"] = true;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Salam, {GameState.Name}! Macəra başlayır... Başlanğıc Balans: {GameState.Balance}$");
            Console.WriteLine($"Başlanğıc il: {GameState.CurrentYear}");
            Console.WriteLine("Şəxsi məlumatlarınızı daxil etdiyiniz üçün Şəxsiyyət Vəsiqəniz avtomatik olaraq verildi. ✅");
            Thread.Sleep(2000);
        }

        // Əsas Oyun Dövrü
        static void GameLoop()
        {
            while (true)
            {
                // YENİ: Oyunçu ölübsə, dövrü sonlandır
                if (GameState.IsDead)
                {
                    Console.WriteLine("\nOyunu sonlandırmaq üçün Enter düyməsini bas...");
                    Console.ReadLine();
                    return;
                }

                // Əgər saat gecdirsə məcburi yuxu
                if (GameState.Hour >= 22)
                {
                    GameState.ForceSleep("Gecdir — məcburi yatış");
                    continue;
                }

                Console.Clear();
                GameState.ShowStatus();

                // YENİ: Hər fəaliyyətdən əvvəl ani təhlükə yoxlaması (1% şans)
                LifeEvents.CheckForImmediateDanger(afterTimeTravel: false);

                // Ölüm yoxlaması yenidən
                if (GameState.IsDead) continue;

                Console.WriteLine("-----------------------------------");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Nə etmək istəyirsən? (Fəaliyyət Seçimi)");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("1. 💼 İşə get (Gündəlik limit: 2 dəfə)");
                Console.WriteLine("2. 🛍️ Mağazaya get (Fiziki alış)");
                Console.WriteLine("3. 💻 Evdən onlayn alış (Ev Shop)");
                Console.WriteLine("4. 🚕 Taksi ilə gəz (Şəhəri kəşf et)");
                Console.WriteLine("5. ☕ Kafeyə get (Yemək/İçmək)");

                // Yaşa görə Kazino/Kart oyunu kilidi
                if (GameState.Age >= 18)
                {
                    Console.WriteLine("6. 🎰 Kazinoya get (18+, Sənəd tələb olunur!)");
                    Console.WriteLine("7. ♠️ Kart oyunu oyna (18+, Blackjack)");
                }
                else
                {
                    Console.WriteLine("6. 🚫 Kazino (Yaşınız (18+) çatmır!)");
                    Console.WriteLine("7. 🚫 Kart oyunu (Yaşınız (18+) çatmır!)");
                }

                Console.WriteLine("8. 📝 Sənədlər Menyu (Pasport, Sürücülük üçün müraciət)");
                Console.WriteLine("9. 💬 Chat (Dostlarla danış / Whatsapp)");
                Console.WriteLine("10. 💤 Dincəl / Yatmaq (Günü sonlandır)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("11. ⏳ Zamanda Səyahət (10 il irəli sıçrayış)"); // YENİ: Zamanda Səyahət
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("12. 🚪 Oyunu bitir");
                Console.Write("Seçimin: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice.Trim())
                {
                    case "1": Activities.TryWork(); break;
                    case "2": Store.OpenStore(inStore: true); break;
                    case "3": Store.OpenStore(inStore: false); break;
                    case "4": Activities.Taxi(); break;
                    case "5": Store.Cafe(); break;
                    case "6":
                        if (GameState.Age >= 18) Casino.OpenCasinoMenu();
                        else UI.ShowMessage("Yaşınız (18+) Kazino üçün çatmır!", ConsoleColor.Red);
                        break;
                    case "7":
                        if (GameState.Age >= 18) Casino.PlayBlackjack();
                        else UI.ShowMessage("Yaşınız (18+) Kart Oyunu üçün çatmır!", ConsoleColor.Red);
                        break;
                    case "8": DocumentsMenu(); break;
                    case "9": Chatbot.ChatbotMenu(); break;
                    case "10": GameState.ForceSleep("İstirahət etdiniz"); break;
                    case "11": LifeEvents.TimeTravelDecision(); break; // YENİ funksiya çağırılır
                    case "12":
                        Console.WriteLine($"Sağ ol, {GameState.Name}! Sonda Balansın: {GameState.Balance}$");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Yanlış seçim, yenidən cəhd et.");
                        Thread.Sleep(700);
                        break;
                }
            }
        }

        // Sənədlər Menyu (Pasport və Sürücülük üçün Müraciət)
        static void DocumentsMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--- 📝 Şəxsi Sənədlər Menyu ---");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"Şəxsi məlumat: Ad: {GameState.Name}, Yaş: {GameState.Age}");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("Cari Sənədlər:");

            int idx = 1;
            foreach (var doc in GameState.Documents)
            {
                Console.WriteLine($"{idx}. {doc.Key}: {(doc.Value ? "✅ VAR" : "❌ YOXDUR")}");
                idx++;
            }

            Console.WriteLine($"\nƏşya Çantası ({GameState.Inventory.Count} əşya):");
            if (GameState.Inventory.Any())
            {
                Console.WriteLine(string.Join(", ", GameState.Inventory));
            }
            else
            {
                Console.WriteLine("Boşdur.");
            }

            // Pasport və ya Sürücülük üçün müraciət etmək
            Console.WriteLine("\nƏlavə Sənədlər üçün Müraciət Et:");
            // Pasport və Sürücülük artıq daxili funksiya kimi idarə olunur.
            Console.WriteLine("1. 🛂 Pasport üçün Müraciət (300$, Yaş 18+)");
            Console.WriteLine("2. 🚗 Sürücülük Vəsiqəsi üçün Müraciət (200$, Yaş 18+)");
            Console.WriteLine("3. Geri");
            Console.Write("Seçim: ");

            string s = Console.ReadLine() ?? "";

            switch (s.Trim())
            {
                case "1": ApplyForDocument("Pasport (Beynəlxalq)", 300, 18); break;
                case "2": ApplyForDocument("Sürücülük Vəsiqəsi", 200, 18); break;
                default: break;
            }
        }

        // Sənəd üçün müraciət prosesi
        static void ApplyForDocument(string docName, int cost, int minAge)
        {
            if (GameState.Documents.TryGetValue(docName, out bool hasDoc) && hasDoc)
            {
                UI.ShowMessage($"{docName} artıq səndə var.", ConsoleColor.Red);
                return;
            }

            if (GameState.Age < minAge)
            {
                UI.ShowMessage($"{docName} almaq üçün minimum yaş {minAge} olmalıdır.", ConsoleColor.Red);
                return;
            }

            if (GameState.Balance < cost)
            {
                UI.ShowMessage($"Pulun çatmır. Tələb olunan xərc: {cost}$", ConsoleColor.Red);
                return;
            }

            // Xərc və Sənəd verilməsi
            GameState.Balance -= cost;
            GameState.Documents[docName] = true;

            UI.ShowMessage($"Təbriklər! {docName} üçün müraciət uğurla tamamlandı. Xərc: {cost}$", ConsoleColor.Green);
            GameState.NextHour(2); // Proses 2 saat vaxt aparır
        }
    }
}
