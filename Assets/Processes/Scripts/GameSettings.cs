#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{

    using UnityEngine;

    public class GameSettings : MonoBehaviour
    {
        private static GameSettings s_Instance;
        public static GameSettings Instance => s_Instance;

        public VisualSetting VisualSettings;

        void Awake()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
            }
        }

        void OnDestroy()
        {
            s_Instance = null;
        }
    }

    [System.Serializable]
    public class VisualSetting
    {
        public float FallSpeed = 10.0f;
        public AnimationCurve FallAccelerationCurve;
        public AnimationCurve BounceCurve;
        public AnimationCurve SquishCurve;

        // public AnimationCurve MatchFlyCurve;
        // public AnimationCurve CoinFlyCurve;

        // public GameObject BonusModePrefab;

        // public GameObject HintPrefab;

        // public VisualEffect CoinVFX;

        // public VisualEffect WinEffect;
        // public VisualEffect LoseEffect;
    }

}