using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using ReservationApp.Services;
using ReservationApp.Models;

namespace ReservationApp.Forms
{
    public class MainForm : Form
    {
        private ReservationService reservationService;
        private OrderService orderService;
        private TableManagementService tableManagementService;
        
        // Поля, не участвующие в логике Dashboard, удалены (CS0414)
        private TabControl mainTabControl = null!;
        private MenuStrip mainMenu = null!;
        private StatusStrip statusStrip = null!;
        private IContainer components = null!;

        // Контролы для Dashboard
        private Label lblTodayStats = null!;
        private Label lblReservationsCount = null!;
        private Label lblActiveOrdersCount = null!;
        private Label lblRevenueToday = null!;
        private ListBox listActiveReservations = null!;
        private ListBox listActiveOrders = null!;

        public MainForm()
        {
            reservationService = new ReservationService();
            orderService = new OrderService(reservationService);
            tableManagementService = new TableManagementService(reservationService);
            
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            // Настройка главного окна
            this.Text = "Система управления рестораном";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Настройка TabControl
            mainTabControl = new TabControl { Dock = DockStyle.Fill };
            this.Controls.Add(mainTabControl);

            // Создание вкладок
            mainTabControl.TabPages.Add(CreateDashboardPage());
            mainTabControl.TabPages.Add(CreateManagementPage("Tables", "Управление столами"));
            mainTabControl.TabPages.Add(CreateManagementPage("Reservations", "Управление бронированиями"));
            mainTabControl.TabPages.Add(CreateManagementPage("Orders", "Управление заказами"));
            mainTabControl.TabPages.Add(CreateManagementPage("Menu", "Управление меню"));
            mainTabControl.TabPages.Add(CreateStatisticsPage());

            // Настройка главного меню
            mainMenu = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Выход", null, (s, e) => Application.Exit());
            fileMenu.DropDownItems.Add(exitItem);
            mainMenu.Items.Add(fileMenu);
            this.Controls.Add(mainMenu);

            // Настройка статус-бара
            statusStrip = new StatusStrip();
            statusStrip.Items.Add(new ToolStripStatusLabel($"Время запуска: {DateTime.Now:HH:mm}"));
            this.Controls.Add(statusStrip);

            // Обработчик для переключения вкладок
            mainTabControl.SelectedIndexChanged += (s, e) => LoadData();
        }

        private TabPage CreateDashboardPage()
        {
            var page = new TabPage("Dashboard") { Padding = new Padding(10) };
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                ColumnStyles = { new ColumnStyle(SizeType.Percent, 50), new ColumnStyle(SizeType.Percent, 50) },
                RowStyles = { new RowStyle(SizeType.Absolute, 100), new RowStyle(SizeType.Percent, 50), new RowStyle(SizeType.Percent, 50) }
            };

            // Статистика за сегодня (Row 0)
            lblTodayStats = new Label { Text = "Статистика за сегодня", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font(Font, FontStyle.Bold) };
            panel.Controls.Add(lblTodayStats, 0, 0);
            panel.SetColumnSpan(lblTodayStats, 2);

            var statsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowStyles = { new RowStyle(SizeType.Percent, 100) },
                BackColor = SystemColors.ControlLight
            };

