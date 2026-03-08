using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static System.Security.Cryptography.ECCurve;

// 锟斤拷值锟姐法锟斤拷锟斤拷
[System.Serializable]
public enum BuffOperationType
    {
    /// <summary>
    /// 锟接凤拷锟斤拷值
    /// </summary>
    Additive,
    /// <summary>
    /// 锟剿凤拷锟斤拷值
    /// </summary>
    Multiplicative
}

[System.Serializable]
// 锟戒化效锟斤拷锟斤拷锟斤拷
public enum BuffCurveType
    {
    /// <summary>
    /// 瞬锟斤拷浠拷锟剿诧拷锟斤拷锟绞?
    /// </summary>
    Instant,
    /// <summary>
    /// 锟斤拷锟斤拷衰锟斤拷
    /// </summary>
    LinearDecay,
    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷瞬锟斤拷锟斤拷失
    /// </summary>
    LinearGrowth,
    /// <summary>
    /// 锟斤拷锟诫缓锟斤拷
    /// </summary>
    EaseInEaseOut,

}

public enum BuffCurveTypeDIY
{
    LineAve,
    LineIn,
    LineOut,
    StantIn,
    StantOut
}

// 效锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
[System.Serializable]
public enum BuffStackType
    {
    /// <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟角碉拷锟斤拷
    /// </summary>
    Basic,
    /// <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟角革拷锟斤拷
    /// </summary>
    Override,
    /// <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟斤拷取锟斤拷锟斤拷锟街?
    /// </summary>
    MaxValue,
    /// <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟斤拷取锟斤拷小锟斤拷值
    /// </summary>
    MinValue,
    ///  <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟斤拷取锟筋长时锟斤拷
    /// </summary>
    MaxDuration,
    ///  <summary>
    /// 同锟斤拷锟斤拷锟斤拷锟斤拷取锟斤拷锟绞憋拷锟?
    /// </summary>
    MinDuration
}

[System.Serializable]
public struct DIYBuff<T>
{
    public float bufftime;
    public T buffvalue;
    public BuffCurveTypeDIY buffCurve;

}

// 锟斤拷锟斤拷值锟接匡拷
public interface IBaseValue<T>: IPoolObject
{
    T Value { get; set; }
    T GetOriginalValue();
    void SetOriginalValue(T value);
    void Init(T value,Action <T> act);
    Action<T> ValueChange {  get; set; }
}

    // 锟斤拷锟斤拷值锟斤拷锟斤拷实锟斤拷
    public class IntValue : IBaseValue<int>
{
        private int originalValue;
    Action <int> action { get; set; }
    
    public int Value { get => originalValue; set => originalValue = value; }
    public Action<int> ValueChange { get => action; set => action = value; }

    public int GetOriginalValue() => originalValue;

    public void Init(int value,Action<int> act=null )
    {
        originalValue = value;
        action = act;
    }

    public void ResetInfo()
    {
        originalValue = 0;
        action= null;
    }

    public void SetOriginalValue(int value) => originalValue = value;

}

public class FloatValue : IBaseValue<float>
{
    private float originalValue;

    Action<float > action { get; set; } 
    public float Value { get => originalValue; set => originalValue = value; }
    public Action<float> ValueChange { get => action; set => action = value; }

    public float GetOriginalValue() => originalValue;

    public void Init(float value,Action<float> act )
    {
        originalValue = value;
        action = act;
    }

    public void ResetInfo()
    {
        originalValue = 0f;
        action = null;
    }

    public void SetOriginalValue(float value) => originalValue = value;
}

public class Vector2Value : IBaseValue<Vector2>
{
    private Vector2 originalValue;
    Action<Vector2> action { get; set; }
    public Vector2 Value { get => originalValue; set => originalValue = value; }
    public Action<Vector2> ValueChange { get => action; set => action = value; }

    public Vector2 GetOriginalValue() => originalValue;

    public void Init(Vector2 value, Action<Vector2> act)
    {
        originalValue = value;
        action = act;
    }

    public void ResetInfo()
    {
        originalValue = Vector2.zero;
        action = null;
    }

    public void SetOriginalValue(Vector2 value) => originalValue = value;

}
public class Vector3Value : IBaseValue<Vector3>
{
    private Vector3 originalValue;
    Action <Vector3> action { get; set; }
    public Vector3 Value { get => originalValue; set => originalValue = value; }
    public Action<Vector3> ValueChange { get => action; set => action = value; }

    public Vector3 GetOriginalValue() => originalValue;

    public void Init(Vector3 value,Action<Vector3> act )
    {
        originalValue = value;
        action = act;
    }

    public void ResetInfo()
    {
        originalValue = Vector3.zero;
        action = null;
    }

    public void SetOriginalValue(Vector3 value) => originalValue = value;
}

// 锟斤拷锟斤拷效锟斤拷
public class BuffEffect<T> : IPoolObject
{
    public string Id { get; set; }
    public T Amount { get; set; }
    //public List<(float, T, BuffCurveTypeDIY)>  AmountDIY { get; set; }
    public List<DIYBuff<T>> AmountDIY { get; set; }

    public float Duration { get; set; }
    public float StartTime { get; set; }
    public BuffOperationType OperationType { get; set; }
    public BuffCurveType CurveType { get; set; }
    public BuffStackType StackType { get; set; }
    public Action<BuffEffect<T>> OnComplete { get; set; }


    private float GetProgress(float startTime,float duration)
    {
        return Mathf.Clamp01((Time.time - startTime) / duration);
    }

    public T GetCurrentAmount()
    {
        if (AmountDIY != null && AmountDIY.Count > 0)
        {
            return AddValues(GetCurrentAmountWithDIY(), GetCurrentAmountWithSinge(),true);
        }
        else
        return GetCurrentAmountWithSinge();
    }


    T GetCurrentAmountWithSinge()
    {
        float progress = GetProgress(StartTime, Duration);

        switch (CurveType)
        {
            case BuffCurveType.Instant:
                return progress < 1f ? Amount : default(T);

            case BuffCurveType.LinearDecay:
                float decayFactor = 1f - progress;
                return MultiplyByScalar(Amount, decayFactor);

            case BuffCurveType.LinearGrowth:
                if (progress < 1f)
                    return MultiplyByScalar(Amount, progress);
                else
                    return default(T);
            case BuffCurveType.EaseInEaseOut:
                //float easeProgress = Mathf.Sin(progress * Mathf.PI * 0.5f); // Ease-in
                //if (progress > 0.5f)
                //    easeProgress = Mathf.Sin((1f - progress) * Mathf.PI * 0.5f); // Ease-out
                float peak = Mathf.Sin(Mathf.PI * 0.25f); // 锟斤拷0.7071
                float raw = progress <= 0.5f
                    ? Mathf.Sin(progress * Mathf.PI * 0.5f)
                    : Mathf.Sin((1f - progress) * Mathf.PI * 0.5f);
                float easeProgress = raw / peak;
                return MultiplyByScalar(Amount, easeProgress);

            default:
                return Amount;
        }
    }

