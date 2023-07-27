using Klondike.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;

namespace Klondike {
    public class Program {
        public static void MainUriginal(string[] args) {
            if (args != null && args.Length > 0 && ((args.Length - 1) & 1) == 1) {
                Console.WriteLine($"Invalid argument count.");
                args = null;
            }

            if (args == null || args.Length == 0) {
                Console.WriteLine(
@$"Minimal Klondike
Klondike.exe [Options] [CardSet]

DrawCount (Default=1)
-D #

Initial Moves
-M ""Moves To Play Initially""

Max States (Default=50,000,000) (About 1GB RAM Per 22 Million)
-S #

Solve Seed 123 from GreenFelt:
Klondike.exe 123

Solve Given CardSet With Initial Moves:
Klondike.exe -D 1 -M ""HE KE @@@@AD GD LJ @@AH @@AJ GJ @@@@AG @AB"" 081054022072134033082024052064053012061013042093084124092122062031083121113023043074051114091014103044131063041102101133011111071073034123104112021132032094");
                return;
            }

            string cardSet = args[^1].Replace("\"", "");
            int drawCount = 1;
            string moveSet = null;
            int maxStates = 50_000_000;

            for (int i = 0; i < args.Length - 1; i++) {
                if (args[i] == "-D" && i + 1 < args.Length) {
                    if (!int.TryParse(args[i + 1], out drawCount)) {
                        Console.WriteLine($"Invalid DrawCount argument {args[i + 1]}. Defaulting to 1.");
                        drawCount = 1;
                    }
                    i++;
                } else if (args[i] == "-S" && i + 1 < args.Length) {
                    if (!int.TryParse(args[i + 1], NumberStyles.AllowThousands, null, out maxStates)) {
                        Console.WriteLine($"Invalid MaxStates argument {args[i + 1]}. Defaulting to 50,000,000.");
                        maxStates = 50_000_000;
                    }
                    i++;
                } else if (args[i] == "-M" && i + 1 < args.Length) {
                    moveSet = args[i + 1];
                    i++;
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (cardSet.Length < 11) {
                uint.TryParse(cardSet, out uint seed);
                SolveGame(seed, drawCount, moveSet, maxStates);
            } else {
                SolveGame(cardSet, drawCount, moveSet, maxStates);
            }

            sw.Stop();
            Console.WriteLine($"Done {sw.Elapsed}");
        }


        public static void Main(string[] args)
        {
           

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for(uint seed = 19; seed <= 19; seed++) 
            {
                SolveGame(seed, 1, null, 20_000_000);
                SolveGame(seed, 3, null, 20_000_000);
            }
         
          
            sw.Stop();
            Console.WriteLine($"Done {sw.Elapsed}");
        }

        private static SolveDetail SolveGameShuffleGreenFelt(uint deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000) {
            Board board = new Board(drawCount);
            board.ShuffleGreenFelt(deal);
            if (!string.IsNullOrEmpty(movesMade)) {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;

            return SolveGame(board, maxStates);
        }

        private static SolveDetail SolveGame(uint deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000)
        {
            Board board = new Board(drawCount);
            board.Shuffle((int)deal);
            if (!string.IsNullOrEmpty(movesMade))
            {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;

            return SolveGameToCsv((int)deal, drawCount, board, maxStates);
        }
        private static SolveDetail SolveGame(string deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000) {
            Board board = new Board(drawCount);
            board.SetDeal(deal);
            if (!string.IsNullOrEmpty(movesMade)) {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;

            return SolveGame(board, maxStates);
        }
        private static SolveDetail SolveGame(Board board, int maxStates) {
            Console.WriteLine($"Deal:\n{board.GetDeal()}");
            Console.WriteLine();
            Console.WriteLine($"DealForCardGame:\n{board.GetDealForCardGame()}");
            Console.WriteLine();
            Console.WriteLine($"DealForCardGame2:\n{board.GetDealForCardGame2()}");
            Console.WriteLine();
            Console.WriteLine(board);

            //// SolveDetail result = board.Solve(250, 15, maxStates);
            ////  SolveDetail result = board.Solve(250, 15, 50_000_000,true);
            //// SolveDetail result = board.SolveWithCount(250, 20, 50_000_000, false);
            SolveDetail result = board.SolveWithCount(250, 20, 20_000_000, true);

            Console.WriteLine($"MovesOriginal:\n{board.MovesMadeOutput}");
            Console.WriteLine();
            Console.WriteLine($"Moves:\n{board.MovesMadeOutput2}");
            Console.WriteLine();
            Console.WriteLine($"MovesForCardGame:\n{board.MovesMadeOutputForCardGame}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"(Deal Result: {result.Result} Foundation: {board.CardsInFoundation} Moves: {board.MovesMade} Rounds: {board.TimesThroughDeck} States: {result.States} Took: {result.Time})");
            Console.WriteLine($"SolutionCount: {result.SolutionCount}");
            return result;
        }

        private static SolveDetail SolveGameToCsv(int seed, int drawCount, Board board, int maxStates)
        {
            //"path/to/file.txt"
            // string filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_"+Math.Ceiling(seed/10.0);

            string filePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/cardseed_" + Math.Ceiling(seed / 10.0)+".csv";

            string line = "This is a new line.";
            SolveDetail result;
            // 使用 StreamWriter 追加写入文件
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                // 获取StreamWriter所使用的基础流
                FileStream fileStream = (FileStream)writer.BaseStream;

                // 获取基础流的当前位置
                long currentPosition = fileStream.Position;
                Console.WriteLine("Current position in the file: " + currentPosition);

                if (fileStream.Position == 0)
                {
                    writer.Write($"seed,drawCount,GetDeal,GetDealForCardGame,GetDealForCardGame2,MovesMadeOutputForCardGame,result.Result,result.CardsInFoundation,result.MovesMade,result.TimesThroughDeck,result.States,result.Time,result.SolutionCount");
                    writer.WriteLine();

                    Console.WriteLine("Currently at the beginning of the file.");
                }

                //seed,drawCount,
                Console.Write($"{seed},{drawCount},'{board.GetDeal()}',{board.GetDealForCardGame()},{board.GetDealForCardGame2()},");     

                writer.Write($"{seed},{drawCount},'{board.GetDeal()}',{board.GetDealForCardGame()},{board.GetDealForCardGame2()},");
               

                //// SolveDetail result = board.Solve(250, 15, maxStates);
                ////  SolveDetail result = board.Solve(250, 15, 50_000_000,true);
                //// SolveDetail result = board.SolveWithCount(250, 20, 50_000_000, false);
                result = board.SolveWithCount(250, 20, 50_000_000, false);

              
                Console.Write($"{board.MovesMadeOutputForCardGame},{result.Result},{board.CardsInFoundation},{board.MovesMade},{board.TimesThroughDeck},{result.States},{result.Time},{result.SolutionCount}");

                writer.Write($"{board.MovesMadeOutputForCardGame},{result.Result},{board.CardsInFoundation},{board.MovesMade},{board.TimesThroughDeck},{result.States},{result.Time},{result.SolutionCount}");
              

                Console.WriteLine();
                writer.WriteLine();
               

            }


            return result;
        }

        private static SolveDetail SolveGameToFile(int seed, int drawCount, Board board, int maxStates)
        {
            //"path/to/file.txt"
            // string filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_"+Math.Ceiling(seed/10.0);

            string filePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/cardseed_" + Math.Ceiling(seed / 10.0);

            string line = "This is a new line.";
            SolveDetail result;
            // 使用 StreamWriter 追加写入文件
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {

                writer.WriteLine();
                Console.WriteLine();
                if (drawCount == 1)
                {
                    writer.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                    Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                }
                else
                {
                    writer.WriteLine("************************************************************************************************");
                    Console.WriteLine("************************************************************************************************");
                }

                Console.WriteLine($"seed:\n{seed}");
                Console.WriteLine();
                Console.WriteLine($"drawCount:\n{drawCount}");
                Console.WriteLine();
                Console.WriteLine($"Deal:\n{board.GetDeal()}");
                Console.WriteLine();

               
              
                writer.WriteLine($"seed:\n{seed}");
                writer.WriteLine();
                writer.WriteLine($"drawCount:\n{drawCount}");
                writer.WriteLine();
                writer.WriteLine($"Deal:\n{board.GetDeal()}");
                writer.WriteLine();

                Console.WriteLine($"DealForCardGame:\n{board.GetDealForCardGame()}");
                Console.WriteLine();

                Console.WriteLine($"DealForCardGame2:\n{board.GetDealForCardGame2()}");
                Console.WriteLine();
                Console.WriteLine(board);


                writer.WriteLine($"DealForCardGame2:\n{board.GetDealForCardGame2()}");
                writer.WriteLine();
                writer.WriteLine(board);

                //// SolveDetail result = board.Solve(250, 15, maxStates);
                ////  SolveDetail result = board.Solve(250, 15, 50_000_000,true);
                //// SolveDetail result = board.SolveWithCount(250, 20, 50_000_000, false);
                result = board.SolveWithCount(250, 20, 20_000_000, false);

                Console.WriteLine($"MovesOriginal:\n{board.MovesMadeOutput}");
                Console.WriteLine();
                Console.WriteLine($"Moves:\n{board.MovesMadeOutput2}");
                Console.WriteLine();
                Console.WriteLine($"MovesForCardGame:\n{board.MovesMadeOutputForCardGame}");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"(Deal Result: {result.Result} Foundation: {board.CardsInFoundation} Moves: {board.MovesMade} Rounds: {board.TimesThroughDeck} States: {result.States} Took: {result.Time})");
                Console.WriteLine($"SolutionCount: {result.SolutionCount}");


                writer.WriteLine();
                writer.WriteLine($"MovesForCardGame:\n{board.MovesMadeOutputForCardGame}");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine($"(Deal Result: {result.Result} Foundation: {board.CardsInFoundation} Moves: {board.MovesMade} Rounds: {board.TimesThroughDeck} States: {result.States} Took: {result.Time})");
                writer.WriteLine($"SolutionCount: {result.SolutionCount}");

                Console.WriteLine();
                writer.WriteLine();
                if (drawCount == 3)
                {
                    Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                    writer.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
                }
                else
                {
                    Console.WriteLine("************************************************************************************************");
                    writer.WriteLine("************************************************************************************************");
                }

            }


            return result;
        }


        static void ReadAndWriteFile()
        {
            string filePath = "path/to/your/file.txt"; // 请替换成您实际的文件路径

            List<string> lines = ReadFileLinesToList(filePath);

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
        }

        static List<string> ReadFileLinesToList(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("文件未找到。");
            }
            catch (IOException ex)
            {
                Console.WriteLine("发生IO错误：" + ex.Message);
            }

            return lines;
        }
    }
}
