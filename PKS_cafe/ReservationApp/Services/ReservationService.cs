using System;
using System.Collections.Generic;
using ReservationApp.Models; // if you use your models
using System.Linq;

namespace ReservationApp.Services // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class ReservationService
    {
        private List<Table> tables = new();
        private List<Reservation> reservations = new();
        private int nextTableId = 1;
        private int nextReservationId = 1;
        private int nextClientId = 1;

        public ReservationService()
        {
            SeedSampleData();
        }

        private void SeedSampleData()
        {
            // Создаем столы
            CreateTable(TableLocation.Window, 4, out _);
            CreateTable(TableLocation.Window, 2, out _);
            CreateTable(TableLocation.Aisle, 6, out _);
            CreateTable(TableLocation.Exit, 4, out _);
            CreateTable(TableLocation.Back, 8, out _);
            CreateTable(TableLocation.Aisle, 2, out _);

            // Создаем тестовые бронирования
            var table1 = tables[0];
            var table2 = tables[1];

            CreateReservation("Иван Иванов", "79161234567",
                DateTime.Today.AddHours(18), DateTime.Today.AddHours(20),
                "День рождения", table1, out _);

            CreateReservation("Петр Петров", "79169876543",
                DateTime.Today.AddHours(20), DateTime.Today.AddHours(22),
                "Обычный ужин", table2, out _);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void CreateTable(TableLocation location, int seats, out Table table)
        {
            table = new Table().CreateTable(nextTableId++, location, seats);
            tables.Add(table);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void CreateReservation(string clientName, string phone, DateTime start,
            DateTime end, string comment, Table table, out Reservation reservation)
        {
            reservation = new Reservation().CreateReservation(nextReservationId++, nextClientId++,
                clientName, phone, start, end, comment, table);
            reservations.Add(reservation);
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public List<Table> GetAvailableTables(DateTime start, DateTime end, int seats)
        {
            return tables.Where(t =>
                t.IsActive &&
                t.Seats >= seats &&
                IsTableAvailableForPeriod(t, start, end)
            ).ToList();
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        private bool IsTableAvailableForPeriod(Table table, DateTime start, DateTime end)
        {
            var currentTime = start;
            while (currentTime < end)
            {
                if (table.Reservations.ContainsKey(currentTime))
                {
                    return false;
                }
                currentTime = currentTime.AddHours(1);
            }
            return true;
        }

        public List<Reservation> GetAllReservations()
        {
            return reservations.OrderByDescending(r => r.StartTime).ToList();
        }

        public List<Reservation> GetTodayReservations()
        {
            return reservations.Where(r => r.StartTime.Date == DateTime.Today)
                              .OrderBy(r => r.StartTime)
                              .ToList();
        }

        public List<Table> GetAllTables()
        {
            return tables;
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public bool FindReservation(string name, string phoneLast4, out Reservation? reservation)
        {
            reservation = reservations.FirstOrDefault(r =>
                r.ClientName.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                r.Phone.EndsWith(phoneLast4));
            return reservation != null;
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void CancelReservation(int reservationId, out bool success)
        {
            var reservation = reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation != null)
            {
                reservation.CancelReservation(out success);
                if (success)
                {
                    reservations.Remove(reservation);
                }
            }
            else
            {
                success = false;
            }
        }

        public List<Reservation> GetActiveReservations()
        {
            return reservations.Where(r => r.IsActive()).ToList();
        }
    }
}