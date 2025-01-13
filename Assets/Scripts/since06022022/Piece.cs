using UnityEngine;

namespace since06022022
{
    public class Piece
    {
        public Constants.PieceType Type { get; }
        public int Price { get; }
        public GameObject GameObject { get; set; }
        
        public int Rotation { get; set; }

        public Piece(Constants.PieceType type, int price)
        {
            Type = type;
            Price = price;
        }
    }
}