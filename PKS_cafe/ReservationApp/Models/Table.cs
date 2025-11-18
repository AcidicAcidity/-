using System;
using System.Collections.Generic;
using System.Linq;

namespace ReservationApp.Models // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class Table
    {
        public int Id { get; set; }
        public TableLocation Location { get; set; }
        public int Seats { get; set; }
        public bool IsActive { get; set; } = true;
        // Ключ: DateTime (начало часа), Значение: string (инфо о брони)
        public Dictionary<DateTime, string> Reservations { get; set; } = new();

        public void AddReservation(in DateTime dateTime, in string info)
        {
            if (Reservations.ContainsKey(dateTime))
                throw new InvalidOperationException($"Стол {Id} занят в {dateTime:HH:mm}");
            Reservations.Add(dateTime, info);
        }

        public void RemoveReservation(in DateTime dateTime)
        {
            Reservations.Remove(dateTime);
        }

        public Table CreateTable(in int id, in TableLocation location, in int seats)
        {
            Id = id;
            Location = location;
            Seats = seats;
            return this;
        }

        public void EditTable(ref Table table, in TableLocation newLocation, in int newSeats)
        {
            if (table.Reservations.Any(r => r.Key.Date == DateTime.Today && r.Key > DateTime.Now))
            {
                throw new InvalidOperationException("Нельзя редактировать стол с активными бронированиями");
            }

            table.Location = newLocation;
            table.Seats = newSeats;
        }

        public void PrintTableInfo(out string info)
        {
            info = $"Стол #{Id} | {Location} | {Seats} мест | " +
                   $"{(IsActive ? "Активен" : "Неактивен")}";

            var todayReservations = Reservations.Where(r => r.Key.Date == DateTime.Today);
            if (todayReservations.Any())
            {
                info += $"\nБронирования на сегодня:";
                foreach (var res in todayReservations.OrderBy(r => r.Key))
                {
                    info += $"\n{res.Key:HH:mm} - {res.Value}";
                }
            }
        }

        public bool IsAvailableForReservation(in DateTime dateTime)
        {
            return !Reservations.ContainsKey(dateTime) && IsActive;
        }

        public override string ToString() => $"Стол #{Id} ({Seats} мест, {Location})";
    }
}