using System;
using System.Collections.Generic;
using System.Linq;

namespace ReservationApp.Models
{
    public class Waiter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime HireDate { get; set; } = DateTime.Now;

        public Waiter CreateWaiter(int id, string name, string phone)
        {
            Id = id;
            Name = name;
            Phone = phone;
            return this;
        }

        public void PrintWaiterInfo(out string info)
        {
            info = $"Официант #{Id}\nИмя: {Name}\nТелефон: {Phone}\n" +
                   $"Статус: {(IsActive ? "Активен" : "Неактивен")}\n" +
                   $"Дата найма: {HireDate:dd.MM.yyyy}";
        }
    }
}