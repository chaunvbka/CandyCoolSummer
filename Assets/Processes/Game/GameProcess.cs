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

        public void OnStart()
        {
            Debug.Log("GameProcess.OnStart()");
            // Create home screen UI
            var xml = Resources.Load<VisualTreeAsset>(UIPaths.UXMLPaths[(int)UXMLIndex.Game_ui]);
            _uiManager.CreateUI<GameUI>(xml);

            // Create model
            _model = new();

            _background = _assetManager.Background;
            _background.SetActive(true);

            string absolutePath = "BoardLevelPrefabs/Board_Level_";
            string path = absolutePath + _level.ToString();
            var levelBoardPrefab = Resources.Load<GameObject>(path);
            var go = Object.Instantiate(levelBoardPrefab);
            _board = go.GetComponent<Board>();

            _board.Initialize();
        }

        public void OnUpdate()
        {
        }

        void Clear()
        {
            if (_board != null)
            {
                Object.Destroy(_board.gameObject);
                _board = null;
            }
        }

        public void OnExit()
        {
        }
    }
}