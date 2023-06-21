using System.Runtime.InteropServices;
namespace Klondike.Entities {
    [StructLayout(LayoutKind.Sequential, Size = 8, Pack = 1)]
    public struct Card {
        public static readonly Card EMTPY = new Card() { ID = 52, ID2 = 0, Suit = CardSuit.None, Rank = CardRank.None, IsEven = 1, IsRed = 2, RedEven = 2, Order = 0 };
        private static readonly string[] Cards = { "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "TC", "JC", "QC", "KC",
                                                   "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "TD", "JD", "QD", "KD",
                                                   "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "TS", "JS", "QS", "KS",
                                                   "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "TH", "JH", "QH", "KH", "  "};
        public byte ID;
        public byte ID2;
        public CardSuit Suit;
        public CardRank Rank;
        public byte IsRed;
        public byte IsEven;  //是否偶数
        public byte RedEven;
        public byte Order;
        public int forEmptyPileCardGameID;
        public Card(int id) {
            ID = (byte)id;
            Rank = (CardRank)(id % 13);
            Suit = (CardSuit)(id / 13);
            ID2 = (byte)(((int)Rank << 2) | (int)Suit);
            IsRed = (byte)((int)Suit & 1);
            IsEven = (byte)((int)Rank & 1);
            RedEven = (byte)(IsRed ^ IsEven);  //位运算，异或，两个位相同为0，相异为1
            Order = (byte)((int)Suit >> 1);
        }
        public override string ToString() {
            return Cards[ID];
        }

        public int forCardGameID
        {
           
            get {

                int forGameID = -1;

                if (Suit == CardSuit.None) 
                {
                    //空牌
                    forGameID= forEmptyPileCardGameID;
                }
                else 
                {
                    //0:红方块(Diamonds)，1:红心(Hearts),2:黑梅（Clubs）,3：黑桃（Spade）
                    int forGameSuit = 0;
                    if (Suit == CardSuit.Diamonds)
                    {
                        forGameSuit = 0;
                    }
                    else if (Suit == CardSuit.Hearts)
                    {
                        forGameSuit = 1;
                    }
                    else if (Suit == CardSuit.Clubs)
                    {
                        forGameSuit = 2;
                    }
                    else if (Suit == CardSuit.Spades)
                    {
                        forGameSuit = 3;
                    }

                    forGameID = 13 * forGameSuit + ((int)Rank + 1);
                }
              



                return forGameID;
            }
        }

        public int forGameSuit 
        {
            get
            {
                //0:红方块(Diamonds)，1:红心(Hearts),2:黑梅（Clubs）,3：黑桃（Spade）
                int forGameSuit = -1;
                if (Suit == CardSuit.Diamonds)
                {
                    forGameSuit = 0;
                }
                else if (Suit == CardSuit.Hearts)
                {
                    forGameSuit = 1;
                }
                else if (Suit == CardSuit.Clubs)
                {
                    forGameSuit = 2;
                }
                else if (Suit == CardSuit.Spades)
                {
                    forGameSuit = 3;
                }

              

                return forGameSuit;
            }
        }
    }
}