#pragma warning disable IDE0130 


namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Board : MonoBehaviour
    {
        // Board class do:
        // 1. Instantiate, destroy and store board cells.
        // 2. Instantiate, destroy and store candies.
        // 3. Instantiate, destroy and store obstacles.

        // Input data.
        [SerializeField]
        private Tilemap _tileBoard;
        private Candy[] _candiesForSpawn = new Candy[6];
        // ================

        /// <summary>
        /// Stores board cells with dictionary (key: cell position, value: cell object).
        /// </summary>
        private Dictionary<Vector3Int, Cell> _boardCells = new();
        /// <summary>
        /// Stores candies with dictionary (key: cell position, value: candy object).
        /// </summary>
        private Dictionary<Vector3Int, Candy> _candies = new();
        /// <summary>
        /// Stores obstacles with dictionary (key: cell position, value: obstacle object).
        /// </summary>
        private Dictionary<Vector3Int, Obstacle> _obstacles = new();

        private Dictionary<CandyType, Candy> _candyLookup = new();

        private readonly AssetManager _assetManager = AssetManager.Instance;
        private readonly PoolManager _poolManager = PoolManager.Instance;

        public void Initialize()
        {
            for (int i = 0; i < 6; i++)
            {
                int index = (int)CandyIndex.BLUE + i;
                _candiesForSpawn[i] = _assetManager.CandyPrefabs[index].GetComponent<Candy>();
            }

            //Fill a lookup of candy type to candy
            foreach (var candy in _candiesForSpawn)
            {
                _candyLookup.Add(candy.Type, candy);
            }

            // Instantiate board cells.
            // Spawn candy randomly at first.
            foreach (var cellPos in _tileBoard.cellBounds.allPositionsWithin)
            {
                if (_tileBoard.HasTile(cellPos))
                {
                    _boardCells.Add(cellPos, new Cell());
                    SpawnCandyAt(cellPos, null);
                }
            }
        }

        public GameObject Instantiate(Candy candyPrefab)
        {
            GameObject go = null;
            var candyType = candyPrefab.Type;

            switch (candyType)
            {
                case CandyType.V_BLUE:
                    go = _poolManager.V_BlueCandyPool.Get();
                    break;
                case CandyType.V_YELLOW:
                    go = _poolManager.V_YellowCandyPool.Get();
                    break;
                case CandyType.V_RED:
                    go = _poolManager.V_RedCandyPool.Get();
                    break;
                case CandyType.V_GREEN:
                    go = _poolManager.V_GreenCandyPool.Get();
                    break;
                case CandyType.V_PURPLE:
                    go = _poolManager.V_PurpleCandyPool.Get();
                    break;
                case CandyType.V_PINK:
                    go = _poolManager.V_PinkCandyPool.Get();
                    break;

                case CandyType.CHOCOLATE_MILK:
                    go = _poolManager.ChocolateMilkCandyPool.Get();
                    break;

                case CandyType.H_BLUE:
                    go = _poolManager.H_BlueCandyPool.Get();
                    break;
                case CandyType.H_YELLOW:
                    go = _poolManager.H_YellowCandyPool.Get();
                    break;
                case CandyType.H_RED:
                    go = _poolManager.H_RedCandyPool.Get();
                    break;
                case CandyType.H_GREEN:
                    go = _poolManager.H_GreenCandyPool.Get();
                    break;
                case CandyType.H_PURPLE:
                    go = _poolManager.H_PurpleCandyPool.Get();
                    break;
                case CandyType.H_PINK:
                    go = _poolManager.H_PinkCandyPool.Get();
                    break;

                case CandyType.BLUE:
                    go = _poolManager.BlueCandyPool.Get();
                    break;
                case CandyType.YELLOW:
                    go = _poolManager.YellowCandyPool.Get();
                    break;
                case CandyType.RED:
                    go = _poolManager.RedCandyPool.Get();
                    break;
                case CandyType.GREEN:
                    go = _poolManager.GreenCandyPool.Get();
                    break;
                case CandyType.PURPLE:
                    go = _poolManager.PurpleCandyPool.Get();
                    break;
                case CandyType.PINK:
                    go = _poolManager.PinkCandyPool.Get();
                    break;

                case CandyType.STING_BLUE:
                    go = _poolManager.STING_BluePool.Get();
                    break;
                case CandyType.STING_YELLOW:
                    go = _poolManager.STING_YellowPool.Get();
                    break;
                case CandyType.STING_RED:
                    go = _poolManager.STING_RedPool.Get();
                    break;
                case CandyType.STING_GREEN:
                    go = _poolManager.STING_GreenPool.Get();
                    break;
                case CandyType.STING_PURPLE:
                    go = _poolManager.STING_PurplePool.Get();
                    break;
                case CandyType.STING_PINK:
                    go = _poolManager.STING_PinkPool.Get();
                    break;

                case CandyType.SWIRL_BLUE:
                    go = _poolManager.SWIRL_BlueCandyPool.Get();
                    break;
                case CandyType.SWIRL_YELLOW:
                    go = _poolManager.SWIRL_YellowCandyPool.Get();
                    break;
                case CandyType.SWIRL_RED:
                    go = _poolManager.SWIRL_RedCandyPool.Get();
                    break;
                case CandyType.SWIRL_GREEN:
                    go = _poolManager.SWIRL_GreenCandyPool.Get();
                    break;
                case CandyType.SWIRL_PURPLE:
                    go = _poolManager.SWIRL_PurpleCandyPool.Get();
                    break;
                case CandyType.SWIRL_PINK:
                    go = _poolManager.SWIRL_PinkCandyPool.Get();
                    break;
            }

            return go;
        }

        public void Destroy(Candy candy)
        {
            var candyType = candy.Type;

            switch (candyType)
            {
                case CandyType.V_BLUE:
                    _poolManager.V_BlueCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.V_YELLOW:
                    _poolManager.V_YellowCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.V_RED:
                    _poolManager.V_RedCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.V_GREEN:
                    _poolManager.V_GreenCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.V_PURPLE:
                    _poolManager.V_PurpleCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.V_PINK:
                    _poolManager.V_PinkCandyPool.Release(candy.gameObject);
                    break;

                case CandyType.CHOCOLATE_MILK:
                    _poolManager.ChocolateMilkCandyPool.Release(candy.gameObject);
                    break;

                case CandyType.H_BLUE:
                    _poolManager.H_BlueCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.H_YELLOW:
                    _poolManager.H_YellowCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.H_RED:
                    _poolManager.H_RedCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.H_GREEN:
                    _poolManager.H_GreenCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.H_PURPLE:
                    _poolManager.H_PurpleCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.H_PINK:
                    _poolManager.H_PinkCandyPool.Release(candy.gameObject);
                    break;

                case CandyType.BLUE:
                    _poolManager.BlueCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.YELLOW:
                    _poolManager.YellowCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.RED:
                    _poolManager.RedCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.GREEN:
                    _poolManager.GreenCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.PURPLE:
                    _poolManager.PurpleCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.PINK:
                    _poolManager.PinkCandyPool.Release(candy.gameObject);
                    break;

                case CandyType.STING_BLUE:
                    _poolManager.STING_BluePool.Release(candy.gameObject);
                    break;
                case CandyType.STING_YELLOW:
                    _poolManager.STING_YellowPool.Release(candy.gameObject);
                    break;
                case CandyType.STING_RED:
                    _poolManager.STING_RedPool.Release(candy.gameObject);
                    break;
                case CandyType.STING_GREEN:
                    _poolManager.STING_GreenPool.Release(candy.gameObject);
                    break;
                case CandyType.STING_PURPLE:
                    _poolManager.STING_PurplePool.Release(candy.gameObject);
                    break;
                case CandyType.STING_PINK:
                    _poolManager.STING_PinkPool.Release(candy.gameObject);
                    break;

                case CandyType.SWIRL_BLUE:
                    _poolManager.SWIRL_BlueCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.SWIRL_YELLOW:
                    _poolManager.SWIRL_YellowCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.SWIRL_RED:
                    _poolManager.SWIRL_RedCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.SWIRL_GREEN:
                    _poolManager.SWIRL_GreenCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.SWIRL_PURPLE:
                    _poolManager.SWIRL_PurpleCandyPool.Release(candy.gameObject);
                    break;
                case CandyType.SWIRL_PINK:
                    _poolManager.SWIRL_PinkCandyPool.Release(candy.gameObject);
                    break;
            }
        }

        Candy SpawnCandyAt(Vector3Int cellPos, Candy candyPrefab)
        {
            if (candyPrefab == null)
                candyPrefab = _candiesForSpawn[Random.Range(0, _candiesForSpawn.Length)];

            var go = Instantiate(candyPrefab);
            go.transform.position = CellToWorldPoint(cellPos);
            var candy = go.GetComponent<Candy>();

            _boardCells[cellPos].ContainingCandy = candy;
            return candy;
        }

        Vector3 CellToWorldPoint(Vector3Int cellPos)
        {
            return new Vector3(cellPos.x + _tileBoard.tileAnchor.x, cellPos.y + _tileBoard.tileAnchor.y, cellPos.z);
        }

        void OnDestroy()
        {
            _boardCells.Clear();
            _candies.Clear();
            _obstacles.Clear();

            _boardCells = null;
            _candies = null;
            _obstacles = null;
        }
    }
}
