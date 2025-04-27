using System.Collections.Generic;
using UnityEngine;

public static class RoomPlacer
{
    /// <summary>
    /// Размещает прямоугольные комнаты на сетке на основе графа.
    /// Гарантирует отсутствие пересечений — если новая комната пересекается,
    /// пытается сместить её по спирали вокруг узла.
    /// </summary>
    public static RoomLayout Place(RoomGraph graph, DungeonSettings settings)
    {
        var rooms = new List<RectInt>();
        var rnd = new System.Random(settings.seed);

        foreach (var node in graph.Nodes)
        {
            // Размер комнаты
            int w = rnd.Next(settings.roomMinSize, settings.roomMaxSize + 1);
            int h = rnd.Next(settings.roomMinSize, settings.roomMaxSize + 1);

            // Центр узла
            int cx = node.x;
            int cy = node.y;

            // Попытка разместить комнату по спирали
            RectInt room = TryPlaceRoom(cx, cy, w, h, rooms, settings);

            rooms.Add(room);
        }

        return new RoomLayout(graph, rooms,settings);
    }

    private static RectInt TryPlaceRoom(int cx, int cy, int w, int h, List<RectInt> existing, DungeonSettings settings)
    {
        // Начнём с по-центру узла
        int x = cx - w / 2;
        int y = cy - h / 2;
        RectInt candidate = new RectInt(x, y, w, h);

        // Проверяем и смещаем по спирали, пока не найдём свободное место
        int attempt = 0;
        int maxAttempts = settings.maxRoomPlacementAttempts;
        while (attempt < maxAttempts)
        {
            bool overlaps = false;
            foreach (var other in existing)
            {
                if (candidate.Overlaps(other))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
                return candidate;

            // Спираль: чередуем смещение по x/y и направление
            int dx = (attempt % 2 == 0 ? (attempt / 2 + 1) : -(attempt / 2 + 1));
            int dy = (attempt % 4 < 2 ? 0 : (attempt / 4 + 1) * (attempt % 8 < 4 ? 1 : -1));

            candidate.x = cx - w / 2 + dx;
            candidate.y = cy - h / 2 + dy;
            attempt++;
        }

        // Если не получилось разместить — возвращаем комнату на узле хотя бы ровно по центру
        return new RectInt(cx - w / 2, cy - h / 2, w, h);
    }
}
