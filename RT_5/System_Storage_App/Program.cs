using System;
using System.Collections.Generic;

namespace WarehouseSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var network = new WarehouseNetwork();

            // Создаем склады
            var general = new Warehouse(1, "общий", 1000, "г. Москва, Общий склад");
            var cold = new Warehouse(2, "холодный", 500, "г. Москва, Холодный склад");
            var sorting = new Warehouse(3, "сортировочный", 300, "г. Москва, Сортировочный склад");
            var disposal = new Warehouse(4, "утилизация", 200, "г. Москва, Склад утилизации");

            network.AddWarehouse(general);
            network.AddWarehouse(cold);
            network.AddWarehouse(sorting);
            network.AddWarehouse(disposal);

            // Создаем тестовую поставку
            var products = new List<Product>
            {
                new Product(1, 10, "Товар A", 10, 100, 40),
                new Product(2, 11, "Товар B", 20, 200, 25),
                new Product(3, 12, "Товар C", 5, 50, 5)
            };

            // Поставка (с автоматическим выбором склада)
            network.Deliver(products);

            // Вывод информации о складах
            general.PrintInfo();
            cold.PrintInfo();
            sorting.PrintInfo();
            disposal.PrintInfo();

            // Анализ сети
            network.AnalyzeNetwork();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
