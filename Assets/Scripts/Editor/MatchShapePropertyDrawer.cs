#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(MatchShape))]
public class MatchShapePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        root.style.width = Length.Percent(100);
        root.style.flexDirection = FlexDirection.Row;

        VisualElement shapePanel = new();
        shapePanel.style.width = Length.Percent(50);

        VisualElement settingPanel = new();
        settingPanel.style.width = Length.Percent(50);

        root.Add(shapePanel);
        root.Add(settingPanel);

        VisualElement mirrorPanel = new()
        {
            style = { flexDirection = FlexDirection.Row }
        };

        VisualElement mirrorProperty = new PropertyField(property.FindPropertyRelative(nameof(MatchShape.CanMirror)), "");
        VisualElement mirrorLabel = new Label("Can be Mirrored");

        mirrorLabel.style.paddingLeft = 5;

        mirrorPanel.Add(mirrorProperty);
        mirrorPanel.Add(mirrorLabel);


        VisualElement rotatePanel = new()
        {
            style = { flexDirection = FlexDirection.Row }
        };

        VisualElement rotateProperty = new PropertyField(property.FindPropertyRelative(nameof(MatchShape.CanRotate)), "");
        VisualElement rotateLabel = new Label("Can be Rotated");

        rotateLabel.style.paddingLeft = 5;

        rotatePanel.Add(rotateProperty);
        rotatePanel.Add(rotateLabel);

        settingPanel.Add(mirrorPanel);
        settingPanel.Add(rotatePanel);

        CreateUI(property, shapePanel);

        return root;
    }

    void CreateUI(SerializedProperty property, VisualElement root)
    {
        root.Clear();

        //need to rebuild the list as we only have access to serializedProperty and easier to work with an array lower
        var cells = property.FindPropertyRelative(nameof(MatchShape.Cells));
        List<Vector3Int> rebuiltCells = new();

        for (int i = 0; i < cells.arraySize; ++i)
        {
            rebuiltCells.Add(cells.GetArrayElementAtIndex(i).vector3IntValue);
        }

        var bound = MatchShape.GetBoundOf(rebuiltCells);

        for (int y = bound.height + 1; y >= -1; y--)
        {
            var line = new VisualElement
            {
                name = "Line" + y
            };
            line.style.width = Length.Percent(100);
            line.style.height = 18;
            line.style.flexDirection = FlexDirection.Row;
            root.Add(line);

            for (int x = bound.x - 1; x <= bound.width + 2; x++)
            {
                var realPos = new Vector3Int(x, y + bound.yMin, 0);

                VisualElement newElem = null;

                if (rebuiltCells.Contains(realPos))
                {
                    //this is a cell
                    var l = new Label
                    {
                        text = "-"
                    };

                    l.style.backgroundColor = Color.black;

                    l.RegisterCallback<ClickEvent>(evt =>
                    {
                        RemoveCell(property, rebuiltCells.IndexOf(realPos));
                        CreateUI(property, root);
                    });

                    newElem = l;
                }
                else if (rebuiltCells.Contains(realPos + Vector3Int.right) ||
                         rebuiltCells.Contains(realPos + Vector3Int.down) ||
                         rebuiltCells.Contains(realPos + Vector3Int.left) ||
                         rebuiltCells.Contains(realPos + Vector3Int.up))
                {
                    //not a cell but cell on the right
                    var l = new Label
                    {
                        text = "+"
                    };

                    l.RegisterCallback<ClickEvent>(evt =>
                    {
                        AddNewCell(property, realPos);
                        CreateUI(property, root);
                    });

                    newElem = l;
                }
                else
                {
                    //not a cell and no cell adjacent
                    var l = new Label
                    {
                        text = " "
                    };

                    newElem = l;
                }

                newElem.style.unityTextAlign = TextAnchor.MiddleCenter;
                newElem.style.width = 18;
                line.Add(newElem);
            }
        }
    }

    void RemoveCell(SerializedProperty property, int index)
    {
        property.serializedObject.Update();

        var cells = property.FindPropertyRelative(nameof(MatchShape.Cells));
        cells.DeleteArrayElementAtIndex(index);

        property.serializedObject.ApplyModifiedProperties();
    }

    void AddNewCell(SerializedProperty property, Vector3Int cell)
    {
        property.serializedObject.Update();

        var cells = property.FindPropertyRelative(nameof(MatchShape.Cells));

        cells.InsertArrayElementAtIndex(cells.arraySize);
        cells.GetArrayElementAtIndex(cells.arraySize - 1).vector3IntValue = cell;

        property.serializedObject.ApplyModifiedProperties();
    }
}

#endif