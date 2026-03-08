//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

///// <summary>
///// 计时器管理器 主要用于开启、停止、重置等等操作来管理计时器（带参数版本）
///// </summary>
//public class TimerMgr : BaseManager<TimerMgr>
//{
//    /// <summary>
//    /// 用于记录当前将要创建的唯一ID的
//    /// </summary>
//    private int TIMER_KEY = 0;
//    /// <summary>
//    /// 用于存储管理所有计时器的字典容器
//    /// </summary>
//    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
//    /// <summary>
//    /// 用于存储管理所有带参数计时器的字典容器
//    /// </summary>
//    private Dictionary<int, TimerItemWithParam<object>> timerWithParamDic = new Dictionary<int, TimerItemWithParam<object>>();
//    private Dictionary<int, TimerItemWithParam<object, object>> timerWithParam2Dic = new Dictionary<int, TimerItemWithParam<object, object>>();
//    private Dictionary<int, TimerItemWithParam<object, object, object>> timerWithParam3Dic = new Dictionary<int, TimerItemWithParam<object, object, object>>();

//    /// <summary>
//    /// 用于存储管理所有计时器的字典容器（不受Time.timeScale影响的计时器）
//    /// </summary>
//    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
//    private Dictionary<int, TimerItemWithParam<object>> realTimerWithParamDic = new Dictionary<int, TimerItemWithParam<object>>();
//    private Dictionary<int, TimerItemWithParam<object, object>> realTimerWithParam2Dic = new Dictionary<int, TimerItemWithParam<object, object>>();
//    private Dictionary<int, TimerItemWithParam<object, object, object>> realTimerWithParam3Dic = new Dictionary<int, TimerItemWithParam<object, object, object>>();

//    /// <summary>
//    /// 待移除列表
//    /// </summary>
//    private List<TimerItem> delList = new List<TimerItem>();
//    private List<TimerItemWithParam<object>> delListWithParam = new List<TimerItemWithParam<object>>();
//    private List<TimerItemWithParam<object, object>> delListWithParam2 = new List<TimerItemWithParam<object, object>>();
//    private List<TimerItemWithParam<object, object, object>> delListWithParam3 = new List<TimerItemWithParam<object, object, object>>();

//    //为了避免内存的浪费 每次while都会生成 
//    //我们直接将其声明为成员变量
//    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.1f);
//    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

//    private Coroutine timer;
//    private Coroutine realTimer;

//    /// <summary>
//    /// 计时器管理器中的唯一计时用的协同程序 的间隔时间
//    /// </summary>
//    private const float intervalTime = 0.1f;

//    private TimerMgr()
//    {
//        //默认计时器就是开启的
//        Start();
//    }

//    //开启计时器管理器的方法
//    public void Start()
//    {
//        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false));
//        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true));
//    }

//    //关闭计时器管理器的方法
//    public void Stop()
//    {
//        MonoMgr.Instance.StopCoroutine(timer);
//        MonoMgr.Instance.StopCoroutine(realTimer);
//    }

//    IEnumerator StartTiming(bool isRealTime)
//    {
//        while (true)
//        {
//            //50毫秒进行一次计时
//            if (isRealTime)
//                yield return waitForSecondsRealtime;
//            else
//                yield return waitForSeconds;

//            // 更新普通计时器
//            UpdateTimer(timerDic, delList, isRealTime ? realTimerDic : timerDic);

//            // 更新带参数计时器（1个参数）
//            UpdateTimerWithParam(isRealTime ? realTimerWithParamDic : timerWithParamDic, delListWithParam);

//            // 更新带参数计时器（2个参数）
//            UpdateTimerWithParam2(isRealTime ? realTimerWithParam2Dic : timerWithParam2Dic, delListWithParam2);

//            // 更新带参数计时器（3个参数）
//            UpdateTimerWithParam3(isRealTime ? realTimerWithParam3Dic : timerWithParam3Dic, delListWithParam3);

//            // 清理待移除列表
//            ClearDelList(timerDic, delList);
//            ClearDelListWithParam(realTimerWithParamDic, timerWithParamDic, delListWithParam);
//            ClearDelListWithParam2(realTimerWithParam2Dic, timerWithParam2Dic, delListWithParam2);
//            ClearDelListWithParam3(realTimerWithParam3Dic, timerWithParam3Dic, delListWithParam3);
//        }
//    }

//    private void UpdateTimer(Dictionary<int, TimerItem> timerDic, List<TimerItem> delList, Dictionary<int, TimerItem> sourceDic)
//    {
//        foreach (TimerItem item in sourceDic.Values)
//        {
//            if (!item.isRuning)
//                continue;

