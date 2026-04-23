#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using System.Collections;
    using Texell.CoreModule;
    using UnityEngine;

    /// <summary>
    /// V = striped vertical, H = striped horizontal.
    /// </summary>
    public enum CandyType
    {
        V_BLUE = 0,
        V_YELLOW = 1,
        V_RED = 2,
        V_GREEN = 3,
        V_PURPLE = 4,
        V_PINK = 5,

        CHOCOLATE_MILK = 6,

        H_BLUE = 7,
        H_YELLOW = 8,
        H_RED = 9,
        H_GREEN = 10,
        H_PURPLE = 11,
        H_PINK = 12,

        BLUE = 13,
        YELLOW = 14,
        RED = 15,
        GREEN = 16,
        PURPLE = 17,
        PINK = 18,

        STING_BLUE = 19,
        STING_YELLOW = 20,
        STING_RED = 21,
        STING_GREEN = 22,
        STING_PURPLE = 23,
        STING_PINK = 24,

        SWIRL_BLUE = 25,
        SWIRL_YELLOW = 26,
        SWIRL_RED = 27,
        SWIRL_GREEN = 28,
        SWIRL_PURPLE = 29,
        SWIRL_PINK = 30,
    }

    public class Candy : MonoBehaviour
    {
        public enum State
        {
            Existing,
            Falling,
            Bouncing,
            Disappearing
        }

        public CandyType Type;

        //TODO:
        //public VisualEffect[] MatchEffectPrefabs;

        public Sprite Sprite;

        // When a candy get added to a match, this match get stored here so we can now if this 
        // candy is currently in a match and cannot be used for anything else.
        public Match CurrentMatch = null;

        // This is set to sqrt(2) when falling in diagonal so the time of a diagonal fall is 
        // the same as a direct one.
        [HideInInspector]
        public float SpeedMultiplier = 1.0f;

        public State CurrentState => _currentState;
        public bool CanMove => _canMove && _currentState is State.Existing;
        public Vector3Int CurrentIndex => _currentIndex;
        public bool Usable => _usable;
        public bool Used => _used;
        public float FallTime => _fallTime;
        public int HitPoint => _hitPoints;

        // If this is set to true, the Use function will be called when swapping or 
        // double clicking the candy.
        // Not used in this base class, but useful for BonusCandy.
        protected bool _canMove = true;
        protected Vector3Int _currentIndex;
        protected bool _usable = false;
        protected bool _used = false;
        protected int _hitPoints = 1;

        private State _currentState = State.Existing;
        private float _fallTime = 0.0f;

        // public virtual void Init(Vector3Int startIdx)
        // {
        //     _currentIndex = startIdx;
        // }
    }
}


