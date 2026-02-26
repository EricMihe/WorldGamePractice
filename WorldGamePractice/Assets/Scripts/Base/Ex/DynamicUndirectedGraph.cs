using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 表示一个动态无向图，支持任意整数标识的顶点，
/// 每条边拥有唯一的递增整型 ID。
/// 适用于顶点数量未知或频繁变化、边密集的场景。
/// </summary>
public class DynamicUndirectedGraph
{
    // 邻接表：每个顶点映射到其邻居字典（邻居 -> 边ID）
    private readonly Dictionary<int, Dictionary<int, int>> _adjacency = new();

    // 反向映射：边ID -> (u, v)，用于通过ID快速定位边（u <= v）
    private readonly Dictionary<int, (int u, int v)> _edgeIdToVertices = new();

    // 全局边ID计数器，每次新增边时递增
    private int _nextEdgeId = 0;

    #region Edge Management

    /// <summary>
    /// 添加一条无向边。如果边已存在，则返回其已有ID；否则分配新ID。
    /// </summary>
    /// <param name="u">边的一个端点</param>
    /// <param name="v">边的另一个端点</param>
    /// <returns>该边的唯一ID</returns>
    /// <exception cref="ArgumentException">当 u == v（自环）时抛出（可根据需求移除）</exception>
    public int AddEdge(int u, int v)
    {
        if (u == v)
            return -1;

        // 标准化顶点顺序：确保 u <= v，使 (u,v) 和 (v,u) 视为同一条边
        int a = Math.Min(u, v);
        int b = Math.Max(u, v);

        // 确保两个顶点在图中存在（懒初始化）
        EnsureVertexExists(a);
        EnsureVertexExists(b);

        var neighborsA = _adjacency[a];
        var neighborsB = _adjacency[b];

        // 若边已存在，直接返回已有ID
        if (neighborsA.TryGetValue(b, out int existingId))
        {
            return existingId;
        }

        // 分配新边ID
        int newEdgeId = _nextEdgeId++;

        // 在邻接表中双向记录
        neighborsA[b] = newEdgeId;
        neighborsB[a] = newEdgeId;

        // 记录反向映射，便于通过ID查找边
        _edgeIdToVertices[newEdgeId] = (a, b);

        return newEdgeId;
    }

    /// <summary>
    /// 通过两个顶点删除一条边。
    /// </summary>
    /// <param name="u">边的一个端点</param>
    /// <param name="v">边的另一个端点</param>
    /// <returns>若边存在并成功删除，返回 true；否则返回 false。</returns>
    public bool RemoveEdge(int u, int v)
    {
        int a = Math.Min(u, v);
        int b = Math.Max(u, v);

        if (!_adjacency.TryGetValue(a, out var neighborsA) ||
            !neighborsA.TryGetValue(b, out int edgeId))
        {
            return false; // 边不存在
        }

        // 从邻接表中移除双向连接
        neighborsA.Remove(b);
        _adjacency[b].Remove(a);

        // 从反向映射中移除
        _edgeIdToVertices.Remove(edgeId);

        return true;
    }
    public int GetEdge(int u, int v)
    {
        int a = Math.Min(u, v);
        int b = Math.Max(u, v);

        if (!_adjacency.TryGetValue(a, out var neighborsA) ||
            !neighborsA.TryGetValue(b, out int edgeId))
        {
            return -1; // 边不存在
        }

        return edgeId;
    }


    /// <summary>
    /// 通过边ID删除一条边。
    /// </summary>
    /// <param name="edgeId">要删除的边的ID</param>
    /// <returns>若ID存在并成功删除，返回 true；否则返回 false。</returns>
    public bool RemoveEdgeById(int edgeId)
    {
        if (!_edgeIdToVertices.TryGetValue(edgeId, out var vertices))
        {
            return false; // ID不存在
        }

        var (a, b) = vertices;

        // 从邻接表中移除双向连接
        _adjacency[a].Remove(b);
        _adjacency[b].Remove(a);

        // 从反向映射中移除
        _edgeIdToVertices.Remove(edgeId);

        return true;
    }

    #endregion

    #region Vertex Management

    /// <summary>
    /// 删除一个顶点所有关联的边。
    /// </summary>
    /// <param name="vertex">要删除的顶点</param>
    /// <returns>若顶点存在并被删除，返回 true；否则返回 false。</returns>
    public bool RemoveVertex(int vertex)
    {
        if (!_adjacency.TryGetValue(vertex, out var neighbors))
        {
            return false; // 顶点不存在
        }

        // 遍历所有邻居，逐个删除关联边
        foreach (int neighbor in new List<int>(neighbors.Keys))
        {
            // 从邻居的邻接表中移除本顶点
            _adjacency[neighbor].Remove(vertex);

            // 获取边ID并从反向映射中移除
            int edgeId = neighbors[neighbor];
            _edgeIdToVertices.Remove(edgeId);
        }

        // 最终移除顶点自身
        //_adjacency.Remove(vertex);

        return true;
    }

