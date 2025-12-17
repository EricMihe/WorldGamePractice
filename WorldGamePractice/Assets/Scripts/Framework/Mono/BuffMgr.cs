using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

    // 增值算法类型
    public enum BuffOperationType
    {
    /// <summary>
    /// 加法增值
    /// </summary>
    Additive,
    /// <summary>
    /// 乘法增值
    /// </summary>
    Multiplicative
}

    // 变化效果类型
    public enum EffectCurveType
    {
    /// <summary>
    /// 瞬间变化后瞬间消失
    /// </summary>
    Instant,
    /// <summary>
    /// 线性衰减
    /// </summary>
    LinearDecay,
    /// <summary>
    /// 线性增长后瞬间消失
    /// </summary>
    LinearGrowth,
    /// <summary>
    /// 缓入缓出
    /// </summary>
    EaseInEaseOut
}

    // 效果叠加类型
    public enum StackType
    {
    /// <summary>
    /// 同类型上是叠加
    /// </summary>
    Basic,
    /// <summary>
    /// 同类型上是覆盖
    /// </summary>
    Override,
    /// <summary>
    /// 同类型上是取最大增值
    /// </summary>
    MaxValue,
    /// <summary>
    /// 同类型上是取最小增值
    /// </summary>
    MinValue,
    ///  <summary>
    /// 同类型上是取最长时间
    /// </summary>
    MaxDuration,
    ///  <summary>
    /// 同类型上是取最短时间
    /// </summary>
    MinDuration
}

// 基础值接口
public interface IBaseValue<T>: IPoolObject
{
    T Value { get; set; }
    T GetOriginalValue();
    void SetOriginalValue(T value);
    void Init(T value);
}

    // 具体值类型实现
    public class IntValue : IBaseValue<int>
{
        private int originalValue;
    public int Value { get => originalValue; set => originalValue = value; }

    public int GetOriginalValue() => originalValue;

    public void Init(int value)
    {
        originalValue = value;
    }

    public void ResetInfo()
    {
        originalValue = 0;
    }

    public void SetOriginalValue(int value) => originalValue = value;
    }

    public class FloatValue : IBaseValue<float>
    {
        private float originalValue;

    public float Value { get => originalValue; set => originalValue = value; }

    public float GetOriginalValue() => originalValue;

    public void Init(float value)
    {
        originalValue = value;
    }

    public void ResetInfo()
    {
        originalValue = 0f;
    }

    public void SetOriginalValue(float value) => originalValue = value;
    }

    public class Vector2Value : IBaseValue<Vector2>
    {
        private Vector2 originalValue;
        public Vector2 Value { get => originalValue; set => originalValue = value; }

    public Vector2 GetOriginalValue() => originalValue;

    public void Init(Vector2 value)
    {
        originalValue = value;
    }

    public void ResetInfo()
    {
        originalValue = Vector2.zero;
    }

    public void SetOriginalValue(Vector2 value) => originalValue = value;
    }

public class Vector3Value : IBaseValue<Vector3>
{
    private Vector3 originalValue;
    public Vector3 Value { get => originalValue; set => originalValue = value; }

    public Vector3 GetOriginalValue() => originalValue;

    public void Init(Vector3 value)
    {
        originalValue = value;
    }

    public void ResetInfo()
    {
        originalValue = Vector3.zero;
    }

    public void SetOriginalValue(Vector3 value) => originalValue = value;
}

// 具体效果
public class BuffEffect<T> : IPoolObject
{
    public string Id { get; set; }
    public T Amount { get; set; }
    public float Duration { get; set; }
    public float StartTime { get; set; }
    public BuffOperationType OperationType { get; set; }
    public EffectCurveType CurveType { get; set; }
    public StackType StackType { get; set; }
    public Action<BuffEffect<T>> OnComplete { get; set; }

    public float GetProgress()
    {
        return Mathf.Clamp01((Time.time - StartTime) / Duration);
    }

