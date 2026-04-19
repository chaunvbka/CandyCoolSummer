#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{

    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class UIScreen : IDisposable
    {
        protected bool _disposed = false;

        protected UIDocument _uiDocument;
        protected VisualElement _root;

        protected Canvas _background;

        public virtual void Initialize(UIDocument uiDocument, VisualTreeAsset visualTreeAsset)
        {
            _uiDocument = uiDocument;
            _uiDocument.visualTreeAsset = visualTreeAsset;
            _root = _uiDocument.rootVisualElement;

            _background = _uiDocument.transform.GetComponentInChildren<Canvas>(true);
        }

        public abstract void Dispose();

    }
}