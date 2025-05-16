using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapBuilder
{
    /// <summary>
    /// —троит изометрический уровень на TilemapТах:
    /// Ц Ђполї вырезаетс€ по комнатам и коридорам;
    /// Ц Ђстеныї став€тс€ вокруг всех клеток пола (4-соседство).
    /// </summary>
    public static void Build(RoomLayout layout, List<RectInt> corridors)
    {
        Debug.Log($"Build: rooms={layout.Rooms.Count}, corridors={corridors.Count}");
        var groundMap = layout.Settings.groundTilemap;
        var wallMap = layout.Settings.wallTilemap;
        var groundTile = layout.Settings.groundTile;
        var wallTile = layout.Settings.wallTile;
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();

        int width = layout.Settings.mapWidth;
        int height = layout.Settings.mapHeight;

        int[,] map = layout.MapData;


        // 3) ќтрисовываем пол
        int painted = 0;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (map[x, y] == 0)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                    painted++;

                }
        Debug.Log($"TilemapBuilder: painted floor tiles = {painted}");

        // 4) ќтрисовываем стены вокруг пола
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
