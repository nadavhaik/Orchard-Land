using System;
using System.Collections.Generic;

public class FunctionalComparer<T> : IComparer<T>
{
    private readonly Func<T, T, int> _comparer;
    
    private FunctionalComparer(Func<T, T, int> comparer)
    {
        this._comparer = comparer;
    }
    public static IComparer<T> Create(Func<T, T, int> comparer)
    {
        return new FunctionalComparer<T>(comparer);
    }
    
    public static IComparer<T> Create(Func<T, float> compareValGetter)
    {
        return new FunctionalComparer<T>((t1, t2) =>
        {
            var t1CompVal = compareValGetter(t1);
            var t2CompVal = compareValGetter(t2);
            return t1CompVal < t2CompVal ? -1 : t1CompVal > t2CompVal ? 1 : 0;
        });
    }
    public int Compare(T x, T y)
    {
        return _comparer(x, y);
    }
}