    int i = 0;
    T GetCurrentAmountWithDIY()
    {   
        float progress = 0;
        if (i == 0)
        {
            progress = GetProgress(StartTime, AmountDIY[i].bufftime);
      
        }
        else if(i< AmountDIY.Count&& i != 0)
        {
            if(AmountDIY[i].bufftime > AmountDIY[i - 1].bufftime)
            {
                progress = GetProgress(AmountDIY[i - 1].bufftime+ StartTime, AmountDIY[i].bufftime - AmountDIY[i - 1].bufftime);
   
            }
            else
            {
                var item1 = AmountDIY[i - 1].bufftime;
                var item2 = AmountDIY[i].buffvalue;
                var item3 = AmountDIY[i].buffCurve;
                AmountDIY[i]=new DIYBuff<T> { bufftime= item1 , buffvalue = item2 , buffCurve= item3 } ;
                //(float, T, BuffCurveTypeDIY) temp = (item1, item2, item3);
     
                return AmountDIY[i++].buffvalue;
            }

        }
        else
        {
            // progress = GetProgress(piontList[i - 1].Item1, Duration - piontList[i - 1].Item1);
            
            progress = GetProgress(AmountDIY[AmountDIY.Count - 1].bufftime + StartTime, Duration- AmountDIY[AmountDIY.Count - 1].bufftime);
            return progress < 0.98f ? AmountDIY[AmountDIY.Count - 1].buffvalue : default(T);
        }

        if(progress > 0.98f) i++;
        
        if (i < AmountDIY.Count)
        {
            switch (AmountDIY[i].buffCurve)
            {
                case BuffCurveTypeDIY.LineAve:
                    if (i <= 0) return MultiplyByScalar(AmountDIY[i].buffvalue, progress);
                    return AddValues(AmountDIY[i - 1].buffvalue, MultiplyByScalar(AddValues(AmountDIY[i].buffvalue, AmountDIY[i - 1].buffvalue, false), progress));
                
                case BuffCurveTypeDIY.LineIn:
                    float easeProgressIn_DIY = Mathf.Sin(progress * Mathf.PI * 0.5f);
                    if (i <= 0) return MultiplyByScalar(AmountDIY[i].buffvalue, easeProgressIn_DIY);
                    return AddValues(AmountDIY[i - 1].buffvalue, MultiplyByScalar(AddValues(AmountDIY[i].buffvalue, AmountDIY[i - 1].buffvalue, false), easeProgressIn_DIY));
                
                case BuffCurveTypeDIY.LineOut:
                    float easeProgressOut_DIY = Mathf.Sin((progress + 3) * Mathf.PI * 0.5f) + 1;
                    if (i <= 0) return MultiplyByScalar(AmountDIY[i].buffvalue, easeProgressOut_DIY);
                    return AddValues(AmountDIY[i - 1].buffvalue, MultiplyByScalar(AddValues(AmountDIY[i].buffvalue, AmountDIY[i - 1].buffvalue, false), easeProgressOut_DIY)); 
                   
                case BuffCurveTypeDIY.StantOut:
                    if (i <= 0) return progress < 0.98f ? default(T) : AmountDIY[i].buffvalue;
                    return progress < 0.98f ? AmountDIY[i - 1].buffvalue : AmountDIY[i].buffvalue;

                case BuffCurveTypeDIY.StantIn:
                    return AmountDIY[i].buffvalue;

                default:
                    return default(T);
            }
        }
        else return default(T);
        
    }
    private T AddValues(T a, T b,bool isAdd=true)
    {
        if (typeof(T) == typeof(int))
        {
            if (isAdd) return (T)(object)((int)(object)a + (int)(object)b);
            else return (T)(object)((int)(object)a - (int)(object)b);
        }
            
        else if (typeof(T) == typeof(float))
        {
            if (isAdd) return (T)(object)((float)(object)a + (float)(object)b);
            else return (T)(object)((float)(object)a - (float)(object)b);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            if (isAdd) return (T)(object)((Vector2)(object)a + (Vector2)(object)b);
            else return (T)(object)((Vector2)(object)a - (Vector2)(object)b);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            if (isAdd) return (T)(object)((Vector3)(object)a + (Vector3)(object)b);
            else return (T)(object)((Vector3)(object)a - (Vector3)(object)b);
        }
        else
            return a;
    }

    public float GetRemainingtime()
    {
        return Duration-(Time.time - StartTime);
    }

    private T MultiplyByScalar(T value, float scalar)
    {
        if (typeof(T) == typeof(int))
        {
            int intValue = Convert.ToInt32(value);
            int result = (int)(intValue * scalar);
            return (T)(object)result;
        }         
        else if (typeof(T) == typeof(float))
            return (T)(object)((float)(object)value * scalar);
        else if (typeof(T) == typeof(Vector2))
            return (T)(object)((Vector2)(object)value * scalar);
        else if (typeof(T) == typeof(Vector3))
            return (T)(object)((Vector3)(object)value * scalar);
        else
            return value;
    }

    public bool IsExpired()
    {
        return Time.time >= StartTime + Duration;
    }

    public void ResetInfo()
    {
        Id = "";
        Amount = default(T);
        if (AmountDIY != null) AmountDIY.Clear();  
        i = 0;
        Duration = 0;
        StartTime = 0;
        OperationType = BuffOperationType.Additive;
        CurveType = BuffCurveType.Instant;
        StackType = BuffStackType.Basic;
        OnComplete = null;
    }
    public void Init(string effectId, T amount, float duration,List<DIYBuff <T>> amountDIY, float starttime,
                                BuffOperationType opType = BuffOperationType.Additive,
                                BuffCurveType curveType = BuffCurveType.Instant,
                                BuffStackType stackType = BuffStackType.Basic,
                                Action<BuffEffect<T>> onComplete = null)
    {
        Id = effectId;
        Amount = amount;
        Duration = duration;
        StartTime = Time.time;
        OperationType = opType;
        CurveType = curveType;
        StackType = stackType;
        OnComplete = onComplete;
        AmountDIY = amountDIY;  
    }
}

// 锟斤拷锟斤拷锟斤拷锟斤拷锟斤拷
public class VariableManager<T>
{
    private Dictionary<int, IBaseValue<T>> variables = new Dictionary<int, IBaseValue<T>>();
    private Dictionary<int, List<BuffEffect<T>>> effects = new Dictionary<int, List<BuffEffect<T>>>();
    private Dictionary<int, T> lastvariables = new Dictionary<int, T>();
    private HashSet<string> expiredEffects = new HashSet<string>();
    public void RegisterVariable(int id, T initialValue,Action<T> act)
    {
        if (!variables.ContainsKey(id))
        {

            if (typeof(T) == typeof(int))
            {
                IBaseValue<int> variable = PoolMgr.Instance.GetObj<IntValue>();
                variable.Init((int)(object)initialValue, (Action<int>)(object)act);
                variables[id] = (IBaseValue<T>)variable;
                lastvariables[id] = variables[id].GetOriginalValue();
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(float))
            {
                IBaseValue<float> variable = PoolMgr.Instance.GetObj<FloatValue>();
                variable.Init((float)(object)initialValue, (Action<float>)(object)act);
                variables[id] = (IBaseValue<T>)variable;
                lastvariables[id] = variables[id].GetOriginalValue();
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(Vector2))
            {
                IBaseValue<Vector2> variable = PoolMgr.Instance.GetObj<Vector2Value>();
                variable.Init((Vector2)(object)initialValue, (Action<Vector2>)(object)act);
                variables[id] = (IBaseValue<T>)variable;
                lastvariables[id] = variables[id].GetOriginalValue();
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(Vector3))
            {
                IBaseValue<Vector3> variable = PoolMgr.Instance.GetObj<Vector3Value>();
                variable.Init((Vector3)(object)initialValue, (Action<Vector3>)(object)act);
                variables[id] = (IBaseValue<T>)variable;
                lastvariables[id] = variables[id].GetOriginalValue();
                effects[id] = new List<BuffEffect<T>>();
            }
            //variables[id].ValueChange?.Invoke(initialValue);
        }
    }
    //public void RegisterVariable(string id, IBaseValue<T> variable)
    //{
    //    if (!variables.ContainsKey(id))
    //    {
    //        variables[id] = variable;
    //        effects[id] = new List<BuffEffect<T>>();
    //    }
    //}

