using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MinoPreset")]
public class MinoShapePreset : ScriptableObject
{
    public string Name = "Mino preset";
    public Color Color = Color.magenta;

    public Vector2Int BoxSize = new Vector2Int(1, 1);
    public Vector2Int[] Offsets = new Vector2Int[] { new Vector2Int(0, 0) };

    [Tooltip("Anchor within bounding box. Must be divisible by 0.5 in both directions.")]
    public Vector2 Anchor = new Vector2Int(0, 0);
}
