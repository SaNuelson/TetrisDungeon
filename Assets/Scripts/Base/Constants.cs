using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Direction4
{
    Up,
    Right,
    Down,
    Left
}

public static class EnumExtensions
{
    public static T Next<T>(this T val) where T : Enum
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(val.GetType());
        int j = Array.IndexOf<T>(Arr, val) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }

    public static T Prev<T>(this T val) where T : Enum
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(val.GetType());
        int j = Array.IndexOf<T>(Arr, val) - 1;
        return (j < 0) ? Arr[Arr.Length - 1] : Arr[j];
    }

    public static Vector2Int ToVec2Int(this Direction4 direction) => direction switch
    {
        Direction4.Up => Vector2Int.up,
        Direction4.Down => Vector2Int.down,
        Direction4.Left => Vector2Int.left,
        Direction4.Right => Vector2Int.right,
        _ => throw new NotImplementedException()
    };

}

public static class Constants
{

}