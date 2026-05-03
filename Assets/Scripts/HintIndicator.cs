#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class HintIndicator : IDisposable, IHintAction
    {
        private bool _disposed = false;

        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;
        private Board _board;
        private MatchFinding _matchFinding;

        private List<Vector3Int> _pickedHints = new();
        private readonly List<List<Vector3Int>> _possibleMatchs = new();
        private List<Vector3Int> _matchedCells;
        private float _timeAppearHint = 0.0f;
        /// <summary>
        /// If true, run hint animation.
        /// </summary>
        private bool _enableHint;
        private const float AppearHintDuration = 1;
        private float[] timers;
        private const float HintSpeed = 3;


        public List<Vector3Int> PickedHints => _pickedHints;


        public void Init(Board board, MatchFinding matchFinding)
        {
            _grid = board.Grid;
            _boardCells = board.BoardCells;
            _board = board;
            _matchFinding = matchFinding;
            _matchedCells = matchFinding.MatchedCells;
        }

        public void OnUpdate()
        {
            if (_pickedHints.Count > 0 && _enableHint)
            {
                _timeAppearHint += Time.deltaTime;
                if (_timeAppearHint > AppearHintDuration)
                {
                    HintAnimation();

                    // Prevent increment hint counter time forever.
                    if (_timeAppearHint > AppearHintDuration + 1)
                    {
                        _timeAppearHint = AppearHintDuration + 1;
                    }
                }
            }
        }

        public void FindHintCells()
        {
            //Debug.Log("FindHintCells");
            FindAllPossibleMatch();

            if (_possibleMatchs.Count == 0)
            {
                Debug.LogWarning("Reshuffle");
                ReshuffleBoard();
            }
            else
            {
                StartHint();
            }
        }

        public void StartHint()
        {
            _enableHint = true;
        }

        public void StopHint()
        {
            _timeAppearHint = 0.0f;
            _enableHint = false;
        }

        void FindAllPossibleMatch()
        {
            _possibleMatchs.Clear();

            foreach (var entry in _boardCells)
            {
                bool possibleMatch = false;

                var cellPos = entry.Key;
                var cell = entry.Value;

                var topPos = cellPos + Vector3Int.up;
                var downPos = cellPos + Vector3Int.down;
                var rightPos = cellPos + Vector3Int.right;
                var leftPos = cellPos + Vector3Int.left;

                var hasTopCell = _boardCells.TryGetValue(topPos, out var topCell);
                var hasDownCell = _boardCells.TryGetValue(downPos, out var downCell);
                var hasRightCell = _boardCells.TryGetValue(rightPos, out var rightCell);
                var hasLeftCell = _boardCells.TryGetValue(leftPos, out var leftCell);

                if (cell.ContainingCandy != null)
                {
                    if (hasTopCell && topCell.ContainingCandy != null && !possibleMatch)
                    {
                        // Swap the cell
                        (cell.ContainingCandy, topCell.ContainingCandy) =
                        (topCell.ContainingCandy, cell.ContainingCandy);

                        if (_matchFinding.FindMatch(topPos, Vector3Int.zero, false))
                        {
                            _matchedCells.Remove(topPos);
                            _matchedCells.Add(cellPos);
                            possibleMatch = true;
                        }
                        else
                        {
                            if (_matchFinding.FindMatch(cellPos, Vector3Int.zero, false))
                            {
                                _matchedCells.Remove(cellPos);
                                _matchedCells.Add(topPos);
                                possibleMatch = true;
                            }
                        }

                        // Swap back
                        (cell.ContainingCandy, topCell.ContainingCandy) =
                        (topCell.ContainingCandy, cell.ContainingCandy);
                    }

                    if (hasDownCell && downCell.ContainingCandy != null && !possibleMatch)
                    {
                        // Swap the cell
                        (cell.ContainingCandy, downCell.ContainingCandy) =
                        (downCell.ContainingCandy, cell.ContainingCandy);

                        if (_matchFinding.FindMatch(downPos, Vector3Int.zero, false))
                        {
                            _matchedCells.Remove(downPos);
                            _matchedCells.Add(cellPos);
                            possibleMatch = true;
                        }
                        else
                        {
                            if (_matchFinding.FindMatch(cellPos, Vector3Int.zero, false))
                            {
                                _matchedCells.Remove(cellPos);
                                _matchedCells.Add(downPos);
                                possibleMatch = true;
                            }
                        }

                        // Swap back
                        (cell.ContainingCandy, downCell.ContainingCandy) =
                        (downCell.ContainingCandy, cell.ContainingCandy);
                    }

                    if (hasRightCell && rightCell.ContainingCandy != null && !possibleMatch)
                    {
                        // Swap the cell
                        (cell.ContainingCandy, rightCell.ContainingCandy) =
                        (rightCell.ContainingCandy, cell.ContainingCandy);

                        if (_matchFinding.FindMatch(rightPos, Vector3Int.zero, false))
                        {
                            _matchedCells.Remove(rightPos);
                            _matchedCells.Add(cellPos);
                            possibleMatch = true;
                        }
                        else
                        {
                            if (_matchFinding.FindMatch(cellPos, Vector3Int.zero, false))
                            {
                                _matchedCells.Remove(cellPos);
                                _matchedCells.Add(rightPos);
                                possibleMatch = true;
                            }
                        }

                        // Swap back
                        (cell.ContainingCandy, rightCell.ContainingCandy) =
                        (rightCell.ContainingCandy, cell.ContainingCandy);
                    }

                    if (hasLeftCell && leftCell.ContainingCandy != null && !possibleMatch)
                    {
                        // Swap the cell
                        (cell.ContainingCandy, leftCell.ContainingCandy) =
                        (leftCell.ContainingCandy, cell.ContainingCandy);

                        if (_matchFinding.FindMatch(leftPos, Vector3Int.zero, false))
                        {
                            _matchedCells.Remove(leftPos);
                            _matchedCells.Add(cellPos);
                            possibleMatch = true;
                        }
                        else
                        {
                            if (_matchFinding.FindMatch(cellPos, Vector3Int.zero, false))
                            {
                                _matchedCells.Remove(cellPos);
                                _matchedCells.Add(leftPos);
                                possibleMatch = true;
                            }
                        }

                        // Swap back
                        (cell.ContainingCandy, leftCell.ContainingCandy) =
                        (leftCell.ContainingCandy, cell.ContainingCandy);
                    }
                }

                if (possibleMatch)
                {
                    var temp = new List<Vector3Int>();
                    foreach (var pos in _matchedCells)
                    {
                        temp.Add(pos);
                    }
                    _possibleMatchs.Add(temp);
                    _matchedCells.Clear();
                }
            }

            _pickedHints = _possibleMatchs[UnityEngine.Random.Range(0, _possibleMatchs.Count)];
            timers = new float[_pickedHints.Count];
        }

        void HintAnimation()
        {
            for (int i = 0; i < _pickedHints.Count; i++)
            {
                var pos = _pickedHints[i];

                if (_boardCells.TryGetValue(pos, out var cell) && cell.CanBeMoved)
                {
                    var worldPos = _grid.GetCellCenterWorld(pos);
                    var jumpCurve = GameSettings.Instance.VisualSettings.HintJumpCurve;
                    var scaleXCurve = GameSettings.Instance.VisualSettings.LiqidScaleXCurve;
                    var scaleYCurve = GameSettings.Instance.VisualSettings.LiqidScaleYCurve;

                    var candy = cell.ContainingCandy;

                    if (timers[i] < jumpCurve.keys[jumpCurve.length - 1].time)
                    {
                        timers[i] += Time.deltaTime * HintSpeed;

                        candy.transform.position = worldPos + Vector3.up * jumpCurve.Evaluate(timers[i]);
                        candy.transform.localScale =
                            new Vector3(scaleXCurve.Evaluate(timers[i]), scaleYCurve.Evaluate(timers[i]), 1);
                    }
                    else
                    {
                        candy.transform.position = worldPos;
                        candy.transform.localScale = Vector3.one;

                        timers[i] = 0;
                    }
                }

            }
        }

        void ReshuffleBoard()
        {
            List<Candy> candies = new();
            List<Vector3Int> listPos = new();
            foreach (var entry in _boardCells)
            {
                if (entry.Value.CanBeMoved)
                {
                    candies.Add(entry.Value.ContainingCandy);
                    listPos.Add(entry.Key);
                }
            }

            foreach (var pos in listPos)
            {
                int index = UnityEngine.Random.Range(0, candies.Count);
                var candy = candies[index];
                candy.transform.position = _grid.GetCellCenterWorld(pos);
                _boardCells[pos].ContainingCandy = candy;

                candies.RemoveAt(index);
            }

            // Make sure don't has a match.
            foreach (var pos in listPos)
            {
                _board.CheckNoMatch(pos);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }

}