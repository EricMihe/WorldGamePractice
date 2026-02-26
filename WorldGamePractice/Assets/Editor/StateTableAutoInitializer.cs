// Editor/StateTableCreator.cs
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Animations;

public class StateTableCreator
{
    
    [MenuItem("Assets/Create/配置/状态配置/创建状态机对应角色状态配置")]
    public static void CreateStateTable()
    {
        // 弹出保存对话框
        string path = EditorUtility.SaveFilePanelInProject(
            "Create State Table",
            "NewStateTable",
            "asset",
            "Save State Table Asset");

        if (string.IsNullOrEmpty(path)) return;

        // 获取用户输入的文件名（不含扩展名）
        string fileName = Path.GetFileNameWithoutExtension(path);

        // 创建 ScriptableObject 实例
        StateTableObject table = ScriptableObject.CreateInstance<StateTableObject>();

        // 查找同名 controller
        AnimatorController controller = FindAnimatorControllerByName(fileName);
        if (controller != null)
        {
            List<string> stateNames = ExtractAllStateNames(controller);
            table.states.Clear();
            foreach (string name in stateNames)
            {
                table.states.Add(new StateEntity { statename =name,animClipName = name });
            }
            Debug.Log($"绑定 {stateNames.Count} 个状态到 {fileName}");
        }
        else
        {
            Debug.LogWarning($"未找到 {fileName}.controller");
        }

        // 保存资产
        AssetDatabase.CreateAsset(table, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = table;
    }

    private static AnimatorController FindAnimatorControllerByName(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"t:animatorcontroller {name}");
        foreach (string guid in guids)
        {
            string p = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(p) == name)
                return AssetDatabase.LoadAssetAtPath<AnimatorController>(p);
        }
        return null;
    }

    private static List<string> ExtractAllStateNames(AnimatorController ctrl)
    {
        var list = new List<string>();
        foreach (var layer in ctrl.layers)
            TraverseStateMachine(layer.stateMachine, list);
        return list;
    }

    private static void TraverseStateMachine(UnityEditor.Animations.AnimatorStateMachine sm, List<string> output)
    {
        foreach (var s in sm.states) output.Add(s.state.name);
        foreach (var child in sm.stateMachines)
            TraverseStateMachine(child.stateMachine, output);
    }
}