using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GraphGenerator
{
    /// <summary>
    /// Генерирует граф комнат на основе настроек.
    /// Каждая вершина связывается минимум одним ребром (MST),
    /// а затем — ещё neighborCount ребрами к ближайшим соседям.
    /// </summary>
    public static RoomGraph Generate(DungeonSettings settings)
    {
        // 1) Случайные узлы (центры комнат)
        List<Vector2Int> nodes = new List<Vector2Int>();
        var rnd = new System.Random(settings.seed);
        for (int i = 0; i < settings.roomCount; i++)
        {
            int x = rnd.Next(1, settings.mapWidth - 1);
            int y = rnd.Next(1, settings.mapHeight - 1);
            nodes.Add(new Vector2Int(x, y));
        }
        Debug.Log($"GraphGenerator: сгенерировано узлов = {nodes.Count}");

        // 2) Построение MST (гарантия связности)
        var edges = new HashSet<Edge>();
        var used = new HashSet<int> { 0 };
        var unused = new HashSet<int>(Enumerable.Range(1, nodes.Count - 1));

        while (unused.Count > 0)
        {
            float bestDist = float.MaxValue;
            int bestU = -1, bestV = -1;

            foreach (int u in used)
                foreach (int v in unused)
                {
                    Vector2Int diff = nodes[u] - nodes[v];
                    float d = diff.x * diff.x + diff.y * diff.y;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestU = u;
                        bestV = v;
                    }
                }

            edges.Add(new Edge(bestU, bestV));
            used.Add(bestV);
            unused.Remove(bestV);
        }

        // 3) Для каждого узла добавляем neighborCount ближайших соседей
        for (int i = 0; i < nodes.Count; i++)
        {
            // Сортируем остальные узлы по квадрату расстояния
            var nearest = Enumerable.Range(0, nodes.Count)
                .Where(j => j != i)
                .OrderBy(j => {
                    Vector2Int diff = nodes[i] - nodes[j];
                    return diff.x * diff.x + diff.y * diff.y;
                })
                .Take(settings.neighborCount);

            foreach (int j in nearest)
                edges.Add(new Edge(i, j));
        }

        // 4) Возвращаем граф
        return new RoomGraph(nodes, edges.ToList());
    }
}
