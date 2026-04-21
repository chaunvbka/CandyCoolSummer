#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using System.Collections;
    using UnityEngine;

    using Texell.Utility;

    public class PoolManager : IDisposable
    {
        private static PoolManager s_Instance;
        public static PoolManager Instance => s_Instance;

        /// <summary>
        /// We can use all pool data in PoolManager if Initialize function done.
        /// </summary>
        public bool Done => _done;

        private bool _dispose = false;
        private bool _done = false;
        private readonly AssetManager assetManager = AssetManager.Instance;

        // Candy pool.
        public ObjectPool V_BlueCandyPool = new();
        public ObjectPool V_YellowCandyPool = new();
        public ObjectPool V_RedCandyPool = new();
        public ObjectPool V_GreenCandyPool = new();
        public ObjectPool V_PurpleCandyPool = new();
        public ObjectPool V_PinkCandyPool = new();

        public ObjectPool ChocolateMilkCandyPool = new();

        public ObjectPool H_BlueCandyPool = new();
        public ObjectPool H_YellowCandyPool = new();
        public ObjectPool H_RedCandyPool = new();
        public ObjectPool H_GreenCandyPool = new();
        public ObjectPool H_PurpleCandyPool = new();
        public ObjectPool H_PinkCandyPool = new();

        public ObjectPool BlueCandyPool = new();
        public ObjectPool YellowCandyPool = new();
        public ObjectPool RedCandyPool = new();
        public ObjectPool GreenCandyPool = new();
        public ObjectPool PurpleCandyPool = new();
        public ObjectPool PinkCandyPool = new();

        public ObjectPool STING_BluePool = new();
        public ObjectPool STING_YellowPool = new();
        public ObjectPool STING_RedPool = new();
        public ObjectPool STING_GreenPool = new();
        public ObjectPool STING_PurplePool = new();
        public ObjectPool STING_PinkPool = new();

        public ObjectPool SWIRL_BlueCandyPool = new();
        public ObjectPool SWIRL_YellowCandyPool = new();
        public ObjectPool SWIRL_RedCandyPool = new();
        public ObjectPool SWIRL_GreenCandyPool = new();
        public ObjectPool SWIRL_PurpleCandyPool = new();
        public ObjectPool SWIRL_PinkCandyPool = new();

        public PoolManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("PoolManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public IEnumerator Initialize()
        {
            // Candy pool.
            yield return V_BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_BLUE], 5);
            yield return V_YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_YELLOW], 5);
            yield return V_RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_RED], 5);
            yield return V_GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_GREEN], 5);
            yield return V_PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_PURPLE], 5);
            yield return V_PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_PINK], 5);

            yield return ChocolateMilkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.CHOCOLATE_MILK], 3);

            yield return H_BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_BLUE], 5);
            yield return H_YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_YELLOW], 5);
            yield return H_RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_RED], 5);
            yield return H_GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_GREEN], 5);
            yield return H_PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_PURPLE], 5);
            yield return H_PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_PINK], 5);

            yield return BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.BLUE], 10);
            yield return YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.YELLOW], 10);
            yield return RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.RED], 10);
            yield return GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.GREEN], 10);
            yield return PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.PURPLE], 10);
            yield return PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.PINK], 10);

            yield return STING_BluePool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_BLUE], 5);
            yield return STING_YellowPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_YELLOW], 5);
            yield return STING_RedPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_RED], 5);
            yield return STING_GreenPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_GREEN], 5);
            yield return STING_PurplePool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_PURPLE], 5);
            yield return STING_PinkPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_PINK], 5);

            yield return SWIRL_BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_BLUE], 5);
            yield return SWIRL_YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_YELLOW], 5);
            yield return SWIRL_RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_RED], 5);
            yield return SWIRL_GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_GREEN], 5);
            yield return SWIRL_PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PURPLE], 5);
            yield return SWIRL_PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PINK], 5);

            _done = true;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            // Candy pool.
            V_BlueCandyPool?.Dispose();
            V_YellowCandyPool?.Dispose();
            V_RedCandyPool?.Dispose();
            V_GreenCandyPool?.Dispose();
            V_PurpleCandyPool?.Dispose();
            V_PinkCandyPool?.Dispose();

            ChocolateMilkCandyPool?.Dispose();

            H_BlueCandyPool?.Dispose();
            H_YellowCandyPool?.Dispose();
            H_RedCandyPool?.Dispose();
            H_GreenCandyPool?.Dispose();
            H_PurpleCandyPool?.Dispose();
            H_PinkCandyPool?.Dispose();

            BlueCandyPool?.Dispose();
            YellowCandyPool?.Dispose();
            RedCandyPool?.Dispose();
            GreenCandyPool?.Dispose();
            PurpleCandyPool?.Dispose();
            PinkCandyPool?.Dispose();

            STING_BluePool?.Dispose();
            STING_YellowPool?.Dispose();
            STING_RedPool?.Dispose();
            STING_GreenPool?.Dispose();
            STING_PurplePool?.Dispose();
            STING_PinkPool?.Dispose();

            SWIRL_BlueCandyPool?.Dispose();
            SWIRL_YellowCandyPool?.Dispose();
            SWIRL_RedCandyPool?.Dispose();
            SWIRL_GreenCandyPool?.Dispose();
            SWIRL_PurpleCandyPool?.Dispose();
            SWIRL_PinkCandyPool?.Dispose();

            s_Instance = null;
        }
    }
}