//            if (item.callBack != null)
//            {
//                item.intervalTime -= (int)(intervalTime * 1000);
//                if (item.intervalTime <= 0)
//                {
//                    item.callBack.Invoke();
//                    item.intervalTime = item.maxIntervalTime;
//                }
//            }

//            item.allTime -= (int)(intervalTime * 1000);
//            if (item.allTime <= 0)
//            {
//                // 执行完成回调
//                item.overCallBack?.Invoke();

//                // 检查是否需要自动回收
//                if (item.autoRecycle)
//                {
//                    // 直接回收，不添加到删除列表
//                    var targetDic = timerDic.ContainsKey(item.keyID) ? timerDic : realTimerDic;
//                    targetDic.Remove(item.keyID);
//                    PoolMgr.Instance.PushObj(item);
//                }
//                else
//                {
//                    // 添加到删除列表（原有的处理方式）
//                    delList.Add(item);
//                }
//            }
//        }
//    }

//    private void UpdateTimerWithParam<T>(Dictionary<int, TimerItemWithParam<T>> timerDic, List<TimerItemWithParam<T>> delList)
//    {
//        foreach (TimerItemWithParam<T> item in timerDic.Values)
//        {
//            if (!item.isRuning)
//                continue;

//            if (item.callBack != null)
//            {
//                item.intervalTime -= (int)(intervalTime * 1000);
//                if (item.intervalTime <= 0)
//                {
//                    item.callBack.Invoke(item.paramValue);
//                    item.intervalTime = item.maxIntervalTime;
//                }
//            }

//            item.allTime -= (int)(intervalTime * 1000);
//            if (item.allTime <= 0)
//            {
//                item.overCallBack.Invoke(item.paramValue);
//                delList.Add(item);
//            }
//        }
//    }

//    private void UpdateTimerWithParam2<T0, T1>(Dictionary<int, TimerItemWithParam<T0, T1>> timerDic, List<TimerItemWithParam<T0, T1>> delList)
//    {
//        foreach (TimerItemWithParam<T0, T1> item in timerDic.Values)
//        {
//            if (!item.isRuning)
//                continue;

//            if (item.callBack != null)
//            {
//                item.intervalTime -= (int)(intervalTime * 1000);
//                if (item.intervalTime <= 0)
//                {
//                    item.callBack.Invoke(item.paramValue0, item.paramValue1);
//                    item.intervalTime = item.maxIntervalTime;
//                }
//            }

//            item.allTime -= (int)(intervalTime * 1000);
//            if (item.allTime <= 0)
//            {
//                item.overCallBack.Invoke(item.paramValue0, item.paramValue1);
//                delList.Add(item);
//            }
//        }
//    }

//    private void UpdateTimerWithParam3<T0, T1, T2>(Dictionary<int, TimerItemWithParam<T0, T1, T2>> timerDic, List<TimerItemWithParam<T0, T1, T2>> delList)
//    {
//        foreach (TimerItemWithParam<T0, T1, T2> item in timerDic.Values)
//        {
//            if (!item.isRuning)
//                continue;

//            if (item.callBack != null)
//            {
//                item.intervalTime -= (int)(intervalTime * 1000);
//                if (item.intervalTime <= 0)
//                {
//                    item.callBack.Invoke(item.paramValue0, item.paramValue1, item.paramValue2);
//                    item.intervalTime = item.maxIntervalTime;
//                }
//            }

//            item.allTime -= (int)(intervalTime * 1000);
//            if (item.allTime <= 0)
//            {
//                item.overCallBack.Invoke(item.paramValue0, item.paramValue1, item.paramValue2);
//                delList.Add(item);
//            }
//        }
//    }

//    private void ClearDelList(Dictionary<int, TimerItem> timerDic, List<TimerItem> delList)
//    {
//        for (int i = 0; i < delList.Count; i++)
//        {
//            timerDic.Remove(delList[i].keyID);
//            PoolMgr.Instance.PushObj(delList[i]);
//        }
//        delList.Clear();
//    }

//    private void ClearDelListWithParam<T>(Dictionary<int, TimerItemWithParam<T>> realDic, Dictionary<int, TimerItemWithParam<T>> normalDic, List<TimerItemWithParam<T>> delList)
//    {
//        for (int i = 0; i < delList.Count; i++)
//        {
//            var item = delList[i];
//            if (realDic.ContainsKey(item.keyID))
//            {
//                realDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//            else if (normalDic.ContainsKey(item.keyID))
//            {
//                normalDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//        }
//        delList.Clear();
//    }

