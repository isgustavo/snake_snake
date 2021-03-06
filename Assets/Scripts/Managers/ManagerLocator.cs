using System;
using System.Collections.Generic;
using UnityEngine;

public static class ManagerLocator
{
    private static readonly Dictionary<Type, object> managers = new Dictionary<Type, object>();

    public static void RegisterManager<T>(T t)
    {
        if (managers.ContainsKey(typeof(T)) == false)
        {
            managers.Add(typeof(T), t);
        }
        else
        {
            managers[typeof(T)] = t;
        }
    }

    public static void UnRegisterManager<T>()
    {
        if (managers.ContainsKey(typeof(T)))
        {
            managers.Remove(typeof(T));
        }
    }

    public static bool TryGetManager<T>(out T t)
    {
        if (managers.ContainsKey(typeof(T)))
        {
            t = (T) managers[typeof(T)];
            return true;
        } else
        {
            t = default;
            return false;
        }
    }
}



