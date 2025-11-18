using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReservationApp.Models;
using ReservationApp.Services; // ИСПРАВЛЕНО

namespace ReservationApp.Forms // ИСПРАВЛЕНО
{
    public class OrderManagementForm : Form
    {
        private OrderService orderService = null!;
        private ReservationService reservationService = null!;
        private IContainer components = null!;
        private DataGridView dgvActiveOrders = null!;
        private DataGridView dgvAllOrders = null!;
        private DataGridView dgvMenu = null!;
        private ListBox lstSelectedDishes = null!;
        private Button btnCreateOrder = null!;
        private Button btnUpdateStatus = null!;
        private Button btnMarkPaid = null!;
        private Button btnPrintReceipt = null!;
        private Button btnAddDish = null!;
        private Button btnRemoveDish = null!;
        private ComboBox cmbWaiters = null!;
        private ComboBox cmbStatus = null!;
        private NumericUpDown numTableId = null!;
        private TextBox txtOrderComment = null!;
        private Label lblTotalPrice = null!;
        private List<Dish> selectedDishes = new List<Dish>();

        public OrderManagementForm(OrderService orderService, ReservationService reservationService)
        {
            this.orderService = orderService;
            this.reservationService = reservationService;
            InitializeUI();
            LoadData();
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
            this.Text = "Управление заказами";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // ... (Разметка элементов управления, опущено для краткости) ...

            // Инициализация элементов и привязка событий
            dgvMenu = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvActiveOrders = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvAllOrders = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            lstSelectedDishes = new ListBox { Dock = DockStyle.Fill };
            cmbWaiters = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            
            // ... (Инициализация остальных контролов) ...

            cmbStatus.DataSource = Enum.GetValues(typeof(OrderStatus));

            btnAddDish = new Button { Text = ">> Добавить блюдо" };
            btnRemoveDish = new Button { Text = "<< Удалить блюдо" };
            btnCreateOrder = new Button { Text = "Создать заказ" };
            btnUpdateStatus = new Button { Text = "Обновить статус" };
            btnMarkPaid = new Button { Text = "Оплачено" };
            btnPrintReceipt = new Button { Text = "Распечатать чек" };

            // Привязка событий
            btnAddDish.Click += (s, e) => AddDishToOrder();
            btnRemoveDish.Click += (s, e) => RemoveDishFromOrder();
            btnCreateOrder.Click += (s, e) => CreateNewOrder();
            btnUpdateStatus.Click += (s, e) => UpdateOrderStatus();
            btnMarkPaid.Click += (s, e) => MarkOrderAsPaid();
            btnPrintReceipt.Click += (s, e) => PrintReceipt();
            dgvActiveOrders.SelectionChanged += (s, e) => OnActiveOrderSelected();
        }

        private void LoadData()
        {
            dgvMenu.DataSource = orderService.GetAllDishes().Where(d => d.IsAvailable).ToList();
            dgvActiveOrders.DataSource = orderService.GetActiveOrders();
            dgvAllOrders.DataSource = orderService.GetAllOrders();
            cmbWaiters.DataSource = orderService.GetAllWaiters();
            cmbWaiters.DisplayMember = "Name";
            cmbWaiters.ValueMember = "Id";
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            lblTotalPrice.Text = $"Итого: {selectedDishes.Sum(d => d.Price):C}";
        }

        private void AddDishToOrder()
        {
            if (dgvMenu.SelectedRows.Count > 0)
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var dish = dgvMenu.SelectedRows[0].DataBoundItem as Dish;
                if (dish != null)
                {
                    selectedDishes.Add(dish);
                    lstSelectedDishes.Items.Add($"{dish.Name} ({dish.Price:C})");
                    UpdateTotalPrice();
                }
            }
        }

        private void RemoveDishFromOrder()
        {
            if (lstSelectedDishes.SelectedIndex != -1)
            {
                int index = lstSelectedDishes.SelectedIndex;
                selectedDishes.RemoveAt(index);
                lstSelectedDishes.Items.RemoveAt(index);
                UpdateTotalPrice();
            }
        }

