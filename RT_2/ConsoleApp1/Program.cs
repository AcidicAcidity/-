using System;
// Это календарь на 2025 год 
class Program
{
    static void Main() //вывод управления
    {

        Console.WriteLine("=== КАЛЕНДАРЬ НА 2025 ГОД ===");

        while (true)
        {
            try
            {
                Console.WriteLine("\nДля выхода из программы введите 'выход'");
                Console.WriteLine("\nЭта программа показывет календарь на выбранный месяц");
                Console.WriteLine("\nВведите номер месяца (1-12) или название месяца:");
                string input = Console.ReadLine();

                if (input?.ToLower() == "выход")
                {
                    Console.WriteLine("До свидания!");
                    break;
                }

                int month = MonthNumber(input);
                ShowCalendar(2025, month);

                Console.WriteLine("\nНажмите Enter чтобы продолжить...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
// Функция ввода месяца 
    static int MonthNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new Exception("Вы ничего не ввели!");
        }

        // Проверка на ввод (словами или цифрами)
        if (int.TryParse(input, out int number))
        {
            if (number >= 1 && number <= 12)
            {
                return number;
            }
            else
            {
                throw new Exception("Месяц должен быть от 1 до 12!");
            }
        }

        string lowerInput = input.ToLower().Trim();
        // возможность вводить скоращённое название месяца
        if (lowerInput == "январь" || lowerInput.StartsWith("янв")) return 1;
        if (lowerInput == "февраль" || lowerInput.StartsWith("фев")) return 2;
        if (lowerInput == "март" || lowerInput.StartsWith("мар")) return 3;
        if (lowerInput == "апрель" || lowerInput.StartsWith("апр")) return 4;
        if (lowerInput == "май" || lowerInput.StartsWith("май")) return 5;
        if (lowerInput == "июнь" || lowerInput.StartsWith("июн")) return 6;
        if (lowerInput == "июль" || lowerInput.StartsWith("июл")) return 7;
        if (lowerInput == "август" || lowerInput.StartsWith("авг")) return 8;
        if (lowerInput == "сентябрь" || lowerInput.StartsWith("сен")) return 9;
        if (lowerInput == "октябрь" || lowerInput.StartsWith("окт")) return 10;
        if (lowerInput == "ноябрь" || lowerInput.StartsWith("ноя")) return 11;
        if (lowerInput == "декабрь" || lowerInput.StartsWith("дек")) return 12;

        throw new Exception($"Не понял '{input}'. Введите число (1-12) или название месяца!");
    }

    static void ShowCalendar(int year, int month) //вывод календаря
    {
        string[] monthNames = {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };

        string monthName = monthNames[month - 1];

        Console.WriteLine($"\n┌─────────────────────────────────────┐");
        Console.WriteLine($"│           {monthName} {year}           │");
        Console.WriteLine($"├─────────────────────────────────────┤");
        Console.WriteLine($"│  Пн  Вт  Ср  Чт  Пт  Сб  Вс ");
        Console.WriteLine($"├─────────────────────────────────────┤");

        DateTime firstDay = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int startDay = (int)firstDay.DayOfWeek;

        int dayCounter = 1;

        for (int week = 0; week < 6; week++)
        {
            if (dayCounter > daysInMonth) break;

            Console.Write("│ ");

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                if ((week == 0 && dayOfWeek < startDay) || dayCounter > daysInMonth)
                {
                    Console.Write("   ");
                }
                else
                {
                    bool isToday = (year == DateTime.Now.Year &&
                                   month == DateTime.Now.Month &&
                                   dayCounter == DateTime.Now.Day);

                    if (isToday)
                    {
                        Console.Write($"[{dayCounter,2}]");
                    }
                    else if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        Console.Write($" {dayCounter,2} ");
                    }
                    else
                    {
                        Console.Write($" {dayCounter,2} ");
                    }

                    dayCounter++;
                }
            }

            Console.WriteLine("  ");
        }

        Console.WriteLine("└─────────────────────────────────────┘");

        //Вывод сегодняшнего числа 
        Console.WriteLine("Сегодня: Число [" + DateTime.Now.Day + "] Месяц [" + DateTime.Now.Month + "] Год [" + DateTime.Now.Year + "]");
    }

}