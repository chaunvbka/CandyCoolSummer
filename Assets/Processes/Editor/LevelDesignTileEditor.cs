#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(LevelDesignTile))]
    public class LevelDesignTileEditor : Editor
    {
        LevelDesignTile _levelDesignTile;

        void OnEnable()
        {
            _levelDesignTile = target as LevelDesignTile;
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            Texture2D preview = AssetPreview.GetAssetPreview(_levelDesignTile.Sprite);
            if (preview != null)
            {
                GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit);
            }
        }
    }
}
