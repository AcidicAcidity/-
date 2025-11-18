using System;

class Program
{
    static bool CheckPlacement(int modulesCount, int moduleWidth, int moduleHeight, int fieldWidth, int fieldHeight, int padding)
    {
        int horizontalModules = fieldWidth / (moduleWidth + 2 * padding);
        int verticalModules = fieldHeight / (moduleHeight + 2 * padding);
        if (horizontalModules * verticalModules >= modulesCount) return true;

        horizontalModules = fieldWidth / (moduleHeight + 2 * padding);
        verticalModules = fieldHeight / (moduleWidth + 2 * padding);
        return horizontalModules * verticalModules >= modulesCount;
    }

    static void Main()
    {
        Console.Write("Введите количество модулей (n): ");
        int modulesCount = int.Parse(Console.ReadLine());
        Console.Write("Введите размеры модуля (a b): ");
        string[] dimensions = Console.ReadLine().Split();
        int moduleA = int.Parse(dimensions[0]), moduleB = int.Parse(dimensions[1]);
        Console.Write("Введите размеры поля (w h): ");
        dimensions = Console.ReadLine().Split();
        int fieldW = int.Parse(dimensions[0]), fieldH = int.Parse(dimensions[1]);

        int minPadding = 0, maxPadding = Math.Min(fieldW, fieldH) / 2, optimalPadding = -1;

        while (minPadding <= maxPadding)
        {
            int currentPadding = (minPadding + maxPadding) / 2;
            if (CheckPlacement(modulesCount, moduleA, moduleB, fieldW, fieldH, currentPadding))
            {
                optimalPadding = currentPadding;
                minPadding = currentPadding + 1;
            }
            else
            {
                maxPadding = currentPadding - 1;
            }
        }

        Console.WriteLine($"Максимальная толщина защиты: {optimalPadding}");
    }
}