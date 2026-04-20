#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Texell.CoreModule;
    using Texell.Utility;

    using AudioSettings = AudioSettings;

    public class AudioSettingsElement : VisualElement, IDisposable
    {
        private readonly VisualElement _root;
        private Toggle _musicMuteToggle;
        private Toggle _sfxMuteToggle;
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;
        private Label _musicVolumeLabel;
        private Label _sfxVolumeLabel;

        private readonly AudioManager _audioManager = AudioManager.Instance;
        private readonly AudioSettings _audioSettings = AudioSettings.Instance;

        public AudioClip ClickSound;

        public AudioSettingsElement(VisualElement rootElement)
        {
            _root = rootElement;

            LoadElements();
            RegisterEvents();

            bool musicMute = Convert.ToBoolean(PlayerPrefs.GetInt(AudioSettings.MusicMute_Key));
            bool sfxMute = Convert.ToBoolean(PlayerPrefs.GetInt(AudioSettings.SFXMute_Key));

            float musicVolume = PlayerPrefs.GetFloat(AudioSettings.MusicVolume_Key);
            float sfxVolume = PlayerPrefs.GetFloat(AudioSettings.SFXVolume_Key);

            NonMono.StartCoroutine(Initialize(musicMute, sfxMute));

            _musicVolumeSlider.value = musicVolume * 100;
            _sfxVolumeSlider.value = sfxVolume * 100;

            _musicVolumeLabel.text = (musicVolume * 100).ToString("F0");
            _sfxVolumeLabel.text = (sfxVolume * 100).ToString("F0");
        }

        IEnumerator Initialize(bool musicMute, bool sfxMute)
        {
            yield return new WaitForEndOfFrame();
            _musicMuteToggle.value = !musicMute;
            _sfxMuteToggle.value = !sfxMute;
        }

        void LoadElements()
        {
            _musicMuteToggle = _root.Q<Toggle>("settings__music-mute-toggle");
            _sfxMuteToggle = _root.Q<Toggle>("settings__sfx-mute-toggle");
            _musicVolumeSlider = _root.Q<Slider>("settings__music-volume-slider");
            _sfxVolumeSlider = _root.Q<Slider>("settings__sfx-volume-slider");
            _musicVolumeLabel = _root.Q<Label>("settings__music-volume-label");
            _sfxVolumeLabel = _root.Q<Label>("settings__sfx-volume-label");
        }

        void RegisterEvents()
        {
            _musicMuteToggle.RegisterCallback<ClickEvent>(OnMusicMuteToggleClicked);
            _sfxMuteToggle.RegisterCallback<ClickEvent>(OnSFXMuteToggleClicked);
            _musicMuteToggle.RegisterValueChangedCallback(OnMusicMuteToggleValueChanged);
            _sfxMuteToggle.RegisterValueChangedCallback(OnSFXMuteToggleValueChanged);
            _musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeSliderValueChanged);
            _sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeSliderValueChanged);
        }

        void UnregisterEvents()
        {
            _musicMuteToggle.UnregisterCallback<ClickEvent>(OnMusicMuteToggleClicked);
            _sfxMuteToggle.UnregisterCallback<ClickEvent>(OnSFXMuteToggleClicked);
            _musicMuteToggle.UnregisterCallback<ChangeEvent<bool>>(OnMusicMuteToggleValueChanged);
            _sfxMuteToggle.UnregisterCallback<ChangeEvent<bool>>(OnSFXMuteToggleValueChanged);
            _musicVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnMusicVolumeSliderValueChanged);
            _sfxVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnSFXVolumeSliderValueChanged);
        }

        void OnMusicMuteToggleClicked(ClickEvent evt)
        {
            _audioManager.PlaySFX(ClickSound);
        }

        void OnSFXMuteToggleClicked(ClickEvent evt)
        {
            _audioManager.PlaySFX(ClickSound);
        }

        void OnMusicMuteToggleValueChanged(ChangeEvent<bool> evt)
        {
            _audioSettings.OnMusicMuteChanged(!evt.newValue);
        }

        void OnSFXMuteToggleValueChanged(ChangeEvent<bool> evt)
        {
            _audioSettings.OnSFXMuteChanged(!evt.newValue);
        }


        void OnMusicVolumeSliderValueChanged(ChangeEvent<float> evt)
        {
            _musicVolumeLabel.text = evt.newValue.ToString("F0");
            _musicMuteToggle.value = evt.newValue > 0;
            _audioSettings.OnMusicVolumeChanged(evt.newValue / 100);
        }

        void OnSFXVolumeSliderValueChanged(ChangeEvent<float> evt)
        {
            _sfxVolumeLabel.text = evt.newValue.ToString("F0");
            _sfxMuteToggle.value = evt.newValue > 0;
            _audioSettings.OnSFXVolumeChanged(evt.newValue / 100);
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}