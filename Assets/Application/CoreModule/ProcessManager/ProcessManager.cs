#pragma warning disable IDE0130

namespace Texell.CoreModule
{
    using System;
    using System.Collections;
    using Texell.Utility;
    using UnityEngine;

    public class ProcessManager : IDisposable
    {
        private static ProcessManager s_Instance;
        public static ProcessManager Instance => s_Instance;

        private bool _dispose = false;
        /// <summary>
        /// Number of process that can be managed.
        /// </summary>
        private int _count;
        private IProcess[] _processList;
        private IProcess _loadingProcess;

        private IProcess _currentProcess;


        public ProcessManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("ProcessManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;
        }

        /// <summary>
        /// Set the number of process that can be managed.
        /// </summary>
        /// <param name="count"></param>
        public void SetCount(int count)
        {
            if (_processList != null)
            {
                Debug.LogError("Cannot set count process after create process.");
                return;
            }
            _count = count;
        }

        public void CreateProcess<T>(byte index, bool loading = false) where T : IProcess, new()
        {
            if (_processList == null)
            {
                if (_count == 0)
                {
                    Debug.LogError("Set count process once before create process.");
                    return;
                }
                _processList = new IProcess[_count];
            }

            var process = new T();
            if (loading)
            {
                _loadingProcess = process;
            }
            else
            {
                _processList[index] = process;
            }
        }

        /// <summary>
        /// Destroy loading process.
        /// </summary>
        public void DestroyLoading()
        {
            _loadingProcess = null;
        }

        /// <summary>
        /// Run the active process. Only one process can run at a time.
        /// </summary>
        public void Run()
        {
            _currentProcess = _loadingProcess;
            _currentProcess?.OnStart();
        }

        public void TransitionTo(ProcessIndex index)
        {
            NonMono.StartCoroutine(FadeInOut(index));
        }

        public void OnUpdate()
        {
            _currentProcess?.OnUpdate();
        }

        IEnumerator FadeInOut(ProcessIndex index)
        {
            yield return Transition.Instance.FadeIn();
            _currentProcess.OnExit();
            _currentProcess = _processList[(int)index];
            _currentProcess?.OnStart();
            yield return Transition.Instance.FadeOut();
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            _currentProcess?.OnExit();
            _currentProcess = null;
            _loadingProcess = null;
            _processList = null;
            s_Instance = null;
        }
    }
}