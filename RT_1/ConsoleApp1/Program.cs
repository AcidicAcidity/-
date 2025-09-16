//Это калькулятор :)
using System;

class Calculator
{
    private double memory = 0;
    private double currentValue = 0;
    private string display = "0";
    private bool newInput = true;

    public void Run()
    {
        Console.WriteLine("Калькулятор - Введите команду:");
        Console.WriteLine("Доступные операции: +, -, *, /, %, 1/x, x^2, sqrt, M+, M-, MR, C (clear), off (выход)");
        
        while (true)
        {
            Console.WriteLine($"\nТекущее значение: {display}");
            Console.WriteLine($"Память: {memory}");
            Console.Write("Введите число или команду: ");
            
            string input = Console.ReadLine()?.Trim() ?? "";
            
            if (input.ToLower() == "off")
            {
                Console.WriteLine("Выход из калькулятора.");
                break;
            }
            
            if (input.ToLower() == "c")
            {
                Clear();
                continue;
            }
            
            if (input.ToLower() == "help")
            {
                ShowHelp();
                continue;
            }
            
            ProcessInput(input);
        }
    }

    private void ShowHelp()
    {
        Console.WriteLine("\n=== СПРАВКА ПО КОМАНДАМ КАЛЬКУЛЯТОРА ===");
        Console.WriteLine("+          - сложение");
        Console.WriteLine("-          - вычитание");
        Console.WriteLine("*          - умножение");
        Console.WriteLine("/          - деление");
        Console.WriteLine("%          - остаток от деления");
        Console.WriteLine("1/x        - обратное число");
        Console.WriteLine("x^2        - возведение в квадрат");
        Console.WriteLine("sqrt       - квадратный корень");
        Console.WriteLine("M+         - добавить в память");
        Console.WriteLine("M-         - вычесть из памяти");
        Console.WriteLine("MR         - восстановить из памяти");
        Console.WriteLine("C          - очистить калькулятор");
        Console.WriteLine("help       - показать справку");
        Console.WriteLine("off        - выход из программы");
        Console.WriteLine("=========================================");
    }