    public T GetCurrentAmount()
    {
        float progress = GetProgress();

        switch (CurveType)
        {
            case EffectCurveType.Instant:
                return progress < 1f ? Amount : default(T);

            case EffectCurveType.LinearDecay:
                float decayFactor = 1f - progress;
                return MultiplyByScalar(Amount, decayFactor);

            case EffectCurveType.LinearGrowth:
                if (progress < 1f)
                    return MultiplyByScalar(Amount, progress);
                else
                    return default(T);

            case EffectCurveType.EaseInEaseOut:
                float easeProgress = Mathf.Sin(progress * Mathf.PI * 0.5f); // Ease-in
                if (progress > 0.5f)
                    easeProgress = Mathf.Sin((1f - progress) * Mathf.PI * 0.5f); // Ease-out
                return MultiplyByScalar(Amount, easeProgress);

            default:
                return Amount;
        }
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
        //return (T)(object)((int)(object)value * scalar);               
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
        Duration = 0;
        StartTime = 0;
        OperationType = BuffOperationType.Additive;
        CurveType = EffectCurveType.Instant;
        StackType = StackType.Basic;
        OnComplete = null;
    }
    public void Init(string effectId, T amount, float duration, float starttime,
                                BuffOperationType opType = BuffOperationType.Additive,
                                EffectCurveType curveType = EffectCurveType.Instant,
                                StackType stackType = StackType.Basic,
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
    }
}

// 变量管理器
public class VariableManager<T>
{
    private Dictionary<string, IBaseValue<T>> variables = new Dictionary<string, IBaseValue<T>>();
    private Dictionary<string, List<BuffEffect<T>>> effects = new Dictionary<string, List<BuffEffect<T>>>();
    private HashSet<string> expiredEffects = new HashSet<string>();
    public void RegisterVariable(string id, T initialValue)
    {
        if (!variables.ContainsKey(id))
        {

            if (typeof(T) == typeof(int))
            {
                IBaseValue<int> variable = PoolMgr.Instance.GetObj<IntValue>();
                variable.Init((int)(object)initialValue);
                variables[id] = (IBaseValue<T>)variable;
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(float))
            {
                IBaseValue<float> variable = PoolMgr.Instance.GetObj<FloatValue>();
                variable.Init((float)(object)initialValue);
                variables[id] = (IBaseValue<T>)variable;
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(Vector2))
            {
                IBaseValue<Vector2> variable = PoolMgr.Instance.GetObj<Vector2Value>();
                variable.Init((Vector2)(object)initialValue);
                variables[id] = (IBaseValue<T>)variable;
                effects[id] = new List<BuffEffect<T>>();
            }
            if (typeof(T) == typeof(Vector3))
            {
                IBaseValue<Vector3> variable = PoolMgr.Instance.GetObj<Vector3Value>();
                variable.Init((Vector3)(object)initialValue);
                variables[id] = (IBaseValue<T>)variable;
                effects[id] = new List<BuffEffect<T>>();
            }


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

    public void UnregisterVariable(string id)
    {
        if (variables[id]!=null)
        {
            PoolMgr.Instance.PushObj(variables[id],"BuffValue");
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

    public void ApplyBuff(string variableId, BuffEffect<T> effect)
    {
        if (!variables.ContainsKey(variableId)) return;
        if (!effects.ContainsKey(variableId)) return;
        var varEffects = effects[variableId];

        // 根据堆叠类型处理效果
        switch (effect.StackType)
        {
            case StackType.Override:
                var removeEfffectsOverride = varEffects.Where(e => e.StackType == StackType.Override).ToList();
                if(removeEfffectsOverride.Count > 0)
                {
                    foreach (var eff in removeEfffectsOverride)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == StackType.Override);
                }     
                break;

            case StackType.MaxValue:
                // 检查是否当前效果值更大
                var maxEffect = varEffects.Find(e => e.StackType == StackType.MaxValue);
                if (maxEffect != null && CompareValues(effect.Amount, maxEffect.Amount) <= 0)
                    return;
                // 不应用新效果
                var removeEfffectsMaxValue = varEffects.Where(e => e.StackType == StackType.MaxValue).ToList();
                if (removeEfffectsMaxValue.Count > 0)
                {
                    foreach (var eff in removeEfffectsMaxValue)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == StackType.MaxValue);
                }
                break;

            case StackType.MinValue:
                // 检查是否当前效果值更小
                var minEffect = varEffects.Find(e => e.StackType == StackType.MinValue);
                if (minEffect != null && CompareValues(effect.Amount, minEffect.Amount) >= 0)
                    return; // 不应用新效果
                var removeEfffectsMinValue = varEffects.Where(e => e.StackType == StackType.MinValue).ToList();
                if (removeEfffectsMinValue.Count > 0)
                {
                    foreach (var eff in removeEfffectsMinValue)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == StackType.MinValue);
                }   
                break;

            case StackType.MaxDuration:
                // 检查是否当前效果持续时间更长
                var maxDurEffect = varEffects.Find(e => e.StackType == StackType.MaxDuration);
                if (maxDurEffect != null && effect.Duration <= maxDurEffect.Duration)
                    return; // 不应用新效果
                var removeEfffectsMaxDuration = varEffects.Where(e => e.StackType == StackType.MaxDuration).ToList();
                if (removeEfffectsMaxDuration.Count > 0)
                {
                    foreach (var eff in removeEfffectsMaxDuration)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == StackType.MaxDuration);
                }

                break;

            case StackType.MinDuration:
                // 检查是否当前效果持续时间更短
                var minDurEffect = varEffects.Find(e => e.StackType == StackType.MinDuration);
                if (minDurEffect != null && effect.Duration >= minDurEffect.Duration)
                    return; // 不应用新效果
                var removeEfffectsMinDuration = varEffects.Where(e => e.StackType == StackType.MinDuration).ToList();
                if (removeEfffectsMinDuration.Count > 0)
                {
                    foreach (var eff in removeEfffectsMinDuration)
                    {
                        PoolMgr.Instance.PushObj(eff);
                    }
                    varEffects.RemoveAll(e => e.StackType == StackType.MinDuration);
               }
                    
                break;
        }

        varEffects.Add(effect);
    }

