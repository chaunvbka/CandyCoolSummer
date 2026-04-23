#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "CandySpawnerTile", menuName = "CandyCoolSummer/Tile/CandySpawner")]
    public class CandySpawnerTile : TileBase
    {
        public Sprite Sprite;

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

            // This tile is only used in editor to help design the level. At runtime, we notify
            // the board that this tile is a place for a candy. The Board will take care of 
            // creating a candy there.
            //Board.RegisterCandySpawner(position);

            return base.StartUp(position, tilemap, go);
        }
    }

}