using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectEx
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {

        var t = go.GetComponentInChildren <T>();
        if (t != null)
        {
            return t;
        }
        else
        {
            t = go.AddComponent<T>();
            return t;
        }
    }
}