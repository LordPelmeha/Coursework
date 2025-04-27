// Assets/Scripts/MapGenerator/DungeonValidator.cs
using System.Collections.Generic;
using UnityEngine;

public static class DungeonValidator
{
    public static void Validate(RoomLayout layout)
    {
        int width = layout.Settings.mapWidth;
        int height = layout.Settings.mapHeight;

        bool[,] walkable = new bool[width, height];
        // ѕомечаем комнаты и коридоры как проходимые
        foreach (var room in layout.Rooms)
            for (int x = room.xMin; x < room.xMax; x++)
                for (int y = room.yMin; y < room.yMax; y++)
                    walkable[x, y] = true;

        foreach (var c in layout.Corridors)
            for (int x = c.xMin; x < c.xMax; x++)
                for (int y = c.yMin; y < c.yMax; y++)
                    walkable[x, y] = true;

        // BFS от центра стартовой комнаты
        Vector2 startF = layout.Rooms[0].center;
        Vector2Int start = new Vector2Int(
            Mathf.RoundToInt(startF.x),
            Mathf.RoundToInt(startF.y)
        );
        var visited = new bool[width, height];
        var queue = new Queue<Vector2Int>();
        visited[start.x, start.y] = true;
        queue.Enqueue(start);

        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            foreach (var d in dirs)
            {
                int nx = cur.x + d.x, ny = cur.y + d.y;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                if (!visited[nx, ny] && walkable[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        // ѕровер€ем достижимость каждой комнаты
        foreach (var room in layout.Rooms)
        {
            bool reached = false;
            for (int x = room.xMin; x < room.xMax && !reached; x++)
                for (int y = room.yMin; y < room.yMax; y++)
                    if (visited[x, y])
                    {
                        reached = true;
                        break;
                    }
            if (!reached)
                Debug.LogError($" омната {room} недостижима от старта!");
        }
    }
}
