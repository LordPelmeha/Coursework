using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapBuilder
{
    /// <summary>
    /// Строит изометрический уровень на Tilemap’ах:
    /// – «пол» вырезается по комнатам и коридорам;
    /// – «стены» ставятся вокруг всех клеток пола (4-соседство).
    /// </summary>
    public static void Build(RoomLayout layout, List<RectInt> corridors)
    {
        var groundMap = layout.Settings.groundTilemap;
        var wallMap = layout.Settings.wallTilemap;
        var groundTile = layout.Settings.groundTile;
        var wallTile = layout.Settings.wallTile;

        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();

        int width = layout.Settings.mapWidth;
        int height = layout.Settings.mapHeight;

        // локальная карта: 0 = пол, 1 = стена
        int[,] map = new int[width, height];

        // 1) Вырезаем комнаты, но с проверкой границ
        foreach (var room in layout.Rooms)
        {
            int xStart = Mathf.Max(0, room.xMin);
            int xEnd = Mathf.Min(width, room.xMax);
            int yStart = Mathf.Max(0, room.yMin);
            int yEnd = Mathf.Min(height, room.yMax);

            for (int x = xStart; x < xEnd; x++)
                for (int y = yStart; y < yEnd; y++)
                    map[x, y] = 0;
        }

        // 2) Вырезаем коридоры, тоже с проверкой
        foreach (var c in corridors)
        {
            int xStart = Mathf.Max(0, c.xMin);
            int xEnd = Mathf.Min(width, c.xMax);
            int yStart = Mathf.Max(0, c.yMin);
            int yEnd = Mathf.Min(height, c.yMax);

            for (int x = xStart; x < xEnd; x++)
                for (int y = yStart; y < yEnd; y++)
                    map[x, y] = 0;
        }

        // 3) Отрисовываем пол
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                    groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }

        // 4) Отрисовываем стены вокруг пола
        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != 0) continue;

                foreach (var d in dirs)
                {
                    int nx = x + d.x;
                    int ny = y + d.y;
                    // проверка границ перед отрисовкой
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        if (map[nx, ny] == 1)
                            wallMap.SetTile(new Vector3Int(nx, ny, 0), wallTile);
                    }
                }
            }
        }
    }
}
