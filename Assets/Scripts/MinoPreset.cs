using UnityEditor;
using UnityEngine;

[System.Serializable] public class MinoPreset
{
    public string Name = "New mino";
    public Vector2Int[] Offsets;
    public Color ForegroundColor = Color.yellow;
    public Color BackgroundColor = Color.black;

    public MinoPreset GetShadow()
    {
        // Offsets should be read-only during run-time so no copy needed
        return new MinoPreset()
        {
            Name = Name,
            Offsets = Offsets,
            ForegroundColor = new Color(0.8f, 0.8f, 0.8f),
            BackgroundColor = new Color(0.6f, 0.6f, 0.6f)
        };
    }
}
