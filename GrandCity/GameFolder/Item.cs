// Məlumat modeli
namespace CityLifeGameV3
{
    // Oyun zamanı alına bilən əşyaları təmsil edir.
    public class Item
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public Item(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}
