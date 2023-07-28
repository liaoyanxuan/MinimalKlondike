using Klondike.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klondike
{
    public class GameCard
    {

        private int id;
        private int rank;
        private int suit;
        private bool isOpened;
        private bool isMovedToCommunity;
        private bool isMovedToThron;
        private GameCard() { }
        public GameCard(int id, int rank, int suit)
        {
            this.id = id;
            this.rank = rank;
            this.suit = suit;
            this.isOpened = false;
            this.isMovedToCommunity = false;
            this.isMovedToThron = false;
        }
        public GameCard(int id, int rank, int suit, bool isOpened)
            : this(id, rank, suit)
        {
            this.isOpened = isOpened;
        }
        private GameCard(int id, int rank, int suit, bool isOpened, bool isMovedToCommunity, bool isMovedToThron)
            : this(id, rank, suit)
        {
            this.isOpened = isOpened;
            this.isMovedToCommunity = isMovedToCommunity;
            this.isMovedToThron = isMovedToThron;
        }
        public int Id { get { return id; } set { id = value; } }
        public int Rank { get { return rank; } set { rank = value; } }
        public int Suit { get { return suit; } set { suit = value; } }
        public bool IsOpened
        {
            get
            {
                return isOpened;
            }
            set
            {
                //UnityEngine.Debug.Log ("Card Id: " + id + " set Is Open: " + value);
                isOpened = value;
            }
        }
        public bool IsPayedForCommunity { get { return isMovedToCommunity; } set { isMovedToCommunity = value; } }
        public bool IsPayedForThron { get { return isMovedToThron; } set { isMovedToThron = value; } }
        private const int RED_SUIT_RANGE = 1;
        private const int ACE = 1;
        private const int KING = 13;
        public bool IsRed { get { return (suit <= RED_SUIT_RANGE) ? true : false; } }
        public bool IsAce { get { return (rank.Equals(ACE)) ? true : false; } }
        public bool IsKing { get { return (rank.Equals(KING)) ? true : false; } }
        public GameCard Copy()
        {
            return new GameCard(id, rank, suit, isOpened, isMovedToCommunity, isMovedToThron);
        }
        public void SetCard(int id, int rank, int suit, bool isOpened)
        {
            this.id = id;
            this.rank = rank;
            this.suit = suit;
            this.isOpened = isOpened;
        }
        public override string ToString()
        {
            return string.Format("[Card: Id={0}, Rank={1}, Suit={2}, IsOpened={3}, IsRed={4}, IsAce={5}, IsKing={6}]", Id, Rank, Suit, IsOpened, IsRed, IsAce, IsKing);
        }
    }
}
