using System.Globalization;
using System.Text;
using Serilog;
using Serilog.Sinks.File;

namespace TestingApp_Lab;

// Класс студента (пользователя), хранит данные о тестировании
public class Student
{
    public static int CorrectAnswers;
    public static int Answers;

    // Оценка качества выполнения теста в процентах
    public static int GetPersentage()
    {
        return CorrectAnswers * 100 / Answers;
    }
}

class Program
{
    // Переменные, доступные всему классу Program
    
    // Данные о вопросах из файла
    private static string[]? _questionsData;

    // Номер строки (Инетервал строк между вопросами динамичный, т.к количество вариантов ответа может быть разным)
    private static int _stringNumber = 1;
    
    // Объект логгера
    private static ILogger _logger;
    
    // Иниализация логгера
    static void InitializeLogger()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.File(
                "test_log.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _logger.Information("===========================");
        _logger.Information("Логгер инициализирован. Программа запущена");
    }
    
    static void Main()
    {
        InitializeLogger();
        Console.Title = "Тестирование";
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        
        // Считывание данных из файла
        LoadData("test.txt");
        
        // Получение количества вопросов (первая строка в файле)
        var questionsAmount = int.Parse(_questionsData[0]);
        
        // Пока студент не ответит на все вопросы, тест не кончится
        while (Student.Answers < questionsAmount)
        {
            AskQuestion(_stringNumber);
        }
        
        // Получение результатов
        GetResult();
    }
    
    // Считывание данных из файла
    private static void LoadData(string fileName)
    {
        try
        {
            // Считывание данных из файла в массив строк
            _questionsData = File.ReadAllLines(fileName, Encoding.UTF8);
            _logger.Information("Данные о тесте успешно загружены");
        }
        
        catch (Exception exception)
        {
            // Вывод ошибки
            Console.WriteLine($"Ошибка: {exception.Message}");
            _logger.Error($"Ошибка при загрузке данных: {exception.Message}");
        }
    }

    // Процесс прохождения одного вопроса
    private static void AskQuestion(int stringNumber)
    {
        // Отображение вопроса и получение основных данных
        var data = ShowVariants(stringNumber);
        // Правильный ответ
        var correctAnswer = data[0];
        // Количество вариантов ответа
        var variantsAmount = data[1];
        // Получение ответа
        GetAnswer(correctAnswer, variantsAmount);
    }

    // Отображение вопроса и вариантов ответа и получение основных данных
    private static int[] ShowVariants(int stringNumber)
    {
        _logger.Information("Вывод вопроса и вариантов ответа");
        Console.Clear();
        var question = _questionsData[stringNumber];
        var variantsAmount = int.Parse(_questionsData[stringNumber + 1]);
        var correctAnswer = int.Parse(_questionsData[stringNumber + 2]);
        Console.WriteLine(question);
        for (var i =  0; i < variantsAmount; i++)
            Console.WriteLine($"  {(i + 1)}) {_questionsData[stringNumber + 3 + i]}");
        return [correctAnswer, variantsAmount];
    }

    private static void GetAnswer(int correctAnswer, int variantsAmount)
    {
        // Ввод ответа студентом(пользователем)
        Console.Write("Выберите вариант ответа: ");
        string input = Console.ReadLine();
        _logger.Information($"Ввод ответа на вопрос. Ввод: {input}");
        Console.WriteLine();
        
        // Если такой вариант ответа есть, обработка идет дальше
        if (int.TryParse(input, out var answer) && answer >= 1 && answer <= variantsAmount)
        {
            // Общее количество пройденных вопросов увеличивается 
            Student.Answers++;
            
            // Сдвиг строки дальше (переход к следующим вопросам)
            _stringNumber += 3 + variantsAmount;
            
            // Правильный ответ
            if (answer == correctAnswer)
            {
                _logger.Information($"Введен правильный ответ {answer}");
                ConsoleWriteColoredLine(ConsoleColor.Green, "Вы ответили правильно!");
                Student.CorrectAnswers++;
            }
            
            
            // Неправильный ответ
            else
            {
                _logger.Information($"Введен неправильный ответ {answer}, правильный ответ: {correctAnswer}");
                ConsoleWriteColoredLine(ConsoleColor.Red, $"Вы ответили неправильно, правильный ответ: {correctAnswer}");
            }            
            WaitForPress();
        }
        // Если неверный формат, вывод ошибки
        else
        {
            ConsoleWriteColoredLine(ConsoleColor.Red, "Неверный ввод!");
            _logger.Error("Неверный формат ввода ответа.");
            WaitForPress();
            ShowVariants(_stringNumber);
        }
    }

    // Получение результатов
    private static void GetResult()
    {
        Console.Clear();
        var result = Student.GetPersentage();
        Console.WriteLine($"Результат тестирования: {Student.CorrectAnswers} / {Student.Answers}\n" +
                          $"Оценка: {result}%");
        _logger.Information($"Результат {result}%");
        WaitForPress();
    }
    
    // Вывод цветной строки в консоль
    private static void ConsoleWriteColoredLine(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    // Ожидание нажатия
    static void WaitForPress()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}