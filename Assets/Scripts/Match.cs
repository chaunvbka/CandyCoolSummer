#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using System;
    using Unity.Collections;
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

    public struct Match : IDisposable
    {
        /// <summary>
        /// List of cell position in a match.
        /// </summary>
        public NativeList<Vector3Int> MatchingCells;

        public NativeArray<float> DeletionTimer;

        //this is forced deletion, usually from a bonus. Used to remove obstacle
        public bool ForcedDeletion;

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

        public Match(MatchType type, Vector3Int point, Vector3Int direction)
        {
            MatchingCells = new(Allocator.Persistent);
            DeletionTimer = new(1, Allocator.Persistent);
            ForcedDeletion = false;
            Type = type;
            Direction = direction;
            CombinedPoint = point;
            CombinedPrefab = null;
        }

        public readonly void AddCandy(Candy candy)
        {
            if (candy.CurrentMatch != null)
                return;

            MatchingCells.Add(candy.CurrentPosition);
            candy.CurrentMatch = this;
        }

        public void Dispose()
        {
            MatchingCells.Dispose();
            DeletionTimer.Dispose();
        }
    }
}