using System;
using System.Collections.Generic;
using ReservationApp.Models; // if you use your models
using System.Linq;

namespace ReservationApp.Services // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class OrderService
    {
        private List<Dish> dishes = new();
        private List<Order> orders = new();
        private List<Waiter> waiters = new();
        private int nextDishId = 1;
        private int nextOrderId = 1;
        private int nextWaiterId = 1;
        private ReservationService reservationService;

        public OrderService(ReservationService resService)
        {
            reservationService = resService;
            SeedSampleData();
        }

        private void SeedSampleData()
        {
            // Создаем официантов
            waiters.Add(new Waiter().CreateWaiter(nextWaiterId++, "Анна Сергеева", "79161112233"));
            waiters.Add(new Waiter().CreateWaiter(nextWaiterId++, "Дмитрий Петров", "79162223344"));
            waiters.Add(new Waiter().CreateWaiter(nextWaiterId++, "Мария Иванова", "79163334455"));

            // Создаем меню
            dishes.Add(new Dish().CreateDish(nextDishId++, "Цезарь с курицей",
                "Курица, салат айсберг, пармезан, сухарики, соус цезарь", 450, DishCategory.Salads, 10, "Популярный салат"));
            dishes.Add(new Dish().CreateDish(nextDishId++, "Грибной крем-суп",
                "Шампиньоны, сливки, специи", 320, DishCategory.Soups, 15, "Нежный суп-пюре"));
            dishes.Add(new Dish().CreateDish(nextDishId++, "Стейк из лосося",
                "Филе лосося, лимон, спаржа", 890, DishCategory.MainCourses, 25, "Подается с овощами"));
            dishes.Add(new Dish().CreateDish(nextDishId++, "Чизкейк",
                "Сливочный сыр, печенье, ягодный соус", 380, DishCategory.Desserts, 5, "Классический чизкейк"));
            dishes.Add(new Dish().CreateDish(nextDishId++, "Латте",
                "Кофе, молоко", 250, DishCategory.Drinks, 3, "Бодрящий напиток"));

            // Создаем тестовые заказы
            var table1 = reservationService.GetAllTables().FirstOrDefault(t => t.Id == 1);
            var waiter1 = waiters[0];
            var dish1 = dishes[0];
            var dish3 = dishes[2];

            if (table1 != null)
            {
                CreateOrder(table1.Id, waiter1.Id, waiter1.Name, "Без лука", out _, dish1, dish3);
            }
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void CreateOrder(int tableId, int waiterId, string waiterName, string comment,
            out Order order, params Dish[] dishes)
        {
            order = new Order().CreateOrder(nextOrderId++, tableId, waiterId, waiterName, comment, dishes);
            orders.Add(order);
        }

        public List<Order> GetActiveOrders()
        {
            return orders.Where(o => o.Status != OrderStatus.Closed).OrderBy(o => o.OrderTime).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return orders.OrderByDescending(o => o.OrderTime).ToList();
        }

        public Order? GetOrderById(int orderId)
        {
            return orders.FirstOrDefault(o => o.Id == orderId);
        }

        public List<Dish> GetAllDishes()
        {
            return dishes.OrderBy(d => d.Category).ToList();
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void AddNewDish(string name, string ingredients, decimal price,
            DishCategory category, int cookingTime, string description, out Dish dish)
        {
            dish = new Dish().CreateDish(nextDishId++, name, ingredients, price, category, cookingTime, description);
            dishes.Add(dish);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void EditDish(int dishId, string newName, string newIngredients,
            decimal newPrice, DishCategory newCategory, int newCookingTime, string newDescription, out bool success)
        {
            var dish = dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish != null)
            {
                dish.EditDish(ref dish, newName, newIngredients, newPrice, newCookingTime, newDescription, dish.IsAvailable);
                dish.Category = newCategory;
                success = true;
            }
            else
            {
                success = false;
            }
        }

        public Dictionary<string, int> GetDishStatistics(DateTime startDate, DateTime endDate)
        {
            return orders.Where(o => o.CloseTime >= startDate && o.CloseTime <= endDate && o.Status == OrderStatus.Closed)
                         .SelectMany(o => o.Dishes)
                         .GroupBy(d => d.Name)
                         .ToDictionary(g => g.Key, g => g.Count())
                         .OrderByDescending(kvp => kvp.Value)
                         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public Dictionary<string, decimal> GetWaiterRevenue(DateTime startDate, DateTime endDate)
        {
            return orders.Where(o => o.CloseTime >= startDate && o.CloseTime <= endDate && o.IsPaid)
                         .GroupBy(o => o.WaiterName)
                         .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalPrice))
                         .OrderByDescending(kvp => kvp.Value)
                         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public decimal GetTotalRevenueForPeriod(DateTime startDate, DateTime endDate)
        {
            return orders.Where(o => o.CloseTime >= startDate && o.CloseTime <= endDate && o.IsPaid)
                         .Sum(o => o.TotalPrice);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void UpdateOrderStatus(int orderId, OrderStatus newStatus, out bool success)
        {
            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.UpdateStatus(newStatus, out success);
            }
            else
            {
                success = false;
            }
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void MarkOrderAsPaid(int orderId, out bool success)
        {
            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.MarkAsPaid(out success);
            }
            else
            {
                success = false;
            }
        }

        public List<Waiter> GetAllWaiters()
        {
            return waiters.Where(w => w.IsActive).ToList();
        }

        public void AddWaiter(in string name, in string phone, out Waiter waiter)
        {
            waiter = new Waiter().CreateWaiter(nextWaiterId++, name, phone);
            waiters.Add(waiter);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void ToggleDishAvailability(int dishId, out bool newStatus)
        {
            var dish = dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish != null)
            {
                dish.IsAvailable = !dish.IsAvailable;
                newStatus = dish.IsAvailable;
            }
            else
            {
                newStatus = false;
            }
        }
    }
}