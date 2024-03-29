﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SudokuSolver
{
    class SudokuSolver
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                //Any sudoku passed as argument
                foreach (string sudoku in args)
                {
                    SudokuSolver s = new SudokuSolver { Board = new StringBuilder(sudoku) };
                    s.SolveBoard();
                    s.PrintBoard();
                    Console.WriteLine(); 
                }
            } else
            {
                //Project euler problem 96, parsing sudoku.txt before
                string sudokusFile = System.IO.File.ReadAllText("sudokus.txt");
                string[] sudokus = Regex.Split(sudokusFile, @"(Grid\s\d{2})\n((?:\d{9}\n?){9})");
                double output = 0;
                var timer = System.Diagnostics.Stopwatch.StartNew();
                
                for (int number = 2; number < sudokus.Length; number += 3)
                {
                    string sudoku = sudokus[number].Replace("\n", "");
                    SudokuSolver s = new SudokuSolver { Board = new StringBuilder(sudoku) };
                    s.SolveBoard();
                    output += int.Parse(s.Board.ToString().Substring(0, 3));
                }
                timer.Stop();
                Console.WriteLine("Solved {0} sudokus in {1} ms and the " +
                                  "sum of the 3-digit numbers found at " +
                                  "the top left is {2}.",
                                   sudokus.Length/3,
                                   timer.ElapsedMilliseconds, 
                                   output);
            }
            Console.ReadLine();
        }

        public StringBuilder Board;

        public void SolveBoard()
        {
            if (boardIsSolved())
                return;

            StringBuilder solvedBoard = new StringBuilder();
            bool BoardIsSolvable = FindNumberAtPosition(Board, 0, out solvedBoard);
            if (BoardIsSolvable)
                Board = solvedBoard;
            else
                Console.WriteLine("This board is not possible");
        }

        private bool FindNumberAtPosition(StringBuilder initialBoard, int currentPosition, out StringBuilder nextBoard)
        {
            //Recursive approch
            nextBoard = new StringBuilder();
            if (currentPosition >= 81)
            {
                nextBoard = initialBoard;
                return true;
            }

            if (initialBoard[currentPosition] != '0')
                return FindNumberAtPosition(initialBoard, currentPosition + 1, out nextBoard);
                
            int x = currentPosition % 9;
            int y = (currentPosition - x) / 9;
            List<char> possibilities = Possibilities(initialBoard, x, y);
            if (possibilities.Count == 0)
                return false;
            else if (possibilities.Count == 1)
            {
                initialBoard[currentPosition] = possibilities.First();
                return FindNumberAtPosition(initialBoard, currentPosition + 1, out nextBoard);
            }
            else
            {
                foreach (char possibility in possibilities)
                {
                    StringBuilder newBoard = new StringBuilder(initialBoard.ToString());
                    newBoard[currentPosition] = possibility;

                    bool boardSolved = FindNumberAtPosition(newBoard, currentPosition + 1, out nextBoard);
                    if (!boardSolved)
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }

        public void PrintBoard()
        {
            Console.Write(Regex.Replace(Board.ToString(), ".{9}", "$0\n"));
        }

        private bool boardIsSolved()
        {
            return !Board.ToString().Contains("0");
        }

        private List<char> Possibilities(StringBuilder board, int x, int y)
        {
            List<char> possibilities = new List<char> {'1', '2', '3', '4', '5', '6', '7', '8', '9' };
            return possibilities
                .Except(NumbersInRow(board, y))
                .Except(NumbersInColumn(board, x))
                .Except(NumbersInSquare(board, x / 3, y/3))
                .ToList();
        }

        private List<char> NumbersInRow(StringBuilder board, int row)
        {
            List<char> numbersInRow = new List<char>();
            for (int x = 0; x < 9; x++)
            {
                char n = board[x + row * 9];
                if (n != '0')
                    numbersInRow.Add(n);
            }

            return numbersInRow;
        }

        private List<char> NumbersInColumn(StringBuilder board, int column)
        {
            List<char> NumbersInColumn = new List<char>();
            for (int y = 0; y < 9; y++)
            {
                char n = board[column + y * 9];
                if (n != '0')
                    NumbersInColumn.Add(n);
            }

            return NumbersInColumn;
        }

        private List<char> NumbersInSquare(StringBuilder board, int posX, int posY)
        {
            List<char> numbersInSquare = new List<char>();
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    char n = board[posX * 3 + x + (posY * 3 + y) * 9];
                    if (n != '0')
                        numbersInSquare.Add(n);
                }

            return numbersInSquare;
        }
    }
}