//    private void ClearDelListWithParam2<T0, T1>(Dictionary<int, TimerItemWithParam<T0, T1>> realDic, Dictionary<int, TimerItemWithParam<T0, T1>> normalDic, List<TimerItemWithParam<T0, T1>> delList)
//    {
//        for (int i = 0; i < delList.Count; i++)
//        {
//            var item = delList[i];
//            if (realDic.ContainsKey(item.keyID))
//            {
//                realDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//            else if (normalDic.ContainsKey(item.keyID))
//            {
//                normalDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//        }
//        delList.Clear();
//    }

//    private void ClearDelListWithParam3<T0, T1, T2>(Dictionary<int, TimerItemWithParam<T0, T1, T2>> realDic, Dictionary<int, TimerItemWithParam<T0, T1, T2>> normalDic, List<TimerItemWithParam<T0, T1, T2>> delList)
//    {
//        for (int i = 0; i < delList.Count; i++)
//        {
//            var item = delList[i];
//            if (realDic.ContainsKey(item.keyID))
//            {
//                realDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//            else if (normalDic.ContainsKey(item.keyID))
//            {
//                normalDic.Remove(item.keyID);
//                PoolMgr.Instance.PushObj(item);
//            }
//        }
//        delList.Clear();
//    }

//    /// <summary>
//    /// 创建单个计时器
//    /// </summary>
//    public int CreateTimer(float allTime, UnityAction overCallBack, bool autoRecycle = false,float intervalTime = 0,  UnityAction callBack = null,bool isRealTime = false)
//    {
//        int alltimeInt = (int)(allTime * 1000);
//        int intervalTimeInt= (int)(intervalTime * 1000);
//        int keyID = ++TIMER_KEY;
//        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
//        timerItem.InitInfo(keyID, alltimeInt, overCallBack, intervalTimeInt, callBack, autoRecycle);

//        if (isRealTime)
//            realTimerDic.Add(keyID, timerItem);
//        else
//            timerDic.Add(keyID, timerItem);
//        return keyID;
//    }

//    /// <summary>
//    /// 创建单个计时器（带1个参数版本）
//    /// </summary>
//    public int CreateTimerWithParam<T>(float allTime, UnityAction<T> overCallBack, T paramValue, float intervalTime = 0,  UnityAction<T> callBack = null, bool isRealTime = false)
//    {
//        int alltimeInt = (int)(allTime * 1000);
//        int intervalTimeInt = (int)(intervalTime * 1000);
//        int keyID = ++TIMER_KEY;
//        TimerItemWithParam<T> timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T>>();
//        timerItem.InitInfo(keyID, alltimeInt, overCallBack, paramValue, intervalTimeInt, callBack);

//        if (isRealTime)
//            realTimerWithParamDic.Add(keyID, (TimerItemWithParam<object>)(object)timerItem);
//        else
//            timerWithParamDic.Add(keyID, (TimerItemWithParam<object>)(object)timerItem);
//        return keyID;
//    }

//    /// <summary>
//    /// 创建单个计时器（带2个参数版本）
//    /// </summary>
//    public int CreateTimerWithParam2<T0, T1>(float allTime, UnityAction<T0, T1> overCallBack, T0 paramValue0, T1 paramValue1,
//        float  intervalTime = 0, UnityAction<T0, T1> callBack = null,bool isRealTime = false)
//    {
//        int alltimeInt = (int)(allTime * 1000);
//        int intervalTimeInt = (int)(intervalTime * 1000);
//        int keyID = ++TIMER_KEY;
//        TimerItemWithParam<T0, T1> timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T0, T1>>();
//        timerItem.InitInfo(keyID, alltimeInt, overCallBack, paramValue0, paramValue1, intervalTimeInt, callBack);

//        if (isRealTime)
//            realTimerWithParam2Dic.Add(keyID, (TimerItemWithParam<object, object>)(object)timerItem);
//        else
//            timerWithParam2Dic.Add(keyID, (TimerItemWithParam<object, object>)(object)timerItem);
//        return keyID;
//    }

