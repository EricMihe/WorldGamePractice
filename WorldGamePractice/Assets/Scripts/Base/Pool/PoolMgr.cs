using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 锟斤拷锟诫（锟斤拷锟斤拷锟叫碉拷锟斤拷锟捷ｏ拷锟斤拷锟斤拷
/// </summary>
public class PoolData
{
    //锟斤拷锟斤拷锟芥储锟斤拷锟斤拷锟叫的讹拷锟斤拷 锟斤拷录锟斤拷锟斤拷没锟斤拷使锟矫的讹拷锟斤拷
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //锟斤拷锟斤拷锟斤拷录使锟斤拷锟叫的讹拷锟斤拷锟?
    private List<GameObject> usedList = new List<GameObject>();

    //锟斤拷锟斤拷锟斤拷锟斤拷 锟斤拷锟斤拷锟斤拷同时锟斤拷锟节的讹拷锟斤拷锟斤拷锟斤拷薷锟斤拷锟?
    private int maxNum;

    //锟斤拷锟斤拷锟斤拷锟斤拷锟?锟斤拷锟斤拷锟斤拷锟叫诧拷锟街癸拷锟斤拷锟侥讹拷锟斤拷
    private GameObject rootObj;

    //锟斤拷取锟斤拷锟斤拷锟斤拷锟角凤拷锟叫讹拷锟斤拷
    public int Count => dataStack.Count;

    public int UsedCount => usedList.Count;

    /// <summary>
    /// 锟斤拷锟斤拷使锟斤拷锟叫讹拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷斜冉锟?小锟节凤拷锟斤拷true 锟斤拷要实锟斤拷锟斤拷
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;

    /// <summary>
    /// 锟斤拷始锟斤拷锟斤拷锟届函锟斤拷
    /// </summary>
    /// <param name="root">锟斤拷锟接ｏ拷锟斤拷锟斤拷兀锟斤拷锟斤拷锟斤拷锟?/param>
    /// <param name="name">锟斤拷锟诫父锟斤拷锟斤拷锟斤拷锟斤拷锟?/param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //锟斤拷锟斤拷锟斤拷锟斤拷时 锟脚会动态锟斤拷锟斤拷 锟斤拷锟斤拷锟斤拷锟接癸拷系
        if(PoolMgr.isOpenLayout)
        {
            //锟斤拷锟斤拷锟斤拷锟诫父锟斤拷锟斤拷
            rootObj = new GameObject(name);
            //锟酵癸拷锟接革拷锟斤拷锟斤拷锟斤拷锟斤拷锟接癸拷系
            rootObj.transform.SetParent(root.transform);
        }

        //锟斤拷锟斤拷锟斤拷锟斤拷时 锟解部锟较讹拷锟角会动态锟斤拷锟斤拷一锟斤拷锟斤拷锟斤拷锟?
        PushUsedList(usedObj);

        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("锟斤拷为使锟矫伙拷锟斤拷毓锟斤拷艿锟皆わ拷锟斤拷锟斤拷锟斤拷锟斤拷锟絇oolObj锟脚憋拷 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷");
            return;
        }
        //锟斤拷录锟斤拷锟斤拷锟斤拷锟斤拷值
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// 锟接筹拷锟斤拷锟叫碉拷锟斤拷锟斤拷锟捷讹拷锟斤拷
    /// </summary>
    /// <returns>锟斤拷要锟侥讹拷锟斤拷锟斤拷锟斤拷</returns>
    public GameObject Pop()
    {
        //取锟斤拷锟斤拷锟斤拷
        GameObject obj;

        if (Count > 0)
        {
            //锟斤拷没锟叫碉拷锟斤拷锟斤拷锟斤拷锟斤拷取锟斤拷使锟斤拷
            obj = dataStack.Pop();
            //锟斤拷锟斤拷要使锟斤拷锟斤拷 应锟斤拷要锟斤拷使锟斤拷锟叫碉拷锟斤拷锟斤拷锟斤拷录锟斤拷
            usedList.Add(obj);
        }
        else
        {
            //取0锟斤拷锟斤拷锟侥讹拷锟斤拷 锟斤拷锟斤拷锟侥撅拷锟斤拷使锟斤拷时锟斤拷锟筋长锟侥讹拷锟斤拷
            obj = usedList[0];
            //锟斤拷锟揭帮拷锟斤拷锟斤拷使锟斤拷锟脚的讹拷锟斤拷锟斤拷锟狡筹拷
            usedList.RemoveAt(0);
            //锟斤拷锟斤拷锟斤拷锟接碉拷尾锟斤拷 锟斤拷示 锟饺斤拷锟铰的匡拷始
            usedList.Add(obj);
        }

        //锟斤拷锟斤拷锟斤拷锟?
        obj.SetActive(true);
        //锟较匡拷锟斤拷锟接癸拷系
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟斤拷氲斤拷锟斤拷锟斤拷锟斤拷锟斤拷
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        //失锟斤拷锟斤拷锟斤拷锟斤拷亩锟斤拷锟?
        obj.SetActive(false);
        //锟斤拷锟斤拷锟接︼拷锟斤拷锟侥革拷锟斤拷锟斤拷锟斤拷 锟斤拷锟斤拷锟斤拷锟接癸拷系
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        //通锟斤拷栈锟斤拷录锟斤拷应锟侥讹拷锟斤拷锟斤拷锟斤拷
        dataStack.Push(obj);
        //锟斤拷锟斤拷锟斤拷锟斤拷丫锟斤拷锟斤拷锟绞癸拷锟斤拷锟?应锟矫帮拷锟斤拷锟接硷拷录锟斤拷锟斤拷锟斤拷锟狡筹拷
        usedList.Remove(obj);
    }


    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷压锟诫到使锟斤拷锟叫碉拷锟斤拷锟斤拷锟叫硷拷录
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// 锟斤拷锟斤拷锟斤拷锟街典当锟斤拷锟斤拷锟斤拷式锟芥换原锟斤拷 锟芥储锟斤拷锟斤拷锟斤拷锟?
/// </summary>
public abstract class PoolObjectBase { }

