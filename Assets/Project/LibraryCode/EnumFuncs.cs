using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumFuncs
{
    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(random);
    }
}
