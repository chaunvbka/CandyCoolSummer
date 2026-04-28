#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(SpawnerTile))]
    public class SpawnerTileEditor : Editor
    {
        SpawnerTile _tile;

        void OnEnable()
        {
            _tile = target as SpawnerTile;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(_tile.Sprite);
            if (preview != null)
            {
                GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit);
            }
        }
    }
}
