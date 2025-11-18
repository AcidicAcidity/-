using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseSystem
{
    public class Warehouse
    {
        public int Id { get; private set; }
        public string Type { get; private set; } // "холодный", "сортировочный", "общий", "утилизация"
        public double Capacity { get; private set; }
        public string Address { get; private set; }

        private readonly List<Product> _products = new List<Product>();

        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public double UsedVolume => _products.Sum(p => p.UnitVolume);
        public double FreeVolume => Capacity - UsedVolume;

        public Warehouse(int id, string type, double capacity, string address)
        {
            Id = id;
            Type = type;
            Capacity = capacity;
            Address = address;
        }

        public void Edit(string type, double capacity, string address)
        {
            Type = type;
            Capacity = capacity;
            Address = address;
        }

        public bool AddProduct(Product product)
        {
            if (product.UnitVolume <= FreeVolume)
            {
                _products.Add(product);
                return true;
            }

            return false;
        }

        public bool RemoveProduct(Product product)
        {
            return _products.Remove(product);
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Склад Id={Id}, Тип={Type}, Адрес={Address}, Объем={Capacity}, Свободно={FreeVolume}");
            Console.WriteLine("Товары на складе:");
            foreach (var p in _products)
            {
                p.PrintInfo();
            }
        }
    }
}
