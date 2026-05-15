#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;
    using System.Linq;
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Board : MonoBehaviour
    {
        private static Board s_Instance;

        [SerializeField]
        private Tilemap _tileBoard;
        [SerializeField]
        private Tilemap _tileCandyDesign;
        [SerializeField]
        private Tilemap _tileObstacleDesign;
        private readonly Candy[] _candyPrefabs = new Candy[6];
        private Grid _grid;

        /// <summary>
        /// List of cells on the board (key: cell position, value: cell object).
        /// </summary>
        private readonly Dictionary<Vector3Int, Cell> _boardCells = new();

        /// <summary>
        /// Candies has spawned according to tilemap design.
        /// </summary>
        private readonly Dictionary<Vector3Int, Candy> _tilemapCandies = new();

        /// <summary>
        /// Obstacles has spawned according to tilemap design.
        /// </summary>
        private readonly Dictionary<Vector3Int, Obstacle> _tilemapObstacles = new();

        /// <summary>
        /// List of sprite mask object to hide candy on the top board.
        /// </summary>
        private readonly List<GameObject> _hideCandies = new();

        /// <summary>
        /// List of cell position on top of the board for spawn candy.
        /// </summary>
        private readonly List<Vector3Int> _spawnerPositions = new();

        /// <summary>
        /// List of cell position in the board for spawn candy randomly.
        /// </summary>
        private readonly List<Vector3Int> _randomPositions = new();
        private readonly Dictionary<Candy.ColorType, Candy> _candyLookup = new();
        private readonly AssetManager _assetManager = AssetManager.Instance;

        public Grid Grid => _grid;
        public Tilemap TileBoard => _tileBoard;
        public Dictionary<Vector3Int, Cell> BoardCells => _boardCells;
        public Candy[] CandyPrefabs => _candyPrefabs;
        public List<Vector3Int> SpawnerPositions => _spawnerPositions;


        /// <summary>
        /// Called on StartUp TileBase script before all Awake.
        /// </summary>
        /// <param name="cellPos"></param>
        public static void RegisterSpawner(Vector3Int cellPos)
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
                s_Instance.GridReference();
            }

            s_Instance._spawnerPositions.Add(cellPos);

            // Sprite mask
            var mask = Instantiate(s_Instance._assetManager.HideCandyPrefab);
            mask.transform.position = s_Instance._grid.GetCellCenterWorld(cellPos);
            s_Instance._hideCandies.Add(mask);
        }

        /// <summary>
        /// Called on StartUp TileBase script before all Awake.
        /// Spawn specific candy at cellPos position.
        /// </summary>
        /// <param name="cellPos"></param>
        public static void RegisterCandy(Vector3Int cellPos, Candy candyPrefab)
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
                s_Instance.GridReference();
            }

            if (candyPrefab != null)
            {
                var candy = s_Instance.NewCandyAt(cellPos, candyPrefab);
                s_Instance._tilemapCandies.Add(cellPos, candy);
            }
        }

        /// <summary>
        /// Called on StartUp TileBase script before all Awake.
        /// Store cell position for spawn candy ramdomly.
        /// </summary>
        /// <param name="cellPos"></param>
        public static void RegisterRandomCandy(Vector3Int cellPos)
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
                s_Instance.GridReference();
            }

            s_Instance._randomPositions.Add(cellPos);
        }

        /// <summary>
        /// Called on StartUp TileBase script before all Awake.
        /// Spawn specific obstacle at cellPos position.
        /// </summary>
        /// <param name="cellPos"></param>
        public static void RegisterObstacle(Vector3Int cellPos, Obstacle obstaclePrefab)
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
                s_Instance.GridReference();
            }

            if (obstaclePrefab != null)
            {
                var obstacle = ObstacleFactory.Create(obstaclePrefab);
                obstacle.transform.position = s_Instance._grid.GetCellCenterWorld(cellPos);
                s_Instance._tilemapObstacles.Add(cellPos, obstacle);
            }
        }

        void GridReference()
        {
            _grid = gameObject.GetComponent<Grid>();
        }

        void Init()
        {
            if (_tileCandyDesign != null)
                _tileCandyDesign.gameObject.SetActive(false);
            if (_tileObstacleDesign != null)
                _tileObstacleDesign.gameObject.SetActive(false);

            for (int i = 0; i < 6; i++)
            {
                int index = (int)CandyIndex.BLUE + i;
                _candyPrefabs[i] = _assetManager.CandyPrefabs[index].GetComponent<Candy>();
            }

            foreach (var candy in _candyPrefabs)
            {
                _candyLookup.Add(candy.Type, candy);
            }
        }

        public static void ChangeLock(Vector3Int cellPos, bool lockState)
        {
            s_Instance._boardCells[cellPos].Locked = lockState;
        }

        public void GenerateBoard()
        {
            Init();

            // Create board cells.
            foreach (var pos in _tileBoard.cellBounds.allPositionsWithin)
            {
                if (_tileBoard.HasTile(pos))
                {
                    _boardCells.Add(pos, new Cell());
                }
            }

            // Fill board cells with candy object.
            foreach (var entry in _tilemapCandies)
            {
                _boardCells[entry.Key].ContainingCandy = entry.Value;
            }

            // Fill board cells with obstacle object.
            foreach (var entry in _tilemapObstacles)
            {
                var obstacle = entry.Value;
                var cellPos = entry.Key;
                _boardCells[cellPos].Obstacle = obstacle;
                obstacle.Init(cellPos);
            }

            SpawnCandy();
        }

        /// <summary>
        /// Spawns candy randomly, make sure has no match.
        /// </summary>
        void SpawnCandy()
        {
            foreach (var pos in _randomPositions)
            {
                var candyPrefab = _candyPrefabs[Random.Range(0, _candyPrefabs.Length)];
                var candy = NewCandyAt(pos, candyPrefab);
                _boardCells[pos].ContainingCandy = candy;
            }

            // Check if has match, delete old candy and spawn a new.
            foreach (var cellPos in _randomPositions)
            {
                CheckNoMatch(cellPos);
            }
        }

        public void CheckNoMatch(Vector3Int cellPos)
        {
            List<Candy.ColorType> listTypes = new();
            foreach (var entry in _candyLookup)
            {
                listTypes.Add(entry.Key);
            }

            var left = cellPos + Vector3Int.left;
            var leftleft = left + Vector3Int.left;

            var right = cellPos + Vector3Int.right;
            var rightright = right + Vector3Int.right;

            var up = cellPos + Vector3Int.up;
            var upup = up + Vector3Int.up;

            var down = cellPos + Vector3Int.down;
            var downdown = down + Vector3Int.down;

            var leftdown = new Vector3Int(left.x, down.y, 0);
            var rightdown = new Vector3Int(right.x, down.y, 0);
            var leftup = new Vector3Int(left.x, up.y, 0);
            var rightup = new Vector3Int(right.x, up.y, 0);

            var hasLeft = _boardCells.TryGetValue(left, out var leftCell);
            var hasLeftLeft = _boardCells.TryGetValue(leftleft, out var leftleftCell);
            var hasRight = _boardCells.TryGetValue(right, out var rightCell);
            var hasRightRight = _boardCells.TryGetValue(rightright, out var rightrightCell);
            var hasUp = _boardCells.TryGetValue(up, out var upCell);
            var hasUpUp = _boardCells.TryGetValue(upup, out var upupCell);
            var hasDown = _boardCells.TryGetValue(down, out var downCell);
            var hasDownDown = _boardCells.TryGetValue(downdown, out var downdownCell);
            var hasLeftDown = _boardCells.TryGetValue(leftdown, out var leftdownCell);
            var hasRightDown = _boardCells.TryGetValue(rightdown, out var rightdownCell);
            var hasLeftUp = _boardCells.TryGetValue(leftup, out var leftupCell);
            var hasRightUp = _boardCells.TryGetValue(rightup, out var rightupCell);

            if (hasLeft && leftCell.ContainingCandy != null &&
               hasLeftLeft && leftleftCell.ContainingCandy != null &&
               leftCell.ContainingCandy.Type == leftleftCell.ContainingCandy.Type)
            {
                listTypes.Remove(leftCell.ContainingCandy.Type);
            }

            if (hasRight && rightCell.ContainingCandy != null &&
                hasRightRight && rightrightCell.ContainingCandy != null &&
                rightCell.ContainingCandy.Type == rightrightCell.ContainingCandy.Type)
            {
                listTypes.Remove(rightCell.ContainingCandy.Type);
            }

            if (hasUp && upCell.ContainingCandy != null &&
                hasUpUp && upupCell.ContainingCandy != null &&
                upCell.ContainingCandy.Type == upupCell.ContainingCandy.Type)
            {
                listTypes.Remove(upCell.ContainingCandy.Type);
            }

            if (hasDown && downCell.ContainingCandy != null &&
                hasDownDown && downdownCell.ContainingCandy != null &&
                downCell.ContainingCandy.Type == downdownCell.ContainingCandy.Type)
            {
                listTypes.Remove(downCell.ContainingCandy.Type);
            }

            if (hasLeft && leftCell.ContainingCandy != null &&
                hasDown && downCell.ContainingCandy != null &&
                hasLeftDown && leftdownCell.ContainingCandy != null &&
                leftCell.ContainingCandy.Type == downCell.ContainingCandy.Type &&
                leftCell.ContainingCandy.Type == leftdownCell.ContainingCandy.Type)
            {
                listTypes.Remove(leftCell.ContainingCandy.Type);
            }

            if (hasRight && rightCell.ContainingCandy != null &&
                hasDown && downCell.ContainingCandy != null &&
                hasRightDown && rightdownCell.ContainingCandy != null &&
                rightCell.ContainingCandy.Type == downCell.ContainingCandy.Type &&
                rightCell.ContainingCandy.Type == rightdownCell.ContainingCandy.Type)
            {
                listTypes.Remove(rightCell.ContainingCandy.Type);
            }

            if (hasLeft && leftCell.ContainingCandy != null &&
                hasUp && upCell.ContainingCandy != null &&
                hasLeftUp && leftupCell.ContainingCandy != null &&
                leftCell.ContainingCandy.Type == upCell.ContainingCandy.Type &&
                leftCell.ContainingCandy.Type == leftupCell.ContainingCandy.Type)
            {
                listTypes.Remove(leftCell.ContainingCandy.Type);
            }

            if (hasRight && rightCell.ContainingCandy != null &&
                hasUp && upCell.ContainingCandy != null &&
                hasRightUp && rightupCell.ContainingCandy != null &&
                rightCell.ContainingCandy.Type == upCell.ContainingCandy.Type &&
                rightCell.ContainingCandy.Type == rightupCell.ContainingCandy.Type)
            {
                listTypes.Remove(rightCell.ContainingCandy.Type);
            }

            var type = listTypes[Random.Range(0, listTypes.Count)];

            var oldCandy = _boardCells[cellPos].ContainingCandy;
            CandyFactory.Destroy(oldCandy);

            // Spawn a new.
            var candy = NewCandyAt(cellPos, _candyLookup[type]);
            _boardCells[cellPos].ContainingCandy = candy;
        }

        public Candy NewCandyAt(Vector3Int cellPos, Candy prefab)
        {
            var candy = CandyFactory.Create(prefab);
            candy.transform.position = _grid.GetCellCenterWorld(cellPos);
            candy.Init(cellPos);

            return candy;
        }

        void OnDestroy()
        {
            s_Instance = null;
        }
    }
}
