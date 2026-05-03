#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class AssetManager : IDisposable
    {
        private bool _dispose = false;
        private static AssetManager s_Instance;
        public static AssetManager Instance => s_Instance;

        private bool _loaded = false;

        /// <summary>
        /// Return true if assets have been loaded.
        /// </summary>
        public bool Loaded => _loaded;

        /// <summary>
        /// Load large prefab game-background during a loading screen and keep it disabled 
        /// or hidden until needed.
        /// </summary>
        public GameObject Background;
        /// <summary>
        /// Select object show when click on a cell.
        /// </summary>
        public GameObject Select;
        public GameObject HideCandyPrefab;
        public AudioClip[] AudioClips = new AudioClip[Enum.GetNames(typeof(AudioClipIndex)).Length];
        public GameObject[] CandyPrefabs = new GameObject[Enum.GetNames(typeof(CandyIndex)).Length];
        public GameObject[] ObstaclePrefabs = new GameObject[Enum.GetNames(typeof(ObstacleIndex)).Length];

        public AssetManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("AssetManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        /// <summary>
        /// Load all assets asynchronously.
        /// </summary>
        public IEnumerator LoadAssetAsync()
        {
            ResourceRequest request = null;
            UnityEngine.Object go = null;

            // Background
            request = Resources.LoadAsync<GameObject>(AssetPath.BackgroundPrefabPath);
            yield return request;
            go = UnityEngine.Object.Instantiate(request.asset);
            Background = go as GameObject;
            Background.SetActive(false);

            // Select object
            request = Resources.LoadAsync<GameObject>(AssetPath.SelectPrefabPath);
            yield return request;
            go = UnityEngine.Object.Instantiate(request.asset);
            Select = go as GameObject;
            Select.SetActive(false);

            // HideCandy prefab
            request = Resources.LoadAsync<GameObject>(AssetPath.HideCandyPrefabPath);
            yield return request;
            HideCandyPrefab = request.asset as GameObject;


            for (int i = 0; i < Enum.GetNames(typeof(AudioClipIndex)).Length; i++)
            {
                request = Resources.LoadAsync<AudioClip>(AssetPath.AudioClipPaths[i]);
                yield return request;
                AudioClips[i] = request.asset as AudioClip;
            }

            for (int i = 0; i < Enum.GetNames(typeof(CandyIndex)).Length; i++)
            {
                request = Resources.LoadAsync<GameObject>(AssetPath.CandyPrefabPaths[i]);
                yield return request;
                CandyPrefabs[i] = request.asset as GameObject;
            }

            for (int i = 0; i < Enum.GetNames(typeof(ObstacleIndex)).Length; i++)
            {
                request = Resources.LoadAsync<GameObject>(AssetPath.ObstaclePrefabPaths[i]);
                yield return request;
                ObstaclePrefabs[i] = request.asset as GameObject;
            }

            _loaded = true;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            AudioClips = null;
            s_Instance = null;
        }

        ~AssetManager()
        {
            Dispose();
        }
    }

}