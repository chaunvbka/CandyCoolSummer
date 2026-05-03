#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using System.Collections.Generic;
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class BoardInput : IDisposable
    {
        // Function: 
        // Determine input action from user.

        static BoardInput s_Instance = null;
        private bool _disposed = false;

        private bool _blockInput;
        private bool _previousClick;
        private Vector3Int _previousCellPosition;
        private Vector3? _startScreenPosition;
        private Tilemap _tileBoard;
        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;
        private GameObject _selectIcon = null;

        private IBoardAction _boardAction;
        private IHintAction _hintAction;

        private InputManager _inputManager = InputManager.Instance;
        private readonly AssetManager _assetManager = AssetManager.Instance;

        public BoardInput()
        {
            if (s_Instance != null)
            {
                Debug.LogError("BoardInput instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Init(Board board, IBoardAction boardAction, IHintAction hintAction)
        {
            _grid = board.Grid;
            _tileBoard = board.TileBoard;
            _boardCells = board.BoardCells;
            _boardAction = boardAction;
            _hintAction = hintAction;

            _selectIcon = _assetManager.Select;
        }

        public void OnUpdate()
        {
            if (_blockInput)
            {
                return;
            }

            var pressedThisFrame = _inputManager.ClickAction.WasPressedThisFrame();
            var releasedThisFrame = _inputManager.ClickAction.WasReleasedThisFrame();

            var clickPos = _inputManager.ClickPosition.ReadValue<Vector2>();
            var worldPos = Camera.main.ScreenToWorldPoint(clickPos);
            var cellPos = _grid.WorldToCell(worldPos);

            if (pressedThisFrame)
            {
                if (_boardCells.TryGetValue(cellPos, out var cell) && cell.CanBeMoved)
                {
                   _hintAction.StopHint();
                }

                _startScreenPosition = clickPos;

                //After press, show selection icon.
                if (_tileBoard.HasTile(cellPos))
                {
                    _selectIcon.SetActive(true);
                    _selectIcon.transform.position = _grid.GetCellCenterWorld(cellPos);
                }
                else
                {
                    _selectIcon.SetActive(false);
                }

            }
            
            if (releasedThisFrame)
            {
                if (_startScreenPosition == null)
                {
                    return;
                }

                _hintAction.StartHint();

                var startWorldPos = Camera.main.ScreenToWorldPoint(_startScreenPosition.Value);
                var endWorldPos = Camera.main.ScreenToWorldPoint(clickPos);

                // We compute the swipe in world position as then a swipe of 1 is the distance
                // between 2 cell.
                var swipe = endWorldPos - startWorldPos;
                if (swipe.sqrMagnitude < 0.5f * 0.5f)
                {
                    // Click action
                    var currentPosition = cellPos;
                    if (!_previousClick)
                    {
                        // This was select action.(currentPosition)
                        _previousClick = true;
                        _previousCellPosition = cellPos;
                    }
                    else
                    {
                        var leftAdjacent = currentPosition + Vector3Int.left;
                        var rightAdjacent = currentPosition + Vector3Int.right;
                        var upAdjacent = currentPosition + Vector3Int.up;
                        var downAdjacent = currentPosition + Vector3Int.down;
                        if (_previousCellPosition == leftAdjacent || _previousCellPosition == rightAdjacent ||
                            _previousCellPosition == upAdjacent || _previousCellPosition == downAdjacent)
                        {
                            // This was swap action.(_previousCellPosition, currentPosition)
                            // After swap reset previous click state to false, hide selection icon.

                            // The previous cell isn't a valid cell, so we exit
                            if (!_boardCells.TryGetValue(_previousCellPosition, out var previouCell)
                                || !previouCell.CanBeMoved)
                            {
                                return;
                            }

                            _boardAction?.OnSwapAction(_previousCellPosition, currentPosition);

                            _previousClick = false;
                            _selectIcon.SetActive(false);
                        }
                        else
                        {
                            // This was select action. (currentPosition)
                            _previousClick = true;
                            _previousCellPosition = cellPos;
                        }
                    }

                }
                else
                {
                    // Swipe action.

                    var startCellPos = _grid.WorldToCell(startWorldPos);

                    // The starting cell isn't a valid cell, so we exit
                    if (!_boardCells.TryGetValue(startCellPos, out var startCell)
                        || !startCell.CanBeMoved)
                    {
                        return;
                    }

                    var endCellPos = startCellPos;

                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                    {
                        if (swipe.x < 0)
                        {
                            endCellPos += Vector3Int.left;
                        }
                        else
                        {
                            endCellPos += Vector3Int.right;
                        }
                    }
                    else
                    {
                        if (swipe.y > 0)
                        {
                            endCellPos += Vector3Int.up;
                        }
                        else
                        {
                            endCellPos += Vector3Int.down;
                        }
                    }

                    // The end cell isn't a valid cell, so we exit
                    if (!_boardCells.TryGetValue(endCellPos, out var endCell)
                        || !endCell.CanBeMoved)
                    {
                        return;
                    }

                    // This was swap action.(startCellPos, endCellPos)  
                    _boardAction?.OnSwapAction(startCellPos, endCellPos);

                    // After swap, hide selection icon.
                    _selectIcon.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Block user input when in deleting match, candy in falling process.
        /// </summary>
        public static void BlockInput()
        {
            s_Instance._blockInput = true;
            s_Instance._startScreenPosition = null;
        }

        /// <summary>
        /// Unblock user input when deleting match, candy in falling process finish.
        /// </summary>
        public static void UnblockInput()
        {
            s_Instance._blockInput = false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_Instance = null;
        }
    }
}