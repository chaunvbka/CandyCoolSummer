#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using TMPro;
    using UnityEngine;

    public class FpsDisplay : IDisposable
    {
        private bool _dispose = false;
        private static FpsDisplay s_Instance;

        private readonly TextMeshProUGUI _fpsText;
        private float _elapsedTime = 1.0f;
        private int _frameCount = 0;

        public FpsDisplay()
        {
            if (s_Instance != null)
            {
                Debug.LogError("FpsDisplay instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;

            var prefab = Resources.Load<GameObject>("FpsDisplay");
            var instance = UnityEngine.Object.Instantiate(prefab);
            _fpsText = instance.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnUpdate(bool showFPS)
        {
            if (_fpsText)
                _fpsText.gameObject.SetActive(showFPS);

            if (_elapsedTime > 0)
            {
                _elapsedTime -= Time.deltaTime;
                _frameCount += 1;
            }
            else
            {
                _fpsText.text = string.Format("Fps: {0:0.}", _frameCount);

                _elapsedTime = 1.0f;
                _frameCount = 0;
            }
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            s_Instance = null;
        }
    }
}

