using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseSystem
{
    public class WarehouseNetwork
    {
        public List<Warehouse> Warehouses { get; } = new List<Warehouse>();

        public void AddWarehouse(Warehouse warehouse)
        {
            Warehouses.Add(warehouse);
        }

        // Метод поставки товаров
        public void Deliver(List<Product> products)
        {
            bool allLong = products.All(p => p.DaysToExpire >= 30);
            bool allShort = products.All(p => p.DaysToExpire < 30);

            IEnumerable<Warehouse> targetWarehouses;

            if (allLong)
            {
                targetWarehouses = Warehouses.Where(w => w.Type == "общий");
            }
            else if (allShort)
            {
                targetWarehouses = Warehouses.Where(w => w.Type == "холодный");
            }
            else
            {
                targetWarehouses = Warehouses.Where(w => w.Type == "сортировочный");
            }

            // Простейший вариант: кладем все на первый подходящий склад
            var warehouse = targetWarehouses.FirstOrDefault(w => w.FreeVolume >= products.Sum(p => p.UnitVolume));

            if (warehouse == null)
            {
                Console.WriteLine("Нет склада с достаточным свободным объемом для поставки.");
                return;
            }

            foreach (var product in products)
            {
                if (warehouse.AddProduct(product))
                {
                    LogMovement(product, product.UnitVolume, "Поставка", $"Склад {warehouse.Id}");
                }
                else
                {
                    Console.WriteLine($"Не удалось разместить товар {product.Name} на складе {warehouse.Id}");
                }
            }
        }

        // Заглушка внутреннего оптимизационного перемещения с сортировочных складов
        public void OptimizeFromSorting()
        {
            var sortingWarehouses = Warehouses.Where(w => w.Type == "сортировочный");

            foreach (var sorting in sortingWarehouses)
            {
                // Здесь позже реализуете реальное распределение по другим складам
                Console.WriteLine($"Оптимизация сортировочного склада Id={sorting.Id} (реализация позже)");
            }
        }

        // Заглушка внутреннего перемещения указанных товаров
        public void MoveProducts(Warehouse from, Warehouse to, List<Product> products)
        {
            foreach (var product in products)
            {
                if (from.Products.Contains(product) && to.FreeVolume >= product.UnitVolume)
                {
                    from.RemoveProduct(product);
                    to.AddProduct(product);
                    LogMovement(product, product.UnitVolume, $"Склад {from.Id}", $"Склад {to.Id}");
                }
            }
        }

        // Заглушка перемещения товаров с истекшим сроком годности
        public void MoveExpired()
        {
            var disposalWarehouses = Warehouses.Where(w => w.Type == "утилизация").ToList();
            if (!disposalWarehouses.Any())
            {
                Console.WriteLine("Нет складов утилизации.");
                return;
            }

            var disposal = disposalWarehouses.First();

            foreach (var warehouse in Warehouses.Where(w => w.Type != "утилизация"))
            {
                var expired = warehouse.Products.Where(p => p.DaysToExpire <= 0).ToList();
                foreach (var product in expired)
                {
                    if (disposal.FreeVolume >= product.UnitVolume)
                    {
                        warehouse.RemoveProduct(product);
                        disposal.AddProduct(product);
                        LogMovement(product, product.UnitVolume, $"Склад {warehouse.Id}", $"Склад {disposal.Id}");
                    }
                }
            }
        }

        // Заглушка анализа складской сети
        public void AnalyzeNetwork()
        {
            foreach (var warehouse in Warehouses)
            {
                Console.WriteLine($"Склад Id={warehouse.Id}, Тип={warehouse.Type}: статус проверки (логика анализа будет добавлена позже)");
            }
        }

        // Подсчет стоимости товаров на складе
        public double CalculateTotalCost(Warehouse warehouse)
        {
            return warehouse.Products.Sum(p => p.UnitPrice);
        }

        private void LogMovement(Product product, double volume, string from, string to)
        {
            Console.WriteLine($"ЛОГ: Товар={product.Name}, объем={volume}, откуда={from}, куда={to}");
        }
    }
}
