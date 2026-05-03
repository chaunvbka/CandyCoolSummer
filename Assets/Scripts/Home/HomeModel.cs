#pragma warning disable IDE0130

namespace Texell.Processes
{
    using Texell.CoreModule.Model;
    using UnityEngine;
    using System;

    public class HomeModel : BaseModel
    {
        private static HomeModel s_instance;
        public static HomeModel Instance => s_instance;

        public HomeModel()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError("HomeModel instance already exists. Cannot create a new one.");
            }
        }

        public event Action LevelSelected;
        public void OnLevelSelected()
        {
            LevelSelected?.Invoke();
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_instance = null;
        }
    }
}