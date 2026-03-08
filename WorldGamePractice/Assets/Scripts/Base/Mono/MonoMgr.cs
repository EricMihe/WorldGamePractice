using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块管理器
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    /// <summary>
    /// 添加Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }

    /// <summary>
    /// 移除Update帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }

    /// <summary>
    /// 添加FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent += updateFun;
    }
    /// <summary>
    /// 移除FixedUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent -= updateFun;
    }

    /// <summary>
    /// 添加LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent += updateFun;
    }

    /// <summary>
    /// 移除LateUpdate帧更新监听函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent -= updateFun;
    }

    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }

    private Dictionary<IRelate, bool> relates = new Dictionary<IRelate, bool>();
    private Coroutine refreshCoroutine;

    /// <summary>
    /// 标记某个 relate 需要延迟刷新
    /// </summary>
    public bool SetRelateValueLate(IRelate relate)
    {
        if (relate == null || !RelateCenter.Instance.relatedObjects.ContainsKey(relate))
            return false;

        // 如果已经标记为未处理（false），无需重复操作
        if (relates.TryGetValue(relate, out bool isProcessed) && !isProcessed)
            return true;

        // 标记为需要刷新
        relates[relate] = false;

        // 启动协程（如果尚未运行）
        if (refreshCoroutine == null)
        {
            refreshCoroutine = StartCoroutine(DoSetRelateValueLate());
        }

        return true;
    }

    /// <summary>
    /// 延迟刷新协程：仅在有脏数据时运行，处理完自动退出
    /// </summary>
    private IEnumerator DoSetRelateValueLate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            // 收集所有需要刷新的 key（避免遍历时修改字典）
            var toRefresh = new List<IRelate>();
            foreach (var kvp in relates)
            {
                if (!kvp.Value) // value == false 表示未处理
                {
                    toRefresh.Add(kvp.Key);
                }
            }

            // 如果没有待处理项，退出协程
            if (toRefresh.Count == 0)
            {
                refreshCoroutine = null;
                yield break;
            }

            // 批量处理
            foreach (var key in toRefresh)
            {
                // 安全检查：防止 key 已被移除
                if (relates.ContainsKey(key))
                {
                    relates[key] = true; // 标记为已处理
                    if (key.IsHasWait)
                        key.Refresh(); // 允许 Refresh() 修改 relates
                }
            }
        }
    }
}
