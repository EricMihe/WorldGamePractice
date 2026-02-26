
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static RelateCenter;
using static UnityEditor.Progress;

public class RelateCenter : BaseManager<RelateCenter>
{
    public const int ProceedLevelBase = 100;
    private RelateCenter() { }

    public Dictionary<IRelate, int> relatedObjects = new Dictionary<IRelate, int>();

    private int relatedObjectID = 0;

    private DynamicUndirectedGraph dynamicUndirectedGraph = new DynamicUndirectedGraph();

    private Dictionary<int, Dictionary<ICompare, EventInfoBase>> relateObjectProceedAction=new Dictionary<int, Dictionary<ICompare, EventInfoBase>>();

    private Dictionary<int, Dictionary<RelateEntity, EventInfoBase>> relateObjectRelateAction = new Dictionary<int, Dictionary<RelateEntity, EventInfoBase>>();


    private List<RelateEntity> removeRelate = new List<RelateEntity>();

    private List <ICompare> removeProceed= new List<ICompare>();

    private MaxPriorityQueue<EventInfoBase> eventQueue = new MaxPriorityQueue<EventInfoBase>((a, b) => a.executionLevel.CompareTo(b.executionLevel));


    public int AddRelatedObject(IRelate relatedObject)
    {
        if (!relatedObjects.ContainsKey(relatedObject))
        {
            relatedObjects[relatedObject] = relatedObjectID;
            return relatedObjectID++;
        }
        return -1;
    }

    public void AddProceed<T>(T obj, ICompare v, UnityAction<T> action, int level = -1) where T : class, IRelate 
    {
        var currentlevel = level + ProceedLevelBase;

        if (!relateObjectProceedAction.ContainsKey (obj.Id)|| (relateObjectProceedAction.ContainsKey(obj.Id) && relateObjectProceedAction[obj.Id] == null))
        {
            relateObjectProceedAction[obj.Id]=new Dictionary<ICompare, EventInfoBase> ();
            CreateEvent(obj, v, action, currentlevel);
        }
        else if(relateObjectProceedAction.ContainsKey(obj.Id) && relateObjectProceedAction[obj.Id] != null && relateObjectProceedAction[obj.Id].Count < 1)
        {
            CreateEvent(obj, v, action, currentlevel);
        }
        else
        {
            foreach (var item in relateObjectProceedAction[obj.Id].Keys.ToList())
            {
                if(item == null)
                {
                    removeProceed.Add(item);
                    continue;
                }
                else
                {
                    if (item.Compare(v))
                    {
                        if (relateObjectProceedAction[obj.Id][item] != null && relateObjectProceedAction[obj.Id][item] is ProceedEventInfo<T> a)
                        {
                            if (a.executionLevel == currentlevel) a.actions += action;
                            else CreateEvent(obj, v, action, currentlevel);
                            break;
                        }
                        else relateObjectProceedAction[obj.Id][item] = new ProceedEventInfo<T>(action, currentlevel);
                        break;
                    }
                    else CreateEvent(obj, v, action, currentlevel);
                    break;
                }
            }

            if (removeProceed.Count > 0)
            {
                foreach (var item in removeProceed)
                {
                    relateObjectProceedAction[obj.Id].Remove(item);
                }
                removeProceed.Clear();
            }
        }
    }

    public bool RemoveAllProceed(IRelate relatedObject)
    {
        if (relatedObjects.ContainsKey(relatedObject) && relateObjectProceedAction.ContainsKey(relatedObject.Id))
        {
            relateObjectProceedAction.Remove(relatedObject.Id);
            return true;
        }
        return false;
    }

    public bool RemoveProceed(IRelate relatedObject,ICompare v)
    {
        if (relatedObjects.ContainsKey(relatedObject) 
            && relateObjectProceedAction.ContainsKey(relatedObject.Id) 
            && relateObjectProceedAction[relatedObject.Id].Count > 0)
        {

            foreach (var item in relateObjectProceedAction[relatedObject.Id].Keys.ToList())
            {
                if (item == null)
                {
                    removeProceed.Add(item);
                    continue;
                }
                else
                {
                        if (item.Compare(v)) removeProceed.Add(item);
                }
            }

            if (removeProceed.Count > 0)
            {
                foreach (var item in removeProceed)
                {
                    relateObjectProceedAction[relatedObject.Id].Remove(item);
                }
                removeProceed.Clear();
                return true;
            }
        }
        return false;
    }

