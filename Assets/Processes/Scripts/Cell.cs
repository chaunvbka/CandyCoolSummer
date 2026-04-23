#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using UnityEngine;

    public class Cell
    {
        public static readonly Vector3Int[] Neighbours =
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left
        };

        public Candy ContainingCandy;
        public Candy IncomingCandy;
        public Obstacle Obstacle;

        public bool Locked = false;

        public bool CanMatch()
        {
            return ContainingCandy != null;
        }

        public bool CanDelete()
        {
            return !Locked;
        }

        public bool IsEmpty()
        {
            return ContainingCandy == null && IncomingCandy == null;
        }
    }
}

