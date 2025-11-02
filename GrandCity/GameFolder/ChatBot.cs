using System;
using System.Collections.Generic;
using System.Threading; // Gecikmə simulyasiyası üçün saxlandı

namespace CityLifeGameV3
{
    // Chatbot funksionallığı (Simulyasiya edilmiş dost ilə chat)
    public static class Chatbot
    {
        // Təsadüfi cavablar üçün siyahı (API-siz Simulyasiya). Hər dəfə fərqli və situasiyalı cavablar verir.
        private static readonly List<string> SimulatedResponses = new List<string>
        {
            "Təsəvvür et, bu gün səninlə getmək istədiyim yeni bir kafe tapdım! Yeri haqqında nə düşünürsən?", // Yeni situasiya 1
            "Mənə elə gəlir ki, şəhərin o tərəfində maraqlı bir film göstərilir. Birlikdə baxsaq necə olar?", // Yeni situasiya 2
            "Salam! Həyat necə gedir? Dərs/işlər yaxşıdır? Mən bu səhər nəqliyyatda ilişib qalmışdım!", // Mövcud + situasiya
            "Mən də yaxşıyam, amma bir az darıxıram. Səninlə danışmaq əla oldu! Gələn həftə sonu üçün bir planımız varmı?", // Mövcud + situasiya
            "Bu yaxınlarda maraqlı bir şey oldumu? Mənə danış. Məsələn, dünənki futbol matçını izlədinmi?", // Mövcud + situasiya
            "Nə düşünürsən, bu gün nə etməliyik? Hava çox soyuqdur, bəlkə evdə qalaq?", // Situasiya
            "Ah, mənim də internetim yoxdur. Telefonum ölmək üzrədir. Təcili şarj cihazı tapmalıyım!", // Situasiya
            "Həqiqətən? Çox maraqlıdır! Bəs sonra nə oldu? O hadisə sənə necə təsir etdi?",
            "Bir az stress altındayam. Sən necə öhdəsindən gəlirsən? Məsələn, meditasiya edirsən?", // Mövcud + situasiya
            "Yadımdadır, bir dəfə mənə filan kitabdan danışmışdın. Onu oxuyub bitirdinmi?", // Keçmişə aid sual/situasiya
            "Səncə, bu il tətili harada keçirmək daha yaxşı olar? Dağ yoxsa dəniz?" // Yeni situasiya 3
        };

        private static readonly Random RandomGenerator = new Random();

        // Chat tarixçəsi dost adı ilə
        private static List<string> chatHistory = new List<string>
        {
            "Salam! Mən sənin dostun Aydanam. Necəsən? Nə vaxtdır danışmırıq!"
        };
        private static string friendName = "Aydan";

        // Chat menyusu
        public static void ChatbotMenu()
        {
            while (true) // Davamlı söhbət üçün dövrə
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
                    return; // Tamamilə çıx
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    // Oyunçunun mesajını əlavə et
                    chatHistory.Add($"Sən: {input}");

                    // Dostun cavabını al (İndi API-siz, daxili simulyasiya)
                    GetFriendResponse(input);
                }

                // GameState mövcud deyil, lakin original faylda var idi. Simulyasiya məqsədilə saxlanılır.
                // GameState.NextHour(1); // Chat 1 saat vaxt aparır
            } // while dövrəsi burada bitir
        }

        // Dostun cavabını almaq (API-siz, daxili simulyasiya)
        private static void GetFriendResponse(string userMessage)
        {
            Console.Write($"{friendName} yazır... ");

            // Simulyasiya: Cavan gələnə qədər bir az gözləmə
            Thread.Sleep(700);

            string text = GetRandomResponse();

            if (string.IsNullOrWhiteSpace(text))
            {
                // Təsadüfən boş cavab gəlsə, ehtiyat cavab
                text = "Hmm, nə isə yazmaq istədim, amma alınmadı. Təzədən yaz zəhmət olmasa.";
            }

            chatHistory.Add($"{friendName}: {text.Trim()}");

            // Xəta mesajı olmadığı üçün normal cavab mesajını göstər
            Console.WriteLine("Cavab gəldi.");
        }

        // Təsadüfi cavabı qaytaran metod
        private static string GetRandomResponse()
        {
            // Təsadüfi indeks seçilir
            int index = RandomGenerator.Next(SimulatedResponses.Count);
            return SimulatedResponses[index];
        }
    }
}
