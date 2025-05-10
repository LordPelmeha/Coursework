using System;
using System.Collections.Generic;
using UnityEngine;

public static class DrunkardWalkConnector
{
    /// <summary>
    /// Для каждого ребра графа: "бродим" из центра комнаты A в направлении комнаты B,
    /// добавляя квадратные участки коридора толщиной corridorRadius.
    /// Длина пути — drunkardWalkLength шагов, с небольшим рандомизированным отклонением.
    /// </summary>
    public static List<RectInt> Connect(RoomLayout layout, DungeonSettings settings)
    {
        var corridors = new List<RectInt>();
        var edges = layout.Graph.Edges;
        var rnd = new System.Random(settings.seed + 123456);

        foreach (var edge in edges)
        {
            // старт и цель в клеточных координатах
            Vector2 startF = layout.Rooms[edge.a].center;
            Vector2 targetF = layout.Rooms[edge.b].center;
            Vector2 posF = startF;

            for (int step = 0; step < settings.drunkardWalkLength; step++)
            {
                // вектор от текущей позиции к цели
                Vector2 toTarget = (targetF - posF).normalized;

                // случайное отклонение на угол ±drunkardTurnAngle
                float angle = ((float)rnd.NextDouble() * 2f - 1f) * settings.drunkardTurnAngle;
                toTarget = Quaternion.Euler(0, 0, angle) * toTarget;

                // делаем шаг по x и y, округляя к ближайшей клетке
                posF.x += Mathf.Sign(toTarget.x);
                posF.y += Mathf.Sign(toTarget.y);

                // клэмпим в границы карты
                posF.x = Mathf.Clamp(posF.x, 0, settings.mapWidth - 1);
                posF.y = Mathf.Clamp(posF.y, 0, settings.mapHeight - 1);

                // добавляем квадрат коридора радиусом corridorRadius
                int cx = Mathf.RoundToInt(posF.x);
                int cy = Mathf.RoundToInt(posF.y);
                corridors.Add(new RectInt(
                    cx - settings.corridorRadius,
                    cy - settings.corridorRadius,
                    settings.corridorRadius * 2 + 2,
                    settings.corridorRadius * 2 + 2
                ));
            }
        }

        return corridors;
    }
}
