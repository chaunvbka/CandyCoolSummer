#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class MatchEngine : IDisposable
    {
        // Function:
        // Find match, destroy candy, swap candy.

        private bool _disposed = false;

        private Board _board;
        private Tilemap _tileBoard;
        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;
        private List<Vector3Int> _spawnerPositions;
        private Candy[] _candyPrefabs;


        private List<Match> m_TickingMatch = new();
        private List<Vector3Int> _emptyCells = new();
        private List<Vector3Int> m_TickingCells = new();
        private List<Vector3Int> m_NewTickingCells = new();
        private List<Vector3Int> _falledCells = new();
        private List<Vector3Int> _hintCells = new();
        float timer = 0.0f;

        public bool HintAction { get; set; }

        public void Init(Board board)
        {
            _board = board;
            _grid = board.Grid;
            _tileBoard = board.TileBoard;
            _boardCells = board.BoardCells;
            _spawnerPositions = board.SpawnerPositions;
            _candyPrefabs = board.CandyPrefabs;
        }

        public void OnUpdate()
        {
            if (m_TickingMatch.Count > 0)
            {
                MatchTicking();
            }

            if (_emptyCells.Count > 0)
            {
                //EmptyCheck();
            }

            if (m_NewTickingCells.Count > 0)
            {
                m_TickingCells.AddRange(m_NewTickingCells);
                m_NewTickingCells.Clear();
                //incrementHintTimer = false;
            }

            if (m_TickingCells.Count > 0)
            {
                //MoveCandies();
            }

            if (_falledCells.Count > 0)
            {
               // DoMatchCheck();
            }

            // if (m_TickingMatch.Count == 0 && _falledCells.Count == 0 && _uniqueSwapQueue.Count == 0 &&
            //     m_TickingCells.Count == 0 && _emptyCells.Count == 0 && _hintCells.Count == 0)
            // {
            //     bool possibleMatch = FindPossibleMatch();

            //     if (!possibleMatch)
            //     {
            //         Debug.LogWarning("Reshuffle");
            //         ReshuffleBoard();
            //     }
            //     else
            //     {
            //         HintAction = true;
            //     }
            // }

            // //TODO: animation for hint cells
            // if (_hintCells.Count > 0)
            // {
            //     HintAnimation();
            // }
        }



        void MatchTicking()
        {

        }

        void ActivateSpawnerAt(Vector3Int cellPos)
        {
            var candyPrefab = _candyPrefabs[UnityEngine.Random.Range(0, _candyPrefabs.Length)];
            var incomingCandy = CandyFactory.Create(candyPrefab);
            incomingCandy.transform.position = _grid.GetCellCenterWorld(cellPos + Vector3Int.up);

            _boardCells[cellPos].IncomingCandy = incomingCandy;

            incomingCandy.StartMoveTimer();
            incomingCandy.SpeedMultiplier = 1.0f;

            m_NewTickingCells.Add(cellPos);

            // This cell is not empty anymore.
            _emptyCells.Remove(cellPos);
        }




        // void DoMatchCheck()
        // {
        //     foreach (var cellPos in _falledCells)
        //     {
        //         FindMatch(cellPos);
        //     }

        //     _falledCells.Clear();
        // }





        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            //s_Instance = null;
        }
    }

}