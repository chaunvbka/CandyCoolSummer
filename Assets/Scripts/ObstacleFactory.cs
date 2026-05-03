#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{

    using System;
    using Texell.CoreModule;
    using UnityEngine;

    public class ObstacleFactory : IDisposable
    {
        static ObstacleFactory s_Instance = null;
        private bool _disposed = false;

        private readonly PoolManager _poolManager = PoolManager.Instance;

        public ObstacleFactory()
        {
            if (s_Instance != null)
            {
                Debug.LogError("ObstacleFactory instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        /// <summary>
        /// Creates a obstacle object.
        /// </summary>
        /// <param name="prefab"></param>
        public static Obstacle Create(Obstacle prefab)
        {
            var go = s_Instance.Instantiate(prefab);

            return go.GetComponent<Obstacle>();
        }

        /// <summary>
        /// Removes a obstacle object.
        /// </summary>
        /// <param name="obstacle"></param>
        public static void Destroy(Obstacle obstacle)
        {
            s_Instance.InternalDestroy(obstacle);
        }

        GameObject Instantiate(Obstacle obstaclePrefab)
        {
            GameObject go = null;
            var pool = obstaclePrefab.Pool;

            switch (pool)
            {
                case Obstacle.PoolType.TieRope:
                    go = _poolManager.TieRopePool.Get();
                    break;
            }

            return go;
        }

        void InternalDestroy(Obstacle obstacle)
        {
            var pool = obstacle.Pool;

            switch (pool)
            {
                case Obstacle.PoolType.TieRope:
                    _poolManager.TieRopePool.Release(obstacle.gameObject);
                    break;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            s_Instance = null;
        }
    }

}