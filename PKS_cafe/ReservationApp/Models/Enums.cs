using System.Linq;

namespace ReservationApp.Models // ИСПРАВЛЕНО: Добавлено пространство имен
{
    public enum TableLocation
    {
        Window,
        Aisle,
        Exit,
        Back
    }

    public enum DishCategory
    {
        Drinks,
        Salads,
        ColdAppetizers,
        HotAppetizers,
        Soups,
        MainCourses,
        Desserts
    }

    public enum OrderStatus
    {
        Active,
        Cooking,
        Ready,
        Served,
        Closed
    }
}