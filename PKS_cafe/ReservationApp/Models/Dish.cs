using System.Linq;

namespace ReservationApp.Models // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DishCategory Category { get; set; }
        public int CookingTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string Description { get; set; } = string.Empty;

        public Dish CreateDish(in int id, in string name, in string ingredients,
            in decimal price, in DishCategory category, in int cookingTime, in string description)
        {
            Id = id;
            Name = name;
            Ingredients = ingredients;
            Price = price;
            Category = category;
            CookingTime = cookingTime;
            Description = description;
            return this;
        }

        public void EditDish(ref Dish dish, in string newName, in string newIngredients,
            in decimal newPrice, in int newCookingTime, in string newDescription, in bool isAvailable)
        {
            dish.Name = newName;
            dish.Ingredients = newIngredients;
            dish.Price = newPrice;
            dish.CookingTime = newCookingTime;
            dish.Description = newDescription;
            dish.IsAvailable = isAvailable;
        }

        public void PrintDishInfo(out string info)
        {
            info = $"#{Id} {Name}\nКатегория: {Category}\nЦена: {Price} руб.\n" +
                   $"Время готовки: {CookingTime} мин.\nСостав: {Ingredients}\n" +
                   $"{(IsAvailable ? "✅ Доступно" : "❌ Недоступно")}";
        }
    }
}