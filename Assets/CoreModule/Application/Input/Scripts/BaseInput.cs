#pragma warning disable IDE0130

namespace Texell.CoreModule.Input
{

    using System;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public abstract class BaseInput : IDisposable
    {
        protected InputActionAsset _inputActionAsset;
        protected EventSystem _eventSystem;

        public virtual void Initialize(InputActionAsset asset, EventSystem eventSystem)
        {
            _inputActionAsset = asset;
            _eventSystem = eventSystem;
        }

        public abstract void OnUpdate();

        public abstract void Dispose();
    }

}