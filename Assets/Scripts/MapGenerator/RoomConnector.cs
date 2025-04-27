using System.Collections.Generic;
using UnityEngine;

public static class CorridorConnector
{
    /// <summary>
    /// Соединяет комнаты коридорами на основе графа и их прямоугольного расположения.
    /// Для каждого ребра графа прокладывает L-образный коридор:
    /// сначала по X, затем по Y (или наоборот), прямыми проходами в карте.
    /// </summary>
    public static List<RectInt> Connect(RoomLayout layout)
    {
        var corridors = new List<RectInt>();
        var nodes = layout.Graph.Nodes;
        var edges = layout.Graph.Edges;

        foreach (var edge in edges)
        {
            Vector2 centerA = layout.Rooms[edge.a].center;
            Vector2 centerB = layout.Rooms[edge.b].center;
            Vector2Int aCenter = new Vector2Int(
                Mathf.RoundToInt(centerA.x),
                Mathf.RoundToInt(centerA.y)
            );
            Vector2Int bCenter = new Vector2Int(
                Mathf.RoundToInt(centerB.x),
                Mathf.RoundToInt(centerB.y)
            );
            // Решаем произвольно: 50% случаев сначала горизонтально, 50% — вертикально
            bool horizontalFirst = (Random.value > 0.5f);

            if (horizontalFirst)
            {
                // Горизонтальный сегмент
                int xMin = Mathf.Min(aCenter.x, bCenter.x);
                int xMax = Mathf.Max(aCenter.x, bCenter.x);
                corridors.Add(new RectInt(xMin, aCenter.y, xMax - xMin + 1, 1));

                // Вертикальный сегмент
                int yMin = Mathf.Min(aCenter.y, bCenter.y);
                int yMax = Mathf.Max(aCenter.y, bCenter.y);
                corridors.Add(new RectInt(bCenter.x, yMin, 1, yMax - yMin + 1));
            }
            else
            {
                // Вертикальный сегмент
                int yMin = Mathf.Min(aCenter.y, bCenter.y);
                int yMax = Mathf.Max(aCenter.y, bCenter.y);
                corridors.Add(new RectInt(aCenter.x, yMin, 1, yMax - yMin + 1));

                // Горизонтальный сегмент
                int xMin = Mathf.Min(aCenter.x, bCenter.x);
                int xMax = Mathf.Max(aCenter.x, bCenter.x);
                corridors.Add(new RectInt(xMin, bCenter.y, xMax - xMin + 1, 1));
            }
        }

        return corridors;
    }
}
