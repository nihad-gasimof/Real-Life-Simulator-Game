using System;
using System.Threading;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace CityLifeGameV3
{
    // Həyat hadisələri, təhlükə və ölüm funksiyaları
    public static class LifeEvents
    {
        private static readonly HttpClient client = new HttpClient(); // HttpClient-i yenidən təyin et

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

            // Gemini API vasitəsilə 10 ildə baş verə biləcək hadisələr proqnozlaşdırılır
            string prediction = GetFuturePrediction(GameState.Age + 10, GameState.CurrentYear + 10);

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
            GameState.Balance = (int)(GameState.Balance * 1.5) + GameState.Rand.Next(500, 2000);
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

        // Gemini API ilə gələcək proqnozunu almaq
        private static string GetFuturePrediction(int futureAge, int futureYear)
        {
            // Təhlükəsizlik üçün API key boş buraxılır
            const string apiKey = "";
            const string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent?key={apiKey}";

            var systemPrompt = $"Sən proqnozlaşdıran bir gələcək falçısısan. '{GameState.Name}' adlı oyunçu 10 il sonra, yəni {futureYear} ilində {futureAge} yaşında olacaq. Onun 10 il sonra baş verəcək həyatını (pul, iş, sevgi və ölüm riski daxil) Azərbaycanca 2-3 qısa cümlə ilə proqnozlaşdır. Proqnoz həm yaxşı, həm də pis xəbərləri ehtiva etməlidir. Proqnozun yalnız proqnoz mətnini qaytar, başqa heç nə yazma.";

            var userQuery = $"Mənim 10 il sonrakı həyatım haqqında proqnoz ver. Cari balansım {GameState.Balance}$.";

            var payload = new
            {
                contents = new[] { new { parts = new[] { new { text = userQuery } } } },
                systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            };

            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // API çağırışı (bloklayıcı çağırış, çünki async konteksti yoxdur)
                var response = client.PostAsync(apiUrl, content).Result;
                response.EnsureSuccessStatusCode();

                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var text = result
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "Proqnoz əldə edilmədi.";

            }
            catch (Exception ex)
            {
                // API xətası zamanı standart proqnoz
                Console.WriteLine($"[XƏTA] Proqnoz API-nə qoşula bilmədi: {ex.Message}");
                return "Gələcəyin qeyri-müəyyəndir. Maliyyə vəziyyətin yaxşılaşacaq, lakin bir qəza riski səni gözləyir.";
            }
        }
    }
}
