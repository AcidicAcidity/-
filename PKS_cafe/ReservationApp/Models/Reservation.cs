using System;
using System.Linq;

namespace ReservationApp.Models // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Comment { get; set; } = string.Empty;
        public Table AssignedTable { get; set; } = null!; // null! добавлено для Table
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Reservation CreateReservation(in int id, in int clientId, in string name,
            in string phone, in DateTime start, in DateTime end, in string comment, in Table table)
        {
            Id = id;
            ClientId = clientId;
            ClientName = name;
            Phone = phone;
            StartTime = start;
            EndTime = end;
            Comment = comment;
            AssignedTable = table;

            // Добавляем в расписание стола
            var currentTime = start;
            while (currentTime < end)
            {
                table.AddReservation(currentTime, $"Бронь #{Id} - {name}");
                currentTime = currentTime.AddHours(1);
            }

            return this;
        }

        public void EditReservation(ref Reservation reservation, in DateTime newStart,
            in DateTime newEnd, in string newComment)
        {
            // Удаляем старые временные слоты
            var currentTime = reservation.StartTime;
            while (currentTime < reservation.EndTime)
            {
                reservation.AssignedTable.RemoveReservation(currentTime);
                currentTime = currentTime.AddHours(1);
            }

            reservation.StartTime = newStart;
            reservation.EndTime = newEnd;
            reservation.Comment = newComment;

            // Добавляем новые
            currentTime = newStart;
            while (currentTime < newEnd)
            {
                reservation.AssignedTable.AddReservation(currentTime, $"Бронь #{reservation.Id} - {reservation.ClientName}");
                currentTime = currentTime.AddHours(1);
            }
        }

        public void CancelReservation(out bool success)
        {
            success = false;
            if (AssignedTable != null && StartTime > DateTime.Now)
            {
                var currentTime = StartTime;
                while (currentTime < EndTime)
                {
                    AssignedTable.RemoveReservation(currentTime);
                    currentTime = currentTime.AddHours(1);
                }
                success = true;
            }
        }

        public bool IsActive()
        {
            return DateTime.Now >= StartTime && DateTime.Now <= EndTime;
        }

        public void PrintReservationInfo(out string info)
        {
            info = $"Бронь #{Id} | {ClientName} ({Phone})\n" +
                   $"Стол: {AssignedTable.Id} | Мест: {AssignedTable.Seats}\n" +
                   $"Время: {StartTime:dd.MM.yyyy HH:mm} - {EndTime:HH:mm}\n" +
                   $"Комментарий: {Comment}";
        }
    }
}