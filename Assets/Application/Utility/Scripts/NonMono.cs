#pragma warning disable IDE0130

namespace Texell.Utility
{
    using System.Collections;
    using UnityEngine;

    public static class NonMono
    {
        private static MonoBehaviour _mono;

        public static void CreateMonoInstance(MonoBehaviour behaviour)
        {
            if (_mono == null)
            {
                _mono = behaviour;
            }
        }

        public static void DestroyMonoInstance()
        {
            if (_mono != null)
            {
                _mono = null;
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (_mono != null)
            {
                return _mono.StartCoroutine(routine);
            }
            else
            {
                return null;
            }
        }

        public static void StopCoroutine(IEnumerator routine)
        {
            if (_mono != null)
            {
                _mono.StopCoroutine(routine);
            }
        }
    }
}