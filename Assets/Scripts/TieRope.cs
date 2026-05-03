#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;

    public class TieRope : Obstacle
    {
        public override void Init(Vector3Int cellPos)
        {
            base.Init(cellPos);
            
            Board.ChangeLock(cellPos, true);
        }
    }
}
