using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace CityLifeGameV3
{
    // Mağaza və alış-veriş fəaliyyətləri (Cafe daxil)
    public static class Store
    {
        // Mağaza kataloqunu ilkin olaraq qurur
        public static void BuildStoreCatalog()
        {
            // Genişləndirilmiş kateqoriyalar. Sənədlər burada satılmır!
            GameState.StoreCatalog["Geyim"] = new Dictionary<string, List<Item>>
            {
                { "Gündəlik Geyim", new List<Item>{ new Item("T-shirt (Basic)", 50), new Item("Jeans (Comfort)", 90) } },
                { "Formal Geyim", new List<Item>{ new Item("Klassik Kostyum", 450), new Item("İpək Köynək", 120) } }
            };
            GameState.StoreCatalog["Elektronika"] = new Dictionary<string, List<Item>>
            {
                { "Fərdi Cihazlar", new List<Item>{ new Item("Telefon (TechCorp)", 400), new Item("Laptop (Premium)", 1500) } },
                { "Əyləncə", new List<Item>{ new Item("Oyun Konsolu (PlayWorld)", 500), new Item("VR Eynək", 800) } }
            };
            // Sənədlər kateqoriyası silindi, çünki onlar DocumentsMenu-dan idarə olunur.
            GameState.StoreCatalog["Qida"] = new Dictionary<string, List<Item>>
            {
                { "LocalKitchen (Naharlar)", new List<Item>{ new Item("Kabab (LocalKitchen)", 18), new Item("Salat (Organic)", 12) } },
                { "SweetShop (Desert)", new List<Item>{ new Item("Desert (Premium)", 15), new Item("Kofe (Latte)", 7) } }
            };
            GameState.StoreCatalog["Evdə"] = new Dictionary<string, List<Item>>
            {
                { "Mebel və Dekor", new List<Item>{ new Item("Mebel Seti (Lüks)", 1200), new Item("Rəsm Əsəri", 500) } }
            };
        }

        // Mağaza menyusunu açır
        public static void OpenStore(bool inStore)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(inStore ? "🏪 Fiziki Mağazada gəzirsən..." : "🌐 Evdə onlayn mağazadasan...");
            UI.Animate(inStore ? "🛍️" : "📦");

            // Kateqoriya seçimi
            int idx = 1;
            var keys = new List<string>(GameState.StoreCatalog.Keys);
            foreach (var k in keys)
            {
                Console.WriteLine($"{idx}. {k}");
                idx++;
            }
            Console.WriteLine($"{idx}. Geri");
            Console.Write("Kateqoriya seç (nömrə): ");

            string s = Console.ReadLine() ?? "";
            if (!int.TryParse(s, out int sel) || sel < 1 || sel > keys.Count + 1) { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }
            if (sel == keys.Count + 1) return;

            string category = keys[sel - 1];
            OpenCategory(category, inStore);
        }

        // Marka seçimi
        private static void OpenCategory(string category, bool inStore)
        {
            Console.Clear();
            Console.WriteLine($"--- Kateqoriya: {category} ---");
            var brands = GameState.StoreCatalog[category];
            int idx = 1;
            var brandKeys = new List<string>(brands.Keys);
            foreach (var b in brandKeys)
            {
                Console.WriteLine($"{idx}. {b}");
                idx++;
            }
            Console.WriteLine($"{idx}. Geri");
            Console.Write("Alt Kateqoriya/Marka seç (nömrə): ");

            string s = Console.ReadLine() ?? "";
            if (!int.TryParse(s, out int sel) || sel < 1 || sel > brandKeys.Count + 1) { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }
            if (sel == brandKeys.Count + 1) return;

            string brand = brandKeys[sel - 1];
            OpenBrand(category, brand, inStore);
        }

        // Məhsul seçimi
        private static void OpenBrand(string category, string brand, bool inStore)
        {
            Console.Clear();
            Console.WriteLine($"--- Məhsullar: {category} / {brand} ---");
            var items = GameState.StoreCatalog[category][brand];
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].Name} - {items[i].Price}$");
            }
            Console.WriteLine($"{items.Count + 1}. Geri");
            Console.Write("Alış üçün nömrəni daxil et: ");

            string s = Console.ReadLine() ?? "";
            if (!int.TryParse(s, out int sel) || sel < 1 || sel > items.Count + 1) { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }
            if (sel == items.Count + 1) return;

            var item = items[sel - 1];
            BuyItem(item, category, inStore);
        }

        // Alış əməliyyatı
        private static void BuyItem(Item item, string category, bool inStore)
        {
            // Bu funksiya yalnız əşyalar üçündür. Sənədlər burada satılmır.
            Console.WriteLine($"\nSeçdiniz: {item.Name} — {item.Price}$");
            Console.WriteLine($"Cari balans: {GameState.Balance}$");
            Console.Write("Almaq istəyirsən? (y/n): ");
            string ans = (Console.ReadLine() ?? "").Trim().ToLower();
            if (ans != "y") { Console.WriteLine("Alışdan imtina edildi."); return; }

            if (GameState.Balance < item.Price)
            {
                UI.ShowMessage("Pulun çatmır 💸", ConsoleColor.Red);
                return;
            }

            GameState.Balance -= item.Price;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Alış tamamlandı: {item.Name}. Qalıq balans: {GameState.Balance}$");
            Console.ForegroundColor = ConsoleColor.White;
            UI.Animate("💰");

            // Oyun Konsolu
            if (item.Name.Contains("Oyun Konsolu") && !GameState.HasGameConsole)
            {
                GameState.HasGameConsole = true;
                UI.ShowMessage("🎉 Oyun Konsolu alındı. Yeni iş açıldı: Oyun Tərtibatçısı!", ConsoleColor.Magenta);
            }
            // Digər əşyalar
            else if (category != "Qida")
            {
                GameState.Inventory.Add(item.Name);
            }

            GameState.NextHour(1); // Alış 1 saat vaxt aparır
        }

        // Cafe fəaliyyəti (qida alır)
        public static void Cafe()
        {
            Console.Clear();
            Console.WriteLine("☕ Kafedəsən — menyu:");
            UI.Animate("🍽️");

            var qidaBrands = GameState.StoreCatalog["Qida"];
            int idx = 1;
            var brandKeys = new List<string>(qidaBrands.Keys);

            foreach (var b in brandKeys)
            {
                Console.WriteLine($"{idx}. {b}");
                idx++;
            }
            Console.WriteLine($"{idx}. Geri");
            Console.Write("Marka seç: ");
            string s = Console.ReadLine() ?? "";

            if (!int.TryParse(s, out int sel) || sel < 1 || sel > brandKeys.Count + 1) { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }
            if (sel == brandKeys.Count + 1) return;

            string brand = brandKeys[sel - 1];
            var items = qidaBrands[brand];
            Console.Clear();
            Console.WriteLine($"--- Sifariş Menyu: {brand} ---");

            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {items[i].Name} - {items[i].Price}$");
            }
            Console.WriteLine($"{items.Count + 1}. Geri");
            Console.Write("Sifariş ver (nömrə): ");
            string ss = Console.ReadLine() ?? "";

            if (!int.TryParse(ss, out int pick) || pick < 1 || pick > items.Count + 1) { UI.ShowMessage("Yanlış seçim.", ConsoleColor.Red); return; }
            if (pick == items.Count + 1) return;

            var item = items[pick - 1];

            if (GameState.Balance >= item.Price)
            {
                GameState.Balance -= item.Price;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSifariş: {item.Name} — ləzzətlə yeyilir!");
                Console.WriteLine($"Qalıq balans: {GameState.Balance}$");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                UI.ShowMessage("Pulun çatmır. Sifariş ləğv edildi.", ConsoleColor.Red);
            }

            GameState.NextHour(1); // Kafedə oturmaq 1 saat vaxt aparır
        }
    }
}
