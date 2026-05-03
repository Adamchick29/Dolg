using System;
using System.Text;

class Hierarchy_Analysis
{
    static int CheckCriteriaAmount()
    {
        while (true)
        {
            // Цикл бесконечный для того, чтобы программа продолжала работу даже после
            // ошибочного ввода и запрашивала ввести данные заново 
            Console.Write("Введите количество критериев (не менее 2): ");
            if (int.TryParse(Console.ReadLine(), out int count) && count >= 2)
                return count;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка: введите целое число больше или равное 2");
            Console.ResetColor();
        }
    }
    
    static double CheckComparsion(string prompt)
    {
        // Цикл бесконечный для того, чтобы программа продолжала работу даже после
        // ошибочного ввода и запрашивала ввести данные заново 
        while (true)
        {
            Console.Write($"{prompt}: ");
            
            // Для удобства и отказоустойчивости приводим все дроби вида "0.00" к "0,00",
            // (Так как C# с установленной русской культурой не считает дродь вида 0.00 при использовании метода
            // TryParse) 
            // И разбиваем ввод на элементы, чтобы потом проверить количество введеных коэффициентов
            string[] input = Console.ReadLine().Replace('.', ',').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Проверяем, введено ли одно число, и не введены ли числа типа "0./00" или "0/.00"
            if (input.Length == 1 &&  !(input[0].Contains('/') && input[0].Contains(','))) 
            {
                // Попытка считать коэффиценты больше 1
                if (int.TryParse(input[0], out int value) && value > 0)
                    return value;
                
                // Попытка считать числа с точкой
                if (double.TryParse(input[0], out double comparsionValue) && comparsionValue > 0 && comparsionValue < 1)
                    return comparsionValue;

                // Попытка считать дроби вида a/b
                if (input[0].Contains('/'))
                {
                    string[] parts = input[0].Split('/');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double upper) &&
                        double.TryParse(parts[1], out double lower) &&
                        lower != 0 &&
                        upper / lower < 1)
                    {
                        if (upper > 0 && lower > 0)
                            return upper / lower;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка: Число должно быть положительным, и либо целым, либо дробью (например 0.25 или 1/4)");
            Console.ResetColor();
        }
    }
    
    static double[,] BuildComparisonMatrix(int size)
    {
        double[,] matrix = new double[size, size];
        double[] sums = new double[size + 1];
        
        // Проставляем главную диагональ однерками, так как это сравнение критериев самих с собой
        for (int i = 0; i < size; i++) {matrix[i, i] = 1.0;}
        
        Console.WriteLine("\nШкала относительной важности:");
        Console.WriteLine("1 - Равная важность");
        Console.WriteLine("3 - Умеренное превосходство");
        Console.WriteLine("5 - Существенное превосходство");
        Console.WriteLine("7 - Значительное превосходство");
        Console.WriteLine("9 - Абсолютное превосходство");
        Console.WriteLine("Для оценки ниже 1 можно использовать формат 1/5 или 0.2\n");

        // Пользователь вводит только верхний треугольник матрицы т.к нижний содержит коэффициенты, обратные верхним 
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                // Перед записью убеждаемся в правильности введенных коэффициентов
                matrix[i, j] = CheckComparsion($"Сравнение критериев {i + 1} и {j + 1}");
                // Коэффициенты, обратные верхним
                matrix[j, i] = 1.0 / matrix[i, j]; 
            }
        }
        return matrix;
    }
    
    static void DisplayMatrix(double[,] matrix)
    {
        // Размерность матрицы
        int matrixSize = matrix.GetLength(0);
        // Веса каждой строки + общая сумма весов
        double[] weights = new double[matrixSize + 1];
        // Вывод таблицы
        Console.WriteLine("\nМатрица попарных сравнений:");
        Console.Write("        ");
        for (int i = 0; i < matrixSize; i++)
        {
            Console.Write($" Крит.{i+1, 2}");
        }
        
        double sum = 0;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(" Сумма");
        Console.ResetColor();
        
        
        for (int i = 0; i < matrixSize; i++)
        {
            double stringSum = 0;
            Console.Write($"Крит.{i + 1,2} ");
            for (int j = 0; j < matrixSize; j++)
            {
                Console.Write($"{matrix[i, j],8:F2}");
                stringSum += matrix[i, j];
            }
            weights[i] = stringSum;
            sum += stringSum;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{stringSum,8:F2}");
            Console.ResetColor();
        }
        // Вывод суммы
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Сумма коэффициентов: {sum :F2}");
        Console.ResetColor();
        
        weights[matrixSize] = sum;
        // Вывод коэффициентов
        DisplayWeights(weights);
    }
    
    static void DisplayWeights(double[] weights)
    {
        double weightsSum = weights[weights.Length - 1];
        Console.WriteLine("\nВесовые коэффициенты:");
        for (int i = 0; i < weights.Length - 1; i++)
            Console.WriteLine($"Критерий {i + 1}: {weights[i] / weightsSum:F3}");
    }

    static void Main()
    {
        // Установка кодировки доя корректного вывода и ввода кириллицы
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        
        // Старт программы
        Console.WriteLine("Анализ иерархий");
        Console.WriteLine("====================================");

        // Проверка введенного числа критериев и их запись в переменную
        int criteriaAmount = CheckCriteriaAmount();

        // Создание матрицы с весовыми коэффициентами, размерность таблицы = кол-ву критериев
        double[,] matrix = BuildComparisonMatrix(criteriaAmount);
        
        DisplayMatrix(matrix);
        Console.WriteLine("Для завершения работы нажмите любую клавишу...");
        Console.ReadKey();
    }
}
    
