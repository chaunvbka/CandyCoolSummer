#pragma warning disable IDE0130

namespace Texell.Utility
{

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool : IDisposable
    {
        private bool _dispose = false;
        private int _poolSize = 1;
        private GameObject _prefab;
        private readonly Stack<GameObject> _pool = new();

        public void Initialize(GameObject prefab, int poolSize = 1)
        {
            _prefab = prefab;
            _poolSize = poolSize;
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject obj = UnityEngine.Object.Instantiate(_prefab);
                obj.SetActive(false);
                _pool.Push(obj);
            }
        }

        public GameObject GetObject()
        {
            if (_pool.Count > 0)
            {
                GameObject obj = _pool.Pop();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                Debug.LogWarning($"ObjectPool: Pool is empty, creating a new object. Pool size: {_poolSize}");
                GameObject obj = UnityEngine.Object.Instantiate(_prefab);
                return obj;
            }
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            if (_pool.Count < _poolSize)
            {
                _pool.Push(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;
            _pool.Clear();
            _prefab = null;
            _poolSize = 0;
        }

        ~ObjectPool()
        {
            Dispose();
        }
    }

}