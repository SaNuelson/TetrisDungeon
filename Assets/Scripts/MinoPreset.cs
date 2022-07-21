using UnityEditor;
using UnityEngine;

[System.Serializable] public class MinoPreset
{
    public string Name = "New mino";
    public Color ForegroundColor = Color.yellow;
    public Color BackgroundColor = Color.black;

    public Vector2Int BoxSize;
    public Vector2Int[] Offsets;

    [Tooltip("Anchor within bounding box. Must be divisible by 0.5 in both directions.")]
    public Vector2 Anchor;
}
