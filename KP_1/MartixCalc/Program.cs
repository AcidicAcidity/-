using System;

class MatrixCalculator
{
    static void Main()
    {
        Console.WriteLine("Добро пожаловать в калькулятор матриц!");
        
        // Ввод размеров матриц
        Console.WriteLine("\nВведите размеры матриц (n x m):");
        Console.Write("n (строки): ");
        int n = ReadPositiveInt();
        Console.Write("m (столбцы): ");
        int m = ReadPositiveInt();

        // Создание матриц
        int[,] matrix1 = new int[n, m];
        int[,] matrix2 = new int[n, m];

        // Заполнение матриц
        Console.WriteLine("\nЗаполнение первой матрицы:");
        FillMatrix(matrix1, "Матрица 1");
        
        Console.WriteLine("\nЗаполнение второй матрицы:");
        FillMatrix(matrix2, "Матрица 2");

        // Вывод матриц
        Console.WriteLine("\nИтоговые матрицы:");
        PrintMatrix(matrix1, "Матрица 1");
        PrintMatrix(matrix2, "Матрица 2");

        // выбор операции с матрицами
        while (true)
        {
            Console.WriteLine("\nВыберите операцию:");
            Console.WriteLine("1 - Сложение матриц");
            Console.WriteLine("2 - Умножение матриц");
            Console.WriteLine("0 - Выход");
            Console.Write("Ваш выбор: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddMatrices(matrix1, matrix2);
                    break;
                case "2":
                    MultiplyMatrices(matrix1, matrix2);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неизвестная команда!");
                    break;
            }
        }
    }

    // Проверка на положительное число для ввода размерности матрицы
    static int ReadPositiveInt()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                return value;
            Console.Write("Ошибка! Введите положительное целое число: ");
        }
    }


    // Проверка на целое число для ввода в матрицу или обозначения границ рандомных чисел
    static int ReadInt()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;
            Console.Write("Ошибка! Введите целое число: ");
        }
    }

    // Выбор способа заполнения матрицы
    static void FillMatrix(int[,] matrix, string name)
    {
        Console.WriteLine($"Способ заполнения '{name}':");
        Console.WriteLine("1 - Ввод с клавиатуры");
        Console.WriteLine("2 - Заполнение случайными числами");
        Console.Write("Ваш выбор: ");

        switch (Console.ReadLine())
        {
            case "1":
                FillFromKeyboard(matrix, name);
                break;
            case "2":
                FillRandom(matrix, name);
                break;
            default:
                Console.WriteLine("Неверный выбор, используется ввод с клавиатуры");
                FillFromKeyboard(matrix, name);
                break;
        }
    }

    // Метод для ввода чисел в матрицу с клавиатуры
    static void FillFromKeyboard(int[,] matrix, string name)
    {
        Console.WriteLine($"\nВведите элементы {name} построчно (через пробел):");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            while (true)
            {
                Console.Write($"Строка {i + 1}: ");
                string[] row = Console.ReadLine().Split();

                if (row.Length != matrix.GetLength(1))
                {
                    Console.WriteLine($"Ошибка! Введите {matrix.GetLength(1)} элементов");
                    continue;
                }

                bool error = false;
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (!int.TryParse(row[j], out matrix[i, j]))
                    {
                        Console.WriteLine("Ошибка! Используйте только целые числа");
                        error = true;
                        break;
                    }
                }
                if (!error) break;
            }
        }
    }

    static void FillRandom(int[,] matrix, string name)
    {
        Console.WriteLine("\nВведите границы случайных чисел:");
        Console.Write("Минимальное значение (a): ");
        int a = ReadInt();
        Console.Write("Максимальное значение (b): ");
        int b = ReadInt();

        if (a > b) (a, b) = (b, a);

        Random rnd = new Random();
        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                matrix[i, j] = rnd.Next(a, b + 1);

        Console.WriteLine($"{name} заполнена случайными числами");
    }

    // Метод для вывода матрицы
    static void PrintMatrix(int[,] matrix, string name)
    {
        Console.WriteLine($"\n{name}:");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
                Console.Write($"{matrix[i, j],4} ");
            Console.WriteLine();
        }
    }

    // Сложение матриц
    static void AddMatrices(int[,] m1, int[,] m2)
    {
        if (m1.GetLength(0) != m2.GetLength(0) ||
            m1.GetLength(1) != m2.GetLength(1))
        {
            Console.WriteLine("Ошибка! Сложение невозможно - размеры матриц не совпадают");
            return;
        }

        int[,] result = new int[m1.GetLength(0), m1.GetLength(1)];
        for (int i = 0; i < result.GetLength(0); i++)
            for (int j = 0; j < result.GetLength(1); j++)
                result[i, j] = m1[i, j] + m2[i, j];

        Console.WriteLine("\nРезультат сложения:");
        PrintMatrix(result, "Сумма матриц");
    }

    // Умножение матриц
    static void MultiplyMatrices(int[,] m1, int[,] m2)
    {
        if (m1.GetLength(1) != m2.GetLength(0))
        {
            Console.WriteLine("Ошибка! Умножение невозможно - количество столбцов первой матрицы " +
                            "не равно количеству строк второй матрицы");
            return;
        }

        int[,] result = new int[m1.GetLength(0), m2.GetLength(1)];
        for (int i = 0; i < m1.GetLength(0); i++)
            for (int j = 0; j < m2.GetLength(1); j++)
                for (int k = 0; k < m1.GetLength(1); k++)
                    result[i, j] += m1[i, k] * m2[k, j];

        Console.WriteLine("\nРезультат умножения:");
        PrintMatrix(result, "Произведение матриц");
    }
}