    public void RemoveBuff(string variableId, string effectId)
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

    public List<BuffEffect<T>> GetBuff(string variableId, string effectId)
    {
        if (!effects.ContainsKey(variableId)) return null; 
        var varEffects = effects[variableId];
        if (varEffects.Count == 0) return null;
        return varEffects.Where(e => e.Id == effectId).ToList();
    }

    public void ReSetBuff(string variableId, string effectId)
    {
        var resetbuff=GetBuff (variableId, effectId);
        if(resetbuff .Count == 0) return;
        foreach (var eff in resetbuff)
        {
            eff.StartTime=Time.time;
        }
    }

    public T GetFinalValue(string variableId)
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

            T currentAmount = effect.GetCurrentAmount();

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

        // 清理过期效果
        foreach (var expiredKey in expiredEffects)
        {
            var parts = expiredKey.Split(':');
            if (parts.Length == 2)
            {
                var expVarId = parts[0];
                var expEffectId = parts[1];
                var varEffectsList = effects[expVarId];
                var expiredEffect = varEffectsList.Find(e => e.Id == expEffectId);
                if (expiredEffect != null)
                {
                    PoolMgr.Instance.PushObj(expiredEffect);
                    expiredEffect.OnComplete?.Invoke(expiredEffect);
                    varEffectsList.Remove(expiredEffect);
                    expiredEffect.OnComplete = null;
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

    public void UpdateVariableValue(string variableId, T newValue)
    {
        if (variables.ContainsKey(variableId))
        {
            variables[variableId].SetOriginalValue(newValue);
        }
    }
}

// 系统管理器
public class BuffMgr : SingletonAutoMono<BuffMgr>
{


    private VariableManager<int> intManager = new VariableManager<int>();
    private VariableManager<float> floatManager = new VariableManager<float>();
    private VariableManager<Vector2> vector2Manager = new VariableManager<Vector2>();
    private VariableManager<Vector3> vector3Manager = new VariableManager<Vector3>();

    public void AddValue<T>(string id, T initialValue=default(T))
    {
        if (typeof(T) == typeof(int))
        {
            int intValue = Convert.ToInt32(initialValue);
            intManager.RegisterVariable(id, intValue);
        }
        else if (typeof(T) == typeof(float))
        {
            float intValue = Convert.ToSingle(initialValue);
            floatManager.RegisterVariable(id, intValue);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.RegisterVariable(id, (Vector2)(object)initialValue);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.RegisterVariable(id, (Vector3)(object)initialValue);
        }
        else Debug.Log("无效类型");
    }

    public void RemoveVariable<T>(string id)
    {
        if (typeof(T) == typeof(int))
        {
            intManager.UnregisterVariable(id);
        }
        else if (typeof(T) == typeof(float))
        {
            floatManager.UnregisterVariable(id);
        }
        else if (typeof(T) == typeof(Vector2))
        {
            vector2Manager.UnregisterVariable(id);
        }

        else if (typeof(T) == typeof(Vector3))
        {
            vector3Manager.UnregisterVariable(id);
        }
        else Debug.Log("无效类型");
    }

    public T GetValue<T>(string id)
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
            Debug.Log("无效类型");
            return default(T);
        }
    }
    public void SetValue<T>(string id, T newValue)
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
        else Debug.Log("无效类型");
        
    }
    /// <summary>
    /// 施加增值效果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="amount">最大增值</param>
    /// <param name="duration">增值时间</param>
    /// <param name="variableId">对哪个变量增值</param>
    /// <param name="curveType">变化类型</param>
    /// <param name="stackType">叠加类型</param>
    /// <param name="opType">增值算法</param>
    /// <param name="effectId">效果ID</param>
    /// <param name="onComplete">完成回调</param>
    public void ApplyBuff<T>(string variableId, T amount, float duration,  
                            EffectCurveType curveType = EffectCurveType.Instant,
                            StackType stackType = StackType.Basic, 
                            BuffOperationType opType = BuffOperationType.Additive,
                            string effectId = "", 
                            Action<BuffEffect<T>> onComplete = null)
    {
        var effect = PoolMgr.Instance.GetObj<BuffEffect<T>>();
        effect.Init(effectId, amount, duration, Time.time, opType, curveType, stackType, onComplete); 
        if (effect.GetType()== typeof(BuffEffect<int>))
        {
            intManager.ApplyBuff(variableId, (BuffEffect<int>)Convert.ChangeType(effect, typeof(BuffEffect<int>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<float>))
        {
            floatManager.ApplyBuff(variableId, (BuffEffect<float>)Convert.ChangeType(effect, typeof(BuffEffect<float>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector2>))
        {
            vector2Manager.ApplyBuff(variableId, (BuffEffect<Vector2>)Convert.ChangeType(effect, typeof(BuffEffect<Vector2>)));
        }
        else if (effect.GetType() == typeof(BuffEffect<Vector3>))
        {
            vector3Manager.ApplyBuff(variableId, (BuffEffect<Vector3>)Convert.ChangeType(effect, typeof(BuffEffect<Vector3>)));
        }
        else Debug.Log("无效类型");
    }

    public void RemoveBuff<T>(string variableId, string effectId)
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
        else Debug.Log("无效类型");
    }
    public List<BuffEffect<T>> GetBuff<T>(string variableId, string effectId)
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
            Debug.Log("无效类型");
            return null;
        }

    }


    /// <summary>
    /// 将运行中的所有同类型同名增值效果重置
    /// </summary>
    /// <param name="variableId"></param>
    /// <param name="effectId"></param>
    public void ReSetBuff<T>(string variableId, string effectId)
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
        else Debug.Log("无效类型");
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
    ///// 将运行中的所有float类型同名增值效果重置
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
    ///// 将运行中的所有Vector2同名增值效果重置
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
    ///// 将运行中的所有Vector3同名增值效果重置
    ///// </summary>
    ///// <param name="variableId"></param>
    ///// <param name="effectId"></param>
    //public void ReSetVector3Buff(string variableId, string effectId)
    //{
    //    vector3Manager.ReSetBuff(variableId, effectId);
    //}
    //#endregion
}