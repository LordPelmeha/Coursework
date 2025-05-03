using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RoomPlacer
{
    /// <summary>
    /// Размещает прямоугольные комнаты по графу:
    /// – пробуем поставить по спирали так, чтобы не пересекались;
    /// – как только нашли положение без пересечений, жёстко клэмпим
    ///   в границы карты и возвращаем.
    /// </summary>
    public static RoomLayout Place(RoomGraph graph, DungeonSettings settings)
    {
        var rnd = new System.Random(settings.seed);
        var rooms = new List<RectInt>();

        foreach (var node in graph.Nodes)
        {
            // случайный размер
            int w = rnd.Next(settings.roomMinSize, settings.roomMaxSize + 1);
            int h = rnd.Next(settings.roomMinSize, settings.roomMaxSize + 1);

            // пытаемся разместить и сразу клэмпим в границы
            var room = TryPlaceRoom(node.x, node.y, w, h, rooms, settings);
            rooms.Add(room);
        }

        // формируем итоговый Layout и сразу генерим коридоры
        var layout = new RoomLayout(graph, rooms, settings);
        var corridors = CorridorConnector.Connect(layout);
        layout.SetCorridors(corridors);
        return layout;
    }

    private static RectInt TryPlaceRoom(
        int cx, int cy,
        int w, int h,
        List<RectInt> existing,
        DungeonSettings settings)
    {
        // базовая позиция «по центру узла»
        int baseX = cx - w / 2;
        int baseY = cy - h / 2;
        var candidate = new RectInt(baseX, baseY, w, h);

        int maxAttempts = settings.maxRoomPlacementAttempts;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // если не пересекается — клэмпим и возвращаем
            bool overlaps = existing.Any(o => o.Overlaps(candidate));
            if (!overlaps)
            {
                int clampedX = Mathf.Clamp(candidate.x,
                    0, settings.mapWidth - w);
                int clampedY = Mathf.Clamp(candidate.y,
                    0, settings.mapHeight - h);
                return new RectInt(clampedX, clampedY, w, h);
            }

            // иначе спиральное смещение
            int dx = (attempt % 2 == 0
                ? (attempt / 2 + 1)
                : -(attempt / 2 + 1));
            int dy = (attempt % 4 < 2
                ? 0
                : (attempt / 4 + 1) * (attempt % 8 < 4 ? 1 : -1));

            candidate.x = baseX + dx;
            candidate.y = baseY + dy;
        }

        // по окончании попыток — всё равно клэмпим
        int finalX = Mathf.Clamp(candidate.x,
            0, settings.mapWidth - w);
        int finalY = Mathf.Clamp(candidate.y,
            0, settings.mapHeight - h);
        return new RectInt(finalX, finalY, w, h);
    }
}
