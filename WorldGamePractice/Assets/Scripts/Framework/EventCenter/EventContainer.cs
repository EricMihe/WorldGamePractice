using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 事件中心模块 
/// </summary>
public class EventContainer
{
    //用于记录对应事件 关联的 对应的逻辑
    private Dictionary<E_EventName, EventInfoBase> eventDic = new Dictionary<E_EventName, EventInfoBase>();


    /// <summary>
    /// 触发事件 
    /// </summary>
    /// <param name="eventName">事件名字</param>
    public void EventTrigger<T>(E_EventName eventName, T info)
    {
        //存在关心我的人 才通知别人去处理逻辑
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应的逻辑
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// 触发事件 无参数
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventName eventName)
    {
        //存在关心我的人 才通知别人去处理逻辑
        if (eventDic.ContainsKey(eventName))
        {
            //去执行对应的逻辑
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }

    /// <summary>
    /// 触发有返回值的事件（返回所有结果）
    /// </summary>
    public List<TReturn> EventTriggerWithReturn<T, TReturn>(E_EventName eventName, T info)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            return eventInfo.InvokeAll(info);
        }
        return new List<TReturn>();
    }

    /// <summary>
    /// 触发无参有返回值的事件（返回所有结果）
    /// </summary>
    public List<TReturn> EventTriggerWithReturn<TReturn>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            return eventInfo.InvokeAll();
        }
        return new List<TReturn>();
    }

    /// <summary>
    /// 触发有返回值的事件（返回最后一个结果）
    /// </summary>
    public TReturn EventTriggerWithReturnLast<T, TReturn>(E_EventName eventName, T info)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            return eventInfo.InvokeLast(info);
        }
        return default(TReturn);
    }

    /// <summary>
    /// 触发无参有返回值的事件（返回最后一个结果）
    /// </summary>
    public TReturn EventTriggerWithReturnLast<TReturn>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            return eventInfo.InvokeLast();
        }
        return default(TReturn);
    }

    /// <summary>
    /// 触发有返回值的事件（返回第一个结果）
    /// </summary>
    public TReturn EventTriggerWithReturnFirst<T, TReturn>(E_EventName eventName, T info)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            return eventInfo.InvokeFirst(info);
        }
        return default(TReturn);
    }

    /// <summary>
    /// 触发无参有返回值的事件（返回第一个结果）
    /// </summary>
    public TReturn EventTriggerWithReturnFirst<TReturn>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            return eventInfo.InvokeFirst();
        }
        return default(TReturn);
    }

    /// <summary>
    /// 添加事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(E_EventName eventName, UnityAction<T> func)
    {
        //如果已经存在关心事件的委托记录 直接添加即可
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }

    public void AddEventListener(E_EventName eventName, UnityAction func)
    {
        //如果已经存在关心事件的委托记录 直接添加即可
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }

    /// <summary>
    /// 添加有返回值的事件监听者
    /// </summary>
    public void AddEventListenerWithReturn<T, TReturn>(E_EventName eventName, System.Func<T, TReturn> func)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, new EventInfoWithReturn<T, TReturn>());
        }

        if (eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            eventInfo.AddAction(func);
        }
        else
        {
            Debug.LogError($"事件 {eventName} 已存在，但与所需类型不匹配！");
        }
    }

    /// <summary>
    /// 添加无参有返回值的事件监听者
    /// </summary>
    public void AddEventListenerWithReturn<TReturn>(E_EventName eventName, System.Func<TReturn> func)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            eventDic.Add(eventName, new EventInfoWithReturn<TReturn>());
        }

        if (eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            eventInfo.AddAction(func);
        }
        else
        {
            Debug.LogError($"事件 {eventName} 已存在，但与所需类型不匹配！");
        }
    }

    /// <summary>
    /// 移除事件监听者
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T>(E_EventName eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }

    public void RemoveEventListener(E_EventName eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }

    /// <summary>
    /// 移除有返回值的事件监听者
    /// </summary>
    public void RemoveEventListenerWithReturn<T, TReturn>(E_EventName eventName, System.Func<T, TReturn> func)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            eventInfo.RemoveAction(func);
        }
    }

    /// <summary>
    /// 移除无参有返回值的事件监听者
    /// </summary>
    public void RemoveEventListenerWithReturn<TReturn>(E_EventName eventName, System.Func<TReturn> func)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            eventInfo.RemoveAction(func);
        }
    }

    /// <summary>
    /// 清空所有事件的监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 清除指定某一个事件的所有监听
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }

    /// <summary>
    /// 检查无参无返回值事件是否存在监听者
    /// </summary>
    /// <param name="eventName">事件类型</param>
    /// <returns>是否存在监听者</returns>
    public bool HasEventListener(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfo eventInfo)
        {
            return eventInfo.HasActions();
        }
        return false;
    }

    /// <summary>
    /// 检查有参无返回值事件是否存在监听者
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件类型</param>
    /// <returns>是否存在监听者</returns>
    public bool HasEventListener<T>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfo<T> eventInfo)
        {
            return eventInfo.HasActions();
        }
        return false;
    }

    /// <summary>
    /// 检查有参有返回值事件是否存在监听者
    /// </summary>
    public bool HasEventListenerWithReturn<T, TReturn>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<T, TReturn> eventInfo)
        {
            return eventInfo.HasActions();
        }
        return false;
    }

    /// <summary>
    /// 检查无参有返回值事件是否存在监听者
    /// </summary>
    public bool HasEventListenerWithReturn<TReturn>(E_EventName eventName)
    {
        if (eventDic.ContainsKey(eventName) && eventDic[eventName] is EventInfoWithReturn<TReturn> eventInfo)
        {
            return eventInfo.HasActions();
        }
        return false;
    }
}
