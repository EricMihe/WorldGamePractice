// Assets/Editor/StateMachineEditorWindow.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity 编辑器窗口：可视化状态机编辑器
/// 支持添加节点、拖拽连线、移动节点、删除节点
/// </summary>
public class StateMachineEditorWindow : EditorWindow
{
    // ===== 数据部分 =====
    private List<StateMachineNode> nodes = new List<StateMachineNode>();
    private StateMachineNode draggingNode = null;          // 当前正在拖动的节点
    private StateMachineNode connectionStartNode = null;   // 正在拖拽连线的起始节点
    private Vector2 dragOffset;                            // 拖动偏移量
    private Vector2 scrollPosition = Vector2.zero;         // 滚动区域偏移（用于大画布）

    // 菜单入口：在菜单栏中添加 "Tools/State Machine Editor"
    [MenuItem("Tools/State Machine Editor")]
    public static void ShowWindow()
    {
        GetWindow<StateMachineEditorWindow>("State Machine");
    }

    // 窗口 GUI 绘制入口
    private void OnGUI()
    {
        // 顶部工具栏
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (GUILayout.Button("Add State", EditorStyles.toolbarButton))
        {
            AddNewState(Event.current.mousePosition);
        }
        if (GUILayout.Button("Clear All", EditorStyles.toolbarButton))
        {
            nodes.Clear();
            connectionStartNode = null;
            draggingNode = null;
        }
        GUILayout.EndHorizontal();

        // 创建可滚动的大画布区域
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);

        // Step 1: 处理事件（点击、拖拽等）
        ProcessEvents();

        // Step 2: 绘制所有连线（在节点下方，避免遮挡）
        DrawConnections();

        // Step 3: 绘制所有节点
        DrawNodes();

        // 如果正在拖拽连线，则绘制临时线
        if (connectionStartNode != null)
        {
            Handles.DrawBezier(
                connectionStartNode.position.center,
                Event.current.mousePosition + scrollPosition,
                connectionStartNode.position.center + Vector2.right * 50,
                Event.current.mousePosition + scrollPosition - Vector2.left * 50,
                Color.yellow, null, 3f);
        }

        EditorGUILayout.EndScrollView();
    }

    // 添加新状态节点
    private void AddNewState(Vector2 mousePosition)
    {
        // 将鼠标位置转换为画布坐标（考虑滚动偏移）
        Vector2 canvasPos = mousePosition - scrollPosition;
        nodes.Add(new StateMachineNode("State " + (nodes.Count + 1), canvasPos));
    }

    // 处理鼠标/键盘事件
    private void ProcessEvents()
    {
        Event e = Event.current;
        EventType eventType = e.type;
        Vector2 mousePos = e.mousePosition + scrollPosition; // 转换为画布坐标

        switch (eventType)
        {
            case EventType.MouseDown:
                if (e.button == 0) // 左键
                {
                    HandleLeftClick(mousePos);
                }
                else if (e.button == 1) // 右键：删除节点
                {
                    HandleRightClick(mousePos);
                }
                break;

            case EventType.MouseDrag:
                if (draggingNode != null)
                {
                    draggingNode.position.position = mousePos + dragOffset;
                    e.Use(); // 标记事件已处理，防止滚动
                }
                break;

            case EventType.MouseUp:
                if (e.button == 0)
                {
                    if (connectionStartNode != null)
                    {
                        HandleConnectionEnd(mousePos);
                    }
                    draggingNode = null;
                    connectionStartNode = null;
                }
                break;
        }
    }

    // 处理左键点击
    private void HandleLeftClick(Vector2 mousePos)
    {
        // 从后往前遍历（确保点击上层节点）
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            StateMachineNode node = nodes[i];
            if (node.IsMouseOver(mousePos))
            {
                // 如果已有起始节点，说明是要创建连接
                if (connectionStartNode != null && connectionStartNode != node)
                {
                    // 不做立即连接，等到 MouseUp 时再判断（避免误触）
                    return;
                }

                // 否则开始拖动节点 或 设置连线起点
                draggingNode = node;
                connectionStartNode = node;
                dragOffset = node.position.position - mousePos;
                return;
            }
        }

        // 点击空白处：取消连线
        connectionStartNode = null;
    }

    // 处理右键点击（删除节点）
    private void HandleRightClick(Vector2 mousePos)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].IsMouseOver(mousePos))
            {
                string guidToDelete = nodes[i].guid;
                nodes.RemoveAt(i);

                // 删除所有指向该节点的连接
                foreach (var node in nodes)
                {
                    node.transitions.RemoveAll(guid => guid == guidToDelete);
                }

                connectionStartNode = null;
                draggingNode = null;
                Repaint();
                return;
            }
        }
    }

    // 处理连线结束（松开左键）
    private void HandleConnectionEnd(Vector2 mousePos)
    {
        if (connectionStartNode == null) return;

        // 查找目标节点
        StateMachineNode targetNode = null;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].IsMouseOver(mousePos) && nodes[i] != connectionStartNode)
            {
                targetNode = nodes[i];
                break;
            }
        }

        // 如果找到目标且尚未连接，则添加连接
        if (targetNode != null && !connectionStartNode.transitions.Contains(targetNode.guid))
        {
            connectionStartNode.transitions.Add(targetNode.guid);
        }
    }

    // 绘制所有节点
    private void DrawNodes()
    {
        foreach (var node in nodes)
        {
            // 使用 BeginGroup 创建局部坐标系
            GUILayout.BeginArea(node.position);

            // 节点样式
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                normal = { textColor = Color.white },
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };

            // 绘制节点背景
            GUILayout.Box(node.stateName, style, GUILayout.Width(node.position.width), GUILayout.Height(node.position.height));

            GUILayout.EndArea();
        }
    }

    // 绘制所有连接线
    private void DrawConnections()
    {
        Handles.color = Color.green;
        foreach (var fromNode in nodes)
        {
            foreach (string toGuid in fromNode.transitions)
            {
                StateMachineNode toNode = nodes.Find(n => n.guid == toGuid);
                if (toNode != null)
                {
                    // 使用贝塞尔曲线让连线更美观
                    Handles.DrawBezier(
                        fromNode.position.center,
                        toNode.position.center,
                        fromNode.position.center + Vector2.right * 50,
                        toNode.position.center - Vector2.right * 50,
                        Color.green, null, 2f);
                }
            }
        }
    }
}