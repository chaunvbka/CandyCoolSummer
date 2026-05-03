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

        // Candy pools.
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

        public ObjectPool StingBluePool = new();
        public ObjectPool StingYellowPool = new();
        public ObjectPool StingRedPool = new();
        public ObjectPool StingGreenPool = new();
        public ObjectPool StingPurplePool = new();
        public ObjectPool StingPinkPool = new();

        public ObjectPool SwirlBlueCandyPool = new();
        public ObjectPool SwirlYellowCandyPool = new();
        public ObjectPool SwirlRedCandyPool = new();
        public ObjectPool SwirlGreenCandyPool = new();
        public ObjectPool SwirlPurpleCandyPool = new();
        public ObjectPool SwirlPinkCandyPool = new();

        // Obstacle pools.
        public ObjectPool TieRopePool = new();

        public PoolManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("PoolManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Initialize()
        {
            // Candy pools.
            V_BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_BLUE], 10);
            V_YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_YELLOW], 10);
            V_RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_RED], 10);
            V_GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_GREEN], 10);
            V_PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_PURPLE], 10);
            V_PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.V_PINK], 10);

            ChocolateMilkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.CHOCOLATE_MILK], 5);

            H_BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_BLUE], 10);
            H_YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_YELLOW], 10);
            H_RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_RED], 10);
            H_GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_GREEN], 10);
            H_PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_PURPLE], 10);
            H_PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.H_PINK], 10);

            BlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.BLUE], 20);
            YellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.YELLOW], 20);
            RedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.RED], 20);
            GreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.GREEN], 20);
            PurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.PURPLE], 20);
            PinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.PINK], 20);

            StingBluePool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_BLUE], 5);
            StingYellowPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_YELLOW], 5);
            StingRedPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_RED], 5);
            StingGreenPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_GREEN], 5);
            StingPurplePool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_PURPLE], 5);
            StingPinkPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.STING_PINK], 5);

            SwirlBlueCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_BLUE], 5);
            SwirlYellowCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_YELLOW], 5);
            SwirlRedCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_RED], 5);
            SwirlGreenCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_GREEN], 5);
            SwirlPurpleCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PURPLE], 5);
            SwirlPinkCandyPool.Initialize(assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PINK], 5);

            // Obstacle pools.
            TieRopePool.Initialize(assetManager.ObstaclePrefabs[(int)ObstacleIndex.TieRope], 5);

            _done = true;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            // Candy pools.
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

            StingBluePool?.Dispose();
            StingYellowPool?.Dispose();
            StingRedPool?.Dispose();
            StingGreenPool?.Dispose();
            StingPurplePool?.Dispose();
            StingPinkPool?.Dispose();

            SwirlBlueCandyPool?.Dispose();
            SwirlYellowCandyPool?.Dispose();
            SwirlRedCandyPool?.Dispose();
            SwirlGreenCandyPool?.Dispose();
            SwirlPurpleCandyPool?.Dispose();
            SwirlPinkCandyPool?.Dispose();

            // Obstacle pools.
            TieRopePool?.Dispose();

            s_Instance = null;
        }
    }
}
