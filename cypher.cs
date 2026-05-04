namespace Caesar_Cipher_lab;

class Program
{
    private const string EngAlphabet = "abcdefghijklmnopqrstuvwxyz";
    private const string RusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    static void Main()
    {
        Console.InputEncoding = System.Text.Encoding.UTF8;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        while (true)
        {
            ShowMainMenu();
            var choice = GetMenuChoice();
            
            switch (choice)
            {
                case 1:
                    ChooseCipherOperation("cipher");
                    break;
                case 2:
                    ChooseCipherOperation("decipher");
                    break;
                case 0:
                    Console.WriteLine("Завершение работы...");
                    return;
                default:
                    PrintError("Неверный выбор меню");
                    break;
            }
            
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("=== ШИФР ЦЕЗАРЯ ===\n" +
                          "1. Шифрование\n" +
                          "2. Дешифрование\n" +
                          "0. Выход");
        Console.Write("Выберите действие: ");
    }

    private static int GetMenuChoice()
    {
        return int.TryParse(Console.ReadLine(), out int choice) ? choice : -1;
    }

    private static void ChooseCipherOperation(string operation)
    {
        var displayedOperation = (operation == "cipher" ? "ШИФРОВАНИЕ" : "ДЕШИФРОВАНИЕ");
        Console.WriteLine($"\n=== {displayedOperation} ===");
        
        var language = GetLanguage();
        var message = GetMessage();
        var shift = GetShift(language);
        
        var result = operation == "cipher" ? 
            CaesarCipher(message, shift.Value, language) : 
            CaesarCipher(message, -shift.Value, language);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nРезультат: {result}");
        Console.ResetColor();
    }

    private static string GetLanguage()
    {
        while (true)
        {
            Console.Write("Выберите язык (en/ru): ");
            var language = Console.ReadLine()?.ToLower().Trim();
            
            if (language == "en" || language == "ru")
                return language;
            else PrintError("Неверный язык. Используйте: en - английский, ru - русский");
        }
    }

    private static string GetMessage()
    {
        Console.Write("Введите сообщение: ");
        return Console.ReadLine() ?? "";
    }

    private static int? GetShift(string language)
    {
        int maxShift = language == "en" ? EngAlphabet.Length - 1 : RusAlphabet.Length - 1;
        
        while (true)
        {
            Console.Write($"Введите шаг сдвига (1-{maxShift}): ");
            var input = Console.ReadLine();
            
            if (input?.ToLower() == "выход")
                return null;
            
            if (int.TryParse(input, out int shift) && shift > 0 && shift <= maxShift)
                return shift;
            
            PrintError($"Неверный шаг сдвига! Введите число от 1 до {maxShift}");
        }
    }

    private static string CaesarCipher(string message, int shift, string language)
    {
        var result = new System.Text.StringBuilder();
        string alphabet = language == "en" ? EngAlphabet : RusAlphabet;
        int alphabetLength = alphabet.Length;
        
        if (shift < 0) shift += alphabetLength;

        foreach (char c in message)
        {
            if (char.IsLetter(c))
            {
                char charInAlphabet = char.ToLower(c);
                int indexInAplhabet = alphabet.IndexOf(charInAlphabet);
                
                if (indexInAplhabet >= 0)
                {
                    int newIndex = (indexInAplhabet + shift) % alphabetLength;
                    char newChar = alphabet[newIndex];
                    
                    result.Append(char.IsUpper(c) ? char.ToUpper(newChar) : newChar);
                }
                else
                {
                    result.Append(c);
                }
            }
            else
            {
                result.Append(c);
            }
        }
        
        return result.ToString();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Ошибка: {message}");
        Console.ResetColor();
    }
}