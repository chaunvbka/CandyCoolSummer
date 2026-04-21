#pragma warning disable IDE0130 


namespace Texell.CandyCoolSummer
{
    using System.Collections;
    using System.Collections.Generic;
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Board
    {
        // Input data.
        public Tilemap TileBoard;
        public Candy[] CandiesForSpawn = new Candy[6];
        // ================

        public Dictionary<Vector3Int, Cell> BoardCells = new();

        private Dictionary<CandyType, Candy> _candyLookup = new();

        private readonly AssetManager _assetManager = AssetManager.Instance;
        private readonly PoolManager _poolManager = PoolManager.Instance;


        public IEnumerator Initialize()
        {
            TileBoard = GameObject.Find("TileBoard").GetComponent<Tilemap>();

            Debug.Log(" _poolManager.Done: " + _poolManager.Done);
            yield return new WaitUntil(() => _poolManager.Done == true);
            Debug.Log(" _poolManager.Done: " + _poolManager.Done);
            for (int i = 0; i < 6; i++)
            {
                int index = (int)CandyIndex.BLUE + i;
                CandiesForSpawn[i] = _assetManager.CandyPrefabs[index].GetComponent<Candy>();
            }

            //Fill a lookup of candy type to candy
            foreach (var candy in CandiesForSpawn)
            {
                _candyLookup.Add(candy.Type, candy);
            }

            // 1.Create board cells.
            // 2. Random spawn candy at first.
            foreach (var cellPos in TileBoard.cellBounds.allPositionsWithin)
            {
                if (TileBoard.HasTile(cellPos))
                {
                    BoardCells.Add(cellPos, new Cell());
                    SpawnCandyAt(cellPos, null);
                }
            }
        }

        Candy SpawnCandyAt(Vector3Int cellPos, Candy candyPrefab)
        {
            if (candyPrefab == null)
                candyPrefab = CandiesForSpawn[Random.Range(0, CandiesForSpawn.Length)];

            var go = candyPrefab.Instantiate();
            go.transform.position = CellToWorldPoint(cellPos);
            var candy = go.GetComponent<Candy>();

            BoardCells[cellPos].ContainingCandy = candy;
            return candy;
        }

        Vector3 CellToWorldPoint(Vector3Int cellPos)
        {
            return new Vector3(cellPos.x + TileBoard.tileAnchor.x, cellPos.y + TileBoard.tileAnchor.y, cellPos.z);
        }

        /// <summary>
        /// Generate a candy in every cell, making sure we don't have any match.
        /// </summary>
        void GenerateBoard()
        {

        }
    }
}
