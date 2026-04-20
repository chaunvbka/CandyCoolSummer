#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class NotifyClosePopup : UIPopupElement
    {
        private Button _closeButton;
        private Label _messageLabel;

        private readonly AudioManager _audioManager = AudioManager.Instance;

        public AudioClip ClickSound;

        public NotifyClosePopup(VisualElement rootElement, Canvas background) : base(rootElement, background)
        {
            LoadElements();
            RegisterEvents();
        }

        void LoadElements()
        {
            _closeButton = _root.Q<Button>("close-button");
            _messageLabel = _root.Q<Label>("message__label");
        }

        void RegisterEvents()
        {
            _closeButton.clicked += OnCloseButtonClicked;
        }

        void UnregisterEvents()
        {
            _closeButton.clicked -= OnCloseButtonClicked;
        }

        void OnCloseButtonClicked()
        {
            _audioManager.PlaySFX(ClickSound);
            Hide();
        }

        public void SetMessage(string message)
        {
            _messageLabel.text = message;
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            UnregisterEvents();
        }
    }
}