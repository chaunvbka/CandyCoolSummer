#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CandyFalling : IDisposable
    {
        private bool _disposed = false;

        private Grid _grid;
        private Candy[] _candyPrefabs;
        private List<Vector3Int> _spawnerPositions;
        private Dictionary<Vector3Int, Cell> _boardCells;

        private List<Vector3Int> _emptyCells;
        private MatchFinding _matchFinding;
        private HintIndicator _hintIdicator;

        private List<Vector3Int> _tickingCells = new();
        private List<Vector3Int> _fallingCells = new();
        private List<Vector3Int> _fallingCellsToFindMatch = new();

        public void Init(Board board, MatchDeleting deleting, MatchFinding finding, HintIndicator hint)
        {
            _grid = board.Grid;
            _candyPrefabs = board.CandyPrefabs;
            _spawnerPositions = board.SpawnerPositions;
            _boardCells = board.BoardCells;

            _emptyCells = deleting.EmptyCells;
            _matchFinding = finding;
            _hintIdicator = hint;
        }

        public void OnUpdate()
        {
            if (_emptyCells.Count > 0)
            {
               _hintIdicator.PickedHints.Clear();

                EmptyCheck();
            }

            if (_tickingCells.Count > 0)
            {
                _fallingCells.AddRange(_tickingCells);
                _tickingCells.Clear();
            }

            if (_fallingCells.Count > 0)
            {
                MoveCandies();
            }

            if (_fallingCells.Count == 0 && _tickingCells.Count == 0)
            {
                if (_fallingCellsToFindMatch.Count > 0)
                {
                    FindMatchFalling();
                }
                else
                {
                    if (_hintIdicator.PickedHints.Count == 0)
                        _hintIdicator.FindHintCells();
                }
            }

        }

        void MoveCandies()
        {
            //sort bottom left to top right, so we minimize timing issue (a gem on top try to fall into a cell that is 
            //not yet empty but will be empty once the bottom gem move away)
            _fallingCells.Sort((a, b) =>
            {
                int yCmp = a.y.CompareTo(b.y);
                if (yCmp == 0)
                {
                    return a.x.CompareTo(b.x);
                }

                return yCmp;
            });

            for (int i = 0; i < _fallingCells.Count; i++)
            {
                var cellPos = _fallingCells[i];

                var currentCell = _boardCells[cellPos];
                var targetPosition = _grid.GetCellCenterWorld(cellPos);

                if (currentCell.IncomingCandy != null && currentCell.ContainingCandy != null)
                {
                    Debug.LogError(
                        $"A ticking cell at {cellPos} have incoming gems {currentCell.IncomingCandy} containing gem {currentCell.ContainingCandy}");
                    continue;
                }

                // When candy fall, disable hint.
                _hintIdicator.StopHint();

                //update either position or state.
                if (currentCell.IncomingCandy != null && currentCell.IncomingCandy.CurrentState == Candy.State.Falling)
                {
                    var candy = currentCell.IncomingCandy;

                    candy.TickMoveTimer(Time.deltaTime);

                    var maxDistance = GameSettings.Instance.VisualSettings.FallAccelerationCurve.Evaluate(candy.FallTime) *
                                      Time.deltaTime * GameSettings.Instance.VisualSettings.FallSpeed * candy.SpeedMultiplier;

                    candy.transform.position = Vector3.MoveTowards(candy.transform.position, targetPosition,
                        maxDistance);

                    if (Vector3.Distance(candy.transform.position, targetPosition) < 0.001f)
                    {
                        candy.transform.position = targetPosition;

                        _fallingCells.RemoveAt(i);
                        i--;

                        currentCell.IncomingCandy = null;
                        currentCell.ContainingCandy = candy;
                        candy.MoveTo(cellPos);

                        // Reached target position, now check if continue falling or finished its fall.
                        if (_emptyCells.Contains(cellPos + Vector3Int.down) &&
                            _boardCells.TryGetValue(cellPos + Vector3Int.down, out var belowCell))
                        {
                            currentCell.ContainingCandy = null;
                            belowCell.IncomingCandy = candy;

                            candy.SpeedMultiplier = 1.0f;

                            var target = cellPos + Vector3Int.down;
                            _tickingCells.Add(target);

                            _emptyCells.Remove(target);
                            _emptyCells.Add(cellPos);

                            //if we continue falling, this is now an empty space, if there is a gem above it will fall by itself
                            //but if this is a spawner above, we need to spawn a new gem
                            if (_spawnerPositions.Contains(cellPos + Vector3Int.up))
                            {
                                ActivateSpawnerAt(cellPos);
                            }
                        }
                        else if ((!_boardCells.TryGetValue(cellPos + Vector3Int.left, out var leftCell) || leftCell.BlockFall) &&
                            _emptyCells.Contains(cellPos + Vector3Int.down + Vector3Int.left) &&
                            _boardCells.TryGetValue(cellPos + Vector3Int.down + Vector3Int.left, out var belowLeftCell))
                        {
                            // The cell to the left is either non existing or locked, and below that is an empty space, we can fall diagonally.
                            currentCell.ContainingCandy = null;
                            belowLeftCell.IncomingCandy = candy;

                            candy.SpeedMultiplier = 1.41421356237f;


                            var target = cellPos + Vector3Int.down + Vector3Int.left;
                            _tickingCells.Add(target);

                            _emptyCells.Remove(target);
                            _emptyCells.Add(cellPos);

                            //if we continue falling, this is now an empty space, if there is a gem above it will fall by itself
                            //but if this is a spawner above, we need to spawn a new gem
                            if (_spawnerPositions.Contains(cellPos + Vector3Int.up))
                            {
                                ActivateSpawnerAt(cellPos);
                            }
                        }
                        else if ((!_boardCells.TryGetValue(cellPos + Vector3Int.right, out var rightCell) || rightCell.BlockFall) &&
                            _emptyCells.Contains(cellPos + Vector3Int.down + Vector3Int.right) &&
                            _boardCells.TryGetValue(cellPos + Vector3Int.down + Vector3Int.right, out var belowRightCell))
                        {
                            currentCell.ContainingCandy = null;
                            belowRightCell.IncomingCandy = candy;

                            candy.SpeedMultiplier = 1.41421356237f;

                            var target = cellPos + Vector3Int.down + Vector3Int.right;
                            _tickingCells.Add(target);

                            _emptyCells.Remove(target);
                            _emptyCells.Add(cellPos);

                            //if we continue falling, this is now an empty space, if there is a gem above it will fall by itself
                            //but if this is a spawner above, we need to spawn a new gem
                            if (_spawnerPositions.Contains(cellPos + Vector3Int.up))
                            {
                                ActivateSpawnerAt(cellPos);
                            }
                        }
                        else
                        {
                            //re add but this time we will bounce and not fall.
                            _tickingCells.Add(cellPos);
                            candy.StopFalling();
                        }
                    }
                }
                else if (currentCell.ContainingCandy != null && currentCell.ContainingCandy.CurrentState == Candy.State.Bouncing)
                {
                    var candy = currentCell.ContainingCandy;
                    candy.TickMoveTimer(Time.deltaTime);
                    Vector3 center = _grid.GetCellCenterWorld(cellPos);

                    float maxTime = GameSettings.Instance.VisualSettings.BounceCurve
                        .keys[GameSettings.Instance.VisualSettings.BounceCurve.length - 1].time;

                    if (candy.FallTime >= maxTime)
                    {
                        candy.transform.position = center;
                        candy.transform.localScale = Vector3.one;
                        candy.StopBouncing();

                        _fallingCells.RemoveAt(i);
                        i--;

                        if (!_fallingCellsToFindMatch.Contains(cellPos))
                            _fallingCellsToFindMatch.Add(cellPos);
                    }
                    else
                    {
                        candy.transform.position =
                            center + Vector3.up * GameSettings.Instance.VisualSettings.BounceCurve.Evaluate(candy.FallTime);
                        candy.transform.localScale =
                            new Vector3(1, GameSettings.Instance.VisualSettings.SquishCurve.Evaluate(candy.FallTime), 1);
                    }
                }
                else if (currentCell.ContainingCandy != null && currentCell.ContainingCandy.CurrentState == Candy.State.Idle)
                {
                    //a ticking cells should only be falling or bouncing, if neither of those, remove it 
                    _fallingCells.RemoveAt(i);
                    i--;
                    
                    if (!_fallingCellsToFindMatch.Contains(cellPos))
                        _fallingCellsToFindMatch.Add(cellPos);
                }
            }
        }

        /// <summary>
        /// Find match on cells just finish falling.
        /// </summary>
        void FindMatchFalling()
        {
            bool matchFound = false;
            foreach (var cellPos in _fallingCellsToFindMatch)
            {
                // Find match with no direction.
                matchFound = _matchFinding.FindMatch(cellPos, Vector3Int.zero);
            }

            _fallingCellsToFindMatch.Clear();
            if (!matchFound)
            {
                BoardInput.UnblockInput();
                _hintIdicator.StartHint();
            }
        }

        void EmptyCheck()
        {
            for (int i = 0; i < _emptyCells.Count; ++i)
            {
                var emptyCellPos = _emptyCells[i];

                if (!_boardCells[emptyCellPos].IsEmpty())
                {
                    _emptyCells.RemoveAt(i);
                    i--;
                    continue;
                }

                var aboveCellPos = emptyCellPos + Vector3Int.up;
                bool hasAboveCell = _boardCells.TryGetValue(aboveCellPos, out var aboveCell);

                // If we have a candy above an empty cell, make that candy fall.
                if (hasAboveCell && aboveCell.ContainingCandy != null && aboveCell.CanFall)
                {
                    var incomingCandy = aboveCell.ContainingCandy;
                    _boardCells[emptyCellPos].IncomingCandy = incomingCandy;
                    aboveCell.ContainingCandy = null;

                    incomingCandy.StartMoveTimer();
                    incomingCandy.SpeedMultiplier = 1.0f;

                    // Add that empty cell to be ticked so the candy goes down into it.
                    _tickingCells.Add(emptyCellPos);

                    // The above cell is now empty and this cell is not empty anymore.
                    _emptyCells.Add(aboveCellPos);
                    _emptyCells.Remove(emptyCellPos);
                }
                else if ((!hasAboveCell || aboveCell.BlockFall) &&
                    _boardCells.TryGetValue(aboveCellPos + Vector3Int.right, out var aboveRightCell) &&
                    aboveRightCell.ContainingCandy != null && aboveRightCell.CanFall)
                {
                    var incomingCandy = aboveRightCell.ContainingCandy;
                    _boardCells[emptyCellPos].IncomingCandy = incomingCandy;
                    aboveRightCell.ContainingCandy = null;

                    incomingCandy.StartMoveTimer();
                    incomingCandy.SpeedMultiplier = 1.41421356237f;

                    // Add that empty cell to be ticked so the candy goes down into it.
                    _tickingCells.Add(emptyCellPos);

                    // The above cell is now empty and this cell is not empty anymore.
                    _emptyCells.Add(aboveCellPos + Vector3Int.right);
                    _emptyCells.Remove(emptyCellPos);
                }
                else if ((!hasAboveCell || aboveCell.BlockFall) &&
                    _boardCells.TryGetValue(aboveCellPos + Vector3Int.left, out var aboveLeftCell) &&
                    aboveLeftCell.ContainingCandy != null && aboveLeftCell.CanFall)
                {
                    var incomingCandy = aboveLeftCell.ContainingCandy;
                    _boardCells[emptyCellPos].IncomingCandy = incomingCandy;
                    aboveLeftCell.ContainingCandy = null;

                    incomingCandy.StartMoveTimer();
                    incomingCandy.SpeedMultiplier = 1.41421356237f;

                    // Add that empty cell to be ticked so the candy goes down into it.
                    _tickingCells.Add(emptyCellPos);

                    // The above cell is now empty and this cell is not empty anymore.
                    _emptyCells.Add(aboveCellPos + Vector3Int.left);
                    _emptyCells.Remove(emptyCellPos);

                }
                else if (_spawnerPositions.Contains(aboveCellPos))
                {
                    ActivateSpawnerAt(emptyCellPos);
                    Debug.Log("ActivateSpawnerAt: aboveCellPos.");
                }

            }
        }

        void ActivateSpawnerAt(Vector3Int cellPos)
        {
            var candyPrefab = _candyPrefabs[UnityEngine.Random.Range(0, _candyPrefabs.Length)];
            var incomingCandy = CandyFactory.Create(candyPrefab);
            incomingCandy.transform.position = _grid.GetCellCenterWorld(cellPos + Vector3Int.up);
            incomingCandy.Init(cellPos);

            _boardCells[cellPos].IncomingCandy = incomingCandy;

            incomingCandy.StartMoveTimer();
            incomingCandy.SpeedMultiplier = 1.0f;

            _tickingCells.Add(cellPos);

            // This cell is not empty anymore.
            _emptyCells.Remove(cellPos);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }

}