using System;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    // Fisher–Yates shuffle using Unity's Random for consistency with game seed
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    // Alternative shuffle using System.Random with thread-local instance
    [ThreadStatic]
    private static System.Random threadLocalRng;

    public static void ShuffleThreadSafe<T>(this IList<T> list)
    {
        if (threadLocalRng == null)
            threadLocalRng = new System.Random();

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = threadLocalRng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
