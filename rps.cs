using System.Globalization;
using System.Text;
using Serilog;
using Serilog.Sinks.File;


namespace RockPaperScissorsLab;

// Класс игрового бота
public class GameBot
{
//     Бот хранит свой счет
    public static int Score;
//     Бот генерует случайный ход
    public static string GetStep()
    {
        var rand = new Random();
        var step = rand.Next(3);
        switch (step)
        {
            case 0:
                return "Камень";
            case 1:
                return "Ножницы";
            case 2:
                return "Бумага";
        }
        return "";
    }
}

// Класс игрока
public class GameUser
{
//     Игрок хранит свой счет
    public static int Score;
//     Игрок вводит ход или выходит из программы
    public static string GetStep()
    {
        Console.WriteLine("Выберите, как хотите сходить:\n" +
                          "1.Камень\n" +
                          "2.Ножницы\n" +
                          "3.Бумага");
        Console.Write("Мой ход: ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out int max) && max >= 1 && max <= 3)
        {
            switch (input)
            {
                case "1":
                    return "Камень";
                case "2":
                    return "Ножницы";
                case "3":
                    return "Бумага";
                default:
                    return "Ошибка";
            }
        }
        if (input.ToLower() == "выход")
        {
            return "Выход";
        }
        return "Ошибка";
    }
}

// Класс программы
class Program
{
    // Максимальное число побед
    public static int MaxWins;
    
    // Объект логгера
    public static ILogger _logger;
    
    // Иниализация логгера
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
    
    // Главная функция
    static void Main()
    {
        InitializeLogger();
        // Установка кодировок
        Console.Title = "Камень - ножницы - бумага";
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        // Запуск партии
        _logger.Information("Запуск партии");
        Game();
    }
    
    // Получение максимального числа побед
    static int GetMaxWins()
    {
        // Бесконечный цикл для обработки ошибок
        while (true)
        {
            Console.WriteLine("Игра: Камень - ножницы - бумага");
            Console.Write("До скольки побед будет идти игра? : ");
            var input = Console.ReadLine();
            _logger.Information($"Запрос ввести максимальное число побед. Пользователь ввел: {input}");
            if (int.TryParse(input, out int max) && max > 0)
            {
                Console.WriteLine($"Игра будет идти до {max} побед");
                WaitForPress();
                _logger.Information($"Максимальное число побед: {max}");
                return max;
            }
            // Обработка ошибок
            else
            {
                _logger.Error($"Ошибка ввода, ввод: {input}");
                Console.WriteLine("Ошибка ввода, введите положительное число!");
            }

            WaitForPress();
            Console.Clear();
        }
    }

    // Игра
    static void Game()
    {
        // Получение максимального числа побед
        MaxWins = GetMaxWins();
        // Бесконечный цикл до того момента, по игра не закончится или игрок не выйдет сам
        while (true)
        {   
            // Выход из игры, если функция возвращает -1
            if (PlayRound() == -1) break;
            // Конец игры
            if (IsEnd() == -1)
            {
                WaitForPress();
                _logger.Information("Конец партии");
                _logger.Information("----------------------------------");
                break;
            }
        }
    }
    
    // Отырыш одного раунда
    static int PlayRound()
    {
        Console.Clear();
        // Отображение информации о текущей партии
        ShowHeader();
        
        // Получение хода пользователя
        var userStep = GameUser.GetStep();
        _logger.Information($"Ход пользователя: {userStep}");
        // Обработка ошибок
        if (userStep == "Ошибка")
        {
            _logger.Error("Ошибка ввода!");
            Console.WriteLine("Ошибка ввода!");
            WaitForPress();
            return 0;
        }
        
        // Выход из программы
        if (userStep == "Выход")
        {
            _logger.Warning("Выход из программы");
            Console.Clear();
            ShowScore();
            Console.WriteLine("Выход из программы...");
            WaitForPress();
            return -1;
        }
        
        // Получение хода бота
        var botStep = GameBot.GetStep();
        _logger.Information($"Ход бота: {botStep}");
        
        // Сравнение ходов
        Console.WriteLine("---------------------------------");
        Console.WriteLine($"Ваш ход - Ход бота\n" +
                          $"{userStep,7} - {botStep}");
        
        // Получение победителя
        var result = GetRoundResult(userStep, botStep);
        _logger.Information($"Результат раунда для игрока: {result}");
        // Отображение результата
        ShowRoundResult(result);
        WaitForPress();
        return 1;
    }

    // Информация о текущей партии 
    static void ShowHeader()
    {
        Console.WriteLine("=============================");
        Console.WriteLine("  Камень - ножницы - бумага  ");
        Console.WriteLine($"Игра идёт до {MaxWins} побед");
        ShowScore();
        Console.WriteLine("--Для выхода введите  ВЫХОД--");
        Console.WriteLine("=============================");
    }

    // Отображение текущего счета счета
    static void ShowScore()
    {
        Console.WriteLine("-----------------------------");
        Console.WriteLine("          Игрок - Бот        ");
        Console.WriteLine($"Счет:       {GameUser.Score,3} - {GameBot.Score,-3}");
        Console.WriteLine("-----------------------------");
        _logger.Information($"Текущий счёт: {GameUser.Score} - {GameBot.Score} (Игрок - Бот)");
    }
    
    // Результат одного раунда
    static void ShowRoundResult(string result)
    {
        if (result == "Победа")
        {
            // Увеличение счета в пользу пользователя
            GameUser.Score++;
            WriteColoredLine(ConsoleColor.Green, $"Результат: {result}");
        }

        if (result == "Поражение")
        {
            // Увеличение счета в пользу бота
            GameBot.Score++;
            WriteColoredLine(ConsoleColor.Red, $"Результат: {result}");
        }

        if (result == "Ничья") WriteColoredLine(ConsoleColor.Yellow, $"Результат: {result}");
    }

    // Победитель в раунде
    static string GetRoundResult(string step1, string step2)
    {
        if (step1 == step2) return "Ничья";
        if (step1 == "Камень")
        {
            if (step2 == "Ножницы") return "Победа";
            if (step2 == "Бумага") return "Поражение";
        }
        if (step1 == "Ножницы")
        {
            if (step2 == "Камень") return "Поражение";
            if (step2 == "Бумага") return "Победа";
        }
        if (step1 == "Бумага")
        {
            if (step2 == "Камень") return "Победа";
            if (step2 == "Ножницы") return "Поражение";
        }

        return "";
    }

    // Проверка, не достигнуто ли максимальное число побед
    static int IsEnd()
    {
        if (GameUser.Score == MaxWins)
        {
            Console.Clear();
            ShowScore();
            WriteColoredLine(ConsoleColor.Green,"Игра окончена, вы победели!");
            _logger.Information("Партия кончилась победой игрока");
            return -1;
        }
        if (GameBot.Score == MaxWins)
        {
            Console.Clear();
            ShowScore();
            WriteColoredLine(ConsoleColor.Red,"Игра окончена, победил робот.");
            _logger.Information("Партия кончилась победой бота");
            return -1;
        }
        return 1;
    }
    
    // Вывод цветной строки в консоль
    private static void WriteColoredLine(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    // Ожидание нажатия клавиши
    private static void WaitForPress()
    {
        WriteColoredLine(ConsoleColor.Cyan, "Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}