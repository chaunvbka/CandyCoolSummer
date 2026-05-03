#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MatchDeleting : IDisposable
    {
        private bool _disposed = false;

        private Grid _grid;
        private Dictionary<Vector3Int, Cell> _boardCells;
        private List<Match> _tickingMatch;
        private readonly List<Vector3Int> _emptyCells = new();
        private IHintAction _hintAction;

        public List<Vector3Int> EmptyCells => _emptyCells;

        public void Init(Board board, MatchFinding matchFinding, IHintAction hintAction)
        {
            _grid = board.Grid;
            _boardCells = board.BoardCells;
            _tickingMatch = matchFinding.TickingMatch;
            _hintAction = hintAction;
        }

        public void OnUpdate()
        {
            if (_tickingMatch.Count > 0)
            {
                _hintAction.StopHint();
                BoardInput.BlockInput();
                DeleteMatchs();
            }

        }

        void DeleteMatchs()
        {
            for (int i = 0; i < _tickingMatch.Count; ++i)
            {
                var match = _tickingMatch[i];

                const float DeletionSpeed = 1.0f / 0.3f;
                match.DeletionTimer += Time.deltaTime * DeletionSpeed;

                for (int j = 0; j < match.MatchingCells.Count; j++)
                {
                    var cellPos = match.MatchingCells[j];
                    var candy = _boardCells[cellPos].ContainingCandy;

                    if (candy == null)
                    {
                        match.MatchingCells.RemoveAt(j);
                        j--;
                        continue;
                    }

                    if (candy.CurrentState == Candy.State.Bouncing)
                    {
                        candy.transform.position = _grid.GetCellCenterWorld(cellPos);
                        candy.transform.localScale = Vector3.one;
                        candy.StopBouncing();
                    }

                    //forced deletion doesn't wait for end of timer
                    if (match.ForcedDeletion || match.DeletionTimer > 1.0f)
                    {
                        candy.CurrentMatch = null;
                        CandyFactory.Destroy(candy);

                        _boardCells[cellPos].ContainingCandy = null;

                        if (match.ForcedDeletion && _boardCells[cellPos].Obstacle != null)
                        {
                            _boardCells[cellPos].Obstacle.Clear();
                        }

                        // TODO:
                        //callback are only called when this was a match from swipe and not from bonus or other source 
                        // if (!match.ForcedDeletion && m_CellsCallbacks.TryGetValue(gemIdx, out var clbk))
                        // {
                        //     clbk.Invoke();
                        // }


                        match.MatchingCells.RemoveAt(j);
                        j--;

                        // TODO:
                        // match.DeletedCount += 1;
                        // //we only spawn coins for non bonus match
                        // if (match.DeletedCount >= 4 && !match.ForcedDeletion)
                        // {
                        //     GameManager.Instance.ChangeCoins(1);
                        //     GameManager.Instance.PoolSystem.PlayInstanceAt(GameManager.Instance.Settings.VisualSettings.CoinVFX,
                        //         gem.transform.position);
                        // }

                        if (match.CombinedPrefab != null && match.CombinedPoint == cellPos)
                        {
                            Debug.Log("Spawned CombinedCandy");
                            SpawnCombinedCandy(match.CombinedPoint, match.CombinedPrefab);
                        }
                        else
                        {
                            _emptyCells.Add(cellPos);
                        }
                    }
                    else if (candy.CurrentState != Candy.State.Disappearing)
                    {
                        // LevelData.Instance.Matched(gem);

                        // foreach (var matchEffectPrefab in gem.MatchEffectPrefabs)
                        // {
                        //     GameManager.Instance.PoolSystem.PlayInstanceAt(matchEffectPrefab, m_Grid.GetCellCenterWorld(gem.CurrentIndex));
                        // }

                        // gem.gameObject.SetActive(false);

                        candy.Destroyed();
                        //Debug.Log("candy Disappearing 2");
                    }
                }

                if (match.MatchingCells.Count == 0)
                {
                    _tickingMatch.RemoveAt(i);
                    i--;
                }
            }

            Debug.Log("_tickingMatch.Count = " + _tickingMatch.Count);
        }

        void SpawnCombinedCandy(Vector3Int cellPos, Candy prefab)
        {
            var candy = CandyFactory.Create(prefab);
            candy.transform.position = _grid.GetCellCenterLocal(cellPos);
            candy.Init(cellPos);
            _boardCells[cellPos].ContainingCandy = candy;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}