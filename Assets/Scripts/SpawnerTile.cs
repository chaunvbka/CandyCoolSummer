#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEngine.Tilemaps;

    [CreateAssetMenu(fileName = "SpawnerTile", menuName = "CandyCoolSummer/Tile/SpawnerTile")]
    public class SpawnerTile : TileBase
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

            Board.RegisterSpawner(position);

            return base.StartUp(position, tilemap, go);
        }
    }

}