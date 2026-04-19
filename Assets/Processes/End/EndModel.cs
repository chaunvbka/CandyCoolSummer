#pragma warning disable IDE0130

namespace Texell.Processes
{
    using Texell.CoreModule.Model;
    using UnityEngine;
    using System;

    public class EndModel : BaseModel
    {
        private static EndModel s_instance;
        public static EndModel Instance => s_instance;

        public EndModel()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError("EndModel instance already exists. Cannot create a new one.");
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