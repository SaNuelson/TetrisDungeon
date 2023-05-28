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

            DrawDefaultInspector();

            MinoShapePreset preset = target as MinoShapePreset;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preview:");

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