    public int AddRelated<T1, T2>(T1 obj1, T2 obj2, ICompare v1, ICompare v2, UnityAction<T1, T2> action, int level = -1) 
        where T1 : class, IRelate where T2 : class, IRelate
    {
        int edge = dynamicUndirectedGraph.AddEdge(relatedObjects[obj1], relatedObjects[obj2]);
        if (!relateObjectRelateAction.ContainsKey(edge) || (relateObjectRelateAction.ContainsKey(edge) && relateObjectRelateAction[edge] == null))
        {
            relateObjectRelateAction[edge] = new Dictionary<RelateEntity, EventInfoBase>();
            CreateEvent(obj1, obj2, v1, v2, action, edge, level);
        }
        else if (relateObjectRelateAction.ContainsKey(edge) && relateObjectRelateAction[edge] != null && relateObjectRelateAction[edge].Count < 1)
        {
            CreateEvent(obj1, obj2, v1, v2, action, edge, level);
        }
        else
        {
            foreach (var item in relateObjectRelateAction[edge].Keys.ToList())
            {
                if (item != null)
                {
                    if (item.relates[0] == obj1 && item.relates[1] == obj2)
                    {
                        if (item.compares[0].Compare(v1) && item.compares[1].Compare(v2))
                        {
                            if (relateObjectRelateAction[edge][item] != null && relateObjectRelateAction[edge][item] is RelateEventInfo<T1, T2> a)
                            {
                                if (a.executionLevel == level) a.actions += action;
                                else CreateEvent(obj1, obj2, v1, v2, action, edge, level);
                                break;
                            }
                            else relateObjectRelateAction[edge][item] = new RelateEventInfo<T1, T2>(action, level);
                            break;
                        }
                        else CreateEvent(obj1, obj2, v1, v2, action, edge, level);
                        break;

                    }
                    else if (item.relates[0] == obj2 && item.relates[1] == obj1)
                    {
                        if (item.compares[0].Compare(v2) && item.compares[1].Compare(v1))
                        {
                            if (relateObjectRelateAction[edge][item] != null && relateObjectRelateAction[edge][item] is RelateEventInfo<T2, T1> a)
                            {
                                if (a.executionLevel == level) a.actionsR += action;
                                else CreateEvent(obj1, obj2, v1, v2, action, edge, level);
                                break;
                            }
                            else relateObjectRelateAction[edge][item] = new RelateEventInfo<T2, T1>(action, level);
                            break;
                        }
                        else CreateEvent(obj1, obj2, v1, v2, action, edge, level);
                        break;
                    }
                    else CreateEvent(obj1, obj2, v1, v2, action, edge, level);
                    break;
                }
                else
                {
                    removeRelate.Add(item);
                }
            }


            if (removeRelate.Count > 0)
            {
                foreach (var item in removeRelate)
                {
                    relateObjectRelateAction[edge].Remove(item);
                }
                removeRelate.Clear();
            }

        }
        return edge;
    }

    public bool RemoveAllRelated(IRelate relatedObject)
    {
        if (relatedObjects.ContainsKey(relatedObject))
        {
            var allEdge = dynamicUndirectedGraph.GetIncidentEdgeIds(relatedObjects[relatedObject]);
            if (allEdge != null && allEdge.Count > 0)
            {
                foreach (var edge in allEdge)
                {
                    if (relateObjectRelateAction.ContainsKey(edge))
                    {

                        relateObjectRelateAction.Remove(edge);
                    }
                }
            }

            return dynamicUndirectedGraph.RemoveVertex(relatedObjects[relatedObject]);
        }
        return false;
    }

