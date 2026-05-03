using System;
using System.Text;
using Serilog;
using Serilog.Sinks.File;


namespace Lot_Lab
{
    class Program
    {
        private static ILogger _logger;
        static void InitializeLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.File(
                    "lot_log.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _logger.Information("-------------------------------");
            _logger.Information("Логгер инициализирован. Программа запущена");
        }
        
        static int GetBarrelsNumber()
        {
            _logger.Information("Запрос числа бочонков у пользователя.");
            int barrelsNumber;
            while (true)
            {
                Console.Write("Пожалуйста, введите число от 1 до 100: ");
                var input = Console.ReadLine();
                _logger.Information("Ввод пользователя: {Input}", input);
                
                if (int.TryParse(input, out int result) && result >= 1 && result <= 100)
                {
                    barrelsNumber = result;
                    _logger.Information("Ввод корректен. Количество бочонков: {BarrelsNumber}", barrelsNumber);
                    break;
                }
                else
                {
                    _logger.Warning("Некорректный ввод. Введено: {Input}", input);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка ввода! Пожалуйста, введите число от 1 до 100");
                    Console.ResetColor();
                }
            }

            return barrelsNumber;
        }
        
        static List<int> CreateBarrelsList(int barrelsNumber)
        {
            _logger.Information("Создание списка бочонков (от 1 до {BarrelsNumber})", barrelsNumber);
            List<int> barrelsList = new List<int>();
            for (int i = 0; i < barrelsNumber; i++)
            {
                barrelsList.Add(i + 1);
            }
            _logger.Information("Список создан. Размер: {Count}", barrelsList.Count);
            return barrelsList;
        }
        
        static int GetRandomBarrel(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue);
        }
        
        static void Lot(int barrelsNumber)
        {
            List<int> barrelsList = CreateBarrelsList(barrelsNumber);
            _logger.Information("Начало лотереи. Всего бочонков: {InitialCount}", barrelsList.Count);

            Console.WriteLine("\n----------------------------------------------------------" +
                              "\nНажмите Enter, чтобы достать случайный бочонок из мешка" +
                              "\nНажмите Esc для выхода из программы" +
                              "\n----------------------------------------------------------");
            
            while (barrelsList.Count > 0)
            {
                var key = Console.ReadKey(true); // 'true' чтобы не отображать нажатую клавишу
                _logger.Debug("Нажата клавиша: {Key}", key.Key);

                if (key.Key == ConsoleKey.Enter)
                {
                    var randomIndex = GetRandomBarrel(0, barrelsList.Count);
                    var pulledBarrelValue = barrelsList[randomIndex];
                    barrelsList.RemoveAt(randomIndex);
                    _logger.Information("Вытянут бочонок: {Value}. Осталось бочонков: {Remaining}", 
                                        pulledBarrelValue, barrelsList.Count);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nНомер вытянутого боченка: {pulledBarrelValue}");
                    Console.WriteLine($"Вытянуто {barrelsNumber - barrelsList.Count} бочонков");
                    Console.WriteLine($"В мешке осталось {barrelsList.Count} бочонков");
                    Console.ResetColor();
                }

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Выход из программы...");
                    _logger.Information("Пользователь запросил выход (клавиша ESC). Осталось бочонков: {Remaining}", barrelsList.Count);
                    break;
                }
            }

            // Console.WriteLine("\nВ мешке больше нет бочонков!");
            _logger.Information("Лотерея завершена");
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Завершение программы, нажмите любую клавишу...");
            Console.ResetColor();

            Console.ReadKey();
            _logger.Information("Программа завершена.");
        }
        
        static void Main()
        {
            InitializeLogger();
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var barrelsNumber = GetBarrelsNumber();
            Lot(barrelsNumber);
        }
    }
}