//    /// <summary>
//    /// 创建单个计时器（带3个参数版本）
//    /// </summary>
//    public int CreateTimerWithParam3<T0, T1, T2>(float allTime, UnityAction<T0, T1, T2> overCallBack, T0 paramValue0, T1 paramValue1, T2 paramValue2,
//        float intervalTime = 0,  UnityAction<T0, T1, T2> callBack = null, bool isRealTime = false)
//    {
//        int alltimeInt = (int)(allTime * 1000);
//        int intervalTimeInt = (int)(intervalTime * 1000);
//        int keyID = ++TIMER_KEY;
//        TimerItemWithParam<T0, T1, T2> timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T0, T1, T2>>();
//        timerItem.InitInfo(keyID, alltimeInt, overCallBack, paramValue0, paramValue1, paramValue2, intervalTimeInt, callBack);

//        if (isRealTime)
//            realTimerWithParam3Dic.Add(keyID, (TimerItemWithParam<object, object, object>)(object)timerItem);
//        else
//            timerWithParam3Dic.Add(keyID, (TimerItemWithParam<object, object, object>)(object)timerItem);
//        return keyID;
//    }

//    //移除单个计时器
//    public void RemoveTimer(int keyID)
//    {
//        if (timerDic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj(timerDic[keyID]);
//            timerDic.Remove(keyID);
//        }
//        else if (realTimerDic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
//            realTimerDic.Remove(keyID);
//        }
//        else if (timerWithParamDic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object>)timerWithParamDic[keyID]);
//            timerWithParamDic.Remove(keyID);
//        }
//        else if (realTimerWithParamDic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object>)realTimerWithParamDic[keyID]);
//            realTimerWithParamDic.Remove(keyID);
//        }
//        else if (timerWithParam2Dic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object, object>)timerWithParam2Dic[keyID]);
//            timerWithParam2Dic.Remove(keyID);
//        }
//        else if (realTimerWithParam2Dic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object, object>)realTimerWithParam2Dic[keyID]);
//            realTimerWithParam2Dic.Remove(keyID);
//        }
//        else if (timerWithParam3Dic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object, object, object>)timerWithParam3Dic[keyID]);
//            timerWithParam3Dic.Remove(keyID);
//        }
//        else if (realTimerWithParam3Dic.ContainsKey(keyID))
//        {
//            PoolMgr.Instance.PushObj((TimerItemWithParam<object, object, object>)realTimerWithParam3Dic[keyID]);
//            realTimerWithParam3Dic.Remove(keyID);
//        }
//    }

//    /// <summary>
//    /// 重置单个计时器
//    /// </summary>
//    public void ResetTimer(int keyID)
//    {
//        if (timerDic.ContainsKey(keyID))
//        {
//            timerDic[keyID].ResetTimer();
//        }
//        else if (realTimerDic.ContainsKey(keyID))
//        {
//            realTimerDic[keyID].ResetTimer();
//        }
//        else if (timerWithParamDic.ContainsKey(keyID))
//        {
//            timerWithParamDic[keyID].ResetTimer();
//        }
//        else if (realTimerWithParamDic.ContainsKey(keyID))
//        {
//            realTimerWithParamDic[keyID].ResetTimer();
//        }
//        else if (timerWithParam2Dic.ContainsKey(keyID))
//        {
//            timerWithParam2Dic[keyID].ResetTimer();
//        }
//        else if (realTimerWithParam2Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam2Dic[keyID].ResetTimer();
//        }
//        else if (timerWithParam3Dic.ContainsKey(keyID))
//        {
//            timerWithParam3Dic[keyID].ResetTimer();
//        }
//        else if (realTimerWithParam3Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam3Dic[keyID].ResetTimer();
//        }
//    }

//    /// <summary>
//    /// 开启当个计时器 主要用于暂停后重新开始
//    /// </summary>
//    public void StartTimer(int keyID)
//    {
//        if (timerDic.ContainsKey(keyID))
//        {
//            timerDic[keyID].isRuning = true;
//        }
//        else if (realTimerDic.ContainsKey(keyID))
//        {
//            realTimerDic[keyID].isRuning = true;
//        }
//        else if (timerWithParamDic.ContainsKey(keyID))
//        {
//            timerWithParamDic[keyID].isRuning = true;
//        }
//        else if (realTimerWithParamDic.ContainsKey(keyID))
//        {
//            realTimerWithParamDic[keyID].isRuning = true;
//        }
//        else if (timerWithParam2Dic.ContainsKey(keyID))
//        {
//            timerWithParam2Dic[keyID].isRuning = true;
//        }
//        else if (realTimerWithParam2Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam2Dic[keyID].isRuning = true;
//        }
//        else if (timerWithParam3Dic.ContainsKey(keyID))
//        {
//            timerWithParam3Dic[keyID].isRuning = true;
//        }
//        else if (realTimerWithParam3Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam3Dic[keyID].isRuning = true;
//        }
//    }

