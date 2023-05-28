using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

[CustomEditor(typeof(TileFactoryPreset))]
public class TileFactoryPresetEditor : Editor
{
    bool[,] switchMap;
    TileFactoryPreset factoryPreset;
    Texture2D minoPreviewTex;

    public override void OnInspectorGUI()
    {
        if (switchMap == null)
        {
            switchMap = new bool[5, 5];
        }
        if (factoryPreset == null)
        {
            factoryPreset = (TileFactoryPreset)target;
        }

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Test screen");
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < 5; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 5; j++)
            {
                switchMap[i, j] = EditorGUILayout.Toggle(switchMap[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (minoPreviewTex == null)
        {
            minoPreviewTex = GetPreview(switchMap);
        }
        else if (GUI.changed || EditorUtility.IsDirty(serializedObject.targetObject.GetInstanceID()))
        {
            DestroyImmediate(minoPreviewTex);
            minoPreviewTex = GetPreview(switchMap);
        }

        if (minoPreviewTex != null)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(minoPreviewTex, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.LabelField("Invalid preset");
        }

        EditorGUILayout.EndVertical();
    }

    private Texture2D GetPreview(bool[,] fillMap)
    {
        GameObject minoPreview = null;
        minoPreview = new GameObject("Mino Preview");
        var minoPreviewScript = minoPreview.AddComponent<MinoScript>();
        var preset = ScriptableObject.CreateInstance<MinoShapePreset>();

        var offsets = new List<Vector2Int>();
        var boxSize = new Vector2Int();
        for (int i = 0; i < fillMap.GetLength(0); i++)
        {
            for (int j = 0; j < fillMap.GetLength(1); j++)
            {
                if (fillMap[i, j])
                {
                    offsets.Add(new Vector2Int(i, j));
                    if (boxSize.x < i)
                        boxSize.x = i;
                    if (boxSize.y < j)
                        boxSize.y = j;
                }
            }
        }
        preset.BoxSize = boxSize;
        preset.Offsets = offsets.ToArray();

        Texture2D previewTex = null;
        minoPreviewScript.ConstructFromPreset(preset, factoryPreset.GetFactory());
        previewTex = AssetPreview.GetAssetPreview(minoPreview);

        DestroyImmediate(minoPreview);
        return previewTex;
    }
}