    public void UnregisterVariable(int id)
    {
        if (variables[id]!=null)
        {
            PoolMgr.Instance.PushObj(variables[id]);
            variables.Remove(id);
        }
        if (effects.ContainsKey(id))
        {
            if (effects[id].Count > 0)
            {
                foreach (var effect in effects[id])
                {
                    PoolMgr.Instance.PushObj(effect);
                }
                effects.Remove(id);
            }
        }
    }

    public void ReMoveAllBuff(int id)
    {
        if (effects.ContainsKey(id))
        {
            if (effects[id].Count > 0)
            {
                foreach (var effect in effects[id])
                {
                    PoolMgr.Instance.PushObj(effect);
                }
                effects[id].Clear();
            }
        }
    }

    public void AddBuff(int variableId, BuffEffect<T> effect)
    {
        if (!variables.ContainsKey(variableId)) return;
        if (!effects.ContainsKey(variableId)) return;
        var varEffects = effects[variableId];

        // 锟斤拷锟捷堆碉拷锟斤拷锟酵达拷锟斤拷效锟斤拷
        switch (effect.StackType)
        {
            case BuffStackType.Override:
                var removeEfffectsOverride = varEffects.Where(e => e.StackType == BuffStackType.Override).ToList();
                if(removeEfffectsOverride.Count > 0)
                {
                    foreach (var eff in removeEfffectsOverride)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == BuffStackType.Override);
                }     
                break;

            case BuffStackType.MaxValue:
                // 锟斤拷锟斤拷欠锟角靶э拷锟街碉拷锟斤拷锟?
                var maxEffect = varEffects.Find(e => e.StackType == BuffStackType.MaxValue);
                if (maxEffect != null && CompareValues(effect.Amount, maxEffect.Amount) <= 0)
                    return;
                // 锟斤拷应锟斤拷锟斤拷效锟斤拷
                var removeEfffectsMaxValue = varEffects.Where(e => e.StackType == BuffStackType.MaxValue).ToList();
                if (removeEfffectsMaxValue.Count > 0)
                {
                    foreach (var eff in removeEfffectsMaxValue)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == BuffStackType.MaxValue);
                }
                break;

