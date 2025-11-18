using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using ReservationApp.Models; // ИСПРАВЛЕНО
using ReservationApp.Services; // ИСПРАВЛЕНО

namespace ReservationApp.Forms // ИСПРАВЛЕНО
{
    public class TableManagementForm : Form
    {
        private TableManagementService tableService = null!;
        private ReservationService reservationService = null!;
        private IContainer components = null!;
        private DataGridView dgvTables = null!;
        private Button btnAddTable = null!;
        private Button btnEditTable = null!;
        private Button btnToggleStatus = null!;
        private ComboBox cmbLocation = null!;
        private NumericUpDown numSeats = null!;
        private TextBox txtTableInfo = null!;
        private Label lblStatus = null!;

        public TableManagementForm(TableManagementService tableService, ReservationService reservationService)
        {
            this.tableService = tableService;
            this.reservationService = reservationService;
            InitializeUI();
            LoadTables();
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
            this.Text = "Управление столами";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // ... (Разметка элементов управления, опущено для краткости) ...

            // Инициализация элементов и привязка событий
            dgvTables = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnAddTable = new Button { Text = "Добавить стол", Dock = DockStyle.Top };
            btnEditTable = new Button { Text = "Редактировать", Dock = DockStyle.Top };
            btnToggleStatus = new Button { Text = "Сменить статус", Dock = DockStyle.Top };
            txtTableInfo = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill };
            cmbLocation = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            
            // ... (Инициализация остальных контролов) ...

            cmbLocation.DataSource = Enum.GetValues(typeof(TableLocation));

            // Привязка событий
            btnAddTable.Click += (s, e) => AddTable();
            btnEditTable.Click += (s, e) => EditTable();
            btnToggleStatus.Click += (s, e) => ToggleTableStatus();
            dgvTables.SelectionChanged += (s, e) => OnTableSelected();
        }

        private void LoadTables()
        {
            dgvTables.DataSource = null;
            dgvTables.DataSource = reservationService.GetAllTables();
            lblStatus.Text = $"Загружено столов: {reservationService.GetAllTables().Count}";
        }

        private void OnTableSelected()
        {
            if (dgvTables.SelectedRows.Count > 0)
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var table = dgvTables.SelectedRows[0].DataBoundItem as Table;
                if (table != null)
                {
                    txtTableInfo.Text = tableService.GetTableDetailedInfo(table.Id);
                    numSeats.Value = table.Seats;
                    cmbLocation.SelectedItem = table.Location;
                    btnToggleStatus.Text = table.IsActive ? "Сделать неактивным" : "Сделать активным";
                }
            }
            else
            {
                txtTableInfo.Clear();
            }
        }

        private void AddTable()
        {
            try
            {
                reservationService.CreateTable(
                    (TableLocation)cmbLocation.SelectedItem!,
                    (int)numSeats.Value,
                    out Table newTable
                );
                lblStatus.Text = $"Стол #{newTable.Id} успешно добавлен!";
                LoadTables();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка добавления стола: {ex.Message}";
            }
        }

        private void EditTable()
        {
            if (dgvTables.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите стол для редактирования";
                return;
            }

            try
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлены проверки на null
                if (dgvTables.SelectedRows[0].Cells["Id"].Value != null)
                {
                    var tableId = (int)dgvTables.SelectedRows[0].Cells["Id"].Value;
                    
                    tableService.EditTableInfo(
                        tableId,
                        (TableLocation)cmbLocation.SelectedItem!,
                        (int)numSeats.Value,
                        out bool success
                    );

                    if (success)
                    {
                        lblStatus.Text = $"Стол #{tableId} успешно обновлен!";
                        LoadTables();
                        OnTableSelected(); // Обновляем детальную информацию
                    }
                    else
                    {
                        lblStatus.Text = "Не удалось обновить стол";
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка редактирования стола: {ex.Message}";
            }
        }

        // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
        private void ToggleTableStatus()
        {
            if (dgvTables.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите стол";
                return;
            }

            if (dgvTables.SelectedRows[0].Cells["Id"].Value != null)
            {
                var tableId = (int)dgvTables.SelectedRows[0].Cells["Id"].Value;
                
                tableService.ToggleTableStatus(tableId, out bool newStatus);
                
                lblStatus.Text = $"Стол #{tableId} теперь {(newStatus ? "активен" : "неактивен")}";
                LoadTables();
                OnTableSelected();
            }
        }
    }
}