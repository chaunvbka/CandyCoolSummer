#pragma warning disable IDE0130

namespace Texell.Processes
{
    using Texell.CoreModule.Model;
    using UnityEngine;
    using System;

    public class GameModel : BaseModel
    {
        private static GameModel s_instance;
        public static GameModel Instance => s_instance;

        public GameModel()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError("GameModel instance already exists. Cannot create a new one.");
            }
        }


        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_instance = null;
        }
    }

}
