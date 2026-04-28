#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum MatchType
    {
        LineThree,
        LineFour,
        LineFive,
        TLShape,
        SquareShape
    }

    public class Match
    {
        /// <summary>
        /// List of cell position in a match.
        /// </summary>
        public List<Vector3Int> MatchingCandy = new();

        public MatchType Type;

        /// <summary>
        /// The cell position to spawn special candy.
        /// </summary>
        public Vector3Int CombinedPoint;

        /// <summary>
        /// The prefab to spawn special candy.
        /// </summary>
        public CombineCandy CombinedPrefab;
    }
}