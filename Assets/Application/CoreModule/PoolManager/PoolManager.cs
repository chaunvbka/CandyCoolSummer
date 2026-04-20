#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using UnityEngine;

    public class PoolManager : IDisposable
    {
        private static PoolManager s_Instance;
        public static PoolManager Instance => s_Instance;

        private bool _dispose = false;

        public PoolManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("PoolManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            s_Instance = null;
        }
    }
}
