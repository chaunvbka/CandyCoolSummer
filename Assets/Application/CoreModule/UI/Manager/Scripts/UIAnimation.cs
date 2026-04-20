#pragma warning disable IDE0130

namespace Texell.CoreModule.UI
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Texell.Utility;

    public enum AnimationType
    {
        TranslateRepeat,
        TranslateUp,
        TranslateDown,

        ScaleRepeat,
        ScaleUp,
        ScaleDown,

        RotateRepeat,
        RotateLeft,
        RotateRight,
    }

    public class UIAnimation : IDisposable
    {
        private static UIAnimation s_instance;
        public static UIAnimation Instance => s_instance ??= new UIAnimation();

        private VisualElement _visualElement;
        private string _ussClassName;

        public void Run(VisualElement visualElement, string ussClassName, AnimationType animationType)
        {
            _visualElement = visualElement;
            _ussClassName = ussClassName;

            switch (animationType)
            {
                case AnimationType.ScaleRepeat:
                    _visualElement.RegisterCallback<TransitionEndEvent>(OnTransitionEndEvent);
                    NonMono.StartCoroutine(ToggleClass(ussClassName));
                    break;
            }
        }

        public void Stop(VisualElement visualElement, string ussClassName, AnimationType animationType)
        {
            _visualElement = visualElement;
            _ussClassName = ussClassName;

            switch (animationType)
            {
                case AnimationType.ScaleRepeat:
                    _visualElement.UnregisterCallback<TransitionEndEvent>(OnTransitionEndEvent);
                    break;
            }
        }

        void OnTransitionEndEvent(TransitionEndEvent evt)
        {
            _visualElement.ToggleInClassList(_ussClassName);
        }

        IEnumerator ToggleClass(string ussClassName)
        {
            yield return new WaitForEndOfFrame();
            _visualElement?.ToggleInClassList(ussClassName);
        }

        IEnumerator AddClass(string ussClassName)
        {
            yield return new WaitForEndOfFrame();
            _visualElement?.AddToClassList(ussClassName);
        }

        IEnumerator RemoveClass(string ussClassName)
        {
            yield return new WaitForEndOfFrame();
            _visualElement?.RemoveFromClassList(ussClassName);
        }

        public void Dispose()
        {
            s_instance = null;
        }
    }
}