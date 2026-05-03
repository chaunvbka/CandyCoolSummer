#pragma warning disable IDE0130

namespace Texell.Processes
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using Texell.CoreModule.UI;

    public class HomeUI : UIScreen
    {
        private Button _buttonLevel_1;

        private readonly HomeModel _model = HomeModel.Instance;

        public override void Initialize(UIDocument uiDocument, VisualTreeAsset visualTreeAsset)
        {
            base.Initialize(uiDocument, visualTreeAsset);

            // Load the progress bar and title elements
            LoadElements();
            RegisterEvents();
            RegisterCallbacks();

            _background.gameObject.SetActive(true);
        }

        void LoadElements()
        {
            _buttonLevel_1 = _root.Q<Button>("button_level-1");
        }

        void RegisterCallbacks()
        {

        }

        void UnregisterCallbacks()
        {

        }

        void RegisterEvents()
        {
            _buttonLevel_1.clicked += ButtonClick_1;
        }

        void UnregisterEvents()
        {
             _buttonLevel_1.clicked -= ButtonClick_1;
        }

        void ButtonClick_1()
        {
            _model.OnLevelSelected();
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            UnregisterCallbacks();
            UnregisterEvents();
        }
    }
}