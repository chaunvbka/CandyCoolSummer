#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class InputManager : IDisposable
    {
        private bool _dispose = false;
        private static InputManager s_Instance = null;
        public static InputManager Instance => s_Instance;

        private readonly InputActionAsset _inputActionAsset;
        private readonly EventSystem _eventSystem;

        public InputActionAsset InputActions => _inputActionAsset;
        public EventSystem EventSystem => _eventSystem;

        public InputAction ClickAction;
        public InputAction ClickPosition;

        public InputManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("InputManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;

            _inputActionAsset = Resources.Load<InputActionAsset>("InputAction");
            if (_inputActionAsset == null)
            {
                Debug.LogError("Failed to load InputActionAsset!");
                return;
            }

            var prefab = Resources.Load<GameObject>("EventSystem");
            if (prefab == null)
            {
                Debug.LogError("Failed to load EventSystem prefab!");
                return;
            }
            var instance = UnityEngine.Object.Instantiate(prefab);
            instance.name = "EventSystem";
            _eventSystem = instance.GetComponent<EventSystem>();

            //EventSystem.SetUITookitEventSystemOverride(_eventSystem);
            ClickAction = _inputActionAsset.FindAction("ClickAction");
            ClickPosition = _inputActionAsset.FindAction("ClickPosition");
            ClickAction.Enable();
            ClickPosition.Enable();
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            s_Instance = null;
        }
    }
}