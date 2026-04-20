#pragma warning disable IDE0130

namespace Texell.Processes
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    using Texell.CoreModule;
    using Texell.CoreModule.UI;
    using Texell.Utility;

    public class LoadingProcess : IProcess
    {
        private const float k_LoadTime = 5.0f;
        private float _elapsedTime = 0;
        private float _progressValue = 0;
        /// <summary>
        /// Is load assets operation done?
        /// </summary>
        private bool _loadAssetsDone = false;

        private bool _showAds = false;
        private float _timeShowAds = 0;

        private UIManager _uiManager;
        private LoadingModel _model;
        private AssetManager _assetManager;
        //private AdsManager _adsManager;

        public void OnStart()
        {
            //_adsManager = AdsManager.Instance;
            _uiManager = UIManager.Instance;
            _assetManager = AssetManager.Instance;
            _model = new();

            // Create loading screen UI
            var xml = Resources.Load<VisualTreeAsset>(UIPaths.UXMLPaths[(int)UXMLIndex.Loading_ui]);
            _uiManager.CreateUI<LoadingUI>(xml);
            _timeShowAds = Random.Range(0.45f, 0.9f);

            NonMono.StartCoroutine(Initialize());
        }

        IEnumerator Initialize()
        {
            yield return new WaitUntil(() => _assetManager.Loaded);
            _loadAssetsDone = true;
        }

        public void OnUpdate()
        {
            if (_progressValue < 1)
            {
                _progressValue = _elapsedTime / k_LoadTime;

                if (_progressValue > _timeShowAds && !_showAds)
                {
                    //_adsManager.ShowAppOpenAds();
                    _showAds = true;
                }

                if (_progressValue > 0.95f && !_loadAssetsDone)
                {
                    return;
                }

                if (_progressValue >= 1)
                {
                    _progressValue = 1;
                    ProcessManager.Instance.TransitionTo(ProcessIndex.Home);
                }
                _model.OnLoadProgressUpdated(_progressValue * 100);
                _elapsedTime += Time.deltaTime;
            }
        }

        public void OnExit()
        {
            _uiManager.Clear();
            _model.Dispose();

            ProcessManager.Instance.DestroyLoading();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}
