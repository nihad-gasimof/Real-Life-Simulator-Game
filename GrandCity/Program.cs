using System;
using System.Threading;
using System.Linq;

namespace CityLifeGameV3
{
    internal class Program
    {
        // QEYD: Bütün şəxsi məlumatlar artıq GameState.cs-də saxlanılır və buradan istinad edilir.
        // Program.cs-dəki keçici sahələr ləğv edildi, GameState istifadə olunur.

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
            Console.WriteLine("    🌆 Şəhər Həyatı — RPG Başlayır    ");
            Console.WriteLine("***********************************");
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("\n--- 📝 Şəxsiyyət Vəsiqəsi Məlumatlarının Daxil Edilməsi ---");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Bu məlumatlar oyun daxilində sənədləşmə üçün istifadə olunacaq.");
            Console.ForegroundColor = ConsoleColor.White;

            // 1. Ad (Name)
            while (string.IsNullOrWhiteSpace(GameState.Name))
            {
                Console.Write("1. Adınızı daxil edin: ");
                GameState.Name = Console.ReadLine() ?? "";
            }

            // 2. Soyad (Surname)
            while (string.IsNullOrWhiteSpace(GameState.PlayerSurname))
            {
                Console.Write("2. Soyadınızı daxil edin: ");
                GameState.PlayerSurname = Console.ReadLine() ?? "";
            }

