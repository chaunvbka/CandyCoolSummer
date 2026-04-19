#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{

    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class UIPopupElement : IDisposable
    {
        protected bool _disposed = false;

        protected VisualElement _root;
        protected Canvas _background;

        public UIPopupElement(VisualElement rootElement, Canvas background)
        {
            _root = rootElement;
            _background = background;
        }

        public void Show()
        {
            _background.gameObject.SetActive(true);
            _root.style.display = DisplayStyle.Flex;
            _root.BringToFront();
        }

        public void Hide()
        {
            _background.gameObject.SetActive(false);
            _root.style.display = DisplayStyle.None;
        }

        public abstract void Dispose();

    }
}