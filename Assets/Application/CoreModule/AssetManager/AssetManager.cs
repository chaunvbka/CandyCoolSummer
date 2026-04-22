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
        public AudioClip[] AudioClips = new AudioClip[Enum.GetNames(typeof(AudioClipIndex)).Length];
        public GameObject[] CandyPrefabs = new GameObject[Enum.GetNames(typeof(CandyIndex)).Length];

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

            request = Resources.LoadAsync<GameObject>(AssetPath.BackgroundPath);
            yield return request;
            var bg = UnityEngine.Object.Instantiate(request.asset);
            Background = bg as GameObject;
            Background.SetActive(false);

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