            // 3. Yaş (Age)
            while (true)
            {
                Console.Write("3. Yaşınızı daxil edin (məsələn: 25): ");
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out GameState.Age) && GameState.Age > 0 && GameState.Age < 200) break;
                Console.WriteLine("Düzgün yaş daxil edin.");
            }

            // 4. Doğum Tarixi (Date of Birth)
            while (string.IsNullOrWhiteSpace(GameState.PlayerDateOfBirth))
            {
                Console.Write("4. Doğum Tarixinizi daxil edin (Məsələn: 1990-10-25): ");
                GameState.PlayerDateOfBirth = Console.ReadLine() ?? "";
            }

            // 5. Ünvan (Address)
            while (string.IsNullOrWhiteSpace(GameState.PlayerAddress))
            {
                Console.Write("5. Yaşayış Ünvanınızı daxil edin: ");
                GameState.PlayerAddress = Console.ReadLine() ?? "";
            }

            // 6. Qan Qrupu (Blood Group)
            while (string.IsNullOrWhiteSpace(GameState.PlayerBloodGroup))
            {
                Console.Write("6. Qan Qrupunu daxil edin (Məsələn: A+, 0-): ");
                GameState.PlayerBloodGroup = Console.ReadLine() ?? "";
            }

            // 7. Ailə Vəziyyəti (Marital Status)
            while (string.IsNullOrWhiteSpace(GameState.PlayerMaritalStatus))
            {
                Console.Write("7. Ailə Vəziyyətinizi daxil edin (Məsələn: Subay, Evli): ");
                GameState.PlayerMaritalStatus = Console.ReadLine() ?? "";
            }

            // Başlanğıc ilini təyin et
            GameState.CurrentYear = DateTime.Now.Year;

            // Şəxsiyyət Vəsiqəsi pulsuz və məlumat verildikdən sonra avtomatik verilir.
            GameState.Documents["Şəxsiyyət Vəsiqəsi (ID)"] = true;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Salam, {GameState.Name} {GameState.PlayerSurname}! Macəra başlayır...");
            Console.WriteLine($"Başlanğıc Balans: {GameState.Balance}$, Başlanğıc il: {GameState.CurrentYear}");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("✅ Şəxsiyyət Vəsiqəniz (ID) bütün daxil etdiyiniz məlumatlarla verildi.");
            Console.WriteLine($"Əsas məlumatlar: Yaş: {GameState.Age}, DT: {GameState.PlayerDateOfBirth}, Ünvan: {GameState.PlayerAddress}");
            Console.WriteLine($"Əlavə məlumatlar: Qan Qrupu: {GameState.PlayerBloodGroup}, Ailə Vəz.: {GameState.PlayerMaritalStatus}");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(3000);
        }

        // Əsas Oyun Dövrü
        static void GameLoop()
        {
            while (true)
            {
                // Oyunçu ölübsə, dövrü sonlandır
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

                // Hər fəaliyyətdən əvvəl ani təhlükə yoxlaması (1% şans)
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
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("--- 📝 Şəxsi Sənədlər Menyu ---");
                Console.ForegroundColor = ConsoleColor.White;

                // Cari Sənədlər və Əşya Çantası
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

                // Seçimlər
                Console.WriteLine("\nSeçimlər:");
                Console.WriteLine("1. 🛂 Pasport üçün Müraciət (300$, Yaş 18+)");
                Console.WriteLine("2. 🚗 Sürücülük Vəsiqəsi üçün Müraciət (200$, Yaş 18+)");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("3. 👁️ Sənədlərə Bax (Kart Görünüşü)"); // YENİ SEÇİM
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("4. Geri");
                Console.Write("Seçim: ");

                string s = Console.ReadLine() ?? "";

                switch (s.Trim())
                {
                    case "1": ApplyForDocument("Pasport (Beynəlxalq)", 300, 18); break;
                    case "2": ApplyForDocument("Sürücülük Vəsiqəsi", 200, 18); break;
                    case "3": ViewDocumentsMenu(); break; // YENİ funksiya çağırılır
                    case "4": return;
                    default:
                        UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red);
                        break;
                }
            }
        }

        // Sənədlərə Baxış Menyu
        static void ViewDocumentsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("--- 👁️ Sənədlərə Baxış ---");
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Hansı sənədə baxmaq istəyirsiniz?");

                var availableDocs = GameState.Documents.Where(d => d.Value).ToList();
                int selectionIndex = 1;
                var docMap = new Dictionary<int, string>();

                if (!availableDocs.Any())
                {
                    UI.ShowMessage("Baxılacaq sənəd yoxdur. Əvvəlcə Şəxsiyyət Vəsiqəsi avtomatik verilir.", ConsoleColor.Red);
                    break;
                }

                foreach (var doc in availableDocs)
                {
                    Console.WriteLine($"{selectionIndex}. {doc.Key}");
                    docMap.Add(selectionIndex, doc.Key);
                    selectionIndex++;
                }

                Console.WriteLine($"{selectionIndex}. Geri");
                Console.Write("Seçim: ");

                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int choice) && docMap.ContainsKey(choice))
                {
                    ViewDocumentCard(docMap[choice]);
                }
                else if (int.TryParse(s, out int backChoice) && backChoice == selectionIndex)
                {
                    return;
                }
                else
                {
                    UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red);
                }
            }
        }

        // Seçilmiş sənədin "kart" görünüşünü çıxarır
        static void ViewDocumentCard(string docName)
        {
            Console.Clear();

            // Xüsusi sənəd dizaynı
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("║ ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{docName.ToUpper()}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" - AZ 0123456789                         ║");

            // Məlumat Blokları
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.ForegroundColor = ConsoleColor.White;

            // 1. Ad, Soyad
            Console.WriteLine($"║ AD/SOYAD: {GameState.Name.ToUpper()} {GameState.PlayerSurname.ToUpper()}");

            // 2. Yaş, DT
            Console.WriteLine($"║ DOĞUM TARİXİ: {GameState.PlayerDateOfBirth} ({GameState.Age} Yaş)");

            // 3. Ünvan
            Console.WriteLine($"║ ÜNVAN: {GameState.PlayerAddress}");

            // Pasport və Sürücülük üçün əlavə sahələr
            if (docName != "Şəxsiyyət Vəsiqəsi (ID)")
            {
                // Pasport üçün xüsusi sahə
                if (docName.Contains("Pasport"))
                {
                    Console.WriteLine($"║ BEYNƏLXALQ NÖMRƏ: P-{GameState.Age}{GameState.Day}{GameState.Hour}");
                }

                // Sürücülük üçün xüsusi sahə (Yaş 18+, Avtomatik "B" Kateqoriyası)
                if (docName.Contains("Sürücülük"))
                {
                    string category = GameState.Age >= 18 ? "B (Avtomobil)" : "Yoxdur";
                    Console.WriteLine($"║ KATEQORİYA: {category}");
                }
            }

            // Bütün sənədlər üçün ortaq: Qan qrupu və Ailə Vəziyyəti
            Console.WriteLine($"║ QAN QRUPU: {GameState.PlayerBloodGroup}  | AİLƏ VƏZİYYƏTİ: {GameState.PlayerMaritalStatus}");

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.White;

            // Nəticə və Geri qayıtma
            UI.ShowMessage("Sənədə baxmaq üçün Enter düyməsini sıxın...", ConsoleColor.Yellow);
            Console.ReadLine();
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
