#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CandySpawnerTile))]
    public class CandySpawnerTileEditor : Editor
    {
        CandySpawnerTile _candySpawnerTile;

        void OnEnable()
        {
            _candySpawnerTile = target as CandySpawnerTile;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(_candySpawnerTile.Sprite);
            if (preview != null)
            {
                GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit);
            }
        }
    }
}
