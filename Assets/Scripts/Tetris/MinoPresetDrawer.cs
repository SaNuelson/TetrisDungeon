using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tetris.Editor
{
    [CustomPropertyDrawer(typeof(MinoPreset))]
    public class MinoPresetDrawer : PropertyDrawer
    {
        static Dictionary<string, Texture2D> minoPreviews = new Dictionary<string, Texture2D>();

        static TetrisManager tetris;

        private bool CanShowCustomGUI()
        {
            if (tetris == null)
            {
                var tetrises = UnityEngine.Object.FindObjectsOfType<TetrisManager>();
                if (tetrises.Length == 1)
                {
                    tetris = tetrises[0];
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!CanShowCustomGUI())
            {
                OnDefaultGUI(position, property, label);
                return;
            }

            // Render the default property editor
            EditorGUI.PropertyField(position, property, label, true);

            // Extract actual MinoPreset via reflection
            object presetObj = fieldInfo.GetValue(property.serializedObject.targetObject);
            MinoPreset preset = null;
            if (presetObj.GetType().IsArray)
            {
                var index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
                preset = ((MinoPreset[])presetObj)[index];
            }
            else
            {
                preset = presetObj as MinoPreset;
            }

            // Preview label
            var previewLabelRect = new Rect(
                position.x + 50,
                position.y + position.height - 50 - EditorGUIUtility.singleLineHeight / 2,
                100,
                EditorGUIUtility.singleLineHeight);
            GUI.Label(previewLabelRect, "Preview:");

            Texture2D minoPreviewTex = null;
            minoPreviews.TryGetValue(preset.Name, out minoPreviewTex);

            if (minoPreviewTex == null)
            {
                minoPreviewTex = GetPreview(preset);
                minoPreviews[preset.Name] = minoPreviewTex;
            }
            else if (GUI.changed)
            {
                UnityEngine.Object.DestroyImmediate(minoPreviewTex);
                minoPreviewTex = GetPreview(preset);
                minoPreviews[preset.Name] = minoPreviewTex;
            }

            var previewPosition = new Rect(position.x + position.width / 2 - 50, position.y + position.height - 100, 100, 100);
            GUI.DrawTexture(previewPosition, minoPreviewTex);
        }

        private void OnDefaultGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        private Texture2D GetPreview(MinoPreset preset)
        {
            GameObject minoPreview = null;
            minoPreview = new GameObject("Mino Preview");
            var minoPreviewScript = minoPreview.AddComponent<MinoScript>();

            // Method of the script to assemble the actual gameobject from data
            // minoPreviewScript.ConstructPreview(preset);
            minoPreviewScript.ConstructFromPreset(preset, tetris.BlockSprites);

            var previewTex = AssetPreview.GetAssetPreview(minoPreview);
            UnityEngine.Object.DestroyImmediate(minoPreview);
            return previewTex;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (CanShowCustomGUI())
            {
                return EditorGUI.GetPropertyHeight(property) + 100;
            }
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}