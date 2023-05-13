using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GizmoExtensions
{

    public static void DrawRect(Vector3 topLeft, Vector3 botRight)
    {
        var topRight = new Vector3(botRight.x, topLeft.y, topLeft.z / 2f + botRight.z / 2f);
        var botLeft = new Vector3(topLeft.x, botRight.y, topLeft.z / 2f + botRight.z / 2f);
        Gizmos.DrawLine(topLeft, botLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botLeft, botRight);
    }

    public static void DrawGrid(Vector3 topLeft, Vector3 botRight, float cellWidth, float cellHeight)
    {
        var topRight = new Vector3(botRight.x, topLeft.y, topLeft.z / 2f + botRight.z / 2f);
        var botLeft = new Vector3(topLeft.x, botRight.y, topLeft.z / 2f + botRight.z / 2f);

        var width = botRight.x - topLeft.x;
        var xSteps = Mathf.FloorToInt(width / cellWidth);
        for (int i = 1; i < xSteps; i++)
        {
            var dx = new Vector3(1f * i, 0f, 0f);
            Gizmos.DrawLine(topLeft + dx, botLeft + dx);
        }

        var height = topLeft.y - botRight.y;
        var ySteps = Mathf.FloorToInt(height / cellHeight);
        for (int j = 1; j < ySteps; j++)
        {
            var dy = new Vector3(0f, 1f * j, 0f);
            Gizmos.DrawLine(botLeft + dy, botRight + dy);
        }
    }

    public static void DrawX(Vector3 topLeft, Vector3 botRight)
    {
        var topRight = new Vector3(botRight.x, topLeft.y, topLeft.z / 2f + botRight.z / 2f);
        var botLeft = new Vector3(topLeft.x, botRight.y, topLeft.z / 2f + botRight.z / 2f);
        Gizmos.DrawLine(topLeft, botRight);
        Gizmos.DrawLine(botLeft, topRight);
    }

}