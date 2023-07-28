using Klondike.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

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


        public static void MainSeed(string[] args)
        {
           

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for(uint seed = 19; seed <= 19; seed++) 
            {
                SolveGame(seed, 1, null, 50_000_000);
                SolveGame(seed, 3, null, 50_000_000);
            }
         
          
            sw.Stop();
            Console.WriteLine($"Done {sw.Elapsed}");
        }

        public static void Main(string[] args)
        {

            //ReadAndWriteFile();
            //分析cargame题目，输出

            AnalyzeAndWriteFile();
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


        private static SolveDetail SolveGameToCsv(string deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000,int seed=999990)
        {
            Board board = new Board(drawCount);
            board.SetDeal(deal);
            if (!string.IsNullOrEmpty(movesMade))
            {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;

            return SolveGameToCsv(seed, drawCount, board, maxStates);
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


        public static bool IsMacOS()
        {
            // 获取当前操作系统的平台
            PlatformID platform = Environment.OSVersion.Platform;

            // 判断是否为 macOS 平台
            return platform == PlatformID.MacOSX;
        }

        private static SolveDetail SolveGameToCsv(int seed, int drawCount, Board board, int maxStates)
        {
            //"path/to/file.txt"
            // string filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_"+Math.Ceiling(seed/10.0);
            string filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_" + Math.Ceiling(seed / 10.0) + ".csv";


            filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_game100.csv";

            if (IsMacOS()) 
            {
                 filePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/cardseed_" + Math.Ceiling(seed / 10.0) + ".csv";

                filePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/cardseed_game100.csv";
            }
          

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
                result = board.SolveWithCount(250, 20, maxStates, false);

              
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


        static void AnalyzeAndWriteFile()
        {
            //  string filePath = "path/to/your/file.txt"; // 请替换成您实际的文件路径
            string readfilePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\Deal_OneCardData_Hard180_Deck500.txt";

            if (IsMacOS())
            {
                readfilePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/Deal_OneCardData_Hard180_Deck500.txt";
            }

            List<string> lines = ReadFileLinesToList(readfilePath);


            string writefilePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\cardseed_game100.csv";

            if (IsMacOS())
            {

                writefilePath = @"/Users/liaoyanxuan/GitProject/MinimalKlondike/generalgamecard/cardseed_game100.csv";
            }
            List<string> writeFilelines = ReadFileLinesToList(writefilePath);

            int start_i = 0;
            if (writeFilelines.Count > 2) 
            {
                start_i = writeFilelines.Count - 1;
            }

            int end_i = Math.Min(start_i + 100,500);

            for (int i = start_i; i < end_i; i++)
            {
                string line = lines[i];
                SolveGameToCsv(line, 1, null, 10_000_000,100000+10*i);   //输出到csv
            }


        }


        /// <summary>
        /// 解析cargame原题目
        /// </summary>
        static void ReadAndWriteFile()
        {
          //  string filePath = "path/to/your/file.txt"; // 请替换成您实际的文件路径
            string filePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\ThreeCardsData_Hard195_Deck500.txt";

            List<string> lines = ReadFileLinesToList(filePath);

            string WirtefilePath = @"E:\GitprojectE\MinimalKlondike\generalgamecard\Deal_ThreeCardsData_Hard195_Deck500.txt";

        
            // 使用 StreamWriter 追加写入文件
            using (StreamWriter writer = new StreamWriter(WirtefilePath, false))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (i % 2 == 0)   //偶数
                    {
                        string line = lines[i];
                        string dealline = DeckParse(line);
                        Console.WriteLine(dealline);
                        writer.WriteLine(dealline);
                    }
                }
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

        public static string DeckParse(string deck)
        {
            List<GameCard> convertedDeck = new List<GameCard>();
            string[] cardStr = deck.Split('*');
            foreach (string element in cardStr)
            {
                string[] cardOne = element.Split(':');
                int id = int.Parse(cardOne[0]);
                int rank = int.Parse(cardOne[1]);
                int suit = int.Parse(cardOne[2]);
                bool isOpen = (cardOne[3].Equals("1"));
                convertedDeck.Add(new GameCard(id, rank, suit, isOpen));
            }
          

            return GetDeal(convertedDeck);
        }

        public static string GetDeal(List<GameCard> deck)
        {
            StringBuilder cardSet = new StringBuilder(deck.Count * 3);
            int TableauSize = 7;//（7列，共28张牌）
            int TalonSize = 24;  //初始未翻牌size（右上角） 52-28
            for (int k = 1, m = 0; k <= TableauSize; k++)
            {
                for (int i = k, j = m; i <= TableauSize; i++)
                {
                    AppendCard(cardSet, deck[j]);
                    j += i;
                }
                m += k + 1;
            }

            int end = deck.Count - TalonSize;
            for (int i = end; i <= deck.Count - 1; i++)
            {
                AppendCard(cardSet, deck[i]);
            }

            //Console.WriteLine("GetDeal:" + cardSet.ToString());
            return cardSet.ToString();
        }

        private static void AppendCard(StringBuilder cardSet, GameCard card)
        {
            //0:红方块(Diamonds)，1:红心(Hearts),2:黑梅（Clubs）,3：黑桃（Spade）


            //1 - 4 (Clubs,Diamonds,Hearts,Spades)
            int suitO = (int)card.Suit;
            int suit = -1;
            if (suitO == 0)
            {
                suit = 2;
            }
            else if (suitO == 1)
            {
                suit = 3;
            }
            else if (suitO == 2)
            {
                suit = 1;
            }
            else if (suitO == 3)
            {
                suit = 4;
            }


            cardSet.Append($"{(int)(card.Rank - 1) + 1:00}{suit}");
        }
    }
}
