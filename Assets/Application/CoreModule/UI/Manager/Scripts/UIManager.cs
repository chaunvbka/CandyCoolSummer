#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIManager : IDisposable
    {
        private bool _dispose = false;

        private static UIManager s_Instance;
        public static UIManager Instance => s_Instance;

        private UIDocument _popupDocument;
        private UIDocument _screenDocument;

        private UIScreen _screen;

        private UIAnimation _uiAnimation;

        public UIManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("UIManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Initialize()
        {
            var popup = Resources.Load<GameObject>("UIPopup");
            if (popup == null)
            {
                Debug.LogError("Failed to load UIPopup prefab.");
                return;
            }
            var popupInstance = UnityEngine.Object.Instantiate(popup);
            popupInstance.name = "UIPopup";
            _popupDocument = popupInstance.GetComponent<UIDocument>();

            var screen = Resources.Load<GameObject>("UIScreen");
            if (screen == null)
            {
                Debug.LogError("Failed to load UIScreen prefab.");
                return;
            }
            var screenInstance = UnityEngine.Object.Instantiate(screen);
            screenInstance.name = "UIScreen";
            _screenDocument = screenInstance.GetComponent<UIDocument>();

            _uiAnimation = UIAnimation.Instance;
        }

        public T CreatePopup<T>(VisualTreeAsset visualTreeAsset) where T : UIPopup, new()
        {
            if (_popupDocument == null)
            {
                Debug.LogError("Popup document instance is not initialized.");
                return null;
            }
            var popup = new T();
            popup.Initialize(_popupDocument, visualTreeAsset);

            return popup;
        }

        /// <summary>
        /// Creates a new ui screen instance and initializes it with the provided VisualTreeAsset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visualTreeAsset"></param>
        public void CreateUI<T>(VisualTreeAsset visualTreeAsset) where T : UIScreen, new()
        {
            if (_screenDocument == null)
            {
                Debug.LogError("Screen document instance is not initialized.");
                return;
            }
            _screen = new T();
            _screen.Initialize(_screenDocument, visualTreeAsset);
        }

        /// <summary>
        /// Clears old screen instances when loading new scene.
        /// </summary>
        public void Clear()
        {
            if (_popupDocument)
            {
                _popupDocument.visualTreeAsset = null;
            }
            if (_screenDocument)
            {
                _screenDocument.visualTreeAsset = null;
            }

            _screen?.Dispose();
            _screen = null;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            _screen?.Dispose();
            _uiAnimation?.Dispose();
            s_Instance = null;
        }
    }
}