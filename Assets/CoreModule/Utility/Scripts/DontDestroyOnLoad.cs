#pragma warning disable IDE0130

namespace Texell.Utility
{

    using UnityEngine;

    public class DontDestroyOnLoad : MonoBehaviour
    {
        private static DontDestroyOnLoad s_Instance;

        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            s_Instance = null;
        }
    }

}