    public bool RemoveRelated(IRelate relatedObject1, IRelate relatedObject2)
    {
        if (relatedObjects.ContainsKey(relatedObject1) && relatedObjects.ContainsKey(relatedObject2))
        {
            var edge = dynamicUndirectedGraph.GetEdge(relatedObjects[relatedObject1], relatedObjects[relatedObject2]);
            if (edge != -1)
            {
                if (relateObjectRelateAction.ContainsKey(edge))
                {
                    relateObjectRelateAction.Remove(edge);
                }
            }

            return dynamicUndirectedGraph.RemoveEdge(relatedObjects[relatedObject1], relatedObjects[relatedObject2]);
        }
        return false;
    }

    public bool RemoveRelatedWithValue(IRelate obj1, IRelate obj2, ICompare v1, ICompare v2)
    {
        if (relatedObjects.ContainsKey(obj1) && relatedObjects.ContainsKey(obj2))
        {
            var edge = dynamicUndirectedGraph.GetEdge(relatedObjects[obj1], relatedObjects[obj2]);
            if (edge != -1)
            {
                if (relateObjectRelateAction.ContainsKey(edge) && relateObjectRelateAction[edge] != null && relateObjectRelateAction[edge].Count > 0)
                {

                    foreach (var item in relateObjectRelateAction[edge].Keys.ToList())
                    {
                        if (item == null
                            || item.relates[0] == null || item.relates[1] == null
                            || item.compares[0] == null || item.compares[1] == null)
                        {
                            removeRelate.Add(item);
                            continue;
                        }
                        else
                        {
                            if ((item.relates[0] == obj1 && item.relates[1] == obj2) || (item.relates[0] == obj2 && item.relates[1] == obj1))
                            {
                                    if (item.compares[0].Compare(v1) && item.compares[1].Compare(v2)) removeRelate.Add(item);
                            }

                        }
                    }

                    if (removeRelate.Count > 0)
                    {
                        foreach (var item in removeRelate)
                        {
                            relateObjectRelateAction[edge].Remove(item);
                        }
                        removeRelate.Clear();
                    }
                }
                else return false;
            }
            return dynamicUndirectedGraph.RemoveEdge(relatedObjects[obj1], relatedObjects[obj2]);
        }
        return false;
    }

    public bool Trigger(IRelate relatedObject)
    {
        if (relatedObjects.ContainsKey(relatedObject))
        {
            if(relateObjectProceedAction.ContainsKey (relatedObject.Id) 
                && relateObjectProceedAction[relatedObject.Id].Count > 0)
            {
                foreach (var item in relateObjectProceedAction[relatedObject.Id])
                {
                    if (item.Key == null)
                    {
                        removeProceed.Add(item.Key);
                        continue;
                    }
                    if (item.Key.Compare(relatedObject.GetData()))
                    {
                        if (item.Value != null) item.Value.AddIRelateObject(relatedObject);
                        eventQueue.Enqueue(item.Value);
                    }
                }

                if (removeProceed.Count > 0)
                {
                    foreach (var item in removeProceed)
                    {
                        relateObjectProceedAction[relatedObject.Id].Remove(item);
                    }
                    removeProceed.Clear();
                    return true;
                }
            }

            var allEdge = dynamicUndirectedGraph.GetIncidentEdgeIds(relatedObjects[relatedObject]);
            if (allEdge != null && allEdge.Count > 0)
            {
                foreach (var edge in allEdge)
                {
                    if (relateObjectRelateAction.ContainsKey(edge) && relateObjectRelateAction[edge].Count > 0)
                    {
                        foreach (var item in relateObjectRelateAction[edge])
                        {
                            if (item.Key == null
                           || item.Key.relates[0] == null || item.Key.relates[1] == null
                           || item.Key.compares[0] == null || item.Key.compares[1] == null)
                            {
                                removeRelate.Add(item.Key);
                                continue;
                            }
                            if (item.Key.compares[0].Compare(item.Key.relates[0].GetData())
                                && item.Key.compares[1].Compare(item.Key.relates[1].GetData()))
                            {
                                if (item.Value != null) item.Value.AddIRelateObject(item.Key.relates[0], item.Key.relates[1]);
                                eventQueue.Enqueue(item.Value);
                            }
                        }

                        if (removeRelate.Count > 0)
                        {
                            foreach (var item in removeRelate)
                            {
                                relateObjectRelateAction[edge].Remove(item);
                            }
                            removeRelate.Clear();
                        }
                    }
                }
            }
            if (!eventQueue.IsEmpty)
            {
                while (eventQueue.Count > 0)
                {
                    eventQueue.Dequeue().Do();
                    
                }
            }
            else return false;
            return true;
        }
        return false;
    }

