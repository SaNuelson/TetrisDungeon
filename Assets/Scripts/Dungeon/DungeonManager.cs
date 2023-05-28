using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public MinoShapePreset[] RoomShapes;
    public Vector2Int LevelSize;
    public MapGenerator Generator;

    private void Awake()
    {
        Generator.dungeonManager = this;
        GetNextLevel();
    }

    private void GetNextLevel()
    {
        Generator.Generate();
    }

    private void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;
        Gizmos.color = Color.white;
        var tl = transform.position + new Vector3(0f, LevelSize.y, 0f);
        var br = transform.position + new Vector3(LevelSize.x, 0f, 0f);
        var bl = transform.position + new Vector3(0f, 0f, 0f);
        GizmoExtensions.DrawRect(tl, br);

        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
        GizmoExtensions.DrawGrid(tl, br, 1f, 1f);

        if (Generator != null && Generator.FillMap != null)
        {
            for (int i = 0; i < LevelSize.y; i++)
            {
                for (int j = 0; j < LevelSize.x; j++)
                {
                    if (Generator.FillMap[i,j])
                    {
                        GizmoExtensions.DrawX(
                            bl + new Vector3(1f * j, 1f * i, 0f),
                            bl + new Vector3(1f * (j + 1), 1f * (i + 1), 0f));
                    }
                }
            }
        }

        Gizmos.color = oldColor;
    }
}