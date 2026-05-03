#pragma warning disable IDE0130

namespace Texell.Processes
{
    using UnityEngine;
    using UnityEngine.UIElements;

    using Texell.CoreModule;
    using Texell.CoreModule.UI;

    public class HomeProcess : IProcess
    {

        private readonly UIManager _uiManager = UIManager.Instance;
        private readonly AssetManager _assetManager = AssetManager.Instance;
        private readonly PoolManager _poolManager = PoolManager.Instance;
        private HomeModel _model;

        public void OnStart()
        {
            Debug.Log("HomeProcess.OnStart()");
            _model = new();

            // Create home screen UI
            var xml = Resources.Load<VisualTreeAsset>(UIPaths.UXMLPaths[(int)UXMLIndex.Home_ui]);
            _uiManager.CreateUI<HomeUI>(xml);

            RegisterEvents();
        }

        public void OnUpdate()
        {
        }

        void RegisterEvents()
        {
            _model.LevelSelected += OnLevelSelected;
        }

        void UnregisterEvents()
        {
            _model.LevelSelected -= OnLevelSelected;
        }

        void OnLevelSelected()
        {
            Debug.Log("LevelSelected");
            ProcessManager.Instance.TransitionTo(ProcessIndex.Game);
        }

        public void OnExit()
        {
            UnregisterEvents();

            _uiManager?.Clear();
            _model?.Dispose();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}