#pragma warning disable IDE0130 

#if UNITY_EDITOR

namespace Texell.CandyCoolSummer
{

    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(ObstacleTile))]
    public class ObstacleTileEditor : Editor
    {
        ObstacleTile _tile;

        void OnEnable()
        {
            _tile = target as ObstacleTile;
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

#endif