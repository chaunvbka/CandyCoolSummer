#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "CandyTile", menuName = "CandyCoolSummer/Tile/CandyTile")]
    public class CandyTile : TileBase
    {
        public Sprite Sprite;

        [Tooltip("If null this will be a random candy")]
        public Candy CandyPrefab;

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

            if (CandyPrefab)
            {
                // Register cell position has a specific candy.
                Board.RegisterCandy(position, CandyPrefab);
            }
            else
            {
                // Register cell position has random candy.
                Board.RegisterRandomCandy(position);
            }

            return base.StartUp(position, tilemap, go);
        }
    }
}
