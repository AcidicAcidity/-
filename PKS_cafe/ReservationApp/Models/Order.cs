using System;
using System.Collections.Generic;
using System.Linq;

namespace ReservationApp.Models // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public List<Dish> Dishes { get; set; } = new();
        public string Comment { get; set; } = string.Empty;
        public DateTime OrderTime { get; set; }
        public int WaiterId { get; set; }
        public string WaiterName { get; set; } = string.Empty;
        public DateTime? CloseTime { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public bool IsPaid { get; set; }

        public Order CreateOrder(in int id, in int tableId, in int waiterId,
            in string waiterName, in string comment, params Dish[] dishes)
        {
            Id = id;
            TableId = tableId;
            WaiterId = waiterId;
            WaiterName = waiterName;
            Comment = comment;
            OrderTime = DateTime.Now;
            Dishes.AddRange(dishes);
            TotalPrice = dishes.Sum(d => d.Price);
            Status = OrderStatus.Active;
            return this;
        }

        public void EditOrder(ref Order order, in string newComment, params Dish[] newDishes)
        {
            if (order.Status == OrderStatus.Closed)
                throw new InvalidOperationException("Нельзя редактировать закрытый заказ.");

            order.Comment = newComment;
            order.Dishes.Clear();
            order.Dishes.AddRange(newDishes);
            order.TotalPrice = newDishes.Sum(d => d.Price);
        }

        public void UpdateStatus(OrderStatus newStatus, out bool success)
        {
            if (Status != OrderStatus.Closed)
            {
                Status = newStatus;
                if (newStatus == OrderStatus.Closed)
                {
                    CloseTime = DateTime.Now;
                }
                success = true;
            }
            else
            {
                success = false;
            }
        }

        public void MarkAsPaid(out bool success)
        {
            if (!IsPaid)
            {
                IsPaid = true;
                if (Status != OrderStatus.Closed)
                {
                    Status = OrderStatus.Closed;
                    CloseTime = DateTime.Now;
                }
                success = true;
            }
            else
            {
                success = false;
            }
        }

        public void PrintOrderInfo(out string info)
        {
            info = $"Заказ #{Id} | Стол: {TableId} | Официант: {WaiterName}\n" +
                   $"Время: {OrderTime:dd.MM.yyyy HH:mm} | Статус: {Status}\n" +
                   $"Стоимость: {TotalPrice} руб. | {(IsPaid ? "Оплачен" : "Не оплачен")}\n" +
                   $"Блюда ({Dishes.Count}):\n";

            foreach (var dish in Dishes)
            {
                info += $"- {dish.Name} ({dish.Price} руб.)\n";
            }

            if (!string.IsNullOrEmpty(Comment))
                info += $"Комментарий: {Comment}";
        }

        public void PrintReceipt(out string receipt)
        {
            receipt = $"=== ЧЕК #{Id} ===\n" +
                      $"Стол: {TableId}\n" +
                      $"Официант: {WaiterName}\n" +
                      $"Время заказа: {OrderTime:dd.MM.yyyy HH:mm}\n" +
                      $"Время закрытия: {CloseTime:dd.MM.yyyy HH:mm}\n\n";

            foreach (var dish in Dishes)
            {
                receipt += $"{dish.Name} - {dish.Price} руб.\n";
            }

            receipt += $"----------------------------------\n" +
                       $"ИТОГО: {TotalPrice} руб.\n" +
                       $"----------------------------------\n" +
                       $"Спасибо за визит!";
        }
    }
}