    private void CreateEvent<T1, T2>(T1 obj1, T2 obj2, ICompare v1, ICompare v2, UnityAction<T1, T2> action, int edge, int level)
        where T1 : class, IRelate where T2 : class, IRelate
    {
        RelateEventInfo<T1, T2> e = new RelateEventInfo<T1, T2>(action, level);
        relateObjectRelateAction[edge].Add(new RelateEntity(obj1, obj2, v1, v2), e);
    }

    private void CreateEvent<T>(T obj, ICompare v, UnityAction<T> action, int level)
                where T : class, IRelate
    {
        ProceedEventInfo<T> e = new ProceedEventInfo<T>(action, level);
        relateObjectProceedAction[obj.Id].Add(v, e);
    }

    public class RelateEntity
    {
        public IRelate[] relates = new IRelate[2];
        public ICompare[] compares = new ICompare[2];
        public RelateEntity(IRelate relate1, IRelate relate2, ICompare compare1, ICompare compare2)
        {
            this.relates[0] = relate1;
            this.relates[1] = relate2;
            this.compares[0] = compare1;
            this.compares[1] = compare2;
        }
    }

    public class ProceedEventInfo<T> : EventInfoBase where T : class, IRelate
    {
        private T _obj;
        public UnityAction<T> actions;

        public ProceedEventInfo(UnityAction<T> action, int level = -1)
        {
            actions += action;
            executionLevel = level;
        }

        public override void AddIRelateObject(IRelate obj)
        {
            _obj = (T)obj;
        }

        public override void Do()
        {

            if (_obj != null)
            {
                if (actions != null) actions?.Invoke(_obj);
                _obj = null;
            }
            else
            {
                UnityEngine.Debug.Log("没有添加参数");
            }
        }
    }

    public class RelateEventInfo<T1, T2> : EventInfoBase where T1 : class, IRelate where T2 : class, IRelate
    {
        private T1 _obj1;
        private T2 _obj2;

        public UnityAction<T1, T2> actions;
        public UnityAction<T2, T1> actionsR;
        public RelateEventInfo(UnityAction<T1, T2> action, int level = -1)
        {
            actions += action;
            executionLevel = level;
        }

        public RelateEventInfo(UnityAction<T2, T1> action, int level = -1)
        {
            actionsR += action;
            executionLevel = level;
        }

        public override void AddIRelateObject(IRelate obj1, IRelate obj2)
        {
            _obj1 = (T1)obj1;
            _obj2 = (T2)obj2;
        }

        public override void Do()
        {
            if (_obj1 != null && _obj2 != null)
            {
                if (actions != null) actions?.Invoke(_obj1, _obj2);
                if (actionsR != null) actionsR?.Invoke(_obj2, _obj1);
                _obj1 = null;
                _obj2 = null;
            }
            else
            {
                UnityEngine.Debug.Log("没有添加参数");
            }
        }
    }
}

public interface IRelate
{
    public int Id { get; }
    bool IsHasWait { get; }
    void Refresh();
    void RefreshLate();
    ICompare GetData();
}

public class RelatedObject<T> : IRelate where T : IRelatedData<T>, new()
{
    private int id=-1;

    private T currentData;

    private bool isHasWait = false;
    public int Id => id;

    ICompare IRelate.GetData()
    {
        return currentData;
    }

