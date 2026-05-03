using System;
using System.Text;
using Serilog;
using Serilog.Sinks.File;


namespace Logger_Lab
{
    internal class Program
    {
        private static ILogger _logger;
        static void InitializeLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File(
                    "game_log.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

            _logger.Information("Логгер инициализирован. Программа запущена");
        }
        static bool CheckInput(string userInput)
        {
            _logger.Information("Проверка ввода пользователя: {UserInput}", userInput);
            string[] input = userInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int n, k;
            if (input.Length == 2 && int.TryParse(input[0], out n) && int.TryParse(input[1], out k))
            {
                if (n > 0 && k > 0) {_logger.Information("Ввод корректен: N={N}, k={k}", n, k); return true; }
                else {_logger.Warning("Числа должны быть положительными: N={N}, k={k}", n, k); Console.WriteLine("Числа должны быть положительными! \n--------------------"); return false; }
            }
            else { _logger.Warning("Неправильный формат ввода: {UserInput}", userInput); Console.WriteLine("Неправильный ввод \n--------------------"); return false; }
        }

        static void Game(int n, int k)
        {
            Random random = new Random();
            int wantedNumber = random.Next(1, n);
            _logger.Information("Начало игры: N={N}, k={k}, загаданное число={WantedNumber}", n, k, wantedNumber);
            Console.WriteLine("\n-----Начало-----");
            for (int i = k; i > 0;)
            {
                Console.WriteLine($"Попытка {i}");
                Console.WriteLine($"Введите число от 1 до {n}:");

                string input = Console.ReadLine();
                _logger.Information("Попытка {AttemptNumber}, введено: {Input}", i, input);

                if (int.TryParse(input, out int inputNumber))
                {
                    if (inputNumber == wantedNumber)
                    {
                        _logger.Information("Пользователь угадал число! Загаданное число: {WantedNumber}", wantedNumber);
                        Console.WriteLine("Вы угадали!");
                        break;
                    }
                    else if (inputNumber > 0 && inputNumber <= n)
                    {
                        i--;

                        if (inputNumber > wantedNumber)
                        {
                            _logger.Information("Пользователь ввел {InputNumber} > загаданного {WantedNumber}", inputNumber, wantedNumber);
                            Console.WriteLine("Введенное число больше загаданного");
                        }
                        else if (inputNumber < wantedNumber)
                        {
                            _logger.Information("Пользователь ввел {InputNumber} < загаданного {WantedNumber}", inputNumber, wantedNumber);
                            Console.WriteLine("Введенное число меньше загаданного");
                        }

                        if (i == 0)
                        {
                            _logger.Information("Попытки закончились. Загаданное число: {WantedNumber}", wantedNumber);
                            Console.WriteLine($"Попытки закончились! Загаданное число было: {wantedNumber}");
                        }
                    }
                    else
                    {
                        _logger.Warning("Пользователь ввел число вне диапазона: {InputNumber}", inputNumber);
                        Console.WriteLine($"Число должно быть в диапазоне от 1 до {n}");
                    }
                }
                else
                {
                    _logger.Warning("Некорректный ввод числа: {Input}", input);
                    Console.WriteLine("Неправильный ввод. Введите целое число.");
                }
            }
            _logger.Information("Игра завершена");
            _logger.Information("-------------------------------");
            Console.WriteLine("-----Конец------\n");
        }
        static void Main(string[] args)
        {
            InitializeLogger();
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            while (true)
            {
                Console.Write("Введите натуральные числа N и k через пробел \n" +
                              "Для завершения введите ВЫХОД: ");
                string userInput = Console.ReadLine();

                if (userInput.ToLower() == "выход")
                {
                    Console.WriteLine("Завершение работы...");
                    break;
                }    
                if (CheckInput(userInput))
                {
                    string[] numbers = userInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int n = int.Parse(numbers[0]);
                    int k = int.Parse(numbers[1]);

                    Game(n, k);

                }
            }
        }
    }
}
