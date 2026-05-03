#pragma warning disable IDE0130 


namespace Texell.CandyCoolSummer
{
    using UnityEngine;

    public interface IBoardAction
    {
        void OnSwapAction(Vector3Int startPos, Vector3Int endPos);
    }
}