using System;
using System.Collections.Generic;
using ReservationApp.Models; // if you use your models
using System.Linq;

namespace ReservationApp.Services // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class TableManagementService
    {
        private List<Table> tables;
        private ReservationService reservationService;

        public TableManagementService(ReservationService resService)
        {
            reservationService = resService;
            tables = reservationService.GetAllTables();
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void EditTableInfo(int tableId, TableLocation newLocation, int newSeats, out bool success)
        {
            var table = tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
            {
                try
                {
                    table.EditTable(ref table, newLocation, newSeats);
                    success = true;
                }
                catch (InvalidOperationException)
                {
                    success = false;
                    throw;
                }
            }
            else
            {
                success = false;
            }
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public void ToggleTableStatus(int tableId, out bool newStatus)
        {
            var table = tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
            {
                table.IsActive = !table.IsActive;
                newStatus = table.IsActive;
            }
            else
            {
                newStatus = false;
            }
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public string GetTableDetailedInfo(int tableId)
        {
            var table = tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
            {
                table.PrintTableInfo(out string info);
                return info;
            }
            return "Стол не найден";
        }

        // ИСПРАВЛЕНО: убраны модификаторы in
        public List<Table> GetTablesWithReservationsForDate(DateTime date)
        {
            return tables.Where(t => t.Reservations.Any(r => r.Key.Date == date.Date))
                        .OrderBy(t => t.Id)
                        .ToList();
        }
    }
}