            lblReservationsCount = new Label { Text = "Брони:\n0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            lblActiveOrdersCount = new Label { Text = "Активные заказы:\n0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            lblRevenueToday = new Label { Text = "Выручка:\n0.00 руб.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };

            statsPanel.Controls.Add(lblReservationsCount, 0, 0);
            statsPanel.Controls.Add(lblActiveOrdersCount, 1, 0);
            statsPanel.Controls.Add(lblRevenueToday, 2, 0);
            panel.Controls.Add(statsPanel, 0, 1);
            panel.SetColumnSpan(statsPanel, 2);

            // Активные бронирования (Row 1, Column 0)
            var resGroup = new GroupBox { Text = "Активные бронирования", Dock = DockStyle.Fill };
            listActiveReservations = new ListBox { Dock = DockStyle.Fill };
            resGroup.Controls.Add(listActiveReservations);
            panel.Controls.Add(resGroup, 0, 2);

            // Активные заказы (Row 1, Column 1)
            var orderGroup = new GroupBox { Text = "Активные заказы", Dock = DockStyle.Fill };
            listActiveOrders = new ListBox { Dock = DockStyle.Fill };
            orderGroup.Controls.Add(listActiveOrders);
            panel.Controls.Add(orderGroup, 1, 2);

            page.Controls.Add(panel);
            return page;
        }

        private TabPage CreateManagementPage(string name, string text)
        {
            var page = new TabPage(text) { Name = name, Padding = new Padding(10) };
            Button btnOpenForm = new Button
            {
                Text = $"Открыть {text}",
                Size = new Size(200, 50),
                Location = new Point((page.Width - 200) / 2, (page.Height - 50) / 2),
                Anchor = AnchorStyles.None // Убираем авто-якоря для ручной центровки
            };

            // Центровка кнопки (будет обновлена при LoadData)
            btnOpenForm.Location = new Point((page.ClientSize.Width - btnOpenForm.Width) / 2, (page.ClientSize.Height - btnOpenForm.Height) / 2);
            page.Resize += (s, e) =>
            {
                btnOpenForm.Location = new Point((page.ClientSize.Width - btnOpenForm.Width) / 2, (page.ClientSize.Height - btnOpenForm.Height) / 2);
            };

            switch (name)
            {
                case "Tables":
                    btnOpenForm.Click += (s, e) => new TableManagementForm(tableManagementService, reservationService).ShowDialog();
                    break;
                case "Reservations":
                    btnOpenForm.Click += (s, e) => new ReservationManagementForm(reservationService).ShowDialog();
                    break;
                case "Orders":
                    btnOpenForm.Click += (s, e) => new OrderManagementForm(orderService, reservationService).ShowDialog();
                    break;
                case "Menu":
                    btnOpenForm.Click += (s, e) => new MenuManagementForm(orderService).ShowDialog();
                    break;
            }

            page.Controls.Add(btnOpenForm);
            return page;
        }

        private TabPage CreateStatisticsPage()
        {
            var page = new TabPage("Статистика") { Name = "Statistics", Padding = new Padding(10) };
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.TopDown };

            Button btnRevenue = new Button { Text = "Общая статистика выручки", Size = new Size(250, 40) };
            btnRevenue.Click += (s, e) => ShowRevenueStatistics();
            
            Button btnDishStats = new Button { Text = "Статистика по блюдам (Месяц)", Size = new Size(250, 40) };
            btnDishStats.Click += (s, e) => ShowDishStatistics();

            Button btnWaiterStats = new Button { Text = "Статистика по официантам (Месяц)", Size = new Size(250, 40) };
            btnWaiterStats.Click += (s, e) => ShowWaiterStatistics();

            panel.Controls.Add(btnRevenue);
            panel.Controls.Add(btnDishStats);
            panel.Controls.Add(btnWaiterStats);

            page.Controls.Add(panel);
            return page;
        }

        private void LoadData()
        {
            // Обновляем Dashboard
            // ИСПРАВЛЕНО: Добавлена проверка на null для устранения CS8602
            if (mainTabControl?.SelectedTab?.Text == "Dashboard")
            {
                UpdateDashboard();
            }
        }

        private void UpdateDashboard()
        {
            // Статистика за сегодня
            var todayReservations = reservationService.GetTodayReservations();
            var activeOrders = orderService.GetActiveOrders();
            
            var revenueToday = orderService.GetTotalRevenueForPeriod(
                DateTime.Today.Date, 
                DateTime.Today.Date.AddDays(1).AddTicks(-1)
            );

            lblReservationsCount.Text = $"Брони:\n{todayReservations.Count}";
            lblActiveOrdersCount.Text = $"Активные заказы:\n{activeOrders.Count}";
            lblRevenueToday.Text = $"Выручка:\n{revenueToday:C}";

            // Активные бронирования
            listActiveReservations.Items.Clear();
            var activeReservations = reservationService.GetActiveReservations();
            foreach (var res in activeReservations)
            {
                res.PrintReservationInfo(out string info);
                listActiveReservations.Items.Add(info.Replace('\n', ' '));
            }

            // Активные заказы
            listActiveOrders.Items.Clear();
            foreach (var order in activeOrders)
            {
                order.PrintOrderInfo(out string info);
                listActiveOrders.Items.Add(info.Split('\n').First());
            }
        }

