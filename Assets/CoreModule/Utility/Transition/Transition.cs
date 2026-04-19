#pragma warning disable IDE0130

namespace Texell.Utility
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class Transition : IDisposable
    {
        private bool _dispose = false;
        private static Transition s_Instance;
        public static Transition Instance => s_Instance;

        private Canvas _canvas;
        private Image _fadeImage;

        private const int MinTransitionLayer = -100;
        private const int MaxTransitionLayer = 100;
        private const float FadeDuration = 1.0f;

        private readonly Color _transparent = new(0, 0, 0, 0);
        private readonly Color _black = new(0, 0, 0, 1);

        public Transition()
        {
            if (s_Instance != null)
            {
                Debug.LogError("Transition instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        public void Initialize()
        {
            var go = Resources.Load<GameObject>("Transition");
            if (go == null)
            {
                Debug.LogError("Failed to load Transition prefab.");
                return;
            }
            var instance = UnityEngine.Object.Instantiate(go);
            instance.name = "Transition";

            var canvas = instance.GetComponent<Canvas>();
            var fadeImage = instance.GetComponentInChildren<Image>();
            if (canvas == null || fadeImage == null)
            {
                Debug.LogError("Failed to find Canvas or Image component in Transition object.");
                return;
            }

            _canvas = canvas;
            _fadeImage = fadeImage;
            _canvas.sortingOrder = MinTransitionLayer;

            // Hide ui for input get active.
            _fadeImage.gameObject.SetActive(false);
        }

        public IEnumerator FadeIn()
        {
            _canvas.sortingOrder = MaxTransitionLayer;
            _fadeImage.gameObject.SetActive(true);

            yield return FadeInRoutine();

        }

        public IEnumerator FadeOut()
        {
            yield return FadeOutRoutine();

            _canvas.sortingOrder = MinTransitionLayer;
            _fadeImage.gameObject.SetActive(false);
        }

        IEnumerator FadeInRoutine()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < FadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _fadeImage.color = Color.Lerp(_transparent, _black, elapsedTime / FadeDuration);
                yield return null;
            }
        }

        IEnumerator FadeOutRoutine()
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < FadeDuration)
            {
                if (_fadeImage == null)
                {
                    continue;
                }
                elapsedTime += Time.deltaTime;

                _fadeImage.color = Color.Lerp(_black, _transparent, elapsedTime / FadeDuration);
                yield return null;
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
