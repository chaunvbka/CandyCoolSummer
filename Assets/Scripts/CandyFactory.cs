#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System;
    using Texell.CoreModule;
    using UnityEngine;

    public class CandyFactory : IDisposable
    {
        static CandyFactory s_Instance = null;
        private bool _disposed = false;

        private readonly PoolManager _poolManager = PoolManager.Instance;

        public CandyFactory()
        {
            if (s_Instance != null)
            {
                Debug.LogError("CandyFactory instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        /// <summary>
        /// Creates a candy object.
        /// </summary>
        /// <param name="prefab"></param>
        public static Candy Create(Candy prefab)
        {
            var go = s_Instance.Instantiate(prefab);

            return go.GetComponent<Candy>();
        }

        /// <summary>
        /// Removes a candy object.
        /// </summary>
        /// <param name="candy"></param>
        public static void Destroy(Candy candy)
        {
            s_Instance.InternalDestroy(candy);
        }

        GameObject Instantiate(Candy candyPrefab)
        {
            GameObject go = null;
            var pool = candyPrefab.Pool;

            switch (pool)
            {
                case Candy.PoolType.V_BLUE:
                    go = _poolManager.V_BlueCandyPool.Get();
                    break;
                case Candy.PoolType.V_YELLOW:
                    go = _poolManager.V_YellowCandyPool.Get();
                    break;
                case Candy.PoolType.V_RED:
                    go = _poolManager.V_RedCandyPool.Get();
                    break;
                case Candy.PoolType.V_GREEN:
                    go = _poolManager.V_GreenCandyPool.Get();
                    break;
                case Candy.PoolType.V_PURPLE:
                    go = _poolManager.V_PurpleCandyPool.Get();
                    break;
                case Candy.PoolType.V_PINK:
                    go = _poolManager.V_PinkCandyPool.Get();
                    break;

                case Candy.PoolType.CHOCOLATE_MILK:
                    go = _poolManager.ChocolateMilkCandyPool.Get();
                    break;

                case Candy.PoolType.H_BLUE:
                    go = _poolManager.H_BlueCandyPool.Get();
                    break;
                case Candy.PoolType.H_YELLOW:
                    go = _poolManager.H_YellowCandyPool.Get();
                    break;
                case Candy.PoolType.H_RED:
                    go = _poolManager.H_RedCandyPool.Get();
                    break;
                case Candy.PoolType.H_GREEN:
                    go = _poolManager.H_GreenCandyPool.Get();
                    break;
                case Candy.PoolType.H_PURPLE:
                    go = _poolManager.H_PurpleCandyPool.Get();
                    break;
                case Candy.PoolType.H_PINK:
                    go = _poolManager.H_PinkCandyPool.Get();
                    break;

                case Candy.PoolType.BLUE:
                    go = _poolManager.BlueCandyPool.Get();
                    break;
                case Candy.PoolType.YELLOW:
                    go = _poolManager.YellowCandyPool.Get();
                    break;
                case Candy.PoolType.RED:
                    go = _poolManager.RedCandyPool.Get();
                    break;
                case Candy.PoolType.GREEN:
                    go = _poolManager.GreenCandyPool.Get();
                    break;
                case Candy.PoolType.PURPLE:
                    go = _poolManager.PurpleCandyPool.Get();
                    break;
                case Candy.PoolType.PINK:
                    go = _poolManager.PinkCandyPool.Get();
                    break;

                case Candy.PoolType.STING_BLUE:
                    go = _poolManager.StingBluePool.Get();
                    break;
                case Candy.PoolType.STING_YELLOW:
                    go = _poolManager.StingYellowPool.Get();
                    break;
                case Candy.PoolType.STING_RED:
                    go = _poolManager.StingRedPool.Get();
                    break;
                case Candy.PoolType.STING_GREEN:
                    go = _poolManager.StingGreenPool.Get();
                    break;
                case Candy.PoolType.STING_PURPLE:
                    go = _poolManager.StingPurplePool.Get();
                    break;
                case Candy.PoolType.STING_PINK:
                    go = _poolManager.StingPinkPool.Get();
                    break;

                case Candy.PoolType.SWIRL_BLUE:
                    go = _poolManager.SwirlBlueCandyPool.Get();
                    break;
                case Candy.PoolType.SWIRL_YELLOW:
                    go = _poolManager.SwirlYellowCandyPool.Get();
                    break;
                case Candy.PoolType.SWIRL_RED:
                    go = _poolManager.SwirlRedCandyPool.Get();
                    break;
                case Candy.PoolType.SWIRL_GREEN:
                    go = _poolManager.SwirlGreenCandyPool.Get();
                    break;
                case Candy.PoolType.SWIRL_PURPLE:
                    go = _poolManager.SwirlPurpleCandyPool.Get();
                    break;
                case Candy.PoolType.SWIRL_PINK:
                    go = _poolManager.SwirlPinkCandyPool.Get();
                    break;
            }

            return go;
        }

        void InternalDestroy(Candy candy)
        {
            if (!candy)
                Debug.Log("candy = null");
            var pool = candy.Pool;

            switch (pool)
            {
                case Candy.PoolType.V_BLUE:
                    _poolManager.V_BlueCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.V_YELLOW:
                    _poolManager.V_YellowCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.V_RED:
                    _poolManager.V_RedCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.V_GREEN:
                    _poolManager.V_GreenCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.V_PURPLE:
                    _poolManager.V_PurpleCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.V_PINK:
                    _poolManager.V_PinkCandyPool.Release(candy.gameObject);
                    break;

                case Candy.PoolType.CHOCOLATE_MILK:
                    _poolManager.ChocolateMilkCandyPool.Release(candy.gameObject);
                    break;

                case Candy.PoolType.H_BLUE:
                    _poolManager.H_BlueCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.H_YELLOW:
                    _poolManager.H_YellowCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.H_RED:
                    _poolManager.H_RedCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.H_GREEN:
                    _poolManager.H_GreenCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.H_PURPLE:
                    _poolManager.H_PurpleCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.H_PINK:
                    _poolManager.H_PinkCandyPool.Release(candy.gameObject);
                    break;

                case Candy.PoolType.BLUE:
                    _poolManager.BlueCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.YELLOW:
                    _poolManager.YellowCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.RED:
                    _poolManager.RedCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.GREEN:
                    _poolManager.GreenCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.PURPLE:
                    _poolManager.PurpleCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.PINK:
                    _poolManager.PinkCandyPool.Release(candy.gameObject);
                    break;

                case Candy.PoolType.STING_BLUE:
                    _poolManager.StingBluePool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.STING_YELLOW:
                    _poolManager.StingYellowPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.STING_RED:
                    _poolManager.StingRedPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.STING_GREEN:
                    _poolManager.StingGreenPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.STING_PURPLE:
                    _poolManager.StingPurplePool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.STING_PINK:
                    _poolManager.StingPinkPool.Release(candy.gameObject);
                    break;

                case Candy.PoolType.SWIRL_BLUE:
                    _poolManager.SwirlBlueCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.SWIRL_YELLOW:
                    _poolManager.SwirlYellowCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.SWIRL_RED:
                    _poolManager.SwirlRedCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.SWIRL_GREEN:
                    _poolManager.SwirlGreenCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.SWIRL_PURPLE:
                    _poolManager.SwirlPurpleCandyPool.Release(candy.gameObject);
                    break;
                case Candy.PoolType.SWIRL_PINK:
                    _poolManager.SwirlPinkCandyPool.Release(candy.gameObject);
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