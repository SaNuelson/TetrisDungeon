using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Utils
    {

        public static Texture2D CreateSolidTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(width, height);
            var texPixs = texture.GetPixels();
            for (var i = 0; i < texPixs.Length; ++i)
            {
                texPixs[i] = color;
            }

            texture.SetPixels(texPixs);
            texture.Apply();
            return texture;
        }

        public static void DrawGizmoRect(Vector3 a, Vector3 b, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;

            var axay = new Vector3(a.x, a.y, 0f);
            var axby = new Vector3(a.x, b.y, 0f);
            var bxay = new Vector3(b.x, a.y, 0f);
            var bxby = new Vector3(b.x, b.y, 0f);

            Gizmos.DrawLine(axay, axby);
            Gizmos.DrawLine(axay, bxay);
            Gizmos.DrawLine(axby, bxby);
            Gizmos.DrawLine(bxay, bxby);

            Gizmos.color = oldColor;
        }
        
        public static void DrawGizmoCrossedRect(Vector3 a, Vector3 b, Color color)
        {
            DrawGizmoRect(a, b, color);
            var oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawLine(a, b);

            Gizmos.color = oldColor;
        }

    }

    public static class RandomUtils
    {
        // TODO: Remove in favor of RandomExtensions
        public static Vector2Int Vec2Int(int maxX, int maxY)
        {
            return new Vector2Int(Random.Range(0, maxX), Random.Range(0, maxY));
        }

        public static T Sample<T>(this T[] items)
        {
            return items[Random.Range(0, items.Length)];
        }

        public static T Sample<T>(this List<T> items)
        {
            return items[Random.Range(0, items.Count)];
        }
    }

}
