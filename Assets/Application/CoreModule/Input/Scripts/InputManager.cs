#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class InputManager : IDisposable
    {
        private static InputManager s_Instance = null;
        public static InputManager Instance => s_Instance;

        private BaseInput _input;
        private readonly InputActionAsset _inputActionAsset;
        private readonly EventSystem _eventSystem;

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
        }

        public void OnUpdate()
        {
            _input?.OnUpdate();
        }

        public void CreateInput<T>() where T : BaseInput, new()
        {
            if (!_inputActionAsset || !_eventSystem)
            {
                Debug.LogError("InputManager is not initialized!");
                return;
            }

            _input = new T();
            _input.Initialize(_inputActionAsset, _eventSystem);
        }

        public void Dispose()
        {
            _input?.Dispose();
            s_Instance = null;
        }
    }
}