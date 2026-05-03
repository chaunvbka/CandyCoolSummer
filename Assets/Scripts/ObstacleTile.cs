#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{

    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "ObstacleTile", menuName = "CandyCoolSummer/Tile/ObstacleTile")]
    public class ObstacleTile : TileBase
    {
        public Sprite Sprite;

        public Obstacle ObstaclePrefab = null;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = !Application.isPlaying ? Sprite : null;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return false;
#endif

            Board.RegisterObstacle(position, ObstaclePrefab);

            return base.StartUp(position, tilemap, go);
        }
    }
}