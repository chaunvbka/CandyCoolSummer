#pragma warning disable IDE0130

namespace Texell.Processes
{

    using UnityEngine;
    using UnityEngine.UIElements;

    using Texell.CoreModule;
    using Texell.CoreModule.UI;
    using Texell.CandyCoolSummer;

    public class GameProcess : IProcess
    {
        // GameProcess class do:
        // 1. Create game ui.
        // 2. Create model.
        // 3. Create game background as sprite render.
        // 4. Create board game level.

        private readonly AssetManager _assetManager = AssetManager.Instance;
        private readonly UIManager _uiManager = UIManager.Instance;
        private GameModel _model;

        private GameObject _background;
        private Board _board;
        private int _level = 1;

        private CandyFactory _candyFactory;
        private ObstacleFactory _obstacleFactory;
        private BoardInput _boardInput;
        private MatchEngine _matchEngine;
        private CandySwapping _swapping;
        private MatchFinding _matchFinding;
        private MatchDeleting _matchDeleting;
        private CandyFalling _candyFalling;
        private HintIndicator _hintIndicator;

        public void OnStart()
        {
            Debug.Log("GameProcess.OnStart()");
            // Create home screen UI
            var xml = Resources.Load<VisualTreeAsset>(UIPaths.UXMLPaths[(int)UXMLIndex.Game_ui]);
            _uiManager.CreateUI<GameUI>(xml);

            // Create model
            _model = new();

            // Create background
            _background = _assetManager.Background;
            _background.SetActive(true);

            _candyFactory = new();
            _obstacleFactory = new();
            _boardInput = new();
            _matchEngine = new();
            _swapping = new();
            _matchDeleting = new();
            _candyFalling = new();
            _hintIndicator = new();

            // Create board 
            var levelBoardPrefab = Resources.Load<GameObject>(AssetPath.BoardPrefabPaths[_level - 1]);
            var go = Object.Instantiate(levelBoardPrefab);
            _board = go.GetComponent<Board>();
            _board.GenerateBoard();

            var matchFindingPrefab = Resources.Load<GameObject>(AssetPath.MatchFindingPrefabPath);
            _matchFinding = Object.Instantiate(matchFindingPrefab).GetComponent<MatchFinding>();
            _matchFinding.Init(_board);

            _swapping.Init(_board, _matchFinding);
            _boardInput.Init(_board, _swapping, _hintIndicator);
            _matchEngine.Init(_board);
            _matchDeleting.Init(_board, _matchFinding, _hintIndicator);
            _candyFalling.Init(_board, _matchDeleting, _matchFinding, _hintIndicator);
            _hintIndicator.Init(_board, _matchFinding);
        }

        public void OnUpdate()
        {
            _boardInput.OnUpdate();
            _matchEngine.OnUpdate();
            _swapping.OnUpdate();
            _matchDeleting.OnUpdate();
            _candyFalling.OnUpdate();
            _hintIndicator.OnUpdate();
        }

        public void OnExit()
        {
            //UnregisterEvents();

            _uiManager?.Clear();
            _model?.Dispose();
            _background?.SetActive(false);

            if (_board != null)
            {
                Object.Destroy(_board.gameObject);
                _board = null;
            }
            _candyFactory.Dispose();
            _obstacleFactory.Dispose();
            _boardInput.Dispose();
            _matchEngine.Dispose();
            _swapping.Dispose();
            _matchDeleting.Dispose();
            _candyFalling.Dispose();
            _hintIndicator.Dispose();

            Resources.UnloadUnusedAssets();
        }
    }
}