//    /// <summary>
//    /// 停止单个计时器 主要用于暂停
//    /// </summary>
//    public void StopTimer(int keyID)
//    {
//        if (timerDic.ContainsKey(keyID))
//        {
//            timerDic[keyID].isRuning = false;
//        }
//        else if (realTimerDic.ContainsKey(keyID))
//        {
//            realTimerDic[keyID].isRuning = false;
//        }
//        else if (timerWithParamDic.ContainsKey(keyID))
//        {
//            timerWithParamDic[keyID].isRuning = false;
//        }
//        else if (realTimerWithParamDic.ContainsKey(keyID))
//        {
//            realTimerWithParamDic[keyID].isRuning = false;
//        }
//        else if (timerWithParam2Dic.ContainsKey(keyID))
//        {
//            timerWithParam2Dic[keyID].isRuning = false;
//        }
//        else if (realTimerWithParam2Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam2Dic[keyID].isRuning = false;
//        }
//        else if (timerWithParam3Dic.ContainsKey(keyID))
//        {
//            timerWithParam3Dic[keyID].isRuning = false;
//        }
//        else if (realTimerWithParam3Dic.ContainsKey(keyID))
//        {
//            realTimerWithParam3Dic[keyID].isRuning = false;
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等等操作来管理计时器（带参数版本）
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    private int TIMER_KEY = 0;

    // 普通计时器
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>();
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();

    // 1个参数
    private Dictionary<int, TimerItemWithParam<object>> timerWithParamDic = new Dictionary<int, TimerItemWithParam<object>>();
    private Dictionary<int, TimerItemWithParam<object>> realTimerWithParamDic = new Dictionary<int, TimerItemWithParam<object>>();

    // 2个参数
    private Dictionary<int, TimerItemWithParam<object, object>> timerWithParam2Dic = new Dictionary<int, TimerItemWithParam<object, object>>();
    private Dictionary<int, TimerItemWithParam<object, object>> realTimerWithParam2Dic = new Dictionary<int, TimerItemWithParam<object, object>>();

    // 3个参数
    private Dictionary<int, TimerItemWithParam<object, object, object>> timerWithParam3Dic = new Dictionary<int, TimerItemWithParam<object, object, object>>();
    private Dictionary<int, TimerItemWithParam<object, object, object>> realTimerWithParam3Dic = new Dictionary<int, TimerItemWithParam<object, object, object>>();

    // 待移除列表（统一用 object 版本，因为存储的就是 object 泛型）
    private List<TimerItem> delList = new List<TimerItem>();
    private List<TimerItemWithParam<object>> delListWithParam = new List<TimerItemWithParam<object>>();
    private List<TimerItemWithParam<object, object>> delListWithParam2 = new List<TimerItemWithParam<object, object>>();
    private List<TimerItemWithParam<object, object, object>> delListWithParam3 = new List<TimerItemWithParam<object, object, object>>();

    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(0.1f);
    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

    private Coroutine timer;
    private Coroutine realTimer;

    private const float intervalTime = 0.1f; // 单位：秒

    private TimerMgr()
    {
        Start();
    }

    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(StartTiming(false));
        realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true));
    }

    public void Stop()
    {
        if (timer != null) MonoMgr.Instance.StopCoroutine(timer);
        if (realTimer != null) MonoMgr.Instance.StopCoroutine(realTimer);
    }

    IEnumerator StartTiming(bool isRealTime)
    {
        while (true)
        {
            yield return isRealTime ? waitForSecondsRealtime : waitForSeconds;

            int deltaMs = (int)(intervalTime * 1000); // 每次减少的毫秒数

            // 更新普通计时器
            UpdateTimer(isRealTime ? realTimerDic : timerDic, delList, deltaMs);

            // 更新带参计时器（1参数）
            UpdateTimerWithParam(isRealTime ? realTimerWithParamDic : timerWithParamDic, delListWithParam, deltaMs);

            // 更新带参计时器（2参数）
            UpdateTimerWithParam2(isRealTime ? realTimerWithParam2Dic : timerWithParam2Dic, delListWithParam2, deltaMs);

            // 更新带参计时器（3参数）
            UpdateTimerWithParam3(isRealTime ? realTimerWithParam3Dic : timerWithParam3Dic, delListWithParam3, deltaMs);

            // 清理
            ClearDelList(timerDic, realTimerDic, delList);
            ClearDelListWithParam(timerWithParamDic, realTimerWithParamDic, delListWithParam);
            ClearDelListWithParam2(timerWithParam2Dic, realTimerWithParam2Dic, delListWithParam2);
            ClearDelListWithParam3(timerWithParam3Dic, realTimerWithParam3Dic, delListWithParam3);
        }
    }

    #region Update Methods

    private void UpdateTimer(Dictionary<int, TimerItem> dic, List<TimerItem> delList, int deltaMs)
    {
        foreach (var kvp in dic.ToArray()) // ToArray 避免在遍历时修改字典
        {
            var item = kvp.Value;
            if (!item.isRuning) continue;

            if (item.callBack != null)
            {
                item.intervalTime -= deltaMs;
                if (item.intervalTime <= 0)
                {
                    item.callBack.Invoke();
                    item.intervalTime = item.maxIntervalTime;
                }
            }

            item.allTime -= deltaMs;
            if (item.allTime <= 0)
            {
                item.overCallBack?.Invoke();

                if (item.autoRecycle)
                {
                    dic.Remove(kvp.Key);
                    PoolMgr.Instance.PushObj(item);
                }
                else
                {
                    delList.Add(item);
                }
            }
        }
    }

    private void UpdateTimerWithParam(Dictionary<int, TimerItemWithParam<object>> dic, List<TimerItemWithParam<object>> delList, int deltaMs)
    {
        foreach (var kvp in dic.ToArray())
        {
            var item = kvp.Value;
            if (!item.isRuning) continue;

            if (item.callBack != null)
            {
                item.intervalTime -= deltaMs;
                if (item.intervalTime <= 0)
                {
                    item.callBack.Invoke(item.paramValue);
                    item.intervalTime = item.maxIntervalTime;
                }
            }

            item.allTime -= deltaMs;
            if (item.allTime <= 0)
            {
                item.overCallBack?.Invoke(item.paramValue);

                if (item.autoRecycle)
                {
                    dic.Remove(kvp.Key);
                    PoolMgr.Instance.PushObj(item);
                }
                else
                {
                    delList.Add(item);
                }
            }
        }
    }

    private void UpdateTimerWithParam2(Dictionary<int, TimerItemWithParam<object, object>> dic, List<TimerItemWithParam<object, object>> delList, int deltaMs)
    {
        foreach (var kvp in dic.ToArray())
        {
            var item = kvp.Value;
            if (!item.isRuning) continue;

            if (item.callBack != null)
            {
                item.intervalTime -= deltaMs;
                if (item.intervalTime <= 0)
                {
                    item.callBack.Invoke(item.paramValue0, item.paramValue1);
                    item.intervalTime = item.maxIntervalTime;
                }
            }

            item.allTime -= deltaMs;
            if (item.allTime <= 0)
            {
                item.overCallBack?.Invoke(item.paramValue0, item.paramValue1);

                if (item.autoRecycle)
                {
                    dic.Remove(kvp.Key);
                    PoolMgr.Instance.PushObj(item);
                }
                else
                {
                    delList.Add(item);
                }
            }
        }
    }

    private void UpdateTimerWithParam3(Dictionary<int, TimerItemWithParam<object, object, object>> dic, List<TimerItemWithParam<object, object, object>> delList, int deltaMs)
    {
        foreach (var kvp in dic.ToArray())
        {
            var item = kvp.Value;
            if (!item.isRuning) continue;

            if (item.callBack != null)
            {
                item.intervalTime -= deltaMs;
                if (item.intervalTime <= 0)
                {
                    item.callBack.Invoke(item.paramValue0, item.paramValue1, item.paramValue2);
                    item.intervalTime = item.maxIntervalTime;
                }
            }

            item.allTime -= deltaMs;
            if (item.allTime <= 0)
            {
                item.overCallBack?.Invoke(item.paramValue0, item.paramValue1, item.paramValue2);

                if (item.autoRecycle)
                {
                    dic.Remove(kvp.Key);
                    PoolMgr.Instance.PushObj(item);
                }
                else
                {
                    delList.Add(item);
                }
            }
        }
    }

    #endregion

    #region Clear Del Lists

    private void ClearDelList(Dictionary<int, TimerItem> normalDic, Dictionary<int, TimerItem> realDic, List<TimerItem> delList)
    {
        foreach (var item in delList)
        {
            normalDic.Remove(item.keyID);
            realDic.Remove(item.keyID); // 安全，不存在也不报错
            PoolMgr.Instance.PushObj(item);
        }
        delList.Clear();
    }

    private void ClearDelListWithParam(Dictionary<int, TimerItemWithParam<object>> normalDic, Dictionary<int, TimerItemWithParam<object>> realDic, List<TimerItemWithParam<object>> delList)
    {
        foreach (var item in delList)
        {
            normalDic.Remove(item.keyID);
            realDic.Remove(item.keyID);
            PoolMgr.Instance.PushObj(item);
        }
        delList.Clear();
    }

    private void ClearDelListWithParam2(Dictionary<int, TimerItemWithParam<object, object>> normalDic, Dictionary<int, TimerItemWithParam<object, object>> realDic, List<TimerItemWithParam<object, object>> delList)
    {
        foreach (var item in delList)
        {
            normalDic.Remove(item.keyID);
            realDic.Remove(item.keyID);
            PoolMgr.Instance.PushObj(item);
        }
        delList.Clear();
    }

    private void ClearDelListWithParam3(Dictionary<int, TimerItemWithParam<object, object, object>> normalDic, Dictionary<int, TimerItemWithParam<object, object, object>> realDic, List<TimerItemWithParam<object, object, object>> delList)
    {
        foreach (var item in delList)
        {
            normalDic.Remove(item.keyID);
            realDic.Remove(item.keyID);
            PoolMgr.Instance.PushObj(item);
        }
        delList.Clear();
    }

    #endregion

    #region Public APIs (保持接口不变)

    public int CreateTimer(float allTime, UnityAction overCallBack, bool autoRecycle = false, float intervalTime = 0, UnityAction callBack = null, bool isRealTime = false)
    {
        int keyID = ++TIMER_KEY;
        var timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        timerItem.InitInfo(keyID, (int)(allTime * 1000), overCallBack, (int)(intervalTime * 1000), callBack, autoRecycle);

        if (isRealTime)
            realTimerDic[keyID] = timerItem;
        else
            timerDic[keyID] = timerItem;

        return keyID;
    }

    public int CreateTimerWithParam<T>(float allTime, UnityAction<T> overCallBack, T paramValue,
        float intervalTime = 0, UnityAction<T> callBack = null, bool isRealTime = false)
    {
        int keyID = ++TIMER_KEY;
        var timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T>>();
        timerItem.InitInfo(keyID, (int)(allTime * 1000), overCallBack, paramValue, (int)(intervalTime * 1000), callBack);

        // 关键：包装成 object 泛型，但只在创建时做一次装箱（T → object）
        var boxed = new TimerItemWithParam<object>
        {
            keyID = keyID,
            allTime = timerItem.allTime,
            maxAllTime = timerItem.maxAllTime,
            intervalTime = timerItem.intervalTime,
            maxIntervalTime = timerItem.maxIntervalTime,
            isRuning = timerItem.isRuning,
            autoRecycle = timerItem.autoRecycle,
            paramValue = paramValue, // T → object（装箱或引用转换）
            overCallBack = (UnityAction<object>)(x => overCallBack((T)x)),
            callBack = callBack == null ? null : (UnityAction<object>)(x => callBack((T)x))
        };

        // 回收原始泛型对象（因为我们复制了数据）
        PoolMgr.Instance.PushObj(timerItem);

        if (isRealTime)
            realTimerWithParamDic[keyID] = boxed;
        else
            timerWithParamDic[keyID] = boxed;

        return keyID;
    }
    
    public int CreateTimerWithParam2<T0, T1>(float allTime, UnityAction<T0, T1> overCallBack, T0 paramValue0, T1 paramValue1,
        float intervalTime = 0, UnityAction<T0, T1> callBack = null, bool isRealTime = false)
    {
        int keyID = ++TIMER_KEY;
        var timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T0, T1>>();
        timerItem.InitInfo(keyID, (int)(allTime * 1000), overCallBack, paramValue0, paramValue1, (int)(intervalTime * 1000), callBack);

        var boxed = new TimerItemWithParam<object, object>
        {
            keyID = keyID,
            allTime = timerItem.allTime,
            maxAllTime = timerItem.maxAllTime,
            intervalTime = timerItem.intervalTime,
            maxIntervalTime = timerItem.maxIntervalTime,
            isRuning = timerItem.isRuning,
            autoRecycle = timerItem.autoRecycle,
            paramValue0 = paramValue0,
            paramValue1 = paramValue1,
            overCallBack = (UnityAction<object, object>)((x, y) => overCallBack((T0)x, (T1)y)),
            callBack = callBack == null ? null : (UnityAction<object, object>)((x, y) => callBack((T0)x, (T1)y))
        };

        PoolMgr.Instance.PushObj(timerItem);

        if (isRealTime)
            realTimerWithParam2Dic[keyID] = boxed;
        else
            timerWithParam2Dic[keyID] = boxed;

        return keyID;
    }

    public int CreateTimerWithParam3<T0, T1, T2>(float allTime, UnityAction<T0, T1, T2> overCallBack, T0 paramValue0, T1 paramValue1, T2 paramValue2,
        float intervalTime = 0, UnityAction<T0, T1, T2> callBack = null, bool isRealTime = false)
    {
        int keyID = ++TIMER_KEY;
        var timerItem = PoolMgr.Instance.GetObj<TimerItemWithParam<T0, T1, T2>>();
        timerItem.InitInfo(keyID, (int)(allTime * 1000), overCallBack, paramValue0, paramValue1, paramValue2, (int)(intervalTime * 1000), callBack);

        var boxed = new TimerItemWithParam<object, object, object>
        {
            keyID = keyID,
            allTime = timerItem.allTime,
            maxAllTime = timerItem.maxAllTime,
            intervalTime = timerItem.intervalTime,
            maxIntervalTime = timerItem.maxIntervalTime,
            isRuning = timerItem.isRuning,
            autoRecycle = timerItem.autoRecycle,
            paramValue0 = paramValue0,
            paramValue1 = paramValue1,
            paramValue2 = paramValue2,
            overCallBack = (UnityAction<object, object, object>)((x, y, z) => overCallBack((T0)x, (T1)y, (T2)z)),
            callBack = callBack == null ? null : (UnityAction<object, object, object>)((x, y, z) => callBack((T0)x, (T1)y, (T2)z))
        };

        PoolMgr.Instance.PushObj(timerItem);

        if (isRealTime)
            realTimerWithParam3Dic[keyID] = boxed;
        else
            timerWithParam3Dic[keyID] = boxed;

        return keyID;
    }

    #endregion

    #region Control Methods (保持接口不变)

    public void RemoveTimer(int keyID)
    {
        TryRemove(ref timerDic, ref realTimerDic, keyID, delList);
        TryRemove(ref timerWithParamDic, ref realTimerWithParamDic, keyID, delListWithParam);
        TryRemove(ref timerWithParam2Dic, ref realTimerWithParam2Dic, keyID, delListWithParam2);
        TryRemove(ref timerWithParam3Dic, ref realTimerWithParam3Dic, keyID, delListWithParam3);
    }

    private void TryRemove<T>(ref Dictionary<int, T> normal, ref Dictionary<int, T> real, int keyID, List<T> delList) where T : class, IPoolObject
    {
        if (normal.TryGetValue(keyID, out T item))
        {
            normal.Remove(keyID);
            PoolMgr.Instance.PushObj(item);
        }
        else if (real.TryGetValue(keyID, out item))
        {
            real.Remove(keyID);
            PoolMgr.Instance.PushObj(item);
        }
    }

    public void ResetTimer(int keyID)
    {
        TryReset(timerDic, keyID);
        TryReset(realTimerDic, keyID);
        TryReset(timerWithParamDic, keyID);
        TryReset(realTimerWithParamDic, keyID);
        TryReset(timerWithParam2Dic, keyID);
        TryReset(realTimerWithParam2Dic, keyID);
        TryReset(timerWithParam3Dic, keyID);
        TryReset(realTimerWithParam3Dic, keyID);
    }

    private void TryReset<T>(Dictionary<int, T> dic, int keyID) where T : class
    {
        if (dic.TryGetValue(keyID, out T item))
        {
            var method = typeof(T).GetMethod("ResetTimer");
            method?.Invoke(item, null);
        }
    }

    public void StartTimer(int keyID)
    {
        SetRunning(keyID, true);
    }

    public void StopTimer(int keyID)
    {
        SetRunning(keyID, false);
    }

    private void SetRunning(int keyID, bool running)
    {
        SetRunning(timerDic, keyID, running);
        SetRunning(realTimerDic, keyID, running);
        SetRunning(timerWithParamDic, keyID, running);
        SetRunning(realTimerWithParamDic, keyID, running);
        SetRunning(timerWithParam2Dic, keyID, running);
        SetRunning(realTimerWithParam2Dic, keyID, running);
        SetRunning(timerWithParam3Dic, keyID, running);
        SetRunning(realTimerWithParam3Dic, keyID, running);
    }

    private void SetRunning<T>(Dictionary<int, T> dic, int keyID, bool running) where T : class
    {
        if (dic.TryGetValue(keyID, out T item))
        {
            var field = typeof(T).GetField("isRuning");
            field?.SetValue(item, running);
        }
    }

    #endregion
}
