using System.Collections.Generic;
public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> ts, int start, int end, float chance)
    {
        for (var i = start; i < end; ++i)
        {
            if (UnityEngine.Random.value < chance)
            {
                var r = UnityEngine.Random.Range(i, end - start);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}