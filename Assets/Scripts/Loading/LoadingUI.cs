#pragma warning disable IDE0130

namespace Texell.Processes
{
    using UnityEngine.UIElements;
    using Texell.CoreModule.UI;

    /// <summary>
    /// This UI fills the screen and shows a progress bar while loading assets.
    /// </summary>
    public class LoadingUI : UIScreen
    {
        private ProgressBar _progressBar;
        private Label _title;
        private readonly LoadingModel _model = LoadingModel.Instance;

        public override void Initialize(UIDocument uiDocument, VisualTreeAsset visualTreeAsset)
        {
            base.Initialize(uiDocument, visualTreeAsset);

            // Load the progress bar and title elements
            LoadElements();
            RegisterEvents();

            _background.gameObject.SetActive(true);
        }

        void LoadElements()
        {
            _progressBar = _root.Q<ProgressBar>("loading__progress-bar");
            _title = _root.Q<Label>(className: "unity-progress-bar__title");
        }

        void RegisterEvents()
        {
            _model.LoadProgressUpdated += OnLoadProgressUpdated;
        }

        void UnregisterEvents()
        {
            _model.LoadProgressUpdated -= OnLoadProgressUpdated;
        }

        private void OnLoadProgressUpdated(float value)
        {
            if (_progressBar == null)
                return;

            _progressBar.value = value;
            _title.text = value.ToString("F0") + "%";
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            UnregisterEvents();
        }
    }

}
