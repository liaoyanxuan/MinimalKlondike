using System;
using System.Runtime.CompilerServices;
namespace Klondike.Entities {
    public sealed class TalonHelper {
        public readonly Card[] StockWaste;
        public readonly int[] CardsDrawn;
        private readonly bool[] StockUsed;

        public TalonHelper(int talonSize) {
            StockWaste = new Card[talonSize];
            CardsDrawn = new int[talonSize];
            StockUsed = new bool[talonSize];
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int Calculate(int drawCount, Pile wastePile, Pile stockPile) {
            int Size = 0;
            Array.Fill(StockUsed, false);
            //一.首先检查waste的首牌First是否可用（具有移动性），二.1不可用，翻牌stock看是否可用，stock最下方开始
            //三.当stock全部翻完还没遇到可用的，（此时有redeal动作）从头检查waste，继续翻牌
            //Check waste
            int wasteSize = wastePile.Size;
            if (wasteSize > 0) {
                StockWaste[Size] = wastePile.BottomNoCheck;  //第一个是wastePile的最下牌
                CardsDrawn[Size++] = 0;            //给索引0，也就是count为0 的情况，此次move无需翻牌
            }

            //Check cards waiting to be turned over from stock
            int stockSize = stockPile.Size;
            int position = stockSize - drawCount;
            //待翻牌；翻1张的情况（先考虑翻1张）：翻3张的情况-->如果stockSize不够，则
            //drawCount，此时只有当stockSize为0，翻牌区无牌；
            //-->如果stockSize不够但>0，则直接翻到最后一张：position为0
            if (position < 0) { position = stockSize > 0 ? 0 : -1; }
            for (int i = position; i >= 0; i -= drawCount) {
                StockWaste[Size] = stockPile[i];
                CardsDrawn[Size++] = stockSize - i;
                StockUsed[i] = true;
            }

            //Check cards already turned over in the waste, meaning we have to "redeal" the deck to get to it
            int amountToDraw = stockSize + 1;
            wasteSize--;  //排除wastePile的首牌bottom;  drawCount为1时，从0开始
            for (position = drawCount - 1; position < wasteSize; position += drawCount) {
                StockWaste[Size] = wastePile[position];
                CardsDrawn[Size++] = -amountToDraw - position;  //获得翻牌次数
            }

            //Check cards in stock after a "redeal". Only happens when draw count > 1 and you have access to more cards in the talon
            if (position > wasteSize && wasteSize >= 0) {
                amountToDraw += stockSize + wasteSize;
                position = stockSize - position + wasteSize;
                for (int i = position; i > 0; i -= drawCount) {
                    if (StockUsed[i]) { break; } //已经在stock使用过，直接跳出
                    StockWaste[Size] = stockPile[i];
                    CardsDrawn[Size++] = i - amountToDraw;
                }
            }

            return Size;
        }
    }
}