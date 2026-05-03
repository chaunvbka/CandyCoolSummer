#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum MatchType
    {
        LineThree,
        LineFourH,
        LineFourV,
        LineFive,
        TLShape,
        SquareShape
    }

    [System.Serializable]
    public class Match
    {
        /// <summary>
        /// List of cell position in a match.
        /// </summary>
        public List<Vector3Int> MatchingCells = new();

        public float DeletionTimer = 0.0f;

        //this is forced deletion, usually from a bonus. Used to remove obstacle
        public bool ForcedDeletion = false;

        public MatchType Type;

        public Vector3Int Direction;

        /// <summary>
        /// The cell position to spawn special candy.
        /// </summary>
        public Vector3Int CombinedPoint;

        /// <summary>
        /// The prefab to spawn special candy.
        /// </summary>
        public Candy CombinedPrefab;

        public void AddCandy(Candy candy)
        {
            if (candy.CurrentMatch != null)
                return;

            MatchingCells.Add(candy.CurrentPosition);
            candy.CurrentMatch = this;
        }
    }
}