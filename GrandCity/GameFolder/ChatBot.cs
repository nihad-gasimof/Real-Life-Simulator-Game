using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;

namespace CityLifeGameV3
{
    // Chatbot funksionallığı (Simulyasiya edilmiş dost ilə chat)
    public static class Chatbot
    {
        private static readonly HttpClient client = new HttpClient();
        // Chat tarixçəsi dost adı ilə
        private static List<string> chatHistory = new List<string>
        {
            "Salam! Mən sənin dostun Aydanam. Necəsən? Nə vaxtdır danışmırıq!"
        };
        private static string friendName = "Aydan";
        private const string GEMINI_MODEL = "gemini-2.5-flash-preview-09-2025";

        // Chat menyusu
        public static void ChatbotMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"--- 💬 Chat ({friendName} ilə) ---");
            Console.WriteLine("-----------------------------------");

            // Tarixçəni göstər
            foreach (var message in chatHistory)
            {
                if (message.StartsWith($"{friendName}:"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (message.StartsWith("Sən:"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(message);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Cavabını yaz (yazmaq üçün Enter, çıxmaq üçün 'q'):");
            Console.Write("Sən: ");

            string input = Console.ReadLine() ?? "";

            if (input.Trim().ToLower() == "q")
            {
                Console.WriteLine("Chatdan çıxdın.");
                Thread.Sleep(500);
                return;
            }

            if (!string.IsNullOrWhiteSpace(input))
            {
                // Oyunçunun mesajını əlavə et
                chatHistory.Add($"Sən: {input}");

                // Dostun cavabını al
                GetFriendResponse(input);
            }

            GameState.NextHour(1); // Chat 1 saat vaxt aparır
        }

        // Gemini API ilə cavab almaq
        private static async void GetFriendResponse(string userMessage)
        {
            Console.Write("Aydan yazır... ");
            UI.Animate("...");

            // Sistem təlimatı - Dostun personası
            var systemPrompt = $"Sən '{friendName}' adlı {GameState.Age} yaşlı bir dostsan. '{GameState.Name}' adlı oyunçu ilə danışırsan. Çox realist, casual (qeyri-rəsmi) və azərbaycan dilində cavab ver. Ona məsləhətlər verə, nəsə soruşa bilərsən. Söhbəti davam etdir. Hər cavabında yalnız bir cümlə olmalıdır, 30 sözü keçməməlidir.";

            // Hazırki chat tarixçəsini API üçün formatla
            var contents = new List<object>();
            foreach (var msg in chatHistory)
            {
                var role = msg.StartsWith($"{friendName}:") ? "model" : "user";
                var text = msg.Replace($"{friendName}: ", "").Replace("Sən: ", "");

                // Məhdudiyyət: 'model'in cavabı tarixçədə 'user'in cavabından sonra gəlməlidir.
                // Biz yalnız son istifadəçi mesajını göndərəcəyik, yoxsa API-nin formatını pozacaq.
                // Sadəlik üçün yalnız son mesajı istifadə edək:
                if (msg == chatHistory[^1])
                {
                    contents.Add(new { role = "user", parts = new[] { new { text = text } } });
                    break;
                }
            }

            var payload = new
            {
                contents = contents,
                systemInstruction = new { parts = new[] { new { text = systemPrompt } } },
            };

            // API çağırışı üçün boş API açarı
            const string apiKey = "";
            const string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{GEMINI_MODEL}:generateContent?key={apiKey}";

            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

                // Cavabı çıxar
                var text = result
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(text))
                {
                    text = "Hmm, nə isə yazmaq istədim, amma alınmadı. Təzədən yaz zəhmət olmasa.";
                }

                chatHistory.Add($"{friendName}: {text.Trim()}");
                Console.WriteLine("Cavab gəldi.");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[XƏTA] Dostun cavab verə bilmədi (API xətası).");
                Console.WriteLine("Dostun deyir: 'Bağışla, internetim yoxdur... zəng gələndə yazaram.'");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Detallar: {ex.Message}");
                chatHistory.Add($"{friendName}: Bağışla, internetim getdi. Sonra yazaram.");
            }
        }
    }
}
