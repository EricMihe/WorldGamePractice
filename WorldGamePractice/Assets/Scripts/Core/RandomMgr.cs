using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏专用随机数生成器，支持种子、权重选择、洗牌等。
/// </summary>
public class GameRandom
{
    private System.Random _random;

    // 构造函数
    public GameRandom() : this(Environment.TickCount) { }
    public GameRandom(int seed)
    {
        _random = new System.Random(seed);
    }

    // 获取当前种子（仅用于调试，无法还原状态）
    public int Seed { get; private set; }

    // ---- 基础随机方法 ----

    /// <summary> [0.0, 1.0) </summary>
    public float Range01() => (float)_random.NextDouble();

    /// <summary> [min, max) 浮点 </summary>
    public float Range(float min, float max) => min + (float)_random.NextDouble() * (max - min);

    /// <summary> [min, max] 整数（包含 max）</summary>
    public int Range(int min, int max) => _random.Next(min, max + 1);

    /// <summary> true/false 概率 p </summary>
    public bool Chance(float probability) => Range01() < probability;

    // ---- 高级功能 ----

    /// <summary>
    /// 按权重随机选择索引（权重越大越可能被选中）
    /// weights: 权重数组，必须非空且非负
    /// </summary>
    public int WeightedIndex(IList<float> weights)
    {
        if (weights == null || weights.Count == 0)
            throw new ArgumentException("Weights list is empty or null.");

        float total = 0f;
        foreach (float w in weights)
        {
            if (w < 0) throw new ArgumentException("Weight cannot be negative.");
            total += w;
        }

        if (total <= 0f) return 0; // fallback

        float rand = Range(0f, total);
        float sum = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            sum += weights[i];
            if (rand <= sum)
                return i;
        }
        return weights.Count - 1; // 安全兜底
    }

    /// <summary>
    /// 按权重随机选择元素
    /// </summary>
    public T WeightedChoice<T>(IList<T> items, IList<float> weights)
    {
        int index = WeightedIndex(weights);
        return items[index];
    }

    /// <summary>
    /// Fisher-Yates 洗牌算法（原地打乱）
    /// </summary>
    public void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); // C# 7.0+ 元组交换
        }
    }

    /// <summary>
    /// 从列表中随机选择一个元素
    /// </summary>
    public T Choice<T>(IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List is empty or null.");
        return list[_random.Next(list.Count)];
    }

    /// <summary>
    /// 从列表中随机选择 N 个不重复元素（无放回）
    /// </summary>
    public List<T> Sample<T>(IList<T> list, int count)
    {
        if (count > list.Count) count = list.Count;
        var copy = new List<T>(list);
        Shuffle(copy);
        return copy.GetRange(0, count);
    }
}

public class RandomMgr :SingletonAutoMono<RandomMgr>
{

    [Header("Random Settings")]
    [SerializeField] private int defaultSeed = -1; // -1 表示使用时间种子
    [SerializeField] private bool useFixedSeed = false;

    private GameRandom _globalRandom;

    public static GameRandom Global => Instance._globalRandom;

    private void Awake()
    {

        int seed = useFixedSeed ? defaultSeed : (defaultSeed == -1 ? System.Environment.TickCount : defaultSeed);
        _globalRandom = new GameRandom(seed);

        Debug.Log($"[RandomManager] Initialized with seed: {seed}");
    }
}



