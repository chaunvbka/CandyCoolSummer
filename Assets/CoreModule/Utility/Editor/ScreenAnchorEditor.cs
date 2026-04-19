#if UNITY_EDITOR

#pragma warning disable IDE0130

namespace Texell.Utility
{

    using UnityEditor;
    using UnityEngine.UIElements;

    [CustomEditor(typeof(ScreenAnchor))]
    public class ScreenAnchorEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our Inspector UI.
            var root = new VisualElement();

            // Load from default reference.
            var inspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Commons/Utility/Editor/ScreenAnchorXML.uxml");
            inspectorXML.CloneTree(root);

            // List<Label> results = root.Query<VisualElement>("offset-unit").Children<Label>(className: "unity-enum-field__label").ToList();
            // foreach (var result in results)
            // {
            //     result.style.display = DisplayStyle.None;
            // }

            return root;
        }
    }

}

#endif
