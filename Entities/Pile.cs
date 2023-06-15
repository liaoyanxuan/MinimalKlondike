﻿using System;
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
    }
}