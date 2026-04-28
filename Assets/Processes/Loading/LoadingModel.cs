#pragma warning disable IDE0130

namespace Texell.Processes
{
    using Texell.CoreModule.Model;
    using UnityEngine;
    using System;

    public class LoadingModel : BaseModel
    {
        private static LoadingModel s_Instance;
        public static LoadingModel Instance => s_Instance;

        public LoadingModel()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Debug.LogError("LoadingModel instance already exists. Cannot create a new one.");
            }
        }

        public event Action<float> LoadProgressUpdated;
        public void OnLoadProgressUpdated(float value)
        {
            LoadProgressUpdated?.Invoke(value);
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_Instance = null;
        }
    }
}
