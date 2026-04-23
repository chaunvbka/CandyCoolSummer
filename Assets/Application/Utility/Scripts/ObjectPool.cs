#pragma warning disable IDE0130 

namespace Texell.Utility
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool : IDisposable
    {
        private bool _dispose = false;
        private GameObject _prefab;
        private Stack<GameObject> _pool = new();

        public IEnumerator Initialize(GameObject prefab, int size = 1)
        {
            _prefab = prefab;
            for (int i = 0; i < size; i++)
            {
                GameObject obj = UnityEngine.Object.Instantiate(_prefab);
                obj.SetActive(false);
                _pool.Push(obj);

                // Wait until the next frame to continue the loop.
                yield return null;
            }
        }

        /// <summary>
        /// Retrieves an object from the pool.
        /// </summary>
        /// <returns></returns>
        public GameObject Get()
        {
            GameObject obj;

            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"ObjectPool: Pool is empty, creating a new object.");
                obj = UnityEngine.Object.Instantiate(_prefab);
            }

            return obj;
        }

        /// <summary>
        /// Returns an object to the pool for reuse.
        /// </summary>
        /// <param name="obj"></param>
        public void Release(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Push(obj);
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;
            _pool.Clear();
            _pool = null;
            _prefab = null;
        }

        ~ObjectPool()
        {
            Dispose();
        }
    }

}