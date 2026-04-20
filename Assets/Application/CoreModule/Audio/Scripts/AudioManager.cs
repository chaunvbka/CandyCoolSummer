#pragma warning disable IDE0130

namespace Texell.CoreModule
{

    using System;
    using UnityEngine;

    public class AudioManager : IDisposable
    {
        private static AudioManager s_Instance = null;
        public static AudioManager Instance => s_Instance;

        private readonly AudioSource _musicAudioSource;
        private readonly AudioSource _sfxAudioSource;
        private AudioSettings _audioSettings;


        public AudioManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("AudioManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;

            var go = Resources.Load<GameObject>("AudioManager");
            if (go == null)
            {
                Debug.LogError("Failed to load AudioManager prefab.");
                return;
            }
            var instance = UnityEngine.Object.Instantiate(go);
            instance.name = "AudioManager";

            instance.transform.Find("MusicAudioSource").TryGetComponent(out _musicAudioSource);
            if (_musicAudioSource == null)
            {
                Debug.LogError("Failed to find MusicAudioSource component.");
                return;
            }
            instance.transform.Find("SFXAudioSource").TryGetComponent(out _sfxAudioSource);
            if (_sfxAudioSource == null)
            {
                Debug.LogError("Failed to find SFXAudioSource component.");
                return;
            }
        }

        /// <summary>
        /// Initialize the AudioSettings and load previous settings on Start() function.
        /// </summary>
        public void InitializeAudioSettings()
        {
            _audioSettings = AudioSettings.Instance;
            _audioSettings?.LoadPreviousSettings();
            RegisterEvents();
        }

        void RegisterEvents()
        {
            _audioSettings.MusicMuteChanged += HandleMusicMuteChanged;
        }

        void UnregisterEvents()
        {
            _audioSettings.MusicMuteChanged -= HandleMusicMuteChanged;
        }

        void HandleMusicMuteChanged(bool mute)
        {
            if (!s_Instance._musicAudioSource.isPlaying && !mute)
            {
                s_Instance._musicAudioSource.Play();
            }
        }

        // Play music with the specified AudioClip
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("The AudioClip value is null.");
                return;
            }

            bool musicMute = Convert.ToBoolean(PlayerPrefs.GetInt(AudioSettings.MusicMute_Key));

            if (!_musicAudioSource.isPlaying)
            {
                _musicAudioSource.clip = clip;
                if (!musicMute)
                    _musicAudioSource.Play();
            }
        }

        // Play sound effects with a short AudioClip
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("The AudioClip value is null.");
                return;
            }

            _sfxAudioSource.PlayOneShot(clip);
        }

        public void Dispose()
        {
            UnregisterEvents();
            _audioSettings?.Dispose();
            s_Instance = null;
        }
    }
}