    /// <summary>
    /// ！！！（设置关联对象值）设置完所有字段后需仅一次更新（Refresh()或者RefreshLate()）！！！
    /// </summary>
    public T newData;

    public bool IsHasWait
    {
        get => isHasWait;
    }

    /// <summary>
    /// 在newData重新设置后调用刷新数据触发关联
    /// </summary>
    public void Refresh()
    {
        if (SetData(newData)) return;
        else Debug.Log("无变化");
    }
    /// <summary>
    /// 在监听的关联函数中newData重新设置后调用刷新数据触发关联
    /// </summary>
    public void RefreshLate()
    {
        isHasWait = true;
        if (MonoMgr.Instance.SetRelateValueLate(this)) return;
        else Debug.Log("没注册");
    }
    /// <summary>
    /// 传入新的数据设置触发关联
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetData(T value)
    {
        if(value .Compare(currentData))
        {
            return false;
        }
        else
        {
            isHasWait = false;

            var temp = currentData;
            currentData = value;
            newData = temp;
            newData.CopyData(currentData);

            return RelateCenter.Instance.Trigger(this);
        }
    }

    /// <summary>
    /// 获取当前的数据
    /// </summary>
    /// <returns></returns>
    public T GetData()
    {
        return currentData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">初始数据</param>
    public RelatedObject(T value)
    {
        newData = value;
        currentData = new T();
        currentData.CopyData(newData);
        var _id = RelateCenter.Instance.AddRelatedObject(this);
        if (_id != -1) id = _id;
        else UnityEngine.Debug.Log("该对象以注册");
       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">初始数据</param>
    /// <param name="relateValue">关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public RelatedObject(T value, ICompare relateValue, UnityAction<RelatedObject<T>> action, int level = -1)
    {
        newData = value;
        currentData = new T();
        currentData.CopyData(newData);
        var _id = RelateCenter.Instance.AddRelatedObject(this);
        if (_id != -1)
        {
            id = _id;
            RelateCenter.Instance.AddProceed(this, relateValue, action, level);
            RelateCenter.Instance.Trigger(this);
        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">初始数据</param>
    /// <param name="relateValue1">关联数据</param>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public RelatedObject(T value, ICompare relateValue1, IRelate obj, ICompare relateValue2, 
        UnityAction<IRelate, IRelate> action, int level=-1)
    {
        newData = value;
        currentData = new T();
        currentData.CopyData(newData);
        var _id = RelateCenter.Instance.AddRelatedObject(this);

        if (_id != -1)
        {
            id = _id;
            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action,level) != -1)
                RelateCenter.Instance.Trigger(this);
            else UnityEngine.Debug.Log("添加关联失败");
        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">初始数据</param>
    /// <param name="relateValue1">关联数据</param>
    /// <param name="obj">关联（触发型）对象</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public RelatedObject(T value, ICompare relateValue1, RelateTrigger obj,
       UnityAction<RelatedObject<T>, RelateTrigger> action, int level = -1)
    {
        newData = value;
        currentData = new T();
        currentData.CopyData(newData);
        var _id = RelateCenter.Instance.AddRelatedObject(this);

        if (_id != -1)
        {
            id = _id;
            ICompare relateValue2 = new RelateData_Bool() { yesorno = false };

            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
                RelateCenter.Instance.Trigger(this);
            else UnityEngine.Debug.Log("添加关联失败");
        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
    }
    /// <summary>
    /// 添加递进事件
    /// </summary>
    /// <param name="relateValue">关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public void AddProceed(ICompare relateValue, UnityAction<RelatedObject<T>> action, int level = -1)
    {
        if (this.id != -1)
        {
            RelateCenter.Instance.AddProceed(this, relateValue, action, level);
            RelateCenter.Instance.Trigger(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
        }
    }
    /// <summary>
    /// 移除递进事件
    /// </summary>
    /// <param name="relateValue">关联数据</param>
    public void RemoveProceed(ICompare relateValue)
    {
        if (this.id != -1 )
        {
            RelateCenter.Instance.RemoveProceed(this, relateValue);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
        }
    }
    /// <summary>
    /// 移除所有递进事件
    /// </summary>
    public void RemoveAllProceed()
    {
        if (this.id != -1)
        {
            RelateCenter.Instance.RemoveAllProceed(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
        }
    }
    /// <summary>
    /// 添加关联事件
    /// </summary>
    /// <typeparam name="T2">关联对象类型</typeparam>
    /// <param name="relateValue1">关联数据</param>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    /// <returns></returns>
    public bool AddRelate<T2>(ICompare relateValue1, T2 obj, ICompare relateValue2, 
        UnityAction<RelatedObject<T>, T2> action, int level = -1) where T2 :class, IRelate
    {

        if (this .id != -1 && obj.Id != -1)
        {
            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
                RelateCenter.Instance.Trigger(this);
            else
            {
                UnityEngine.Debug.Log("添加关联失败");
                return false;
            }
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
        return true;
    }
    /// <summary>
    /// 添加关联事件
    /// </summary>
    /// <param name="relateValue1">关联数据</param>
    /// <param name="obj">关联（触发型）对象</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    /// <returns></returns>
    public bool AddRelate(ICompare relateValue1, RelateTrigger obj, UnityAction<RelatedObject<T>, RelateTrigger> action, int level = -1)
    {

        if (this.id != -1 && obj.Id != -1)
        {
            ICompare relateValue2 = new RelateData_Bool() { yesorno = false };

            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
                RelateCenter.Instance.Trigger(this);
            else
            {
                UnityEngine.Debug.Log("添加关联失败");
                return false;
            }
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 移除关联对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool RemoveRelate(IRelate obj)
    {
        if (this.id != -1&& obj.Id !=-1)
        {
            return RelateCenter.Instance.RemoveRelated(this,obj);           
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
    }
    /// <summary>
    /// 根据关联数据移除关联对象
    /// </summary>
    /// <param name="relateValue1">关联数据</param>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <returns></returns>
    public bool RemoveRelateWithData(ICompare relateValue1, IRelate obj, ICompare relateValue2)
    {
        if (this.id != -1 && obj.Id != -1)
        {
            return RelateCenter.Instance.RemoveRelatedWithValue(this, obj, relateValue1,relateValue2);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        } 
    }

    //public bool RemoveRelateWithValue(ICompare relateValue1,RelateTrigger obj )
    //{
    //    if (this.id != -1 && obj.Id != -1)
    //    {
    //        ICompare relateValue2 = new RelateValue_Bool() { isopen = false };
    //        return RelateCenter.Instance.RemoveRelatedWithValue(this, obj, relateValue1, relateValue2);
    //    }
    //    else
    //    {
    //        UnityEngine.Debug.Log("对象未注册");
    //        return false;
    //    }
    //}

    /// <summary>
    /// 移除所有关联对象
    /// </summary>
    /// <returns></returns>
    public bool RemoveAllRelated()
    {
        if (this.id != -1)
        {
            return RelateCenter.Instance.RemoveAllRelated(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
    }
}

public class RelateTrigger : IRelate
{

    public bool IsHasWait => isHasWait;

    private int id = -1;

    private RelateData_Bool currentData;

    private bool isHasWait = false;
    public int Id => id;

    ICompare IRelate.GetData()
    {
        return currentData;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public void Refresh()
    {
        isHasWait = false;
        currentData.yesorno = true;
        if (RelateCenter.Instance.Trigger(this))
        {
            currentData.yesorno = false;
            return;
        }
        else
        {
            currentData.yesorno = false;
            Debug.Log("无变化");
        }
    }
    /// <summary>
    /// 在关联事件中的触发事件
    /// </summary>
    public void RefreshLate()
    {
        isHasWait = true;
        if (MonoMgr.Instance.SetRelateValueLate(this)) return;
        else Debug.Log("没注册");
    }

    public RelateTrigger()
    {
        currentData = new RelateData_Bool();
        currentData.yesorno = false;
        var _id = RelateCenter.Instance.AddRelatedObject(this);
        if (_id != -1) id = _id;
        else UnityEngine.Debug.Log("该对象以注册");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">是否初始触发</param>
    /// <param name="action">触发事件</param>
    /// <param name="level">事件等级</param>
    public RelateTrigger(bool value, UnityAction<RelateTrigger> action, int level = -1)
    {
        currentData = new RelateData_Bool();
        currentData.yesorno = value;
        var _id = RelateCenter.Instance.AddRelatedObject(this);

        if (_id != -1)
        {
            id = _id;

            ICompare relateValue = new RelateData_Bool() { yesorno = true };

            RelateCenter.Instance.AddProceed(this,relateValue,action, level);
                RelateCenter.Instance.Trigger(this);
        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
        currentData.yesorno = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">是否初始触发</param>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public RelateTrigger(bool value, IRelate obj, ICompare relateValue2,
        UnityAction<IRelate, IRelate> action,int level = -1)
    {
        currentData = new RelateData_Bool();
        currentData.yesorno = value;
        var _id = RelateCenter.Instance.AddRelatedObject(this);

        if (_id != -1)
        {
            id = _id;

            ICompare relateValue1=new RelateData_Bool() { yesorno = true };
            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
                if (value) RelateCenter.Instance.Trigger(this);
            else UnityEngine.Debug.Log("添加关联失败");

        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
        currentData.yesorno = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">是否初始触发</param>
    /// <param name="obj">关联（触发型）对象</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public RelateTrigger(bool value, RelateTrigger obj,
        UnityAction<RelateTrigger, RelateTrigger> action, int level = -1)
    {
        currentData = new RelateData_Bool();
        currentData.yesorno = value;
        var _id = RelateCenter.Instance.AddRelatedObject(this);

        if (_id != -1)
        {
            id = _id;

            ICompare relateValue1 = new RelateData_Bool() { yesorno = true };
            ICompare relateValue2 = new RelateData_Bool() { yesorno = false };
            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
                if(value)RelateCenter.Instance.Trigger(this);
            else UnityEngine.Debug.Log("添加关联失败");
        }
        else
        {
            UnityEngine.Debug.Log("该对象以注册");
        }
        currentData.yesorno = false;
    }

    /// <summary>
    /// 添加递进事件
    /// </summary>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    public void AddProceed(UnityAction<RelateTrigger> action, int level = -1)
    {
        if (this.id != -1)
        {
            ICompare relateValue = new RelateData_Bool() { yesorno = true };
            RelateCenter.Instance.AddProceed(this, relateValue, action, level);
            RelateCenter.Instance.Trigger(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
        }
    }
    /// <summary>
    /// 移除所有递进事件
    /// </summary>
    public void RemoveProceed()
    {
        if (this.id != -1)
        {
            RelateCenter.Instance.RemoveAllProceed(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
        }
    }
    /// <summary>
    /// 添加关联事件
    /// </summary>
    /// <typeparam name="T2">关联对象类型</typeparam>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    /// <returns></returns>
    public bool AddRelate<T2>(T2 obj, ICompare relateValue2,
       UnityAction<RelateTrigger, T2> action, int level=-1) where T2 : class, IRelate
    {

        if (this.id != -1 && obj.Id != -1)
        {
            ICompare relateValue1 = new RelateData_Bool() { yesorno = true };

            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
            {
                RelateCenter.Instance.Trigger(this);
                currentData.yesorno = false;
            }
            else
            {
                UnityEngine.Debug.Log("添加关联失败");
                currentData.yesorno = false;
                return false;
            }
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            currentData.yesorno = false;
            return false;
        }
        currentData.yesorno = false;
        return true;
    }
    /// <summary>
    /// 添加关联事件
    /// </summary>
    /// <param name="obj">关联（触发型）对象</param>
    /// <param name="action">关联事件</param>
    /// <param name="level">事件等级</param>
    /// <returns></returns>
    public bool AddRelate(RelateTrigger obj,UnityAction<RelateTrigger, RelateTrigger> action, int level = -1)
    {

        if (this.id != -1 && obj.Id != -1)
        {
            ICompare relateValue1 = new RelateData_Bool() { yesorno = true };
            ICompare relateValue2 = new RelateData_Bool() { yesorno = false};

            if (RelateCenter.Instance.AddRelated(this, obj, relateValue1, relateValue2, action, level) != -1)
            {
                RelateCenter.Instance.Trigger(this);
                currentData.yesorno = false;
            }
            else
            {
                UnityEngine.Debug.Log("添加关联失败");
                currentData.yesorno = false;
                return false;
            }
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            currentData.yesorno = false;
            return false;
        }
        currentData.yesorno = false;
        return true;
    }
    /// <summary>
    /// 移除关联对象（以及对应的关联事件）
    /// </summary>
    /// <param name="obj">关联对象</param>
    /// <returns></returns>
    public bool RemoveRelate(IRelate obj)
    {
        if (this.id != -1 && obj.Id != -1)
        {
            return RelateCenter.Instance.RemoveRelated(this, obj);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
    }
    /// <summary>
    /// 根据数据移除关联对象（以及对应的关联事件）
    /// </summary>
    /// <param name="obj">关联对象</param>
    /// <param name="relateValue2">关联对象的关联数据</param>
    /// <returns></returns>
    public bool RemoveRelateWithData(IRelate obj,ICompare relateValue2)
    {
        if (this.id != -1 && obj.Id != -1)
        {
            ICompare relateValue1 = new RelateData_Bool() { yesorno = true };
            return RelateCenter.Instance.RemoveRelatedWithValue(this, obj, relateValue1, relateValue2);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
    }

    //public bool RemoveRelateWithValue(RelateTrigger obj)
    //{
    //    if (this.id != -1 && obj.Id != -1)
    //    {
    //        ICompare relateValue1 = new RelateValue_Bool() { isopen = true };
    //        ICompare relateValue2 = new RelateValue_Bool() { isopen = false };
    //        return RelateCenter.Instance.RemoveRelatedWithValue(this, obj, relateValue1, relateValue2);
    //    }
    //    else
    //    {
    //        UnityEngine.Debug.Log("对象未注册");
    //        return false;
    //    }
    //}

    /// <summary>
    /// 移除所有关联对象（以及对应的关联数据）
    /// </summary>
    /// <returns></returns>
    public bool RemoveAllRelated()
    {
        if (this.id != -1)
        {
            return RelateCenter.Instance.RemoveAllRelated(this);
        }
        else
        {
            UnityEngine.Debug.Log("对象未注册");
            return false;
        }
    }

}


public interface ICompare
{
    public bool Compare(ICompare t);

    public static ICompare Any
    {
        get => new RelatedData_Any();
    }
}

public interface IRelatedData<T> : ICompare where T : IRelatedData<T>
{
    /// <summary>
    /// 比较是否相等，可以在初始化数据的时候添加特殊的初始值作为任意值，当任意值和特定值做比较时返回true
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool Compare(T t);

    bool ICompare.Compare(ICompare t)
    {
        
        if (typeof (T)== typeof(RelatedData_Any))
        {
            return true;
        }
        else if(typeof(T) != typeof(RelatedData_Any)&&t.GetType() == typeof(RelatedData_Any))
        {
            Debug.LogError("不能和任意值比较");
            return false;
        }
        if (t is T _t)
        {
            return Compare(_t);
        }
        else return false;
    }
    public void CopyData(T t);
}

public  class RelatedData_Any : IRelatedData<RelatedData_Any>
{
    public bool Compare(RelatedData_Any t)
    {
        return true;
    }

    public void CopyData(RelatedData_Any t)
    {
        throw new NotImplementedException();
    }
}

public class RelateData_Bool : IRelatedData<RelateData_Bool>
{
    public bool yesorno=false; 
    public bool Compare(RelateData_Bool t)
    {
        if(this.yesorno== t.yesorno)
            return true;
        else return false;
    }

    public void CopyData(RelateData_Bool t)
    {
        this .yesorno = t.yesorno;

    }
}