    /// <summary>
    /// 判断指定顶点是否存在于图中。
    /// </summary>
    /// <param name="vertex">要检查的顶点</param>
    /// <returns>若顶点存在，返回 true；否则返回 false。</returns>
    public bool ContainsVertex(int vertex)
    {
        return _adjacency.ContainsKey(vertex);
    }

    /// <summary>
    /// 判断指定顶点是否存在至少一条边（即是否非孤立）。
    /// </summary>
    /// <param name="vertex">要检查的顶点</param>
    /// <returns>若顶点存在且度数 > 0，返回 true；否则返回 false。</returns>
    public bool HasAnyEdge(int vertex)
    {
        return _adjacency.TryGetValue(vertex, out var neighbors) && neighbors.Count > 0;
    }

    /// <summary>
    /// 获取与指定顶点相邻的所有顶点（邻居）。
    /// </summary>
    /// <param name="vertex">目标顶点</param>
    /// <returns>邻居顶点列表（只读副本）；若顶点不存在，返回空列表。</returns>
    public IReadOnlyList<int> GetAdjacentVertices(int vertex)
    {
        if (_adjacency.TryGetValue(vertex, out var neighbors))
        {
            return new List<int>(neighbors.Keys);
        }
        return Array.Empty<int>();
    }

    /// <summary>
    /// 获取与指定顶点关联的所有边的ID。
    /// </summary>
    /// <param name="vertex">目标顶点</param>
    /// <returns>边ID列表（只读副本）；若顶点不存在或无边，返回空列表。</returns>
    public IReadOnlyList<int> GetIncidentEdgeIds(int vertex)
    {
        if (_adjacency.TryGetValue(vertex, out var neighbors))
        {
            return new List<int>(neighbors.Values);
        }
        return Array.Empty<int>();
    }

    /// <summary>
    /// 获取与指定顶点关联的所有边的详细信息（邻居 + 边ID）。
    /// </summary>
    /// <param name="vertex">目标顶点</param>
    /// <returns>边详情列表（只读副本）；若顶点不存在或无边，返回空列表。</returns>
    public IReadOnlyList<(int neighbor, int edgeId)> GetIncidentEdges(int vertex)
    {
        if (_adjacency.TryGetValue(vertex, out var neighbors))
        {
            var result = new List<(int, int)>(neighbors.Count);
            foreach (var kvp in neighbors)
            {
                result.Add((kvp.Key, kvp.Value));
            }
            return result;
        }
        return Array.Empty<(int, int)>();
    }

    #endregion

    #region Graph-Wide Operations

    /// <summary>
    /// 清空整个图，移除所有顶点和边。
    /// </summary>
    public void Clear()
    {
        _adjacency.Clear();
        _edgeIdToVertices.Clear();
        _nextEdgeId = 0;
    }

    /// <summary>
    /// 获取图中所有顶点的集合。
    /// </summary>
    /// <returns>顶点ID的只读集合。</returns>
    public IEnumerable<int> GetAllVertices()
    {
        return _adjacency.Keys;
    }

    /// <summary>
    /// 枚举图中所有边的信息。
    /// </summary>
    /// <returns>每条边以 (u, v, edgeId) 形式返回，其中 u &lt;= v。</returns>
    public IEnumerable<(int u, int v, int edgeId)> GetAllEdges()
    {
        foreach (var kvp in _edgeIdToVertices)
        {
            yield return (kvp.Value.u, kvp.Value.v, kvp.Key);
        }
    }

    /// <summary>
    /// 获取当前图中的顶点总数。
    /// </summary>
    public int VertexCount => _adjacency.Count;

    /// <summary>
    /// 获取当前图中的边总数。
    /// </summary>
    public int EdgeCount => _edgeIdToVertices.Count;

    #endregion

    #region Helper Methods

    /// <summary>
    /// 确保指定顶点存在于邻接表中（若不存在则创建空条目）。
    /// </summary>
    /// <param name="vertex">要确保存在的顶点</param>
    private void EnsureVertexExists(int vertex)
    {
        if (!_adjacency.ContainsKey(vertex))
        {
            _adjacency[vertex] = new Dictionary<int, int>();
        }
    }

    #endregion

    #region Debugging / Utility

    /// <summary>
    /// 打印图的当前结构（用于调试）。
    /// </summary>
    public void PrintGraph()
    {
        if (_adjacency.Count == 0)
        {
            Console.WriteLine("图为空");
            return;
        }

        foreach (var kvp in _adjacency.OrderBy(x => x.Key))
        {
            int vertex = kvp.Key;
            Console.Write($"Vertex {vertex}: ");
            foreach (var neighbor in kvp.Value.OrderBy(n => n.Key))
            {
                Console.Write($"({neighbor.Key}, id={neighbor.Value}) ");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"总顶点数: {VertexCount}, 总边数: {EdgeCount}");
    }

    #endregion
}