#pragma warning disable IDE0130

//#define EXTERNAL_MODULES

namespace Texell.CoreModule.Application
{
    using UnityEngine;
    using Texell.Utility;
    using Texell.CoreModule.ProcessManager;
    using Texell.CoreModule.UI;
    using Texell.CoreModule;

#if EXTERNAL_MODULES
    using Texell.AdsModule;
    using Texell.IAPModule;
    using Texell.GPGModule;
#endif

    /// <summary>
    /// UnityApp object is an instance application.
    /// Manage all modules.
    /// </summary>
    public class UnityApp : MonoBehaviour
    {
        private static UnityApp s_Instance;
        private ApplicationSettings _appSettings;

        private FpsDisplay _fpsDisplay;
        private UIManager _uiManager;
        private Transition _transition;
        private AssetManager _assetManager;
        private AppDataManager _appDataManager;
        private AudioManager _audioManager;
        private InputManager _inputManager;
        private ProcessManager _processManager;
        private PoolManager _poolManager;

#if EXTERNAL_MODULES
        private AdsManager _adsManager;
        private GPGSAndroid _gpgsAndroid;
        private IAPManager _iapManager;
#endif

        public static UnityApp Instance => s_Instance;
        public ApplicationSettings AppSettings => _appSettings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            var go = new GameObject("UnityApp");
            go.AddComponent<UnityApp>();
        }

        // Note: Do not call initialize (eg: s_instance = this) from method mark with 
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)].
        void Awake()
        {
            // Set the target frame rate to 120 FPS and disable vSync.
            // This is done to ensure that the game runs at a consistent frame rate.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;

            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _appSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");

            Debug.unityLogger.logEnabled = _appSettings.DebugMode;

            if (!_appSettings.Active)
            {
                return;
            }

            NonMono.CreateMonoInstance(s_Instance);

            _fpsDisplay = new();
            _uiManager = new();
            _transition = new();
            _assetManager = new();
            _appDataManager = new();
            _audioManager = new();
            _inputManager = new();
            _processManager = new();
            _poolManager = new();

#if EXTERNAL_MODULES
            _adsManager = new();
            _iapManager = new();
            _gpgsAndroid = new();

            _adsManager.Initialize();
            _gpgsAndroid.Initialize();
#endif

            _uiManager.Initialize();
            _transition.Initialize();

            // Set up process manager with number of process to manage.
            _processManager.SetCount(System.Enum.GetValues(typeof(ProcessIndex)).Length);
            _processManager.CreateProcess<Processes.LoadingProcess>(byte.MaxValue, true);
            _processManager.CreateProcess<Processes.HomeProcess>((byte)ProcessIndex.Home);
            _processManager.CreateProcess<Processes.GameProcess>((byte)ProcessIndex.Game);
            _processManager.CreateProcess<Processes.EndProcess>((byte)ProcessIndex.End);
        }

        void Start()
        {
            if (!_appSettings.Active)
            {
                return;
            }

#if EXTERNAL_MODULES
            _adsManager.InitializeMobileAds();
#endif
            _audioManager.InitializeAudioSettings();

            if (!_assetManager.Loaded)
            {
                StartCoroutine(_assetManager.LoadAssetAsync());
            }

            // Run a process after all module aready initialize.
            _processManager.Run();
        }

        void Update()
        {
            if (!_appSettings.Active)
            {
                return;
            }
#if EXTERNAL_MODULES
            _adsManager.OnUpdate();
#endif
            _inputManager.OnUpdate();
            _processManager.OnUpdate();
            _fpsDisplay.OnUpdate(_appSettings.ShowFPS);
        }

        void OnApplicationQuit()
        {
            if (!_appSettings.Active)
            {
                s_Instance = null;
                return;
            }
            NonMono.DestroyMonoInstance();

            _fpsDisplay?.Dispose();
            _uiManager?.Dispose();
            _transition?.Dispose();
            _assetManager?.Dispose();
            _appDataManager?.Dispose();
            _audioManager?.Dispose();
            _inputManager?.Dispose();
            _processManager?.Dispose();
            _poolManager?.Dispose();

#if EXTERNAL_MODULES
            _iapManager?.Dispose();
            _gpgsAndroid?.Dispose();
            _adsManager?.Dispose();
#endif

            s_Instance = null;
        }
    }

}