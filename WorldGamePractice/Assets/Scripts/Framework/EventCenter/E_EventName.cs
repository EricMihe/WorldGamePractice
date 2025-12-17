using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventName 
{
    /// <summary>
    /// 怪物死亡事件 —— 参数：Monster
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励 —— 参数：int
    /// </summary>
    E_Player_GetReward,
    /// <summary>
    /// 测试用事件 —— 参数：无
    /// </summary>
    E_Test,
    /// <summary>
    /// 场景切换时进度变化获取
    /// </summary>
    E_SceneLoadChange,

    /// <summary>
    /// 输入系统触发技能1 行为
    /// </summary>
    E_Input_Skill1,
    /// <summary>
    /// 输入系统触发技能2 行为
    /// </summary>
    E_Input_Skill2,
    /// <summary>
    /// 输入系统触发技能3 行为
    /// </summary>
    E_Input_Skill3,

    /// <summary>
    /// 水平热键 -1~1的事件监听
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 竖直热键 -1~1的事件监听
    /// </summary>
    E_Input_Vertical,

    //玩家输入相关

    //角色状态服务相关
    Anim_Service,
    Effect_Service,
    Physics_Service,
    Hit_Service,

    //角色监听事件
    DoUpdate,
    DoStart,
    DoEnable,
    DoDisable,
    DoDestroy
}

/// <summary>
/// 用于 里式替换原则 装载 子类的父类
/// </summary>
public abstract class EventInfoBase { }

/// <summary>
/// 用来包裹 对应观察者 函数委托的 类
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : EventInfoBase
{
    //真正观察者 对应的 函数信息 记录在其中
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }

    /// <summary>
    /// 检查是否存在监听者
    /// </summary>
    public bool HasActions()
    {
        return actions != null && actions.GetInvocationList().Length > 0;
    }
}

/// <summary>
/// 主要用来记录无参无返回值委托
/// </summary>
public class EventInfo : EventInfoBase
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }

    /// <summary>
    /// 检查是否存在监听者
    /// </summary>
    public bool HasActions()
    {
        return actions != null && actions.GetInvocationList().Length > 0;
    }
}

/// <summary>
/// 用来包裹 有参数有返回值观察者 函数委托的类
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
/// <typeparam name="TReturn">返回值类型</typeparam>
public class EventInfoWithReturn<T, TReturn> : EventInfoBase
{
    // 存储返回多个返回值的委托列表
    private List<System.Func<T, TReturn>> actionList = new List<System.Func<T, TReturn>>();

    public void AddAction(System.Func<T, TReturn> action)
    {
        actionList.Add(action);
    }

    public void RemoveAction(System.Func<T, TReturn> action)
    {
        actionList.Remove(action);
    }

    /// <summary>
    /// 触发所有监听函数并返回结果列表
    /// </summary>
    public List<TReturn> InvokeAll(T info)
    {
        List<TReturn> results = new List<TReturn>();
        foreach (var action in actionList)
        {
            if (action != null)
            {
                results.Add(action.Invoke(info));
            }
        }
        return results;
    }

    /// <summary>
    /// 触发所有监听函数并返回最后一个结果
    /// </summary>
    public TReturn InvokeLast(T info)
    {
        if (actionList.Count > 0)
        {
            return actionList[actionList.Count - 1].Invoke(info);
        }
        return default(TReturn);
    }

    /// <summary>
    /// 触发所有监听函数并返回第一个结果
    /// </summary>
    public TReturn InvokeFirst(T info)
    {
        if (actionList.Count > 0)
        {
            return actionList[0].Invoke(info);
        }
        return default(TReturn);
    }

    /// <summary>
    /// 检查是否存在监听者
    /// </summary>
    public bool HasActions()
    {
        return actionList.Count > 0;
    }

    public void Clear()
    {
        actionList.Clear();
    }
}

/// <summary>
/// 用来包裹 无参有返回值观察者 函数委托的类
/// </summary>
/// <typeparam name="TReturn">返回值类型</typeparam>
public class EventInfoWithReturn<TReturn> : EventInfoBase
{
    // 存储返回多个返回值的委托列表
    private List<System.Func<TReturn>> actionList = new List<System.Func<TReturn>>();

    public void AddAction(System.Func<TReturn> action)
    {
        actionList.Add(action);
    }

    public void RemoveAction(System.Func<TReturn> action)
    {
        actionList.Remove(action);
    }

    /// <summary>
    /// 触发所有监听函数并返回结果列表
    /// </summary>
    public List<TReturn> InvokeAll()
    {
        List<TReturn> results = new List<TReturn>();
        foreach (var action in actionList)
        {
            if (action != null)
            {
                results.Add(action.Invoke());
            }
        }
        return results;
    }

    /// <summary>
    /// 触发所有监听函数并返回最后一个结果
    /// </summary>
    public TReturn InvokeLast()
    {
        if (actionList.Count > 0)
        {
            return actionList[actionList.Count - 1].Invoke();
        }
        return default(TReturn);
    }

    /// <summary>
    /// 触发所有监听函数并返回第一个结果
    /// </summary>
    public TReturn InvokeFirst()
    {
        if (actionList.Count > 0)
        {
            return actionList[0].Invoke();
        }
        return default(TReturn);
    }

    /// <summary>
    /// 检查是否存在监听者
    /// </summary>
    public bool HasActions()
    {
        return actionList.Count > 0;
    }

    public void Clear()
    {
        actionList.Clear();
    }
}