/// <summary>
/// 锟斤拷锟节存储 锟斤拷锟捷结构锟斤拷 锟斤拷 锟竭硷拷锟斤拷 锟斤拷锟斤拷锟教筹拷mono锟侥ｏ拷锟斤拷锟斤拷锟斤拷
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolObject<T> : PoolObjectBase where T:class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// 锟斤拷要锟斤拷锟斤拷锟矫碉拷 锟斤拷锟捷结构锟洁、锟竭硷拷锟斤拷 锟斤拷锟斤拷锟斤拷要锟教承该接匡拷
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟捷的凤拷锟斤拷
    /// </summary>
    void ResetInfo();
}

/// <summary>
/// 锟斤拷锟斤拷锟?锟斤拷锟斤拷锟?模锟斤拷 锟斤拷锟斤拷锟斤拷
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟叫筹拷锟斤拷锟斤拷锟斤拷锟?
    //值 锟斤拷实锟斤拷锟斤拷锟侥撅拷锟斤拷一锟斤拷 锟斤拷锟斤拷锟斤拷锟?
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    /// <summary>
    /// 锟斤拷锟节存储锟斤拷锟捷结构锟洁、锟竭硷拷锟斤拷锟斤拷锟斤拷 锟斤拷锟接碉拷锟街碉拷锟斤拷锟斤拷
    /// </summary>
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>();

    //锟斤拷锟接革拷锟斤拷锟斤拷
    private GameObject poolObj;

    //锟角凤拷锟斤拷锟斤拷锟街癸拷锟斤拷
    public static bool isOpenLayout = true;

    private PoolMgr() {

        //锟斤拷锟斤拷锟斤拷锟斤拷锟轿拷锟?锟酵达拷锟斤拷
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

    }

    /// <summary>
    /// 锟矫讹拷锟斤拷锟侥凤拷锟斤拷
    /// </summary>
    /// <param name="name">锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷</param>
    /// <returns>锟接伙拷锟斤拷锟斤拷锟饺★拷锟斤拷亩锟斤拷锟?/returns>
    public GameObject GetObj(string name)
    {
        //锟斤拷锟斤拷锟斤拷锟斤拷锟轿拷锟?锟酵达拷锟斤拷
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool");

        GameObject obj;

        #region 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟睫猴拷锟斤拷呒锟斤拷卸锟?
        if(!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            //锟斤拷态锟斤拷锟斤拷锟斤拷锟斤拷
            //没锟叫碉拷时锟斤拷 通锟斤拷锟斤拷源锟斤拷锟斤拷 去实锟斤拷锟斤拷锟斤拷一锟斤拷GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //锟斤拷锟斤拷实锟斤拷锟斤拷锟斤拷锟斤拷锟侥讹拷锟斤拷 默锟较伙拷锟斤拷锟斤拷锟街猴拷锟斤拷锟揭伙拷锟?Clone)
            //锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟?
            obj.name = name;

            //锟斤拷锟斤拷锟斤拷锟斤拷
            if(!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else//实锟斤拷锟斤拷锟斤拷锟斤拷锟侥讹拷锟斤拷 锟斤拷要锟斤拷录锟斤拷使锟斤拷锟叫的讹拷锟斤拷锟斤拷锟斤拷锟斤拷
                poolDic[name].PushUsedList(obj);
        }
        //锟斤拷锟斤拷锟斤拷锟斤拷锟叫讹拷锟斤拷 锟斤拷锟斤拷 使锟斤拷锟叫的讹拷锟斤拷锟斤拷锟斤拷锟斤拷 直锟斤拷去取锟斤拷锟斤拷锟斤拷
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion
        return obj;

    }

    /// <summary>
    /// 锟斤拷取锟皆讹拷锟斤拷锟斤拷锟斤拷萁峁癸拷锟斤拷锟竭硷拷锟斤拷锟斤拷锟?锟斤拷锟斤拷锟教筹拷Mono锟侥ｏ拷
    /// </summary>
    /// <typeparam name="T">锟斤拷锟斤拷锟斤拷锟斤拷</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T: class, IPoolObject,new()
    {
        //锟斤拷锟接碉拷锟斤拷锟斤拷 锟角革拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟?锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
        string poolName = nameSpace + "_" + typeof(T).Name;
        //锟叫筹拷锟斤拷
        if(poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            //锟斤拷锟接碉拷锟斤拷锟角凤拷锟叫匡拷锟皆革拷锟矫碉拷锟斤拷锟斤拷
            if(pool.poolObjs.Count > 0)
            {
                //锟接讹拷锟斤拷锟斤拷取锟斤拷锟斤拷锟斤拷 锟斤拷锟叫革拷锟斤拷
                T obj = pool.poolObjs.Dequeue() as T;
                return obj;
            }
            //锟斤拷锟接碉拷锟斤拷锟角空碉拷
            else
            {
                //锟斤拷锟诫保证锟斤拷锟斤拷锟睫参癸拷锟届函锟斤拷
                T obj = new T();
                return obj;
            }
        }
        else//没锟叫筹拷锟斤拷
        {
            T obj = new T();
            return obj;
        }
        
    }

    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟斤拷蟹锟斤拷锟斤拷锟斤拷
    /// </summary>
    /// <param name="name">锟斤拷锟诫（锟斤拷锟襟）碉拷锟斤拷锟斤拷</param>
    /// <param name="obj">希锟斤拷锟斤拷锟斤拷亩锟斤拷锟?/param>
    public void PushObj(GameObject obj)
    {     
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 锟斤拷锟皆讹拷锟斤拷锟斤拷锟捷结构锟斤拷锟斤拷呒锟斤拷锟?锟斤拷锟斤拷锟斤拷锟斤拷锟?
    /// </summary>
    /// <typeparam name="T">锟斤拷应锟斤拷锟斤拷</typeparam>
    public void PushObj<T>(T obj, string nameSpace = "") where T:class,IPoolObject
    {
        //锟斤拷锟斤拷锟揭癸拷锟絥ull锟斤拷锟斤拷 锟角诧拷锟斤拷锟斤拷锟斤拷锟斤拷
        if (obj == null)
            return;        
        //锟斤拷锟接碉拷锟斤拷锟斤拷 锟角革拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟?锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
        string poolName = nameSpace + "_" + typeof(T).Name;
        var ids = typeof(T).GetGenericArguments();
        if(ids.Length > 0)
        {
            poolName += "<";
            foreach (var id in ids)
            {
                poolName += id.Name + ",";
            }
            poolName += ">";
        }
        
        //锟叫筹拷锟斤拷
        PoolObject<T> pool;
        if (poolObjectDic.ContainsKey(poolName))
            //取锟斤拷锟斤拷锟斤拷 压锟斤拷锟斤拷锟?
            pool = poolObjectDic[poolName] as PoolObject<T>;
        else//没锟叫筹拷锟斤拷
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        //锟节凤拷锟斤拷锟斤拷锟斤拷锟街?锟斤拷锟斤拷锟矫讹拷锟斤拷锟斤拷锟斤拷锟?
        obj.ResetInfo();
        if (obj != null)
        {
            pool.poolObjs.Enqueue(obj);
        }   
    }

    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷拥锟斤拷械锟斤拷锟斤拷锟?
    /// 使锟矫筹拷锟斤拷 锟斤拷要锟斤拷 锟叫筹拷锟斤拷时
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
}
