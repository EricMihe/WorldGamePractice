// Assets/Editor/StateTableObjectEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(StateTableObject))]
public class StateTableObjectEditor : Editor
{
    private bool isSearchActive = false;
    private string searchId = "";

    // 缓存
    private List<int> cachedVisibleIndices = null;
    private string lastUsedSearchId = null;
    private int lastStatesCount = -1;

    public override void OnInspectorGUI()
    {
        // === 顶部控制栏 ===
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("状态搜索", EditorStyles.boldLabel);
        if (isSearchActive)
        {
            if (GUILayout.Button("关闭搜索", GUILayout.Width(80)))
            {
                isSearchActive = false;
                cachedVisibleIndices = null;
            }
        }
        else
        {
            if (GUILayout.Button("开启搜索", GUILayout.Width(80)))
            {
                isSearchActive = true;
                cachedVisibleIndices = null;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // === 核心逻辑 ===
        if (!isSearchActive)
        {
            // 回退到 Unity 默认 Inspector（显示完整 states 列表）
            DrawDefaultInspector();
            return;
        }

        // ===== 搜索模式：自定义绘制 =====
        serializedObject.Update();

        string newSearch = EditorGUILayout.TextField("状态名称:", searchId);
        if (newSearch != searchId)
        {
            searchId = newSearch;
            cachedVisibleIndices = null;
        }

        EditorGUILayout.Space();

        SerializedProperty statesProp = serializedObject.FindProperty("states");
        int currentCount = statesProp.arraySize;

        // 重新计算可见项（带缓存）
        if (cachedVisibleIndices == null ||
            lastUsedSearchId != searchId ||
            lastStatesCount != currentCount)
        {
            cachedVisibleIndices = new List<int>();
            lastUsedSearchId = searchId;
            lastStatesCount = currentCount;
            for (int i = 0; i < currentCount; i++)
            {
                var element = statesProp.GetArrayElementAtIndex(i);
                var nameProp = element.FindPropertyRelative("statename");
                if (nameProp != null)
                {
                    string stateName = nameProp.stringValue;

                    if (searchId.Length > 0 && stateName.StartsWith(searchId))
                    {
                        cachedVisibleIndices.Add(i);
                    }
                }
            }
        }
        else
        {
            const int MAX_DISPLAY = 50;
            int displayCount = Mathf.Min(cachedVisibleIndices.Count, MAX_DISPLAY);

            EditorGUILayout.LabelField($"{displayCount} / {cachedVisibleIndices.Count} 项", EditorStyles.miniLabel);

            for (int di = 0; di < displayCount; di++)
            {
                int index = cachedVisibleIndices[di];
                var element = statesProp.GetArrayElementAtIndex(index);
                var nameProp = element.FindPropertyRelative("statename");
                string displayName = nameProp?.stringValue ?? "null";
                EditorGUILayout.PropertyField(element, new GUIContent($"[Index: {index}] {displayName}"), true);
            }

            if (cachedVisibleIndices.Count > MAX_DISPLAY)
            {
                EditorGUILayout.HelpBox($"结果过多，仅显示前 {MAX_DISPLAY} 项", MessageType.Warning);
            }
        }

        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
    }
}