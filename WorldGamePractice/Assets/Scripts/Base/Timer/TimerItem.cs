using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象 里面存储了计时器的相关数据
/// </summary>
public class TimerItem : IPoolObject
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public int keyID;
    /// <summary>
    /// 计时结束后的委托回调
    /// </summary>
    public UnityAction overCallBack;
    /// <summary>
    /// 间隔一定时间去执行的委托回调
    /// </summary>
    public UnityAction callBack;

    /// <summary>
    /// 表示计时器总的计时时间 毫秒：1s = 1000ms
    /// </summary>
    public int allTime;
    /// <summary>
    /// 记录一开始计时时的总时间 用于时间重置
    /// </summary>
    public int maxAllTime;

    /// <summary>
    /// 间隔执行回调的时间 毫秒 毫秒：1s = 1000ms
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 记录一开始的间隔时间
    /// </summary>
    public int maxIntervalTime;

    /// <summary>
    /// 是否在进行计时
    /// </summary>
    public bool isRuning;

    /// <summary>
    /// 是否在计时结束后自动回收自己
    /// </summary>
    public bool autoRecycle = false;

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间计时结束后的回调</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    /// <param name="autoRecycle">计时结束后是否自动回收</param>
    public void InitInfo(int keyID, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null, bool autoRecycle = false)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
        this.autoRecycle = autoRecycle;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    /// <summary>
    /// 缓存池回收时  清除相关引用数据
    /// </summary>
    public void ResetInfo()
    {
        overCallBack = null;
        callBack = null;
    }
}

/// <summary>
/// 计时器对象 里面存储了计时器的相关数据（带参数版本）
/// </summary>
public class TimerItemWithParam<T> : IPoolObject
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public int keyID;
    /// <summary>
    /// 计时结束后的委托回调（带参数）
    /// </summary>
    public UnityAction<T> overCallBack;
    /// <summary>
    /// 间隔一定时间去执行的委托回调（带参数）
    /// </summary>
    public UnityAction<T> callBack;

    /// <summary>
    /// 传递给回调的参数值
    /// </summary>
    public T paramValue;

    /// <summary>
    /// 表示计时器总的计时时间 毫秒：1s = 1000ms
    /// </summary>
    public int allTime;
    /// <summary>
    /// 记录一开始计时时的总时间 用于时间重置
    /// </summary>
    public int maxAllTime;

    /// <summary>
    /// 间隔执行回调的时间 毫秒 毫秒：1s = 1000ms
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 记录一开始的间隔时间
    /// </summary>
    public int maxIntervalTime;

    /// <summary>
    /// 是否在进行计时
    /// </summary>
    public bool isRuning;

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间计时结束后的回调</param>
    /// <param name="paramValue">传递给回调的参数</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    public void InitInfo(int keyID, int allTime, UnityAction<T> overCallBack, T paramValue, int intervalTime = 0, UnityAction<T> callBack = null, bool autoRecycle = false)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.paramValue = paramValue;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
        this.autoRecycle = autoRecycle;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    /// <summary>
    /// 缓存池回收时  清除相关引用数据
    /// </summary>
    public void ResetInfo()
    {
        overCallBack = null;
        callBack = null;
        paramValue = default(T);
    }


    public bool autoRecycle = false; // 自回收标志
}

// 两个参数的版本
public class TimerItemWithParam<T0, T1> : IPoolObject
{
    /// <summary>
    /// 唯一ID
    /// </summary>
    public int keyID;
    /// <summary>
    /// 计时结束后的委托回调（带参数）
    /// </summary>
    public UnityAction<T0, T1> overCallBack;
    /// <summary>
    /// 间隔一定时间去执行的委托回调（带参数）
    /// </summary>
    public UnityAction<T0, T1> callBack;

    /// <summary>
    /// 传递给回调的参数值
    /// </summary>
    public T0 paramValue0;
    public T1 paramValue1;

    /// <summary>
    /// 表示计时器总的计时时间 毫秒：1s = 1000ms
    /// </summary>
    public int allTime;
    /// <summary>
    /// 记录一开始计时时的总时间 用于时间重置
    /// </summary>
    public int maxAllTime;

    /// <summary>
    /// 间隔执行回调的时间 毫秒 毫秒：1s = 1000ms
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 记录一开始的间隔时间
    /// </summary>
    public int maxIntervalTime;

    /// <summary>
    /// 是否在进行计时
    /// </summary>
    public bool isRuning;

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">总的时间</param>
    /// <param name="overCallBack">总时间计时结束后的回调</param>
    /// <param name="paramValue">传递给回调的参数</param>
    /// <param name="intervalTime">间隔执行的时间</param>
    /// <param name="callBack">间隔执行时间结束后的回调</param>
    public void InitInfo(int keyID, int allTime, UnityAction<T0, T1> overCallBack, T0 paramValue0, T1 paramValue1, int intervalTime = 0, UnityAction<T0, T1> callBack = null, bool autoRecycle = false)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.paramValue0 = paramValue0;
        this.paramValue1 = paramValue1;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
        this.autoRecycle = autoRecycle;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    public void ResetInfo()
    {
        overCallBack = null;
        callBack = null;
        paramValue0 = default(T0);
        paramValue1 = default(T1);
    }

    public bool autoRecycle = false; // 自回收标志
}

// 三个参数的版本
public class TimerItemWithParam<T0, T1, T2> : IPoolObject
{
    public int keyID;
    public UnityAction<T0, T1, T2> overCallBack;
    public UnityAction<T0, T1, T2> callBack;
    public T0 paramValue0;
    public T1 paramValue1;
    public T2 paramValue2;
    public int allTime;
    public int maxAllTime;
    public int intervalTime;
    public int maxIntervalTime;
    public bool isRuning;

    public void InitInfo(int keyID, int allTime, UnityAction<T0, T1, T2> overCallBack, T0 paramValue0, T1 paramValue1, T2 paramValue2,
        int intervalTime = 0, UnityAction<T0, T1, T2> callBack = null,bool autoRecycle = false)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.paramValue0 = paramValue0;
        this.paramValue1 = paramValue1;
        this.paramValue2 = paramValue2;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
        this.autoRecycle = autoRecycle;
    }

    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    public void ResetInfo()
    {
        overCallBack = null;
        callBack = null;
        paramValue0 = default(T0);
        paramValue1 = default(T1);
        paramValue2 = default(T2);
    }

    public bool autoRecycle = false; // 自回收标志
}
