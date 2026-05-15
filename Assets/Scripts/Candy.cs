#pragma warning disable IDE0130

namespace Texell.CandyCoolSummer
{
    using UnityEngine;



    public abstract class Candy : MonoBehaviour
    {
        public enum ColorType
        {
            CHOCOLATE_MILK,

            BLUE,
            YELLOW,
            RED,
            GREEN,
            PURPLE,
            PINK,
        }

        public enum PoolType
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

        public enum State
        {
            Idle,
            Falling,
            Bouncing,
            Disappearing
        }

        public PoolType Pool;
        public ColorType Type;

        public Vector3Int CurrentPosition => _currentPosition;

        public bool CanMove => _canMove && _currentState is State.Idle;

        public State CurrentState => _currentState;
        public float FallTime => _fallTime;

        /// <summary>
        /// This is set to sqrt(2) when falling in diagonal so the time of a diagonal fall 
        /// is the same as a direct one.
        /// </summary>
        [HideInInspector]
        public float SpeedMultiplier = 1.0f;

        public Match? CurrentMatch;

        protected bool _canMove;
        
        protected Vector3Int _currentPosition;

        [SerializeField]
        private State _currentState = State.Idle;
        private float _fallTime = 0.0f;

        public void Init(Vector3Int cellPos)
        {
            _currentState = State.Idle;
            _currentPosition = cellPos;
            CurrentMatch = null;
            _canMove = true;
        }

        public void StartMoveTimer()
        {
            _fallTime = 0;
            _currentState = State.Falling;
        }

        public void MoveTo(Vector3Int newCellPos)
        {
            _currentPosition = newCellPos;
        }

        public void TickMoveTimer(float deltaTime)
        {
            _fallTime += deltaTime;
        }

        public void StopFalling()
        {
            _fallTime = 0;
            _currentState = State.Bouncing;
        }

        public void StopBouncing()
        {
            _currentState = State.Idle;
        }

        public void Destroyed()
        {
            _currentState = State.Disappearing;
        }
    }
}


