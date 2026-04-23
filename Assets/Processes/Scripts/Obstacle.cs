#pragma warning disable IDE0130 


namespace Texell.CandyCoolSummer
{
    using UnityEngine;

    public enum ObstacleType
    {
        Obstacle_1,
        Obstacle_2
    }

    /// <summary>
    /// Class for everything filling a cell and that get notified when a match is made 
    /// adjacent to it. Obstacle has multiple sprite (hp), each damage remove a sprite (1 hp) to destroy
    /// completely a obstacle (hp = 0).
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        private int _healthPoint;
    }
}
