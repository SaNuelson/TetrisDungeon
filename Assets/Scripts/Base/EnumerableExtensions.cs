using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class EnumerableExtensions
{
    public static int ArgMin<T>(this IEnumerable<T> values) where T : IComparable<T>
    {
        IEnumerator<T> enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
            return -1;

        int minIdx = 0;
        T minVal = enumerator.Current;

        int idx = 0;
        while (enumerator.MoveNext())
        {
            idx++;
            if (enumerator.Current.CompareTo(minVal) < 0)
            {
                minIdx = idx;
                minVal = enumerator.Current;
            }
        }

        return minIdx;
    }
    public static int ArgMax<T>(this IEnumerable<T> values) where T : IComparable<T>
    {
        IEnumerator<T> enumerator = values.GetEnumerator();
        if (!enumerator.MoveNext())
            return -1;

        int minIdx = 0;
        T minVal = enumerator.Current;

        int idx = 0;
        while (enumerator.MoveNext())
        {
            idx++;
            if (enumerator.Current.CompareTo(minVal) > 0)
            {
                minIdx = idx;
                minVal = enumerator.Current;
            }
        }

        return minIdx;
    }
}