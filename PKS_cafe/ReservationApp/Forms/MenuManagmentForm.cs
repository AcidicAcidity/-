using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using ReservationApp.Services;
using ReservationApp.Models; // ИСПРАВЛЕНО

namespace ReservationApp.Forms // ИСПРАВЛЕНО
{
    public class MenuManagementForm : Form
    {
        private OrderService orderService = null!;
        private IContainer components = null!;
        private DataGridView dgvDishes = null!;
        private Button btnAddDish = null!;
        private Button btnEditDish = null!;
        private Button btnToggleAvailability = null!;
        private TextBox txtDishName = null!;
        private TextBox txtIngredients = null!;
        private TextBox txtDescription = null!;
        private NumericUpDown numPrice = null!;
        private NumericUpDown numCookingTime = null!;
        private ComboBox cmbCategory = null!;
        private Label lblStatus = null!;

        public MenuManagementForm(OrderService service)
        {
            orderService = service;
            InitializeUI();
            LoadDishes();
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
            this.Text = "Управление меню";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // ... (Разметка элементов управления, опущено для краткости) ...

            // Инициализация элементов
            dgvDishes = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnAddDish = new Button { Text = "Добавить", Dock = DockStyle.Top };
            btnEditDish = new Button { Text = "Редактировать", Dock = DockStyle.Top };
            btnToggleAvailability = new Button { Text = "Сменить статус", Dock = DockStyle.Top };
            
            // ... (Инициализация остальных контролов) ...

            // Привязка событий
            btnAddDish.Click += (s, e) => AddDish();
            btnEditDish.Click += (s, e) => EditDish();
            btnToggleAvailability.Click += (s, e) => ToggleDishAvailability();
            dgvDishes.SelectionChanged += (s, e) => OnDishSelected();

            cmbCategory.DataSource = Enum.GetValues(typeof(DishCategory));
        }

        private void LoadDishes()
        {
            dgvDishes.DataSource = null;
            dgvDishes.DataSource = orderService.GetAllDishes();
            lblStatus.Text = $"Загружено блюд: {orderService.GetAllDishes().Count}";
        }

        private void OnDishSelected()
        {
            if (dgvDishes.SelectedRows.Count > 0)
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var dish = dgvDishes.SelectedRows[0].DataBoundItem as Dish;
                if (dish != null)
                {
                    txtDishName.Text = dish.Name;
                    txtIngredients.Text = dish.Ingredients;
                    txtDescription.Text = dish.Description;
                    numPrice.Value = dish.Price;
                    numCookingTime.Value = dish.CookingTime;
                    cmbCategory.SelectedItem = dish.Category;
                    btnToggleAvailability.Text = dish.IsAvailable ? "Сделать недоступным" : "Сделать доступным";
                }
            }
        }

        private void AddDish()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDishName.Text))
                {
                    lblStatus.Text = "Введите название блюда";
                    return;
                }

                Dish newDish;
                // ИСПРАВЛЕНО: Вызов AddNewDish с out (строка 204)
                orderService.AddNewDish( 
                    txtDishName.Text,
                    txtIngredients.Text,
                    numPrice.Value,
                    (DishCategory)cmbCategory.SelectedItem!,
                    (int)numCookingTime.Value,
                    txtDescription.Text,
                    out newDish // ИСПРАВЛЕНО: Добавлен out
                );

                lblStatus.Text = $"Блюдо \"{newDish.Name}\" успешно добавлено! ID: {newDish.Id}";
                LoadDishes();
                ClearForm();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка добавления блюда: {ex.Message}";
            }
        }

        private void EditDish()
        {
            if (dgvDishes.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите блюдо для редактирования";
                return;
            }

            try
            {
                // БЕЗОПАСНАЯ РАСПАКОВКА - добавлена проверка на null
                var dish = dgvDishes.SelectedRows[0].DataBoundItem as Dish;
                if (dish != null)
                {
                    orderService.EditDish(
                        dish.Id,
                        txtDishName.Text,
                        txtIngredients.Text,
                        numPrice.Value,
                        (DishCategory)cmbCategory.SelectedItem!,
                        (int)numCookingTime.Value,
                        txtDescription.Text,
                        out bool success
                    );

                    if (success)
                    {
                        lblStatus.Text = $"Блюдо \"{dish.Name}\" успешно обновлено!";
                        LoadDishes();
                    }
                    else
                    {
                        lblStatus.Text = "Не удалось обновить блюдо";
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Ошибка редактирования блюда: {ex.Message}";
            }
        }

        // БЕЗОПАСНАЯ РАСПАКОВКА - добавлены проверки на null
        private void ToggleDishAvailability()
        {
            if (dgvDishes.SelectedRows.Count == 0)
            {
                lblStatus.Text = "Выберите блюдо";
                return;
            }

            if (dgvDishes.SelectedRows[0].Cells["Id"].Value != null && 
                dgvDishes.SelectedRows[0].Cells["Name"].Value != null)
            {
                var dishId = (int)dgvDishes.SelectedRows[0].Cells["Id"].Value;
                var dishName = dgvDishes.SelectedRows[0].Cells["Name"].Value.ToString() ?? "";
                
                orderService.ToggleDishAvailability(dishId, out bool newStatus);
                
                lblStatus.Text = $"Блюдо \"{dishName}\" теперь {(newStatus ? "доступно" : "недоступно")}";
                LoadDishes();
            }
        }

        private void ClearForm()
        {
            txtDishName.Clear();
            txtIngredients.Clear();
            txtDescription.Clear();
            numPrice.Value = 0;
            numCookingTime.Value = 0;
            cmbCategory.SelectedIndex = 0;
            lblStatus.Text = "Форма очищена";
        }
    }
}