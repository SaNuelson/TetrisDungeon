using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using com.spacepuppyeditor.Internal;

namespace Assets.Scripts.Tetris
{
    [CustomEditor(typeof(MinoShapePreset))]
    public class MinoShapePresetEditor : Editor
    {
        static Dictionary<string, Texture2D> minoPreviews = new Dictionary<string, Texture2D>();
        static TileFactory tileFactory;

        public MinoShapePresetEditor()
        {
            Debug.Log("MinoShapePresetEditor.OnEnable");
            TileFactoryPreset.DefaultChanged.AddListener(Reset);
        }

        private void Reset()
        {
            Debug.Log("MinoShapePresetEditor.Reset");
            minoPreviews.Clear();
            tileFactory = TileFactoryPreset.GetDefault();
        }

        public override void OnInspectorGUI()
        {
            if (tileFactory != TileFactoryPreset.GetDefault())
            {
                Reset();
            }

            MinoShapePreset preset = target as MinoShapePreset;

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MinoShapePreset.Name)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MinoShapePreset.Color)));
            preset.BoxSize = EditorGUILayout.Vector2IntField("BoxSize", preset.BoxSize);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            for (int i = preset.BoxSize.y - 1; i >= 0 ; i--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < preset.BoxSize.x; j++)
                {
                    var point = new Vector2Int(j, i);
                    var oldValue = preset.Offsets.Contains(point);
                    var newValue = EditorGUILayout.Toggle(oldValue, GUILayout.Width(20f));
                    if (oldValue && !newValue)
                        preset.Offsets.Remove(point);
                    else if (!oldValue && newValue)
                        preset.Offsets.Add(point);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            Texture2D minoPreviewTex = null;
            minoPreviews.TryGetValue(preset.Name, out minoPreviewTex);

            if (minoPreviewTex == null)
            {
                minoPreviewTex = GetPreview(preset);
                minoPreviews[preset.Name] = minoPreviewTex;
            }
            else if (GUI.changed || EditorUtility.IsDirty(serializedObject.targetObject.GetInstanceID()))
            {
                DestroyImmediate(minoPreviewTex);
                minoPreviewTex = GetPreview(preset);
                minoPreviews[preset.Name] = minoPreviewTex;
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

            EditorGUILayout.EndHorizontal();
        }

        private Texture2D GetPreview(MinoShapePreset preset)
        {
            GameObject minoPreview = null;
            minoPreview = new GameObject("Mino Preview");
            var minoPreviewScript = minoPreview.AddComponent<MinoScript>();

            Texture2D previewTex = null;
            try
            {
                minoPreviewScript.ConstructFromPreset(preset, tileFactory);
                previewTex = AssetPreview.GetAssetPreview(minoPreview);
            }
            catch (IndexOutOfRangeException) { }
            
            DestroyImmediate(minoPreview);
            return previewTex;
        }
    }
}