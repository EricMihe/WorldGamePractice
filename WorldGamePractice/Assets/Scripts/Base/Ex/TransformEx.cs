using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class TransformEx
{
    public static Transform FindChildByName(this Transform parent, string name)
    {
        // 获取所有子物体（包括自身）
        Transform[] children = parent.GetComponentsInChildren<Transform>(true); // true = includeInactive

        foreach (Transform child in children)
        {
            if (child.name == name)
                return child;
        }
        return null;
    }

    public static Transform FindFromBottom(this Transform parent, string name)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child; // 找到就返回（最下面的匹配项）
            }
        }
        return null; // 没找到
    }

}
