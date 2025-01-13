using System.Drawing;
using UnityEngine;

namespace since06022022
{
    public static class Constants
    {
        // materials names
        public const string RayMat1Name = "RayRedMat";
        public const string RayMat2Name = "RayBlueMat";
        public const string PieceLockMatName = "PieceLock";
        public const string PieceMatName = "Piece";
        public const string TileSelectMatName = "CellSelect";
        public const string TileMatName = "Cell";
        public const string Color1ButtonMatName = "ButtonRed";
        public const string Color2ButtonMatName = "ButtonBlue";
        public const string TurretBodyMatName = "TurretBody";
        public const string TurretGunMatName = "TurretGun";
        public const string TurretHighlightMatName = "TurretHighlight";
        
        // gameobjects names
        public const string PieceKing1Name = "king 1";
        public const string PieceKing2Name = "king 2";
        public const string RayManagerName = "Ray Manager";
        public const string EmitterName = "Emitter";
        public const string InsideLightGroupName = "Lights";
        public const string InsideLight1Name = "Light 1";
        public const string InsideLight2Name = "Light 2";
        public const string PieceTemporaryName = "Piece Temporary"; // model used when warping a piece
        public const string BoardName = "Board2";
        public const string ReceiverName = "Receiver";
        
        //gameobjects paths
        public const string RayManagerPath = "/Ray Manager (1)";
        public const string PieceManagerPath = "/Piece Manager";
        
        // tags
        public const string TileTag = "Tile";
        public const string PieceTag = "Piece";
        public const string InvokerTag = "Invoker";
        public const string SellerTag = "Seller";
        public const string WallTag = "Wall";
        public const string TileUnavailableTag = "TileUnavailable";
        public const string TileKingTag = "TileKing";

        // other
        public static readonly UnityEngine.Color[] Colors = { UnityEngine.Color.red, new UnityEngine.Color(0f, 0.3294f, 1f, 1f), new UnityEngine.Color(1f, 1f, 0f, 1f)};
        public const int PropagationSpeed = 30;
        public const string ModelPath = "Model/Body";
        public const int TurretPrice = 2;
        public const int MissilePrice = 10;
        public const int StartingScore1 = 1;
        public const int StartingScore2 = 1;
        public enum PieceType
        {
            Turret,
            Missile,
            Shield
        };
        public const int WallPlacementRarityFactor = 14; // must be > 0
        public const int WinningScore = 20;

    }
}