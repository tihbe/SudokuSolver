using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SudokuSolver
{
    class SudokuSolver
    {
        public StringBuilder Board;
        private const int MAX_ITERATION_OVER_BOARD = 5000;
        private int[,] arrayOfPossibilities = new int[9 * 9, 9]; //0: possible; 1: not possible
        static int numberOfFailed = 0;

        static void Main(string[] args)
        {
            string sudokus = System.IO.File.ReadAllText("sudokus.txt");
            string[] suds = Regex.Split(sudokus, @"(Grid\s\d{2})\n((?:\d{9}\n?){9})");
            for (int number = 2; number < suds.Length; number += 3)
            {
                string sudoku = suds[number].Replace("\n", "");
                SudokuSolver s = new SudokuSolver { Board = new StringBuilder(sudoku) };
                s.SolveBoard();
                s.PrintBoard();
            }
            Console.WriteLine(numberOfFailed);
            Console.ReadLine();
        }

        public void SolveBoard()
        {
            iterateOverBoard();
            if (boardIsSolved())
                return;

            string initialBoard = Board.ToString();
            int[,] initialArrayOfPossibilities = new int[9*9,9];
            for (int i = 0; i < 9 * 9; i++)
                for (int j = 0; j < 9; j++)
                    initialArrayOfPossibilities[i, j] = arrayOfPossibilities[i, j];

            bool boardUnsolved = true;

            for (int x = 0; x < 9 && boardUnsolved; x++)
            {
                for (int y = 0; y < 9 && boardUnsolved; y++)
                {
                    int currentPositionInBoard = x + 9 * y;
                    for (int n = 0; n < 9 && boardUnsolved; n++)
                    {
                        if (initialArrayOfPossibilities[currentPositionInBoard, n] == 0)
                        {
                            Board[currentPositionInBoard] = Convert.ToChar(n + 49);
                            iterateOverBoard();
                            if (boardIsSolved())
                            {
                                boardUnsolved = false;
                            } else
                            {
                                Board = new StringBuilder(initialBoard);
                                initialArrayOfPossibilities[currentPositionInBoard, n] = 1;
                                for (int i = 0; i < 9 * 9; i++)
                                    for (int j = 0; j < 9; j++)
                                        arrayOfPossibilities[i, j] = initialArrayOfPossibilities[i, j];
                            }
                        }
                    }
                }
            }

            if (boardUnsolved)
            {
                Console.Write("Failed to solve board\n");
                numberOfFailed++;
            }
                
            return;
        }

        private void iterateOverBoard()
        {
            string _initialBoard;
            for (int interationOverBoard = 0; interationOverBoard < MAX_ITERATION_OVER_BOARD; interationOverBoard++)
            {
                _initialBoard = Board.ToString();
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        int currentPositionInBoard = x + 9 * y;
                        if (Board[currentPositionInBoard] == '0')
                        {
                            char numberFound = '\0';
                            for (int numberToTry = 0; numberToTry < 9; numberToTry++)
                            {
                                if (arrayOfPossibilities[currentPositionInBoard, numberToTry] == 1)
                                    continue;
                                char n = Convert.ToChar(numberToTry + 49);
                                if (!isInRow(n, y) && !isInColumn(n, x) && !isInSquare(n, (int)(x / 3), (int)(y / 3)))
                                {
                                    if (numberFound == '\0')
                                        numberFound = n;
                                    else
                                    {
                                        numberFound = '\0';
                                        break;
                                    }
                                }
                                else
                                    arrayOfPossibilities[currentPositionInBoard, numberToTry] = 1;
                            }
                            if (numberFound != '\0')
                            {
                                Board[currentPositionInBoard] = numberFound;
                            }
                        }
                    }
                }
                //If the board is not "moving" anymore
                if (_initialBoard == Board.ToString())
                    break;
            }
        }

        public void PrintBoard()
        {
            Console.Write(Regex.Replace(Board.ToString(), ".{9}", "$0\n"));
        }

        private bool boardIsSolved()
        {
            return !Board.ToString().Contains("0");
        }

        private bool isInRow(char num, int row)
        {
            for (int x = 0; x<9; x++)
                if (Board[x + row * 9] == num)
                    return true;
            return false;
        }

        private bool isInColumn(char num, int column)
        {
            for (int y = 0; y < 9; y++)
                if (Board[column + y * 9] == num)
                    return true;
            return false;
        }
        
        private bool isInSquare(char num, int positionSquareX, int positionSquareY)
        {
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (Board[positionSquareX*3 + x + (positionSquareY*3 + y) * 9] == num)
                        return true;
            return false;
        }
    }
}
