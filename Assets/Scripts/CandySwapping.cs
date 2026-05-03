#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CandySwapping : IDisposable, IBoardAction
    {
        static CandySwapping s_Instance = null;
        private bool _disposed = false;
        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;

        private enum SwapStage
        {
            None,
            Forward,
            Return
        }

        private Vector3Int _startPos;
        private Vector3Int _endPos;
        private SwapStage _swapStage = SwapStage.None;
        private readonly Queue<bool> _uniqueSwapQueue = new();
        private MatchFinding _matchFinding;

        private const float SwapSpeed = 10.0f;

        public CandySwapping()
        {
            if (s_Instance != null)
            {
                Debug.LogError("CandySwapping instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Init(Board board, MatchFinding matchFinding)
        {
            _grid = board.Grid;
            _boardCells = board.BoardCells;
            _matchFinding = matchFinding;
        }

        public void OnSwapAction(Vector3Int startPos, Vector3Int endPos)
        {
            _startPos = startPos;
            _endPos = endPos;

            if (_uniqueSwapQueue.Count == 0)
            {
                _uniqueSwapQueue.Enqueue(true);
                _swapStage = SwapStage.Forward;
            }

            // Only a swap happen at a time.
            // if (_uniqueSwapQueue.Count == 0 && m_TickingMatch.Count == 0 &&
            //     m_TickingCells.Count == 0)
            // {
            //     _uniqueSwapQueue.Enqueue(true);
            //     _swapStage = SwapStage.Forward;

            //     // When swap occur, reset hint cells. So next time
            //     // we will check possible match again.
            //     _hintCells.Clear();
            //     Debug.Log("HintClear: " + _hintCells.Count);
            // }

        }

        public void OnUpdate()
        {
            if (_uniqueSwapQueue.Count == 1)
            {
                if (_swapStage == SwapStage.Forward)
                {
                    var hasStart = _boardCells.TryGetValue(_startPos, out var startCell);
                    var hasEnd = _boardCells.TryGetValue(_endPos, out var endCell);

                    if (!hasStart || !startCell.CanBeMoved || !hasEnd || !endCell.CanBeMoved)
                    {
                        return;
                    }

                    var startCandy = startCell.ContainingCandy;
                    var endCandy = endCell.ContainingCandy;

                    var startWorldPos = _grid.GetCellCenterWorld(_startPos);
                    var endWorldPos = _grid.GetCellCenterWorld(_endPos);

                    startCandy.transform.position = Vector3.MoveTowards(startCandy.transform.position,
                        endWorldPos, Time.deltaTime * SwapSpeed);
                    endCandy.transform.position = Vector3.MoveTowards(endCandy.transform.position,
                        startWorldPos, Time.deltaTime * SwapSpeed);

                    if (startCandy.transform.position == endWorldPos &&
                        endCandy.transform.position == startWorldPos)
                    {
                        startCandy.MoveTo(_endPos);
                        endCandy.MoveTo(_startPos);

                        startCell.ContainingCandy = endCandy;
                        endCell.ContainingCandy = startCandy;

                        Vector3Int direction = _startPos - _endPos;

                        bool firstFind = _matchFinding.FindMatch(_startPos, direction);
                        bool secondFind = _matchFinding.FindMatch(_endPos, direction);

                        if (!firstFind && !secondFind)
                        {
                            _swapStage = SwapStage.Return;
                        }
                        else if (firstFind || secondFind)
                        {
                            // Finish swap
                            _swapStage = SwapStage.None;
                            _uniqueSwapQueue.Dequeue();
                        }
                    }
                }

                if (_swapStage == SwapStage.Return)
                {
                    // Return swap
                    var hasStart = _boardCells.TryGetValue(_startPos, out var startCell);
                    var hasEnd = _boardCells.TryGetValue(_endPos, out var endCell);

                    if (!hasStart || !startCell.CanBeMoved || !hasEnd || !endCell.CanBeMoved)
                    {
                        return;
                    }

                    var startCandy = startCell.ContainingCandy;
                    var endCandy = endCell.ContainingCandy;

                    var startWorldPos = _grid.GetCellCenterWorld(_startPos);
                    var endWorldPos = _grid.GetCellCenterWorld(_endPos);

                    startCandy.transform.position = Vector3.MoveTowards(startCandy.transform.position,
                        endWorldPos, Time.deltaTime * SwapSpeed);
                    endCandy.transform.position = Vector3.MoveTowards(endCandy.transform.position,
                        startWorldPos, Time.deltaTime * SwapSpeed);

                    if (startCandy.transform.position == endWorldPos &&
                      endCandy.transform.position == startWorldPos)
                    {
                        startCandy.MoveTo(_endPos);
                        endCandy.MoveTo(_startPos);

                        startCell.ContainingCandy = endCandy;
                        endCell.ContainingCandy = startCandy;

                        // Finish swap
                        _swapStage = SwapStage.None;
                        _uniqueSwapQueue.Dequeue();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_Instance = null;
        }
    }

}