    private void ProcessInput(string input)
    {
        // Проверка корректности ввода команд памяти
        if (input.ToUpper() == "M+")
        {
            memory += currentValue;
            Console.WriteLine($"Добавлено в память: {currentValue}");
            return;
        }
        
        if (input.ToUpper() == "M-")
        {
            memory -= currentValue;
            Console.WriteLine($"Вычтено из памяти: {currentValue}");
            return;
        }
        
        if (input.ToUpper() == "MR")
        {
            currentValue = memory;
            display = FormatNumber(memory);
            newInput = true;
            Console.WriteLine($"Восстановлено из памяти: {memory}");
            return;
        }
        
        // Проверка на корректный ввод операции
        if (IsOperation(input))
        {
            PerformOperation(input);
            return;
        }
        
        // Проверка ввода на дурака
        if (IsValidNumber(input))
        {
            if (newInput)
            {
                currentValue = double.Parse(input);
                display = FormatNumber(currentValue);
                newInput = false;
            }
            else
            {
                // Ограничение на кол-во цифр
                string newDisplay = display == "0" ? input : display + input;
                if (newDisplay.Replace(",", "").Replace(".", "").Length <= 16)
                {
                    display = newDisplay;
                    currentValue = double.Parse(display);
                }
                else
                {
                    Console.WriteLine("Ошибка: Число не может содержать более 16 цифр!");
                }
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Ошибка: Вводите только цифры и допустимые операции!");
        }
    }

    private bool IsOperation(string input)
    {
        string[] operations = { "+", "-", "*", "/", "%", "1/x", "x^2", "sqrt" };
        return Array.Exists(operations, op => op.Equals(input, StringComparison.OrdinalIgnoreCase));
    }

    private void PerformOperation(string operation)
    {
        try
        {
            switch (operation.ToLower())
            {
                case "+":
                    Console.Write("Введите второе число: ");
                    string addInput = Console.ReadLine()?.Trim() ?? "";
                    if (IsValidNumber(addInput))
                    {
                        double addNum = double.Parse(addInput);
                        currentValue += addNum;
                        display = FormatNumber(currentValue);
                    }
                    break;
                    
                case "-":
                    Console.Write("Введите второе число: ");
                    string subInput = Console.ReadLine()?.Trim() ?? "";
                    if (IsValidNumber(subInput))
                    {
                        double subNum = double.Parse(subInput);
                        currentValue -= subNum;
                        display = FormatNumber(currentValue);
                    }
                    break;
                    
                case "*":
                    Console.Write("Введите второе число: ");
                    string mulInput = Console.ReadLine()?.Trim() ?? "";
                    if (IsValidNumber(mulInput))
                    {
                        double mulNum = double.Parse(mulInput);
                        currentValue *= mulNum;
                        display = FormatNumber(currentValue);
                    }
                    break;
                    
                case "/":
                    Console.Write("Введите второе число: ");
                    string divInput = Console.ReadLine()?.Trim() ?? "";
                    if (IsValidNumber(divInput))
                    {
                        double divNum = double.Parse(divInput);
                        if (divNum == 0)
                        {
                            Console.WriteLine("Ошибка: Деление на ноль невозможно!");
                            return;
                        }
                        currentValue /= divNum;
                        display = FormatNumber(currentValue);
                    }
                    break;
                    
                case "%":
                    Console.Write("Введите второе число: ");
                    string modInput = Console.ReadLine()?.Trim() ?? "";
                    if (IsValidNumber(modInput))
                    {
                        double modNum = double.Parse(modInput);
                        if (modNum == 0)
                        {
                            Console.WriteLine("Ошибка: Деление на ноль невозможно!");
                            return;
                        }
                        currentValue %= modNum;
                        display = FormatNumber(currentValue);
                    }
                    break;
                    
                case "1/x":
                    if (currentValue == 0)
                    {
                        Console.WriteLine("Ошибка: Деление на ноль невозможно!");
                        return;
                    }
                    currentValue = 1 / currentValue;
                    display = FormatNumber(currentValue);
                    break;
                    
                case "x^2":
                    currentValue *= currentValue;
                    display = FormatNumber(currentValue);
                    break;
                    
                case "sqrt":
                    if (currentValue < 0)
                    {
                        Console.WriteLine("Ошибка: Нельзя извлечь корень из отрицательного числа!");
                        return;
                    }
                    currentValue = Math.Sqrt(currentValue);
                    display = FormatNumber(currentValue);
                    break;
            }
            
            newInput = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private bool IsValidNumber(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        
        // Проверка на корректный ввод числа
        foreach (char c in input)
        {
            if (!char.IsDigit(c) && c != ',' && c != '.' && c != '-')
                return false;
        }
        // Ограничение на 16 цифр в числах с плавающей запятой и отрицательных
        string digitsOnly = input.Replace(",", "").Replace(".", "").Replace("-", "");
        if (digitsOnly.Length > 16)
        {
            Console.WriteLine("Ошибка: Число не может содержать более 16 цифр!");
            return false;
        }
        
        return double.TryParse(input, out _);
    }

    private string FormatNumber(double number)
    {
        string result = number.ToString();
        
        // Убираем лишние нули после запятой
        if (result.Contains(",") || result.Contains("."))
        {
            result = result.TrimEnd('0');
            if (result.EndsWith(",") || result.EndsWith("."))
                result = result.Substring(0, result.Length - 1);
        }
        
        return result;
    }

    private void Clear()
    {
        currentValue = 0;
        display = "0";
        newInput = true;
        Console.WriteLine("Калькулятор очищен.");
    }

    static void Main()
    {
        Calculator calculator = new Calculator();
        calculator.Run();
    }
}