            case BuffStackType.MinValue:
                // 锟斤拷锟斤拷欠锟角靶э拷锟街碉拷锟叫?
                var minEffect = varEffects.Find(e => e.StackType == BuffStackType.MinValue);
                if (minEffect != null && CompareValues(effect.Amount, minEffect.Amount) >= 0)
                    return; // 锟斤拷应锟斤拷锟斤拷效锟斤拷
                var removeEfffectsMinValue = varEffects.Where(e => e.StackType == BuffStackType.MinValue).ToList();
                if (removeEfffectsMinValue.Count > 0)
                {
                    foreach (var eff in removeEfffectsMinValue)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == BuffStackType.MinValue);
                }   
                break;

            case BuffStackType.MaxDuration:
                // 锟斤拷锟斤拷欠锟角靶э拷锟斤拷锟斤拷锟绞憋拷锟斤拷锟斤拷
                var maxDurEffect = varEffects.Find(e => e.StackType == BuffStackType.MaxDuration);
                if (maxDurEffect != null && effect.Duration <= maxDurEffect.Duration)
                    return; // 锟斤拷应锟斤拷锟斤拷效锟斤拷
                var removeEfffectsMaxDuration = varEffects.Where(e => e.StackType == BuffStackType.MaxDuration).ToList();
                if (removeEfffectsMaxDuration.Count > 0)
                {
                    foreach (var eff in removeEfffectsMaxDuration)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == BuffStackType.MaxDuration);
                }

                break;

            case BuffStackType.MinDuration:
                // 锟斤拷锟斤拷欠锟角靶э拷锟斤拷锟斤拷锟绞憋拷锟斤拷锟斤拷
                var minDurEffect = varEffects.Find(e => e.StackType == BuffStackType.MinDuration);
                if (minDurEffect != null && effect.Duration >= minDurEffect.Duration)
                    return; // 锟斤拷应锟斤拷锟斤拷效锟斤拷
                var removeEfffectsMinDuration = varEffects.Where(e => e.StackType == BuffStackType.MinDuration).ToList();
                if (removeEfffectsMinDuration.Count > 0)
                {
                    foreach (var eff in removeEfffectsMinDuration)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == BuffStackType.MinDuration);
               }
                    
                break;
        }

        varEffects.Add(effect);
    }



    public void RemoveBuff(int variableId, string effectId)
    {
        if (!effects.ContainsKey(variableId)) return;

        var varEffects = effects[variableId];
        var removeEfffects = varEffects.Where(e => e.Id == effectId).ToList();
        if (removeEfffects.Count > 0)
        {
            foreach (var eff in removeEfffects)
            {
                PoolMgr.Instance.PushObj(eff);
            }
            varEffects.RemoveAll(e => e.Id == effectId);
        }
    }

    public float[] GetBuffRemainingtime(int variableId, string effectId)
    {
        float[] items= null;
        if (!effects.ContainsKey(variableId))
        {
            Debug.Log("未锟揭碉拷锟斤拷应Buff");
            return null;
        }

        var varEffects = effects[variableId];
        if (varEffects.Count == 0) return null;
        var effectItems = varEffects.Where(e => e.Id == effectId).ToList();
        if (effectItems.Count > 0)
        {
            for (int i = 0; i < effectItems.Count; i++)
            {
                items[i]= effectItems[i].GetRemainingtime ();
                Debug.Log($"锟斤拷{i}锟斤拷:{items[i]}");
            }
        }
        return items;

    }

    public List<BuffEffect<T>> GetBuff(int variableId, string effectId)
    {
        if (!effects.ContainsKey(variableId)) return null; 
        var varEffects = effects[variableId];
        if (varEffects.Count == 0) return null;
        return varEffects.Where(e => e.Id == effectId).ToList();
    }

    public void ReSetBuff(int variableId, string effectId)
    {
        var resetbuff=GetBuff (variableId, effectId);
        if(resetbuff .Count == 0) return;
        foreach (var eff in resetbuff)
        {
            eff.StartTime=Time.time;
        }
    }

    public T GetBaseValue(int variableId)
    {
        if (!variables.ContainsKey(variableId))
            throw new ArgumentException($"Variable {variableId} not found");

        var variable = variables[variableId];

        return variable.GetOriginalValue();
    }

    public T GetFinalValue(int variableId)
    {
        if (!variables.ContainsKey(variableId))
            throw new ArgumentException($"Variable {variableId} not found");

        var variable = variables[variableId];
        var varEffects = effects[variableId];

        T finalValue = variable.GetOriginalValue();

        foreach (var effect in varEffects)
        {
            if (effect.IsExpired())
            {
                expiredEffects.Add($"{variableId}:{effect.Id}");
                continue;
            }

            T currentAmount= effect.GetCurrentAmount();
            

            switch (effect.OperationType)
            {
                case BuffOperationType.Additive:
                    finalValue = AddValues(finalValue, currentAmount);
                    break;
                case BuffOperationType.Multiplicative:
                    finalValue = MultiplyValues(finalValue, AddScalar(currentAmount, 1f));
                    break;
            }
        }


        if (lastvariables.ContainsKey(variableId)&& !Compare(finalValue , lastvariables[variableId]))
        {
            variables[variableId].ValueChange?.Invoke(finalValue);
            lastvariables[variableId]= finalValue; 
        }

        // 锟斤拷锟斤拷锟斤拷锟斤拷效锟斤拷
        foreach (var expiredKey in expiredEffects)
        {
            var parts = expiredKey.Split(':');
            if (parts.Length == 2)
            {
                int expVarId = Convert.ToInt32(parts[0]);
                var expEffectId = parts[1];
                var varEffectsList = effects[expVarId];
                var expiredEffect = varEffectsList.Find(e => e.Id == expEffectId);
                if (expiredEffect != null)
                {
                    expiredEffect.OnComplete?.Invoke(expiredEffect);
                    PoolMgr.Instance.PushObj(expiredEffect);    
                    varEffectsList.Remove(expiredEffect);
                }
            }
        }
        expiredEffects.Clear();

        return finalValue;
    }


    private T AddValues(T a, T b)
    {
        if (typeof(T) == typeof(int))
            return (T)(object)((int)(object)a + (int)(object)b);
        else if (typeof(T) == typeof(float))
            return (T)(object)((float)(object)a + (float)(object)b);
        else if (typeof(T) == typeof(Vector2))
            return (T)(object)((Vector2)(object)a + (Vector2)(object)b);
        else if (typeof(T) == typeof(Vector3))
            return (T)(object)((Vector3)(object)a + (Vector3)(object)b);
        else
            return a;
    }

    private T MultiplyValues(T a, T b)
    {
        if (typeof(T) == typeof(int))
            return (T)(object)((int)(object)a * (int)(object)b);
        else if (typeof(T) == typeof(float))
            return (T)(object)((float)(object)a * (float)(object)b);
        else if (typeof(T) == typeof(Vector2))
        {
            Vector2 va = (Vector2)(object)a;
            Vector2 vb = (Vector2)(object)b;
            return (T)(object)new Vector2(va.x * vb.x, va.y * vb.y);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 va = (Vector3)(object)a;
            Vector3 vb = (Vector3)(object)b;
            return (T)(object)new Vector3(va.x * vb.x, va.y * vb.y, va.z * vb.z);
        }
        else
            return a;
    }

    private bool Compare(T a, T b)
    {
        if (typeof(T) == typeof(int))
            return (int)(object)a == (int)(object)b;
        else if (typeof(T) == typeof(float))
            return (float)(object)a == (float)(object)b;
        else if (typeof(T) == typeof(Vector2))
            return (Vector2)(object)a == (Vector2)(object)b;
        else if (typeof(T) == typeof(Vector3))
            return (Vector3)(object)a == (Vector3)(object)b;
        else
            return false;
    }

    private T AddScalar(T value, float scalar)
    {
        if (typeof(T) == typeof(int))
            return (T)(object)((int)(object)value + (int)scalar);
        else if (typeof(T) == typeof(float))
            return (T)(object)((float)(object)value + scalar);
        else if (typeof(T) == typeof(Vector2))
        {
            Vector2 v = (Vector2)(object)value;
            return (T)(object)new Vector2(v.x + scalar, v.y + scalar);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 v = (Vector3)(object)value;
            return (T)(object)new Vector3(v.x + scalar, v.y + scalar, v.z + scalar);
        }
        else
            return value;
    }

    private int CompareValues(T a, T b)
    {
        if (typeof(T) == typeof(int))
        {
            int ia = (int)(object)a;
            int ib = (int)(object)b;
            return ia.CompareTo(ib);
        }
        else if (typeof(T) == typeof(float))
        {
            float fa = (float)(object)a;
            float fb = (float)(object)b;
            return fa.CompareTo(fb);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            Vector2 va = (Vector2)(object)a;
            Vector2 vb = (Vector2)(object)b;
            float magA = va.magnitude;
            float magB = vb.magnitude;
            return magA.CompareTo(magB);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            Vector3 va = (Vector3)(object)a;
            Vector3 vb = (Vector3)(object)b;
            float magA = va.magnitude;
            float magB = vb.magnitude;
            return magA.CompareTo(magB);
        }
        else
            return 0;
    }

    public void UpdateVariableValue(int variableId, T newValue)
    {
        if (variables.ContainsKey(variableId))
        {
            variables[variableId].SetOriginalValue(newValue);
        }
    }
}

// 系统锟斤拷锟斤拷锟斤拷
public class BuffMgr : SingletonAutoMono<BuffMgr>
{
    private int intvalueKey = 0;
    private int floatvalueKey = 0;
    private int vector2valueKey = 0;
    private int vector3valueKey = 0;

    private VariableManager<int> intManager = new VariableManager<int>();
    private VariableManager<float> floatManager = new VariableManager<float>();
    private VariableManager<Vector2> vector2Manager = new VariableManager<Vector2>();
    private VariableManager<Vector3> vector3Manager = new VariableManager<Vector3>();

    private Dictionary <int,Coroutine> coroutineInt=new Dictionary<int,Coroutine>();
    private Dictionary<int, Coroutine> coroutineFloat = new Dictionary<int, Coroutine>();
    private Dictionary<int, Coroutine> coroutineVector2 = new Dictionary<int, Coroutine>();
    private Dictionary<int, Coroutine> coroutineVector3 = new Dictionary<int, Coroutine>();


