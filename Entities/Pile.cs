using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace Klondike.Entities {
    public unsafe struct Pile : IComparable<Pile> {
        public int Size;
        public int First;  //表示第一个翻开的牌（上面的）
        private readonly int Index;
        private readonly Card[] Cards;

        public Pile(Card[] cards, int index) {
            Cards = cards;
            Index = index;
            Size = 0;
            First = -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() {
            Size = 0;
            First = -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flip(int count = 1) {
            First = Size - count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Card card) {
            Cards[Index + Size++] = card;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(ref Pile to) {
            to.Add(Cards[Index + --Size]); //--Size 就代表删除了
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(ref Pile to, int count) {
            int fromIndex = Index + Size - count; //count表示移动多少张牌
            int toIndex = to.Index + to.Size;
            Span<Card> source = new Span<Card>(Cards, fromIndex, count);
            Span<Card> destination = new Span<Card>(Cards, toIndex, count); //所有Piles共享Cards;只是通过index区分
            source.CopyTo(destination);
            Size -= count; //from--Size减少
            to.Size += count; //from--Size增加
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFlip(ref Pile to, int count) {
            int fromIndex = Index + Size - count;
            int toIndex = to.Index + to.Size;
            Span<Card> source = new Span<Card>(Cards, fromIndex, count);
            Span<Card> destination = new Span<Card>(Cards, toIndex, count);
            source.CopyTo(destination);
            destination.Reverse();
            Size -= count;
            to.Size += count;
        }
        public Card this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Cards[Index + index]; }
        }
        public Card Bottom {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Size > 0 ? Cards[Index + Size - 1] : Card.EMTPY; }
        }
        public Card BottomNoCheck {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Cards[Index + Size - 1]; }
        }
        public Card Top {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Size > 0 ? Cards[Index + First] : Card.EMTPY; }
        }
        public Card TopNoCheck {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Cards[Index + First]; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Card Up(int size) {
            int index = Size - size - 1;
            return index >= 0 ? Cards[Index + index] : Card.EMTPY;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Card UpNoCheck(int size) {
            return Cards[Index + Size - size - 1];
        }
        public int UpSize {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Size - First; }
        }
        public override string ToString() {
            return $"Max: {Cards.Length} Size: {Size} First: {First}";
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Pile other) {
            int upCompare = other.UpSize.CompareTo(UpSize);
            if (upCompare != 0) {
                return upCompare;
            }
            return other.Index.CompareTo(Index);
        }


        public Card moveFromCard(int count) 
        {
            int fromIndex = Index + Size - count;
            return Cards[fromIndex];
          
        }

        internal const int Foundation1 = 1;
        internal const int Foundation2 = 2;
        internal const int Foundation3 = 3;
        internal const int Foundation4 = 4;

        internal const int Tableau1 = 5;
        internal const int Tableau2 = 6;
        internal const int Tableau3 = 7;
        internal const int Tableau4 = 8;
        internal const int Tableau5 = 9;
        internal const int Tableau6 = 10;
        internal const int Tableau7 = 11;

        /*
         *  A = Waste Pile    //-300
            B = Clubs Pile     //黑梅，1，-103
            C = Diamonds Pile //红方，2，-102
            D = Spades Pile   //黑桃，3，-104
            E = Hearts Pile  //红心，4，-101
            F = Tableau 1   //-105
            G = Tableau 2   //-106
            H = Tableau 3   //-107
            I = Tableau 4   //-108
            J = Tableau 5   //-109
            K = Tableau 6   //-110
            L = Tableau 7  // -111
         */
        public Card moveToCard(int toPileIndex)
        {
            int toIndex = -1;

            if (Size == 0) 
            {

               
                //牌堆里是空的，没有牌；
                // Console.WriteLine("No card in this pile");

                Dictionary<int, int> indexToEmptyGameID = new Dictionary<int,int>();
                indexToEmptyGameID[Foundation1] = -103;
                indexToEmptyGameID[Foundation2] = -102;
                indexToEmptyGameID[Foundation3] = -104;
                indexToEmptyGameID[Foundation4] = -101;
                indexToEmptyGameID[Tableau1] = -105;
                indexToEmptyGameID[Tableau2] = -106;
                indexToEmptyGameID[Tableau3] = -107;
                indexToEmptyGameID[Tableau4] = -108;
                indexToEmptyGameID[Tableau5] = -109;
                indexToEmptyGameID[Tableau6] = -110;
                indexToEmptyGameID[Tableau7] = -111;

               

                toIndex = Index;
                Cards[toIndex] = Card.EMTPY;  //只是通过改变size来改变删除数组中的元素，元素本身并没有复值为零
                Cards[toIndex].forEmptyPileCardGameID = indexToEmptyGameID[toPileIndex];
            }
            else 
            {
                 toIndex = Index + Size - 1;
            }
            
            return Cards[toIndex];
        }
    }
}