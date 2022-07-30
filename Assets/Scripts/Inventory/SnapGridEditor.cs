using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SnapGrid))]
public class SnapGridEditor : Editor
{
    SerializedProperty gridSizeProp;
    SerializedProperty gridProp;
    SerializedProperty spriteGroupProp;
    SerializedProperty tileTypeProp;
    SerializedProperty tileManagerProp;

    Vector2 scrollPos;

    void OnEnable()
    {
        gridSizeProp = serializedObject.FindProperty("GridSize");
        gridProp = serializedObject.FindProperty("Grid");
        spriteGroupProp = serializedObject.FindProperty("SpriteGroup");
        tileTypeProp = serializedObject.FindProperty("TileType");
        tileManagerProp = serializedObject.FindProperty("tileFactory");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(gridSizeProp);
        EditorGUILayout.PropertyField(spriteGroupProp);
        EditorGUILayout.PropertyField(tileTypeProp);
        bool force = DrawGrid();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            (target as SnapGrid).Reassemble(force);
    }

    private bool DrawGrid()
    {
        Vector2Int gridSize = gridSizeProp.vector2IntValue;
        gridProp.arraySize = gridSize.x * gridSize.y;
        var toggleSize = EditorGUIUtility.singleLineHeight;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle() { fixedWidth = toggleSize, fixedHeight = toggleSize });
            for (int x = 0; x < gridSize.x; x++)
            {
                var coord = x * gridSize.y + y;
                var oldVal = EditorGUILayout.Toggle(gridProp.GetArrayElementAtIndex(coord).boolValue);
                gridProp.GetArrayElementAtIndex(coord).boolValue = oldVal;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fill")) FillGrid();
        if (GUILayout.Button("Erase")) ClearGrid();
        bool force = false;
        if (GUILayout.Button("Force Reset"))
        {
            ForceReset();
            force = true;
        }
        EditorGUILayout.EndHorizontal();
        return force;
    }

    private void FillGrid()
    {
        Vector2Int gridSize = gridSizeProp.vector2IntValue;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var coord = x * gridSize.y + y;
                gridProp.GetArrayElementAtIndex(coord).boolValue = true;
            }
        }
    }

    private void ClearGrid()
    {
        Vector2Int gridSize = gridSizeProp.vector2IntValue;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var coord = x * gridSize.y + y;
                gridProp.GetArrayElementAtIndex(coord).boolValue = false;
            }
        }
    }

    private void ForceReset()
    {
        foreach (Transform child in (target as SnapGrid).transform)
            DestroyImmediate(child.gameObject);
    }
}
