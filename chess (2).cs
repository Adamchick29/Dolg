using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LessonsC__2
{
    internal class Program
    {
        public static bool IsCorrectInput(string[] userInput)
        {
            if (userInput.Length == 3)
            {
                string cell1 = userInput[0].ToUpper();
                string cell2 = userInput[1].ToUpper();
                if (cell1.Length == 2 && cell2.Length == 2 && cell1 != cell2)
                {
                    string figure = userInput[2].ToUpper();
                    string letters = "ABCDEFGH";
                    string nums = "12345678";
                    string[] figures = { "ФЕРЗЬ", "ЛАДЬЯ", "КОНЬ", "СЛОН" };
                    return (letters.Contains(cell1[0]) && letters.Contains(cell2[0]) &&
                            nums.Contains(cell1[1]) && nums.Contains(cell2[1]) &&
                            figures.Contains(figure));
                }
                else { return false; }
            }

            else { return false;}
        }

        public static string CheckColor(string cell1, string cell2)
        {
            bool cell1parity = (cell1[0] - 'A' + cell1[1] - '1') % 2 == 0;
            bool cell2parity = (cell2[0] - 'A' + cell2[1] - '1') % 2 == 0;
            if (cell1parity == cell2parity)
            {
                return "Введенные клетки одного цвета";
            }
            else
            {
                return "Введенные клетки разных цветов";
            }
        }

        public static bool CheckThreat(string cell1, string cell2, string figure)
        {
            var dx = Math.Abs(cell2[0] - cell1[0]);
            var dy = Math.Abs(cell2[1] - cell1[1]);
            switch (figure)
            {
                case "ФЕРЗЬ":
                    return ((dx == dy) || (dx == 0 && dy != 0) || (dx != 0 && dy == 0));
                case "ЛАДЬЯ":
                    return ((dx == 0 && dy != 0) || (dx != 0 && dy == 0));
                case "СЛОН":
                    return (dx == dy);
                case "КОНЬ":
                    return ((dx == 2 && dy == 1) || (dx == 1 && dy == 2));
                default:
                    return false;
            }
        }

        public static List<string> PotentialMoves(string cell1, string cell2, string figure)
        {
            List <string> potentialMoves = new List<string>();
            foreach (char i in "ABCDEFGH")
            {
                for (int j = 1; j <= 8; j++)
                {
                    string potencialMove = i.ToString() + j;
                    if (CheckThreat(cell1, potencialMove, figure) && CheckThreat(potencialMove, cell2, figure))
                    {
                        potentialMoves.Add(potencialMove);
                    }
                }
            }
            return potentialMoves;
        }

        public static void TestMove(string cell1, string cell2, string figure)
        {
            Console.WriteLine(CheckColor(cell1, cell2));
            string outputFigure = (figure[0].ToString().ToUpper() + figure.Substring(1).ToLower());
            if (CheckThreat(cell1, cell2, figure))
            {
                Console.WriteLine("{0} в клетке {1} угрожает фигуре в клетке {2}.\n", outputFigure, cell1, cell2);
                ShowBoard(cell1, cell2, figure);
            }
            else
            {
                Console.WriteLine("{0} в клетке {1} не угрожает фигуре в клетке {2} \n", outputFigure, cell1, cell2);
                ShowBoard(cell1, cell2, figure);
            }
        }

        public static string GetFigureSymbol(string figure)
        {
            switch (figure)
            {
                case "ФЕРЗЬ": { return "♕"; }
                case "ЛАДЬЯ": { return "♖"; }
                case "СЛОН":  { return "♗"; }
                case "КОНЬ":  { return "♘"; }
                default: { return "F"; }
            }
        }

        public static void ShowBoard(string cell1, string cell2, string figure)
        {
            List<string> potentialMoves = PotentialMoves(cell1, cell2, figure);

            Console.WriteLine("Вид доски: \n");
            Console.WriteLine("  A B C D E F G H");
            for (int row = 8; row >= 1; row--)
            {
                Console.Write(row + " ");
                for (char column = 'A'; column <= 'H'; column++)
                {
                    string currentCell = column.ToString() + row;
                    if (currentCell == cell1) { Console.Write(GetFigureSymbol(figure) + " "); }
                    else if (currentCell == cell2) { Console.Write("♚ "); }
                    else if (potentialMoves.Count != 0 && potentialMoves.Contains(currentCell) && !CheckThreat(cell1, cell2, figure)) { Console.Write("x "); }
                    else { Console.Write(((column - 'A' + row) % 2 == 0 ? "□ " : "■ ")); }
                }
                Console.WriteLine(row);
            }
            Console.WriteLine("  A B C D E F G H \n");
        }

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("====================== Шахматы ======================");
                Console.WriteLine("Введите адреса клеток и атакующую фигуру\n" +
                                  "Формат ввода: <Клетка 1> <Клетка2> <Фигура>\n" +
                                  "Фигуры: Ферзь, Ладья, Слон, Конь\n" +
                                  "Клетки: <ABCDEFGH> - <12345678>");

                Console.WriteLine(" --- Для завершения работы программы введите СТОП ---");
                Console.Write("Ввод: ");
                string[] userInput = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("-----------------------------------------------------");
                if (IsCorrectInput(userInput))
                {
                    string cell1 = userInput[0].ToUpper();
                    string cell2 = userInput[1].ToUpper();
                    string figure = userInput[2].ToUpper();
                    TestMove(cell1, cell2, figure);
                    Console.WriteLine("Если есть возможные ходы для создания угрозы, они отмечены x \n");
                }
                else if (userInput.Length == 1 && userInput[0] == "СТОП")
                {
                    Console.WriteLine("Завершение работы");
                    break;
                }
                else
                {
                    Console.WriteLine("Неправльно введены данные, попробуйте ещё раз. \n");
                }

            }
        }
    }
}