        private void CreateNewOrder()
        {
            if (cmbWaiters.SelectedItem == null)
            {
                MessageBox.Show("Выберите официанта", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selectedDishes.Count == 0)
            {
                MessageBox.Show("Добавьте блюда в заказ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedWaiter = (Waiter)cmbWaiters.SelectedItem;
            
            Order newOrder;
            // ИСПРАВЛЕНО: Добавлено ключевое слово out перед order (строка 329)
            orderService.CreateOrder(
                (int)numTableId.Value,
                selectedWaiter.Id,
                selectedWaiter.Name,
                txtOrderComment.Text,
                out newOrder, // ИСПРАВЛЕНО: Добавлен out
                selectedDishes.ToArray()
            );

            MessageBox.Show($"Заказ #{newOrder.Id} успешно создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            ClearCreationForm();
        }

        private void ClearCreationForm()
        {
            numTableId.Value = 1;
            txtOrderComment.Clear();
            lstSelectedDishes.Items.Clear();
            selectedDishes.Clear();
            UpdateTotalPrice();
        }

        private void OnActiveOrderSelected()
        {
            if (dgvActiveOrders.SelectedRows.Count > 0)
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var order = dgvActiveOrders.SelectedRows[0].DataBoundItem as Order;
                if (order != null)
                {
                    cmbStatus.SelectedItem = order.Status;
                }
            }
        }

        private void UpdateOrderStatus()
        {
            if (dgvActiveOrders.SelectedRows.Count == 0) return;

            // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
            if (dgvActiveOrders.SelectedRows[0].Cells["Id"].Value != null)
            {
                var orderId = (int)dgvActiveOrders.SelectedRows[0].Cells["Id"].Value;
                var newStatus = (OrderStatus)cmbStatus.SelectedItem!;

                orderService.UpdateOrderStatus(orderId, newStatus, out bool success);

                if (success)
                {
                    MessageBox.Show($"Статус заказа #{orderId} обновлен до {newStatus}", "Успех");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить статус (возможно, заказ уже закрыт)", "Ошибка");
                }
            }
        }

        private void MarkOrderAsPaid()
        {
            if (dgvActiveOrders.SelectedRows.Count == 0) return;

            // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
            if (dgvActiveOrders.SelectedRows[0].Cells["Id"].Value != null)
            {
                var orderId = (int)dgvActiveOrders.SelectedRows[0].Cells["Id"].Value;

                orderService.MarkOrderAsPaid(orderId, out bool success);

                if (success)
                {
                    MessageBox.Show($"Заказ #{orderId} оплачен и закрыт", "Успех");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Не удалось пометить заказ как оплаченный", "Ошибка");
                }
            }
        }

        // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
        private void PrintReceipt()
        {
            if (dgvAllOrders.SelectedRows.Count > 0 && dgvAllOrders.SelectedRows[0].Cells["Id"].Value != null)
            {
                var orderId = (int)dgvAllOrders.SelectedRows[0].Cells["Id"].Value;
                var order = orderService.GetOrderById(orderId);
                
                if (order != null && order.Status == OrderStatus.Closed)
                {
                    order.PrintReceipt(out string receipt);
                    
                    var receiptForm = new Form
                    {
                        Text = $"Чек для заказа #{orderId}",
                        Size = new Size(400, 500),
                        StartPosition = FormStartPosition.CenterParent
                    };
                    
                    var textBox = new TextBox
                    {
                        Text = receipt,
                        Multiline = true,
                        ReadOnly = true,
                        Dock = DockStyle.Fill,
                        Font = new Font("Consolas", 10),
                        ScrollBars = ScrollBars.Vertical
                    };
                    
                    receiptForm.Controls.Add(textBox);
                    receiptForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Чек можно распечатать только для закрытых заказов", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}