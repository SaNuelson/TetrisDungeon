using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public static class RandomExtensions
{
    public static bool Flip(this Random random)
        => random.NextBool();

    public static T Choice<T>(this Random random, T[] items)
        => items[random.Next(0, items.Length)];

    public static T Choice<T>(this Random random, List<T> items)
        => items[random.Next(0, items.Count)];

    public static T Choice<T>(this Random random) where T : Enum
        => random.Choice((T[])Enum.GetValues(typeof(T)));

    public static bool NextBool(this Random random)
        => random.NextFloat() < 0.5f;

    public static float NextFloat(this Random random)
        => (float)random.NextDouble();

    public static float NextFloat(this Random random, float maxValue)
        => (float)random.NextDouble() * maxValue;

    public static float NextFloat(this Random random, float minValue, float maxValue)
        => (float)random.NextDouble() * (maxValue - minValue) + minValue;

    public static Vector2 NextVector2(this Random random)
        => new Vector2(random.NextFloat(), random.NextFloat());

    public static Vector2 NextVector2(this Random random, Vector2 maxValue)
        => new Vector2(random.NextFloat(maxValue.x), random.NextFloat(maxValue.y));

    public static Vector2 NextVector2(this Random random, float maxX, float maxY)
        => new Vector2(random.NextFloat(maxX), random.NextFloat(maxY));

    public static Vector2 NextVector2(this Random random, Vector2 minValue, Vector2 maxValue)
        => new Vector2(random.NextFloat(minValue.x, maxValue.x), random.NextFloat(minValue.y, maxValue.y));

    public static Vector2Int NextVector2Int(this Random random)
        => new Vector2Int(random.Next(), random.Next());

    public static Vector2Int NextVector2Int(this Random random, Vector2Int maxValue)
        => new Vector2Int(random.Next(maxValue.x), random.Next(maxValue.y));

    public static Vector2Int NextVector2Int(this Random random, int maxX, int maxY)
        => new Vector2Int(random.Next(maxX), random.Next(maxY));

    public static Vector2Int NextVector2Int(this Random random, Vector2Int minValue, Vector2Int maxValue)
        => new Vector2Int(random.Next(minValue.x, maxValue.x), random.Next(minValue.y, maxValue.y));

    public static Vector3 NextVector3(this Random random)
        => new Vector3(random.NextFloat(), random.NextFloat(), random.NextFloat());

    public static Vector3 NextVector3(this Random random, Vector3 maxValue)
        => new Vector3(random.NextFloat(maxValue.x), random.NextFloat(maxValue.y), random.NextFloat(maxValue.z));

    public static Vector3 NextVector3(this Random random, float maxX, float maxY, float maxZ)
        => new Vector3(random.NextFloat(maxX), random.NextFloat(maxY), random.NextFloat(maxZ));

    public static Vector3 NextVector3(this Random random, Vector3 minValue, Vector3 maxValue)
        => new Vector3(random.NextFloat(minValue.x, maxValue.x), random.NextFloat(minValue.y, maxValue.y), random.NextFloat(minValue.z, maxValue.z));
}