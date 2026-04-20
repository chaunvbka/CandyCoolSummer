#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class UIPopup : IDisposable
    {
        protected const int k_OpaqueBackgroundIndex = 0;
        protected const int k_TransparentBackgroundIndex = 1;

        protected bool _disposed = false;

        protected UIDocument _uiDocument;
        protected VisualElement _root;

        protected Canvas[] _backgrounds;
        protected List<VisualElement> _popupElements;

        public virtual void Initialize(UIDocument uiDocument, VisualTreeAsset visualTreeAsset)
        {
            _uiDocument = uiDocument;
            _uiDocument.visualTreeAsset = visualTreeAsset;
            _root = _uiDocument.rootVisualElement;

            _backgrounds = _uiDocument.transform.GetComponentsInChildren<Canvas>(true);
            _popupElements = _root.Q<VisualElement>().Children().ToList();

            Hide();
        }

        void Hide()
        {
            foreach (var bg in _backgrounds)
            {
                bg.gameObject.SetActive(false);
            }

            foreach (var popup in _popupElements)
            {
                popup.style.display = DisplayStyle.None;
            }
        }

        public abstract void Dispose();

    }
}