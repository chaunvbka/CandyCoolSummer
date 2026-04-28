#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class MatchEngine : IDisposable, IBoardAction
    {
        // Function:
        // Find match, spawn candy, destroy candy, swap candy.

        private enum SwapStage
        {
            None,
            Forward,
            Return
        }

        private bool _disposed = false;

        private Tilemap _tileBoard;
        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;
        private List<Vector3Int> _spawnerPositions;
        private Candy[] _candyPrefabs;

        private Vector3Int _swapPosA;
        private Vector3Int _swapPosB;
        private SwapStage _swapStage = SwapStage.None;
        private readonly Queue<bool> _uniqueSwapQueue = new();

        private const float SWAP_SPEED = 10.0f;

        private List<Match> _FoundMatchs = new();
        private List<Vector3Int> _emptyCells = new();
        private List<Vector3Int> m_TickingCells = new();
        private List<Vector3Int> m_NewTickingCells = new();
        private List<Vector3Int> _falledCells = new();


        public void Init(Grid grid, Tilemap tilemap, Dictionary<Vector3Int, Cell> boardCells,
            List<Vector3Int> spawnerPositions, Candy[] candyPrefabs)
        {
            _grid = grid;
            _tileBoard = tilemap;
            _boardCells = boardCells;
            _spawnerPositions = spawnerPositions;
            _candyPrefabs = candyPrefabs;

        }

        bool FindMatch(Vector3Int cellPos)
        {
            bool matchFound = false;
            MatchType matchType = MatchType.LineThree;


            if (!_boardCells.TryGetValue(cellPos, out var cell) || cell.ContainingCandy == null)
            {
                return false;
            }

            BoundsInt bounds = _tileBoard.cellBounds;

            List<Vector3Int> matchPos = new();

            List<Vector3Int> horizontalLines = new()
            {
                cellPos
            };

            List<Vector3Int> verticalLines = new()
            {
                cellPos
            };

            List<Vector3Int> squareList = new()
            {
                cellPos
            };

            var current = cellPos;
            var left = current + Vector3Int.left;
            var right = current + Vector3Int.right;
            var up = current + Vector3Int.up;
            var down = current + Vector3Int.down;

            var leftdown = new Vector3Int(left.x, down.y, 0);
            var rightdown = new Vector3Int(right.x, down.y, 0);
            var leftup = new Vector3Int(left.x, up.y, 0);
            var rightup = new Vector3Int(right.x, up.y, 0);

            var hasLeft = _boardCells.TryGetValue(left, out var leftCell);
            var hasRight = _boardCells.TryGetValue(right, out var rightCell);
            var hasUp = _boardCells.TryGetValue(up, out var upCell);
            var hasDown = _boardCells.TryGetValue(down, out var downCell);
            var hasLeftDown = _boardCells.TryGetValue(leftdown, out var leftdownCell);
            var hasRightDown = _boardCells.TryGetValue(rightdown, out var rightdownCell);
            var hasLeftUp = _boardCells.TryGetValue(leftup, out var leftupCell);
            var hasRightUp = _boardCells.TryGetValue(rightup, out var rightupCell);

            for (int x = left.x; x >= bounds.xMin; x--)
            {
                var nextPos = new Vector3Int(x, current.y, 0);
                var hasNext = _boardCells.TryGetValue(nextPos, out var nextCell);

                if (hasNext && nextCell.ContainingCandy != null)
                {
                    if (cell.ContainingCandy.Type == nextCell.ContainingCandy.Type)
                    {
                        horizontalLines.Add(nextPos);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!hasNext || nextCell.ContainingCandy == null)
                {
                    break;
                }
            }

            for (int x = right.x; x <= bounds.xMax; x++)
            {
                var nextPos = new Vector3Int(x, current.y, 0);
                var hasNext = _boardCells.TryGetValue(nextPos, out var nextCell);

                if (hasNext && nextCell.ContainingCandy != null)
                {
                    if (cell.ContainingCandy.Type == nextCell.ContainingCandy.Type)
                    {
                        horizontalLines.Add(nextPos);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!hasNext || nextCell.ContainingCandy == null)
                {
                    break;
                }
            }

            for (int y = up.y; y <= bounds.yMax; y++)
            {
                var nextPos = new Vector3Int(current.x, y, 0);
                var hasNext = _boardCells.TryGetValue(nextPos, out var nextCell);

                if (hasNext && nextCell.ContainingCandy != null)
                {
                    if (cell.ContainingCandy.Type == nextCell.ContainingCandy.Type)
                    {
                        verticalLines.Add(nextPos);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!hasNext || nextCell.ContainingCandy == null)
                {
                    break;
                }
            }

            for (int y = down.y; y >= bounds.yMin; y--)
            {
                var nextPos = new Vector3Int(current.x, y, 0);
                var hasNext = _boardCells.TryGetValue(nextPos, out var nextCell);

                if (hasNext && nextCell.ContainingCandy != null)
                {
                    if (cell.ContainingCandy.Type == nextCell.ContainingCandy.Type)
                    {
                        verticalLines.Add(nextPos);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!hasNext || nextCell.ContainingCandy == null)
                {
                    break;
                }
            }

            if (hasLeft && leftCell.ContainingCandy != null &&
                hasDown && downCell.ContainingCandy != null &&
                hasLeftDown && leftdownCell.ContainingCandy != null &&
                cell.ContainingCandy.Type == leftCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == downCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == leftdownCell.ContainingCandy.Type)
            {
                squareList.Add(left);
                squareList.Add(down);
                squareList.Add(leftdown);
            }
            else if (hasRight && rightCell.ContainingCandy != null &&
                hasDown && downCell.ContainingCandy != null &&
                hasRightDown && rightdownCell.ContainingCandy != null &&
                cell.ContainingCandy.Type == rightCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == downCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == rightdownCell.ContainingCandy.Type)
            {
                squareList.Add(right);
                squareList.Add(down);
                squareList.Add(rightdown);
            }
            else if (hasLeft && leftCell.ContainingCandy != null &&
                hasUp && upCell.ContainingCandy != null &&
                hasLeftUp && leftupCell.ContainingCandy != null &&
                cell.ContainingCandy.Type == leftCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == upCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == leftupCell.ContainingCandy.Type)
            {
                squareList.Add(left);
                squareList.Add(up);
                squareList.Add(leftup);
            }
            else if (hasRight && rightCell.ContainingCandy != null &&
                hasUp && upCell.ContainingCandy != null &&
                hasRightUp && rightupCell.ContainingCandy != null &&
                cell.ContainingCandy.Type == rightCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == upCell.ContainingCandy.Type &&
                cell.ContainingCandy.Type == rightupCell.ContainingCandy.Type)
            {
                squareList.Add(right);
                squareList.Add(up);
                squareList.Add(rightup);
            }

            if (horizontalLines.Count <= 2 && verticalLines.Count <= 2 && squareList.Count == 4)
            {
                Debug.Log("Square shape match found!");
                matchFound = true;
                matchType = MatchType.SquareShape;

                foreach (var pos in squareList)
                {
                    if (!matchPos.Contains(pos))
                        matchPos.Add(pos);
                }
            }
            else if (horizontalLines.Count == 3 && verticalLines.Count <= 2)
            {
                matchFound = true;

                if (squareList.Count == 4)
                {
                    Debug.Log("Square shape match found!");
                    matchType = MatchType.SquareShape;

                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    Debug.Log("LineThree horizontal shape match found!");
                    matchType = MatchType.LineThree;

                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }
            else if (verticalLines.Count == 3 && horizontalLines.Count <= 2)
            {
                matchFound = true;

                if (squareList.Count == 4)
                {
                    Debug.Log("Square shape match found!");
                    matchType = MatchType.SquareShape;

                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    Debug.Log("LineThree vertical shape match found!");
                    matchType = MatchType.LineThree;

                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }
            else if (horizontalLines.Count == 4 && verticalLines.Count <= 2)
            {
                Debug.Log("LineFour horizontal shape match found!");
                matchFound = true;
                matchType = MatchType.LineFour;

                if (squareList.Count == 4)
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }

            }
            else if (verticalLines.Count == 4 && horizontalLines.Count <= 2)
            {
                Debug.Log("LineFour vertical shape match found!");
                matchFound = true;
                matchType = MatchType.LineFour;

                if (squareList.Count == 4)
                {
                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }
            else if (horizontalLines.Count == 5)
            {
                Debug.Log("LineFive horizontal shape match found!");
                matchFound = true;
                matchType = MatchType.LineFive;

                if (squareList.Count == 4)
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }
            else if (verticalLines.Count == 5)
            {
                Debug.Log("LineFive vertical shape match found!");
                matchFound = true;
                matchType = MatchType.LineFive;

                if (squareList.Count == 4)
                {
                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }
            else if (horizontalLines.Count >= 3 && verticalLines.Count >= 3 &&
                    horizontalLines.Count < 5 && verticalLines.Count < 5)
            {
                Debug.Log("TL shape match found!");
                matchFound = true;
                matchType = MatchType.TLShape;

                if (squareList.Count == 4)
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in squareList)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
                else
                {
                    foreach (var pos in horizontalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }

                    foreach (var pos in verticalLines)
                    {
                        if (!matchPos.Contains(pos))
                            matchPos.Add(pos);
                    }
                }
            }

            if (matchFound)
            {
                Match match = new()
                {
                    Type = matchType,
                    CombinedPoint = cellPos

                };

                foreach (var pos in matchPos)
                {
                    match.MatchingCandy.Add(pos);
                }

                _FoundMatchs.Add(match);
            }

            return matchFound;
        }

        public void OnUpdate()
        {
            SwapCandy();

            if (_FoundMatchs.Count > 0)
            {
                DeleteMatch();
            }

            if (_emptyCells.Count > 0)
            {
                EmptyCheck();
            }

            if (m_NewTickingCells.Count > 0)
            {
                m_TickingCells.AddRange(m_NewTickingCells);
                m_NewTickingCells.Clear();
                //incrementHintTimer = false;
            }

            if (m_TickingCells.Count > 0)
            {
                MoveCandies();
            }

            if (_falledCells.Count > 0)
            {
                DoMatchCheck();
            }
        }

        public void OnSwapAction(Vector3Int cellPosA, Vector3Int cellPosB)
        {
            _swapPosA = cellPosA;
            _swapPosB = cellPosB;

            // Only a swap happen at a time.
            if (_uniqueSwapQueue.Count == 0 && _FoundMatchs.Count == 0 && 
                m_TickingCells.Count == 0)
            {
                _uniqueSwapQueue.Enqueue(true);
                _swapStage = SwapStage.Forward;
            }
        }

        void SwapCandy()
        {
            if (_uniqueSwapQueue.Count == 1)
            {
                if (_swapStage == SwapStage.Forward)
                {
                    var hasA = _boardCells.TryGetValue(_swapPosA, out var cellA);
                    var hasB = _boardCells.TryGetValue(_swapPosB, out var cellB);

                    if (!hasA || cellA.ContainingCandy == null || !hasB || cellB.ContainingCandy == null)
                    {
                        return;
                    }

                    var candyA = cellA.ContainingCandy;
                    var candyB = cellB.ContainingCandy;

                    var worldPosA = _grid.GetCellCenterWorld(_swapPosA);
                    var worldPosB = _grid.GetCellCenterWorld(_swapPosB);

                    candyA.transform.position = Vector3.MoveTowards(candyA.transform.position, worldPosB, Time.deltaTime * SWAP_SPEED);
                    candyB.transform.position = Vector3.MoveTowards(candyB.transform.position, worldPosA, Time.deltaTime * SWAP_SPEED);

                    // Check if the position of the candyA and candyB are approximately equal.
                    if (Vector3.Distance(candyA.transform.position, worldPosB) < 0.001f &&
                        Vector3.Distance(candyB.transform.position, worldPosA) < 0.001f)
                    {
                        candyA.transform.position = worldPosB;
                        candyB.transform.position = worldPosA;

                        cellA.ContainingCandy = candyB;
                        cellB.ContainingCandy = candyA;

                        bool foundMatchA = FindMatch(_swapPosA);
                        bool foundMatchB = FindMatch(_swapPosB);

                        if (!foundMatchA && !foundMatchB)
                        {
                            _swapStage = SwapStage.Return;
                        }
                        else if (foundMatchA || foundMatchB)
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
                    var hasA = _boardCells.TryGetValue(_swapPosA, out var cellA);
                    var hasB = _boardCells.TryGetValue(_swapPosB, out var cellB);

                    if (!hasA || cellA.ContainingCandy == null || !hasB || cellB.ContainingCandy == null)
                    {
                        return;
                    }

                    var candyA = cellA.ContainingCandy;
                    var candyB = cellB.ContainingCandy;

                    var worldPosA = _grid.GetCellCenterWorld(_swapPosA);
                    var worldPosB = _grid.GetCellCenterWorld(_swapPosB);

                    candyA.transform.position = Vector3.MoveTowards(candyA.transform.position, worldPosB, Time.deltaTime * SWAP_SPEED);
                    candyB.transform.position = Vector3.MoveTowards(candyB.transform.position, worldPosA, Time.deltaTime * SWAP_SPEED);

                    if (Vector3.Distance(candyA.transform.position, worldPosB) < 0.001f &&
                        Vector3.Distance(candyB.transform.position, worldPosA) < 0.001f)
                    {
                        candyA.transform.position = worldPosB;
                        candyB.transform.position = worldPosA;

                        cellA.ContainingCandy = candyB;
                        cellB.ContainingCandy = candyA;

                        // Finish swap
                        _swapStage = SwapStage.None;
                        _uniqueSwapQueue.Dequeue();
                    }
                }
            }
        }

        void DeleteMatch()
        {
            for (int i = 0; i < _FoundMatchs.Count; ++i)
            {
                var match = _FoundMatchs[i];

                foreach (var cellPos in match.MatchingCandy)
                {
                    if (_boardCells[cellPos].ContainingCandy == null)
                    {
                        continue;
                    }
                    CandyFactory.Destroy(_boardCells[cellPos].ContainingCandy);
                    _boardCells[cellPos].ContainingCandy = null;

                    _emptyCells.Add(cellPos);
                }

                _FoundMatchs.RemoveAt(i);
                i--;
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
                    m_NewTickingCells.Add(emptyCellPos);

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
                    m_NewTickingCells.Add(emptyCellPos);

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
                    m_NewTickingCells.Add(emptyCellPos);

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

            _boardCells[cellPos].IncomingCandy = incomingCandy;

            incomingCandy.StartMoveTimer();
            incomingCandy.SpeedMultiplier = 1.0f;

            m_NewTickingCells.Add(cellPos);

            // This cell is not empty anymore.
            _emptyCells.Remove(cellPos);
        }

        void MoveCandies()
        {
            //sort bottom left to top right, so we minimize timing issue (a gem on top try to fall into a cell that is 
            //not yet empty but will be empty once the bottom gem move away)
            m_TickingCells.Sort((a, b) =>
            {
                int yCmp = a.y.CompareTo(b.y);
                if (yCmp == 0)
                {
                    return a.x.CompareTo(b.x);
                }

                return yCmp;
            });

            for (int i = 0; i < m_TickingCells.Count; i++)
            {
                var cellPos = m_TickingCells[i];

                var currentCell = _boardCells[cellPos];
                var targetPosition = _grid.GetCellCenterWorld(cellPos);

                if (currentCell.IncomingCandy != null && currentCell.ContainingCandy != null)
                {
                    Debug.LogError(
                        $"A ticking cell at {cellPos} have incoming gems {currentCell.IncomingCandy} containing gem {currentCell.ContainingCandy}");
                    continue;
                }

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

                        m_TickingCells.RemoveAt(i);
                        i--;

                        currentCell.IncomingCandy = null;
                        currentCell.ContainingCandy = candy;
                        //TODO:
                        //gem.MoveTo(cellIdx);

                        // Reached target position, now check if continue falling or finished its fall.
                        if (_emptyCells.Contains(cellPos + Vector3Int.down) &&
                            _boardCells.TryGetValue(cellPos + Vector3Int.down, out var belowCell))
                        {
                            currentCell.ContainingCandy = null;
                            belowCell.IncomingCandy = candy;

                            candy.SpeedMultiplier = 1.0f;

                            var target = cellPos + Vector3Int.down;
                            m_NewTickingCells.Add(target);

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
                            m_NewTickingCells.Add(target);

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
                            m_NewTickingCells.Add(target);

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
                            m_NewTickingCells.Add(cellPos);
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

                        m_TickingCells.RemoveAt(i);
                        i--;

                        //TODO:
                        // m_CellToMatchCheck.Add(cellIdx);
                        _falledCells.Add(cellPos);
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
                    m_TickingCells.RemoveAt(i);
                    i--;
                }
            }
        }

        void DoMatchCheck()
        {
            foreach (var cellPos in _falledCells)
            {
                FindMatch(cellPos);
            }

            _falledCells.Clear();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            //s_Instance = null;
        }
    }

}