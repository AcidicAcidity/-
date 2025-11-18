using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using ReservationApp.Models; // ИСПРАВЛЕНО
using ReservationApp.Services; // ИСПРАВЛЕНО
using System.Collections.Generic;

namespace ReservationApp.Forms // ИСПРАВЛЕНО
{
    public class ReservationManagementForm : Form
    {
        private ReservationService reservationService = null!;
        private IContainer components = null!;
        private DataGridView dgvReservations = null!;
        private DataGridView dgvAvailableTables = null!;
        private Button btnCreateReservation = null!;
        private Button btnCancelReservation = null!;
        private Button btnShowAvailable = null!;
        private TextBox txtClientName = null!;
        private TextBox txtPhone = null!;
        private DateTimePicker dtpDate = null!;
        private DateTimePicker dtpStartTime = null!;
        private DateTimePicker dtpEndTime = null!;
        private TextBox txtComment = null!;
        private NumericUpDown numSeats = null!;
        private Label lblStatus = null!;

        public ReservationManagementForm(ReservationService service)
        {
            reservationService = service;
            InitializeUI();
            LoadReservations();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeUI()
        {
            components = new Container();
            this.Text = "Управление бронированиями";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // ... (Разметка элементов управления, опущено для краткости) ...

            // Инициализация элементов и привязка событий
            dgvReservations = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvAvailableTables = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnCreateReservation = new Button { Text = "Создать бронь" };
            btnCancelReservation = new Button { Text = "Отменить бронь" };
            btnShowAvailable = new Button { Text = "Найти столы" };

            // ... (Инициализация остальных контролов) ...

            // Привязка событий
            btnShowAvailable.Click += (s, e) => ShowAvailableTables();
            btnCreateReservation.Click += (s, e) => CreateReservation();
            btnCancelReservation.Click += (s, e) => CancelReservation();

            // Настройка DateTimePickers
            dtpDate.Value = DateTime.Today;
            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.CustomFormat = "HH:00";
            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.CustomFormat = "HH:00";
            dtpStartTime.Value = DateTime.Today.AddHours(18);
            dtpEndTime.Value = DateTime.Today.AddHours(20);
        }

        private void LoadReservations()
        {
            dgvReservations.DataSource = null;
            dgvReservations.DataSource = reservationService.GetAllReservations();
            lblStatus.Text = $"Загружено бронирований: {reservationService.GetAllReservations().Count}";
            dgvAvailableTables.DataSource = null; // Очищаем список доступных столов
        }

        private void ShowAvailableTables()
        {
            try
            {
                var startDate = dtpDate.Value.Date.AddHours(dtpStartTime.Value.Hour);
                var endDate = dtpDate.Value.Date.AddHours(dtpEndTime.Value.Hour);
                int seats = (int)numSeats.Value;

                if (startDate >= endDate)
                {
                    lblStatus.Text = "Время начала должно быть раньше времени окончания";
                    return;
                }
                if (startDate < DateTime.Now)
                {
                    lblStatus.Text = "Нельзя бронировать на прошедшее время";
                    return;
                }

                var availableTables = reservationService.GetAvailableTables(startDate, endDate, seats);

                dgvAvailableTables.DataSource = null;
                dgvAvailableTables.DataSource = availableTables;
                lblStatus.Text = $"Найдено доступных столов: {availableTables.Count}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка поиска: {ex.Message}";
            }
        }

        private void CreateReservation()
        {
            if (dgvAvailableTables.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите доступный стол";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtClientName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                lblStatus.Text = "Заполните имя и телефон";
                return;
            }

            try
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var table = dgvAvailableTables.SelectedRows[0].DataBoundItem as Table;
                if (table == null) return;

                var startDate = dtpDate.Value.Date.AddHours(dtpStartTime.Value.Hour);
                var endDate = dtpDate.Value.Date.AddHours(dtpEndTime.Value.Hour);

                reservationService.CreateReservation(
                    txtClientName.Text,
                    txtPhone.Text,
                    startDate,
                    endDate,
                    txtComment.Text,
                    table,
                    out Reservation newReservation
                );

                lblStatus.Text = $"Бронь #{newReservation.Id} создана на стол {table.Id}!";
                LoadReservations();
                ClearForm();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка создания брони: {ex.Message}";
            }
        }

        private void CancelReservation()
        {
            if (dgvReservations.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите бронь для отмены";
                return;
            }

            // БЕЗОПАСНАЯ РАСПАКОВКА - добавлены проверки на null
            if (dgvReservations.SelectedRows[0].Cells["Id"].Value != null && 
                dgvReservations.SelectedRows[0].Cells["ClientName"].Value != null)
            {
                var reservationId = (int)dgvReservations.SelectedRows[0].Cells["Id"].Value;
                var clientName = dgvReservations.SelectedRows[0].Cells["ClientName"].Value.ToString() ?? "";

                var result = MessageBox.Show(
                    $"Отменить бронь #{reservationId} для {clientName}?",
                    "Подтверждение отмены",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    reservationService.CancelReservation(reservationId, out bool success);
                    if (success)
                    {
                        lblStatus.Text = $"Бронь #{reservationId} отменена";
                        LoadReservations();
                    }
                    else
                    {
                        lblStatus.Text = "Не удалось отменить бронь";
                    }
                }
            }
        }

        private void ClearForm()
        {
            txtClientName.Clear();
            txtPhone.Clear();
            txtComment.Clear();
            numSeats.Value = 1;
            dtpDate.Value = DateTime.Today;
            dtpStartTime.Value = DateTime.Today.AddHours(18);
            dtpEndTime.Value = DateTime.Today.AddHours(20);
            dgvAvailableTables.DataSource = null;
        }
    }
}