#pragma warning disable IDE0130

namespace Texell.CoreModule
{


    using System;
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioSettings : IDisposable
    {
        private static AudioSettings s_instance;
        public static AudioSettings Instance => s_instance ??= new AudioSettings();

        // Note: To rename exposed parameter AudioMixer.
        // 1. Double click AudioMixer. 
        // 2. On Exposed Parameter list, click. Chose an old name, right click > Rename.
        // AudioMixer group names
        private const string _MasterParameterVolume = "MasterVolume";
        private const string _MusicParameterVolume = "MusicVolume";
        private const string _SFXParameterVolume = "SFXVolume";

        // PlayerPrefs keys
        public const string MasterVolume_Key = "MasterVolumeKey";
        public const string MusicVolume_Key = "MusicVolumeKey";
        public const string SFXVolume_Key = "SFXVolumeKey";

        public const string MasterMute_Key = "MasterMuteKey";
        public const string MusicMute_Key = "MusicMuteKey";
        public const string SFXMute_Key = "SFXMuteKey";

        // PlayersPrefs default value
        private const float _Default_MasterVolume_Value = 1.0f;
        private const float _Default_MusicVolume_Value = 1.0f;
        private const float _Default_SFXVolume_Value = 1.0f;

        private const int _Default_MasterMute_Value = 0;
        private const int _Default_MusicMute_Value = 0;
        private const int _Default_SFXMute_Value = 0;

        private readonly AudioMixer _audioMixer;


        // AudioMixer event
        public event Action<bool> MasterMuteChanged;
        public event Action<bool> MusicMuteChanged;
        public event Action<bool> SFXMuteChanged;
        public event Action<float> MasterVolumeChanged;
        public event Action<float> MusicVolumeChanged;
        public event Action<float> SFXVolumeChanged;

        public AudioSettings()
        {
            if (s_instance != null)
            {
                return;
            }
            _audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            RegisterEvents();
            SetDefaultSettingsValue();
        }

        public void LoadPreviousSettings()
        {
            // bool masterMute = Convert.ToBoolean(PlayerPrefs.GetInt(MasterMute_Key));
            bool musicMute = Convert.ToBoolean(PlayerPrefs.GetInt(MusicMute_Key));
            bool sfxMute = Convert.ToBoolean(PlayerPrefs.GetInt(SFXMute_Key));

            // HandleMasterMuteChanged(masterMute);
            HandleMusicMuteChanged(musicMute);
            HandleSFXMuteChanged(sfxMute);
        }

        void RegisterEvents()
        {
            MasterMuteChanged += HandleMasterMuteChanged;
            MusicMuteChanged += HandleMusicMuteChanged;
            SFXMuteChanged += HandleSFXMuteChanged;
            MasterVolumeChanged += HandleMasterVolumeChanged;
            MusicVolumeChanged += HandleMusicVolumeChanged;
            SFXVolumeChanged += HandleSFXVolumeChanged;
        }

        void UnregisterEvents()
        {
            MasterMuteChanged -= HandleMasterMuteChanged;
            MusicMuteChanged -= HandleMusicMuteChanged;
            SFXMuteChanged -= HandleSFXMuteChanged;
            MasterVolumeChanged -= HandleMasterVolumeChanged;
            MusicVolumeChanged -= HandleMusicVolumeChanged;
            SFXVolumeChanged -= HandleSFXVolumeChanged;
        }

        void SetDefaultSettingsValue()
        {
            if (!PlayerPrefs.HasKey(MasterVolume_Key))
            {
                PlayerPrefs.SetFloat(MasterVolume_Key, _Default_MasterVolume_Value);
            }
            if (!PlayerPrefs.HasKey(MusicVolume_Key))
            {
                PlayerPrefs.SetFloat(MusicVolume_Key, _Default_MusicVolume_Value);
            }
            if (!PlayerPrefs.HasKey(SFXVolume_Key))
            {
                PlayerPrefs.SetFloat(SFXVolume_Key, _Default_SFXVolume_Value);
            }

            if (!PlayerPrefs.HasKey(MasterMute_Key))
            {
                PlayerPrefs.SetInt(MasterMute_Key, _Default_MasterMute_Value);
            }
            if (!PlayerPrefs.HasKey(MusicMute_Key))
            {
                PlayerPrefs.SetInt(MusicMute_Key, _Default_MusicMute_Value);
            }
            if (!PlayerPrefs.HasKey(SFXMute_Key))
            {
                PlayerPrefs.SetInt(SFXMute_Key, _Default_SFXMute_Value);
            }
        }

        public void OnMasterMuteChanged(bool mute)
        {
            MasterMuteChanged?.Invoke(mute);
        }

        public void OnMusicMuteChanged(bool mute)
        {
            MusicMuteChanged?.Invoke(mute);
        }

        public void OnSFXMuteChanged(bool mute)
        {
            SFXMuteChanged?.Invoke(mute);
        }

        public void OnMasterVolumeChanged(float volume)
        {
            MasterVolumeChanged?.Invoke(volume);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            MusicVolumeChanged?.Invoke(volume);
        }

        public void OnSFXVolumeChanged(float volume)
        {
            SFXVolumeChanged?.Invoke(volume);
        }

        void HandleMasterMuteChanged(bool mute)
        {
            int value = Convert.ToInt32(mute);
            PlayerPrefs.SetInt(MasterMute_Key, value);

            float volume = mute ? 0 : PlayerPrefs.GetFloat(MasterVolume_Key);
            _audioMixer.SetFloat(_MasterParameterVolume, ConvertLinearToDecibel(volume));
        }

        void HandleMusicMuteChanged(bool mute)
        {
            int value = Convert.ToInt32(mute);
            PlayerPrefs.SetInt(MusicMute_Key, value);

            float volume = mute ? 0 : PlayerPrefs.GetFloat(MusicVolume_Key);
            _audioMixer.SetFloat(_MusicParameterVolume, ConvertLinearToDecibel(volume));
        }

        void HandleSFXMuteChanged(bool mute)
        {
            int value = Convert.ToInt32(mute);
            PlayerPrefs.SetInt(SFXMute_Key, value);

            float volume = mute ? 0 : PlayerPrefs.GetFloat(SFXVolume_Key);
            _audioMixer.SetFloat(_SFXParameterVolume, ConvertLinearToDecibel(volume));
        }

        void HandleMasterVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat(MasterVolume_Key, volume);
            _audioMixer.SetFloat(_MasterParameterVolume, ConvertLinearToDecibel(volume));
        }

        void HandleMusicVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat(MusicVolume_Key, volume);
            _audioMixer.SetFloat(_MusicParameterVolume, ConvertLinearToDecibel(volume));
        }

        void HandleSFXVolumeChanged(float volume)
        {
            PlayerPrefs.SetFloat(SFXVolume_Key, volume);
            _audioMixer.SetFloat(_SFXParameterVolume, ConvertLinearToDecibel(volume));
        }

        // Convert from the logarithmic AudioMixer scale (-80dB to 0dB) to linear UI scale (0 to 1) and vice versa
        private float ConvertLinearToDecibel(float linearVolume)
        {
            return Mathf.Log10(Mathf.Max(0.0001f, linearVolume)) * 20.0f;
        }

        // private float ConvertDecibelToLinear(float decibelVolume)
        // {
        //     return Mathf.Pow(10, decibelVolume / 20.0f);
        // }


        public void Dispose()
        {
            UnregisterEvents();
            s_instance = null;
        }
    }

}