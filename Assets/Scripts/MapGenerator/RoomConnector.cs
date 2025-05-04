using System.Collections.Generic;
using UnityEngine;

public static class CorridorConnector
{
    public static List<RectInt> Connect(RoomLayout layout)
    {
        var corridors = new List<RectInt>();
        int mapW = layout.Settings.mapWidth;
        int mapH = layout.Settings.mapHeight;

        foreach (var edge in layout.Graph.Edges)
        {
            Vector2 cA = layout.Rooms[edge.a].center;
            Vector2 cB = layout.Rooms[edge.b].center;
            Vector2Int a = new Vector2Int(Mathf.RoundToInt(cA.x), Mathf.RoundToInt(cA.y));
            Vector2Int b = new Vector2Int(Mathf.RoundToInt(cB.x), Mathf.RoundToInt(cB.y));

            bool horizFirst = Random.value > 0.5f;

            if (horizFirst)
            {
                // Горизонтальный сегмент
                int x0 = Mathf.Min(a.x, b.x), x1 = Mathf.Max(a.x, b.x);
                RectInt h = new RectInt(x0, a.y - 1, x1 - x0 + 1, 3); // высота = 3
                ClampRect(ref h, mapW, mapH);
                corridors.Add(h);

                // Вертикальный сегмент
                int y0 = Mathf.Min(a.y, b.y), y1 = Mathf.Max(a.y, b.y);
                RectInt v = new RectInt(b.x - 1, y0, 3, y1 - y0 + 1); // ширина = 3
                ClampRect(ref v, mapW, mapH);
                corridors.Add(v);
            }
            else
            {
                // Вертикальный сегмент
                int y0 = Mathf.Min(a.y, b.y), y1 = Mathf.Max(a.y, b.y);
                RectInt v = new RectInt(a.x - 1, y0, 3, y1 - y0 + 1); // ширина = 3
                ClampRect(ref v, mapW, mapH);
                corridors.Add(v);

                // Горизонтальный сегмент
                int x0 = Mathf.Min(a.x, b.x), x1 = Mathf.Max(a.x, b.x);
                RectInt h = new RectInt(x0, b.y - 1, x1 - x0 + 1, 3); // высота = 3
                ClampRect(ref h, mapW, mapH);
                corridors.Add(h);
            }
        }

        return corridors;
    }

    private static void ClampRect(ref RectInt r, int mapW, int mapH)
    {
        if (r.xMin < 0) r.xMin = 0;
        if (r.yMin < 0) r.yMin = 0;
        if (r.xMax > mapW) r.xMax = mapW;
        if (r.yMax > mapH) r.yMax = mapH;
    }
}
