#pragma warning disable IDE0130

namespace Texell.Processes
{

    using UnityEngine;
    using Texell.CoreModule;
    using Texell.Utility;
    using Texell.CandyCoolSummer;

    public class GameProcess : IProcess
    {
        Board _board;

        public void OnStart()
        {
            Debug.Log("GameProcess.OnStart()");
            _board = new ();
            NonMono.StartCoroutine(_board.Initialize());
        }

        public void OnUpdate()
        {
        }

        public void OnExit()
        {
        }
    }
}