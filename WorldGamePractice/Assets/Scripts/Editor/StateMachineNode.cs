// Assets/Editor/StateMachineNode.cs
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示状态机中的一个状态节点。
/// 包含位置、名称、出边（连接到的其他状态）等信息。
/// </summary>
[Serializable]
public class StateMachineNode
{
    public string guid;               // 唯一标识符，用于建立连接
    public string stateName = "New State"; // 状态名称
    public Rect position;             // 节点在窗口中的位置和大小
    public List<string> transitions = new List<string>(); // 出边：指向其他节点的 guid

    // 构造函数
    public StateMachineNode(string name, Vector2 pos)
    {
        guid = Guid.NewGuid().ToString();
        stateName = name;
        position = new Rect(pos.x, pos.y, 150, 50); // 默认宽高
    }

    // 判断鼠标是否点击在该节点上
    public bool IsMouseOver(Vector2 mousePos)
    {
        return position.Contains(mousePos);
    }
}