        private void ShowRevenueStatistics()
        {
            try
            {
                // Форма для выбора периода
                var form = new Form { Text = "Статистика выручки", Size = new Size(400, 200), StartPosition = FormStartPosition.CenterParent };
                var dtpStart = new DateTimePicker { Location = new Point(20, 20), Format = DateTimePickerFormat.Short };
                var dtpEnd = new DateTimePicker { Location = new Point(20, 50), Format = DateTimePickerFormat.Short };
                var btnShow = new Button { Text = "Показать", Location = new Point(20, 80) };
                
                form.Controls.Add(new Label { Text = "Начало:", Location = new Point(200, 23) });
                form.Controls.Add(dtpStart);
                form.Controls.Add(new Label { Text = "Конец:", Location = new Point(200, 53) });
                form.Controls.Add(dtpEnd);
                form.Controls.Add(btnShow);

                btnShow.Click += (s, e) =>
                {
                    try
                    {
                        var totalRevenue = orderService.GetTotalRevenueForPeriod(
                            dtpStart.Value.Date, 
                            dtpEnd.Value.Date.AddDays(1).AddTicks(-1)
                        );
                        
                        var stats = orderService.GetWaiterRevenue( 
                            dtpStart.Value.Date, 
                            dtpEnd.Value.Date.AddDays(1).AddTicks(-1)
                        );

                        var message = $"Общая выручка за период: {totalRevenue:C}\n\n";
                        message += "Выручка по официантам:\n";

                        foreach (var stat in stats)
                        {
                            message += $"{stat.Key}: {stat.Value:C}\n";
                        }
                        
                        MessageBox.Show(message, "Статистика выручки");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования статистики: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ShowWaiterStatistics()
        {
            try
            {
                // Форма для выбора периода (дублирование для удобства)
                var form = new Form { Text = "Статистика официантов", Size = new Size(400, 200), StartPosition = FormStartPosition.CenterParent };
                var dtpStart = new DateTimePicker { Location = new Point(20, 20), Format = DateTimePickerFormat.Short };
                var dtpEnd = new DateTimePicker { Location = new Point(20, 50), Format = DateTimePickerFormat.Short };
                var btnShow = new Button { Text = "Показать", Location = new Point(20, 80) };
                
                form.Controls.Add(new Label { Text = "Начало:", Location = new Point(200, 23) });
                form.Controls.Add(dtpStart);
                form.Controls.Add(new Label { Text = "Конец:", Location = new Point(200, 53) });
                form.Controls.Add(dtpEnd);
                form.Controls.Add(btnShow);

                btnShow.Click += (s, e) =>
                {
                    try
                    {
                        var stats = orderService.GetWaiterRevenue(
                            dtpStart.Value.Date, 
                            dtpEnd.Value.Date.AddDays(1).AddTicks(-1)
                        );

                        var message = "Выручка по официантам:\n";
                        foreach (var stat in stats)
                        {
                            message += $"{stat.Key}: {stat.Value:C}\n";
                        }
                        
                        MessageBox.Show(message, "Статистика официантов");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования статистики: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowDishStatistics()
        {
            try
            {
                var stats = orderService.GetDishStatistics(
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), 
                    DateTime.Today.AddDays(1).AddTicks(-1)); 
                    
                var message = "Статистика блюд за текущий месяц (Топ-10):\n";
                int count = 0;
                foreach (var stat in stats.Take(10))
                {
                    message += $"{stat.Key}: {stat.Value} заказов\n";
                    count++;
                }
                
                MessageBox.Show(message, "Статистика блюд");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования статистики: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}