    public int AddValue<T>(T initialValue=default(T),Action <T> valueChange=null)
    {

        if (typeof(T) == typeof(int))
        {
            int intValue = Convert.ToInt32(initialValue);
            Action<int> action = (Action<int>)Convert.ChangeType(valueChange, typeof(Action<T>));
            intManager.RegisterVariable(intvalueKey, intValue, action);
            return intvalueKey++;
        }
        else if (typeof(T) == typeof(float))
        {
            float intValue = Convert.ToSingle(initialValue);
            Action<float> action = (Action<float>)Convert.ChangeType(valueChange, typeof(Action<T>));
            floatManager.RegisterVariable(floatvalueKey, intValue, action);
            return floatvalueKey++;
        }
        else if (typeof(T) == typeof(Vector2))
        {
            Action<Vector2> action = (Action<Vector2>)Convert.ChangeType(valueChange, typeof(Action<T>));
            vector2Manager.RegisterVariable(vector2valueKey, (Vector2)(object)initialValue, action);
            return vector2valueKey++;
        }

        else if (typeof(T) == typeof(Vector3))
        {
            Action<Vector3> action = (Action<Vector3>)Convert.ChangeType(valueChange, typeof(Action<T>));
            vector3Manager.RegisterVariable(vector3valueKey, (Vector3)(object)initialValue, action);
            return vector3valueKey++;
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
        return -1;
    }

    public void RemoveValue<T>(int id)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.UnregisterVariable(id);
            if (coroutineInt.ContainsKey(id) && coroutineInt[id] != null) StopCoroutine(coroutineInt[id]);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.UnregisterVariable(id);
            if (coroutineFloat.ContainsKey(id) && coroutineFloat[id] != null) StopCoroutine(coroutineFloat[id]);
        }
        
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.UnregisterVariable(id);
            if (coroutineVector2.ContainsKey(id) && coroutineVector2[id] != null) StopCoroutine(coroutineVector2[id]);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.UnregisterVariable(id);
            if (coroutineVector3.ContainsKey (id)&&coroutineVector3[id] != null) StopCoroutine(coroutineVector3[id]);
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }

    public void ReMoveAllBuff<T>(int id)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.ReMoveAllBuff(id);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.ReMoveAllBuff(id);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.ReMoveAllBuff(id);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.ReMoveAllBuff(id);
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }

    public T GetValue<T>(int id)
    {
        if (typeof(T) == typeof(int))
        {
            return (T)Convert.ChangeType(intManager.GetFinalValue(id), typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            return (T)Convert.ChangeType(floatManager.GetFinalValue(id), typeof(T));
        }
        else if (typeof(T) == typeof(Vector2))
        {
            return (T)Convert.ChangeType(vector2Manager.GetFinalValue(id), typeof(T));
        }

        else if (typeof(T) == typeof(Vector3))
        {
            return (T)Convert.ChangeType(vector3Manager.GetFinalValue(id), typeof(T));
        }
        else
        {
            Debug.Log("锟斤拷效锟斤拷锟斤拷");
            return default(T);
        }
    }

    public T GetBaseValue<T>(int id)
    {
        if (typeof(T) == typeof(int))
        {
            return (T)Convert.ChangeType(intManager.GetBaseValue(id), typeof(T));
        }
        else if (typeof(T) == typeof(float))
        {
            return (T)Convert.ChangeType(floatManager.GetBaseValue(id), typeof(T));
        }
        else if (typeof(T) == typeof(Vector2))
        {
            return (T)Convert.ChangeType(vector2Manager.GetBaseValue(id), typeof(T));
        }

        else if (typeof(T) == typeof(Vector3))
        {
            return (T)Convert.ChangeType(vector3Manager.GetBaseValue(id), typeof(T));
        }
        else
        {
            Debug.Log("锟斤拷效锟斤拷锟斤拷");
            return default(T);
        }
    }



    public void SetBaseValue<T>(int id, T newValue)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.UpdateVariableValue(id, (int)(object)newValue);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.UpdateVariableValue(id, (float)(object)newValue);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.UpdateVariableValue(id, (Vector2)(object)newValue);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.UpdateVariableValue(id, (Vector3)(object)newValue);
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
        
    }
    /// <summary>
    /// 施锟斤拷锟斤拷值效锟斤拷
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amount">锟斤拷锟斤拷锟街?/param>
    /// <param name="duration">锟斤拷值时锟斤拷</param>
    /// <param name="variableId">锟斤拷锟侥革拷锟斤拷锟斤拷锟斤拷值</param>
    /// <param name="curveType">锟戒化锟斤拷锟斤拷</param>
    /// <param name="stackType">锟斤拷锟斤拷锟斤拷锟斤拷</param>
    /// <param name="opType">锟斤拷值锟姐法</param>
    /// <param name="effectId">效锟斤拷ID</param>
    /// <param name="onComplete">锟斤拷苫氐锟?/param>
    public void AddBuff<T>(int variableId, T amount, float duration,  
                            BuffCurveType curveType = BuffCurveType.Instant,
                            BuffStackType stackType = BuffStackType.Basic, 
                            BuffOperationType opType = BuffOperationType.Additive,
                            string effectId = "", 
                            Action<BuffEffect<T>> onComplete = null)
    {
        var effect = PoolMgr.Instance.GetObj<BuffEffect<T>>();
        effect.Init(effectId, amount, duration,null, Time.time, opType, curveType, stackType, onComplete); 
        if (effect.GetType()== typeof(BuffEffect<int>))
        {
            intManager.AddBuff(variableId, (BuffEffect<int>)Convert.ChangeType(effect, typeof(BuffEffect<int>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<float>))
        {
            floatManager.AddBuff(variableId, (BuffEffect<float>)Convert.ChangeType(effect, typeof(BuffEffect<float>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector2>))
        {
            vector2Manager.AddBuff(variableId, (BuffEffect<Vector2>)Convert.ChangeType(effect, typeof(BuffEffect<Vector2>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector3>))
        {
            vector3Manager.AddBuff(variableId, (BuffEffect<Vector3>)Convert.ChangeType(effect, typeof(BuffEffect<Vector3>)));
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }

    public void ApplyBuff<T>(int variableId, T amount, float duration,
                          BuffCurveType curveType = BuffCurveType.Instant,
                          BuffStackType stackType = BuffStackType.Basic,
                          BuffOperationType opType = BuffOperationType.Additive,
                          string effectId = "",
                          Action<BuffEffect<T>> onComplete = null)
    {
        var effect = PoolMgr.Instance.GetObj<BuffEffect<T>>();
        effect.Init(effectId, amount, duration, null, Time.time, opType, curveType, stackType, onComplete);
        if (effect.GetType() == typeof(BuffEffect<int>))
        {
            intManager.AddBuff(variableId, (BuffEffect<int>)Convert.ChangeType(effect, typeof(BuffEffect<int>)));
            if (coroutineInt.ContainsKey(variableId))
            {
                if (coroutineInt[variableId] != null) StopCoroutine(coroutineInt[variableId]);
                coroutineInt[variableId] = StartCoroutine(DoUpdateIntValue(variableId, duration));
            }
            else coroutineInt[variableId] = StartCoroutine(DoUpdateIntValue(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<float>))
        {
            floatManager.AddBuff(variableId, (BuffEffect<float>)Convert.ChangeType(effect, typeof(BuffEffect<float>)));
            if(coroutineFloat.ContainsKey(variableId))
            {
                if(coroutineFloat[variableId]!=null) StopCoroutine(coroutineFloat[variableId]);
                coroutineFloat[variableId] = StartCoroutine(DoUpdateFloatValue(variableId, duration));
            }
            else coroutineFloat[variableId] = StartCoroutine(DoUpdateFloatValue(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector2>))
        {
            vector2Manager.AddBuff(variableId, (BuffEffect<Vector2>)Convert.ChangeType(effect, typeof(BuffEffect<Vector2>)));
            if (coroutineVector2.ContainsKey(variableId))
            {
                if (coroutineVector2[variableId] != null) StopCoroutine(coroutineVector2[variableId]);
                coroutineVector2[variableId] = StartCoroutine(DoUpdateVector2Value(variableId, duration));
            }
            else coroutineVector2[variableId] = StartCoroutine(DoUpdateVector2Value(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector3>))
        {
            vector3Manager.AddBuff(variableId, (BuffEffect<Vector3>)Convert.ChangeType(effect, typeof(BuffEffect<Vector3>)));
            if (coroutineVector3.ContainsKey(variableId))
            {
                if (coroutineVector3[variableId] != null) StopCoroutine(coroutineVector3[variableId]);
                coroutineVector3[variableId] = StartCoroutine(DoUpdateVector3Value(variableId, duration));
            }
            else coroutineVector3[variableId] = StartCoroutine(DoUpdateVector3Value(variableId, duration));
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");

    }

    //private void Start()
    //{
    //    StartCoroutine(DoUpdateValue());
    //}

    //IEnumerator DoUpdateValue()
    //{
    //    float currenttime;
    //    while (true)
    //    {
    //        currenttime = Time.time;
    //        if(coroutineInt_.Count > 0)
    //        {

    //        }
    //        yield return null;
    //    }
    //}

    //Queue<int> coroutineInt_ = new Queue<int>();
    //Queue<int> coroutineFloat_ = new Queue<int>();
    //Queue<int> coroutineVector2_ = new Queue<int>();
    //Queue<int> coroutineVector3_ = new Queue<int>();

    //void DoUpdateIntValue_(int variableId, float updatetime)
    //{
    //    if (Time.time - currenttime < updatetime)
    //    {

    //    }
    //}


    IEnumerator DoUpdateIntValue(int variableId,float updatetime)
    {
        var currenttime = Time.time;
        while(Time.time - currenttime< updatetime)
        {
            intManager.GetFinalValue(variableId);
            //UpdateintValue?.Invoke(variableId);
            yield return null;
        }
    }
    IEnumerator DoUpdateFloatValue(int variableId, float updatetime)
    {
        var currenttime = Time.time;
        while (Time.time - currenttime < updatetime)
        {
            floatManager.GetFinalValue(variableId);
           // UpdatefloatValue?.Invoke(variableId);
            yield return null;
        }
    }
    IEnumerator DoUpdateVector2Value(int variableId, float updatetime)
    {
        var currenttime = Time.time;
        while (Time.time - currenttime < updatetime)
        {
            vector2Manager.GetFinalValue(variableId);
            //UpdateVector2Value?.Invoke(variableId);
            yield return null;
        }

    }
    IEnumerator DoUpdateVector3Value(int variableId, float updatetime)
    {
        var currenttime = Time.time;
        while (Time.time - currenttime < updatetime)
        {
            vector3Manager.GetFinalValue(variableId);
            // UpdateVector3Value?.Invoke(variableId);
            yield return null;
        }
    }


    public void AddBuffWithDIY<T>(int variableId, float duration, List<DIYBuff <T>> amountDIY,
                            BuffStackType stackType = BuffStackType.Basic,
                            BuffOperationType opType = BuffOperationType.Additive,
                            string effectId = "",
                            Action<BuffEffect<T>> onComplete = null)
    {
        var effect = PoolMgr.Instance.GetObj<BuffEffect<T>>();

        List<DIYBuff<T>> buff = new List<DIYBuff<T>>(); 
        foreach (var item in amountDIY)
        {
            if (item.bufftime >= 0)
            {
                buff.Add(item);
            }
            else Debug.Log($"时锟戒不锟斤拷为锟斤拷:{item}");
       
        }
        for (int i = 0; i < buff.Count; i++)
        {
            
            if (buff[i].bufftime  > duration&& i+1< buff.Count -i)
            {
               if(i + 1!= buff.Count - i) buff.RemoveRange(i + 1, buff.Count - i);
            }
        }
        if (buff[buff.Count - 1].bufftime > duration)
        {
            
            effect.Init(effectId, default, duration, buff, Time.time, opType, default, stackType, (a) =>
            {
                T value = a.GetCurrentAmount();         
              
                if (typeof(T) == typeof(int))
                {
                   
                    var aa = (int)Convert.ChangeType(value, typeof(T));
                    aa += (int)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else if (typeof(T) == typeof(float))
                {
                   
                    var aa = (float)Convert.ChangeType(value, typeof(T));
                    aa += (float)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else if (typeof(T) == typeof(Vector2))
                {
                    
                    var aa = (Vector2)Convert.ChangeType(value, typeof(T));
                    aa += (Vector2)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }

                else if (typeof(T) == typeof(Vector3))
                {
                   
                    var aa = (Vector3)Convert.ChangeType(value, typeof(T));
                    aa += (Vector3)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else
                {
                    Debug.Log("锟斤拷效锟斤拷锟斤拷");
                }
            });
        }
        else effect.Init(effectId, default, duration, buff, Time.time, opType, default, stackType, onComplete);

        if (effect.GetType() == typeof(BuffEffect<int>))
        {
            intManager.AddBuff(variableId, (BuffEffect<int>)Convert.ChangeType(effect, typeof(BuffEffect<int>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<float>))
        {
            floatManager.AddBuff(variableId, (BuffEffect<float>)Convert.ChangeType(effect, typeof(BuffEffect<float>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector2>))
        {
            vector2Manager.AddBuff(variableId, (BuffEffect<Vector2>)Convert.ChangeType(effect, typeof(BuffEffect<Vector2>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector3>))
        {
            vector3Manager.AddBuff(variableId, (BuffEffect<Vector3>)Convert.ChangeType(effect, typeof(BuffEffect<Vector3>)));
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }


    public void ApplyBuffWithDIY<T>(int variableId, float duration, List<DIYBuff <T>> amountDIY,
                            BuffStackType stackType = BuffStackType.Basic,
                            BuffOperationType opType = BuffOperationType.Additive,
                            string effectId = "",
                            Action<BuffEffect<T>> onComplete = null)
    {
        var effect = PoolMgr.Instance.GetObj<BuffEffect<T>>();

        List<DIYBuff<T>> buff = new List<DIYBuff<T>>();
        foreach (var item in amountDIY)
        {
            if (item.bufftime >= 0)
            {
                buff.Add(item);
            }
            else Debug.Log($"时锟戒不锟斤拷为锟斤拷:{item}");

        }

        for (int i = 0; i < amountDIY.Count; i++)
        {

            if (buff[i].bufftime > duration && i + 1 < buff.Count - i)
            {
                if (i + 1 != buff.Count - i) buff.RemoveRange(i + 1, amountDIY.Count - i);
            }
        }
        if (buff[buff.Count - 1].bufftime > duration)
        {

            effect.Init(effectId, default, duration, buff, Time.time, opType, default, stackType, (a) =>
            {
                Debug.Log("xxx");
                T value = a.GetCurrentAmount();
  

                if (typeof(T) == typeof(int))
                {
                    var aa = (int)Convert.ChangeType(value, typeof(T));
                    aa += (int)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else if (typeof(T) == typeof(float))
                {


                    var aa = (float)Convert.ChangeType(value, typeof(T));
                    aa += (float)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else if (typeof(T) == typeof(Vector2))
                {

                    var aa = (Vector2)Convert.ChangeType(value, typeof(T));
                    aa += (Vector2)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }

                else if (typeof(T) == typeof(Vector3))
                {

                    var aa = (Vector3)Convert.ChangeType(value, typeof(T));
                    aa += (Vector3)Convert.ChangeType(buff[buff.Count - 1].buffvalue, typeof(T));
                    BuffMgr.Instance.SetBaseValue<T>(variableId, value);
                }
                else
                {
                    Debug.Log("锟斤拷效锟斤拷锟斤拷");
                }
            });
        }
        else effect.Init(effectId, default, duration, buff, Time.time, opType, default, stackType, onComplete);

        if (effect.GetType() == typeof(BuffEffect<int>))
        {
            intManager.AddBuff(variableId, (BuffEffect<int>)Convert.ChangeType(effect, typeof(BuffEffect<int>)));
            if (coroutineInt.ContainsKey(variableId))
            {
                if (coroutineInt[variableId] != null) StopCoroutine(coroutineInt[variableId]);
                coroutineInt[variableId] = StartCoroutine(DoUpdateIntValue(variableId, duration));
            }
            else coroutineInt[variableId] = StartCoroutine(DoUpdateIntValue(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<float>))
        {
            floatManager.AddBuff(variableId, (BuffEffect<float>)Convert.ChangeType(effect, typeof(BuffEffect<float>)));
            if (coroutineFloat.ContainsKey(variableId))
            {
                if (coroutineFloat[variableId] != null) StopCoroutine(coroutineFloat[variableId]);
                coroutineFloat[variableId] = StartCoroutine(DoUpdateFloatValue(variableId, duration));
            }
            else coroutineFloat[variableId] = StartCoroutine(DoUpdateFloatValue(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector2>))
        {
            vector2Manager.AddBuff(variableId, (BuffEffect<Vector2>)Convert.ChangeType(effect, typeof(BuffEffect<Vector2>)));
            if (coroutineVector2.ContainsKey(variableId))
            {
                if (coroutineVector2[variableId] != null) StopCoroutine(coroutineVector2[variableId]);
                coroutineVector2[variableId] = StartCoroutine(DoUpdateVector2Value(variableId, duration));
            }
            else coroutineVector2[variableId] = StartCoroutine(DoUpdateVector2Value(variableId, duration));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector3>))
        {
            vector3Manager.AddBuff(variableId, (BuffEffect<Vector3>)Convert.ChangeType(effect, typeof(BuffEffect<Vector3>)));
            if (coroutineVector3.ContainsKey(variableId))
            {
                if (coroutineVector3[variableId] != null) StopCoroutine(coroutineVector3[variableId]);
                coroutineVector3[variableId] = StartCoroutine(DoUpdateVector3Value(variableId, duration));
            }
            else coroutineVector3[variableId] = StartCoroutine(DoUpdateVector3Value(variableId, duration));
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }

    public void RemoveBuff<T>(int variableId, string effectId)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.RemoveBuff(variableId, effectId);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.RemoveBuff(variableId, effectId);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.RemoveBuff(variableId, effectId);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.RemoveBuff(variableId, effectId);
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }
    public List<BuffEffect<T>> GetBuff<T>(int variableId, string effectId)
    {
         if (typeof(T) == typeof(int))
        {
            return (List<BuffEffect<T>>)Convert.ChangeType(intManager.GetBuff(variableId, effectId), typeof(List<BuffEffect<T>>));
        }
        else if (typeof(T) == typeof(float))
        {
            return (List<BuffEffect<T>>)Convert.ChangeType(floatManager.GetBuff(variableId, effectId), typeof(List<BuffEffect<T>>));
        }
        else if (typeof(T) == typeof(Vector2))
        {
            return (List<BuffEffect<T>>)Convert.ChangeType(vector2Manager.GetBuff(variableId, effectId), typeof(List<BuffEffect<T>>));
        }

        else if (typeof(T) == typeof(Vector3))
        {
            return (List<BuffEffect<T>>)Convert.ChangeType(vector3Manager.GetBuff(variableId, effectId), typeof(List<BuffEffect<T>>));
        }
        else
        {
            Debug.Log("锟斤拷效锟斤拷锟斤拷");
            return null;
        }

    }

    public float[] GetBuffRemainingtime<T>(int variableId, string effectId)
    {

        if (typeof(T) == typeof(int))
        {
            return intManager.GetBuffRemainingtime(variableId, effectId);
        }
        else if (typeof(T) == typeof(float))
        {
            return floatManager.GetBuffRemainingtime(variableId, effectId);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            return vector2Manager.GetBuffRemainingtime(variableId, effectId);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            return vector3Manager.GetBuffRemainingtime(variableId, effectId);
        }
        else
        {
            Debug.Log("锟斤拷效锟斤拷锟斤拷");
            return null;
        }
    }




    /// <summary>
    /// 锟斤拷锟斤拷锟斤拷锟叫碉拷锟斤拷锟斤拷同锟斤拷锟斤拷同锟斤拷锟斤拷值效锟斤拷锟斤拷锟斤拷
    /// </summary>
    /// <param name="variableId"></param>
    /// <param name="effectId"></param>
    public void ReSetBuff<T>(int variableId, string effectId)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.ReSetBuff(variableId, effectId);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.ReSetBuff(variableId, effectId);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.ReSetBuff(variableId, effectId);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.ReSetBuff(variableId, effectId);
        }
        else Debug.Log("锟斤拷效锟斤拷锟斤拷");
    }

    public void CreateAndApplyBuff<T>(Action<T> valueChange, T amount, float duration,
                          BuffCurveType curveType = BuffCurveType.Instant,
                          BuffStackType stackType = BuffStackType.Basic,
                          BuffOperationType opType = BuffOperationType.Additive,
                          Action<BuffEffect<T>> onComplete = null)
    {
        var tag= AddValue<T>(default(T), valueChange);
        ApplyBuff<T>(tag, amount, duration, curveType, stackType, opType, null, (a) => {
            RemoveValue<T>(tag);
        });
    }
    public void CreateAndApplyBuffWithDIY<T>(Action<T> valueChange,float duration, List<DIYBuff <T>> amountDIY,
                          BuffCurveType curveType = BuffCurveType.Instant,
                          BuffStackType stackType = BuffStackType.Basic,
                          BuffOperationType opType = BuffOperationType.Additive,
                          Action<BuffEffect<T>> onComplete = null)
    {
        var tag = AddValue<T>(default(T), valueChange);
        ApplyBuffWithDIY<T>(tag,duration, amountDIY, stackType, opType, null, (a) => {
            RemoveValue<T>(tag);
        });
    }

    //#region Int Variables

    //public void AddIntVariable(string id, int initialValue = 0)
    //{
    //    intManager.RegisterVariable(id, initialValue);
    //}

    //public void RemoveIntVariable(string id)
    //{
    //    intManager.UnregisterVariable(id);
    //}

    //public int GetIntValue(string id)
    //{
    //    return intManager.GetFinalValue(id);
    //}

    //public void SetIntValue(string id, int newValue)
    //{
    //    intManager.UpdateVariableValue(id, newValue);
    //}

    //public void ApplyIntBuff(string variableId, string effectId, int amount, float duration,
    //                        BuffOperationType opType = BuffOperationType.Additive,
    //                        EffectCurveType curveType = EffectCurveType.Instant,
    //                        StackType stackType = StackType.Basic,
    //                        Action<BuffEffect<int>> onComplete = null)
    //{
    //    var effect = PoolMgr.Instance.GetObj<BuffEffect<int>>();
    //    effect.Init(effectId, amount, duration, Time.time, opType, curveType, stackType, onComplete);
    //    //var effect = new BuffEffect<int>
    //    //    {
    //    //        Id = effectId,
    //    //        Amount = amount,
    //    //        Duration = duration,
    //    //        StartTime = Time.time,
    //    //        OperationType = opType,
    //    //        CurveType = curveType,
    //    //        StackType = stackType,
    //    //        OnComplete = onComplete
    //    //    };

    //    intManager.ApplyBuff(variableId, effect);
    //}

    //public void RemoveIntBuff(string variableId, string effectId)
    //{
    //    intManager.RemoveBuff(variableId, effectId);
    //}

    //public List<BuffEffect<int>>GetIntBuff(string variableId, string effectId)
    //{
    //    return intManager.GetBuff (variableId, effectId);
    //}

    //public void ReSetIntBuff(string variableId, string effectId)
    //{
    //    intManager .ReSetBuff (variableId, effectId);
    //}

    //#endregion

    //#region Float Variables
    //public void AddFloatVariable(string id, float initialValue = 0)
    //{
    //    floatManager.RegisterVariable(id, initialValue);
    //}

    //public void RemoveFloatVariable(string id)
    //{
    //    floatManager.UnregisterVariable(id);
    //}

    //public float GetFloatValue(string id)
    //{
    //    return floatManager.GetFinalValue(id);
    //}

    //public void SetFloatValue(string id, float newValue)
    //{
    //    floatManager.UpdateVariableValue(id, newValue);
    //}

    //public void ApplyFloatBuff(string variableId, string effectId, float amount = 0, float duration = 0,
    //                          BuffOperationType opType = BuffOperationType.Additive,
    //                          EffectCurveType curveType = EffectCurveType.Instant,
    //                          StackType stackType = StackType.Basic,
    //                          Action<BuffEffect<float>> onComplete = null)
    //{
    //    var effect = PoolMgr.Instance.GetObj<BuffEffect<float>>();
    //    effect.Init(effectId, amount, duration, Time.time, opType, curveType, stackType, onComplete);
    //    //var effect = new BuffEffect<float>
    //    //{
    //    //    Id = effectId,
    //    //    Amount = amount,
    //    //    Duration = duration,
    //    //    StartTime = Time.time,
    //    //    OperationType = opType,
    //    //    CurveType = curveType,
    //    //    StackType = stackType,
    //    //    OnComplete = onComplete
    //    //};

    //    floatManager.ApplyBuff(variableId, effect);
    //}

    //public void RemoveFloatBuff(string variableId, string effectId)
    //{
    //    floatManager.RemoveBuff(variableId, effectId);
    //}
    //public List<BuffEffect<float>> GetFloatBuff(string variableId, string effectId)
    //{
    //    return floatManager.GetBuff(variableId, effectId);
    //}
    ///// <summary>
    ///// 锟斤拷锟斤拷锟斤拷锟叫碉拷锟斤拷锟斤拷float锟斤拷锟斤拷同锟斤拷锟斤拷值效锟斤拷锟斤拷锟斤拷
    ///// </summary>
    ///// <param name="variableId"></param>
    ///// <param name="effectId"></param>
    //public void ReSetFloatBuff(string variableId, string effectId)
    //{
    //    floatManager.ReSetBuff(variableId, effectId);
    //}

    //#endregion

    //#region Vector2 Variables
    //public void AddVector2Variable(string id, Vector2 initialValue = default(Vector2))
    //{
    //    vector2Manager.RegisterVariable(id, initialValue);
    //}

    //public void RemoveVector2Variable(string id)
    //{
    //    vector2Manager.UnregisterVariable(id);
    //}

    //public Vector2 GetVector2Value(string id)
    //{
    //    return vector2Manager.GetFinalValue(id);
    //}

    //public void SetVector2Value(string id, Vector2 newValue)
    //{
    //    vector2Manager.UpdateVariableValue(id, newValue);
    //}

    //public void ApplyVector2Buff(string variableId, string effectId, Vector2 amount, float duration,
    //                            BuffOperationType opType = BuffOperationType.Additive,
    //                            EffectCurveType curveType = EffectCurveType.Instant,
    //                            StackType stackType = StackType.Basic,
    //                            Action<BuffEffect<Vector2>> onComplete = null)
    //{
    //    var effect = PoolMgr.Instance.GetObj<BuffEffect<Vector2>>();
    //    effect.Init(effectId, amount, duration, Time.time, opType, curveType, stackType, onComplete);
    //    //var effect = new BuffEffect<Vector2>
    //    //{
    //    //    Id = effectId,
    //    //    Amount = amount,
    //    //    Duration = duration,
    //    //    StartTime = Time.time,
    //    //    OperationType = opType,
    //    //    CurveType = curveType,
    //    //    StackType = stackType,
    //    //    OnComplete = onComplete
    //    //};

    //    vector2Manager.ApplyBuff(variableId, effect);
    //}

    //public void RemoveVector2Buff(string variableId, string effectId)
    //{
    //    vector2Manager.RemoveBuff(variableId, effectId);
    //}

    //public List<BuffEffect<Vector2>> GetVector2Buff(string variableId, string effectId)
    //{
    //    return vector2Manager.GetBuff(variableId, effectId);
    //}
    ///// <summary>
    ///// 锟斤拷锟斤拷锟斤拷锟叫碉拷锟斤拷锟斤拷Vector2同锟斤拷锟斤拷值效锟斤拷锟斤拷锟斤拷
    ///// </summary>
    ///// <param name="variableId"></param>
    ///// <param name="effectId"></param>
    //public void ReSetVector2Buff(string variableId, string effectId)
    //{
    //    vector2Manager.ReSetBuff(variableId, effectId);
    //}
    //#endregion

    //#region Vector3 Variables
    //public void AddVector3Variable(string id, Vector3 initialValue = default(Vector3))
    //{
    //    vector3Manager.RegisterVariable(id, initialValue);
    //}

    //public void RemoveVector3Variable(string id)
    //{
    //    vector3Manager.UnregisterVariable(id);
    //}

    //public Vector3 GetVector3Value(string id)
    //{
    //    return vector3Manager.GetFinalValue(id);
    //}

    //public void SetVector3Value(string id, Vector3 newValue)
    //{
    //    vector3Manager.UpdateVariableValue(id, newValue);
    //}

    //public void ApplyVector3Buff(string variableId, string effectId, Vector3 amount, float duration,
    //                            BuffOperationType opType = BuffOperationType.Additive,
    //                            EffectCurveType curveType = EffectCurveType.Instant,
    //                            StackType stackType = StackType.Basic,
    //                            Action<BuffEffect<Vector3>> onComplete = null)
    //{
    //    var effect = PoolMgr.Instance.GetObj<BuffEffect<Vector3>>();
    //    effect.Init(effectId, amount, duration, Time.time, opType, curveType, stackType, onComplete);
    //    //var effect = new BuffEffect<Vector3>
    //    //{
    //    //    Id = effectId,
    //    //    Amount = amount,
    //    //    Duration = duration,
    //    //    StartTime = Time.time,
    //    //    OperationType = opType,
    //    //    CurveType = curveType,
    //    //    StackType = stackType,
    //    //    OnComplete = onComplete
    //    //};

    //    vector3Manager.ApplyBuff(variableId, effect);
    //}

    //public void RemoveVector3Buff(string variableId, string effectId)
    //{
    //    vector3Manager.RemoveBuff(variableId, effectId);
    //}

    //public List<BuffEffect<Vector3>> GetVector3Buff(string variableId, string effectId)
    //{
    //    return vector3Manager.GetBuff(variableId, effectId);
    //}

    ///// <summary>
    ///// 锟斤拷锟斤拷锟斤拷锟叫碉拷锟斤拷锟斤拷Vector3同锟斤拷锟斤拷值效锟斤拷锟斤拷锟斤拷
    ///// </summary>
    ///// <param name="variableId"></param>
    ///// <param name="effectId"></param>
    //public void ReSetVector3Buff(string variableId, string effectId)
    //{
    //    vector3Manager.ReSetBuff(variableId, effectId);
    //}
    //#endregion
}