#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BoolGrid))]
public class BoolGridDrawer : PropertyDrawer
{
    private const float CellSize = 18f;
    private const float Spacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty widthProp = property.FindPropertyRelative("width");
        SerializedProperty heightProp = property.FindPropertyRelative("height");
        SerializedProperty cellsProp = property.FindPropertyRelative("cells");

        int width = widthProp.intValue;
        int height = heightProp.intValue;

        float y = position.y;

        EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), label);
        y += EditorGUIUtility.singleLineHeight + Spacing;

        if (width <= 0 || height <= 0 || cellsProp.arraySize != width * height)
        {
            EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                "Grid not initialized (width/height/cells mismatch). Call ValidateAndRecreate().");
            return;
        }

        EditorGUI.indentLevel++;

        for (int gridY = 0; gridY < height; gridY++)
        {
            float x = position.x +  EditorGUI.indentLevel;

            for (int gridX = 0; gridX < width; gridX++)
            {
                int index = gridY * width + gridX;
                SerializedProperty cellProp = cellsProp.GetArrayElementAtIndex(index);

                Rect cellRect = new Rect(x, y, CellSize, CellSize);
                cellProp.boolValue = EditorGUI.Toggle(cellRect, cellProp.boolValue);

                x += CellSize + Spacing;
            }

            y += CellSize + Spacing;
        }

        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty widthProp = property.FindPropertyRelative("width");
        SerializedProperty heightProp = property.FindPropertyRelative("height");
        SerializedProperty cellsProp = property.FindPropertyRelative("cells");

        int width = widthProp.intValue;
        int height = heightProp.intValue;

        float headerHeight = EditorGUIUtility.singleLineHeight + Spacing;

        if (width <= 0 || height <= 0 || cellsProp.arraySize != width * height)
        {
            return headerHeight + EditorGUIUtility.singleLineHeight;
        }

        return headerHeight + height * (CellSize + Spacing);
    }
}
#endif