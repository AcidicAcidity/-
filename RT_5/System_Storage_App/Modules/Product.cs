namespace WarehouseSystem
{
    public class Product
    {
        public int Id { get; private set; }
        public int SupplierId { get; private set; }
        public string Name { get; private set; }
        public double UnitVolume { get; private set; }
        public double UnitPrice { get; private set; }
        public int DaysToExpire { get; set; }

        public Product(int id, int supplierId, string name, double unitVolume, double unitPrice, int daysToExpire)
        {
            Id = id;
            SupplierId = supplierId;
            Name = name;
            UnitVolume = unitVolume;
            UnitPrice = unitPrice;
            DaysToExpire = daysToExpire;
        }

        public void Update(string name, double unitVolume, double unitPrice, int daysToExpire)
        {
            Name = name;
            UnitVolume = unitVolume;
            UnitPrice = unitPrice;
            DaysToExpire = daysToExpire;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Товар Id={Id}, Название={Name}, Объем={UnitVolume}, Цена={UnitPrice}, Дней до окончания={DaysToExpire}");
        }
    }
}
