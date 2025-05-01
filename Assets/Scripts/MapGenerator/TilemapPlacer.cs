using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapBuilder
{
    /// <summary>
    /// ������ �������������� ������� �� Tilemap���:
    /// � ���� ���������� �� �������� � ���������;
    /// � ������� �������� ������ ���� ������ ���� (4-���������).
    /// </summary>
    public static void Build(RoomLayout layout, List<RectInt> corridors)
    {
        Debug.Log($"Build: rooms={layout.Rooms.Count}, corridors={corridors.Count}");
        var groundMap = layout.Settings.groundTilemap;
        var wallMap = layout.Settings.wallTilemap;
        var groundTile = layout.Settings.groundTile;
        var wallTile = layout.Settings.wallTile;
        if (groundMap == null || groundTile == null)
            Debug.LogError("TilemapBuilder: groundMap ��� groundTile �� ���������!");
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();

        int width = layout.Settings.mapWidth;
        int height = layout.Settings.mapHeight;

        // ��������� �����: 0 = ���, 1 = �����
        int[,] map = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;
        // 1) �������� �������, �� � ��������� ������
        foreach (var room in layout.Rooms)
        {
            Debug.Log($"Room carve: {room}");
            int xStart = Mathf.Max(0, room.xMin);
            int xEnd = Mathf.Min(width, room.xMax);
            int yStart = Mathf.Max(0, room.yMin);
            int yEnd = Mathf.Min(height, room.yMax);

            for (int x = xStart; x < xEnd; x++)
                for (int y = yStart; y < yEnd; y++)
                    map[x, y] = 0;
        }

        // 2) �������� ��������, ���� � ���������
        foreach (var c in corridors)
        {
            Debug.Log($"Corridor carve: {c}");
            int xStart = Mathf.Max(0, c.xMin);
            int xEnd = Mathf.Min(width, c.xMax);
            int yStart = Mathf.Max(0, c.yMin);
            int yEnd = Mathf.Min(height, c.yMax);

            for (int x = xStart; x < xEnd; x++)
                for (int y = yStart; y < yEnd; y++)
                    map[x, y] = 0;
        }

        // 3) ������������ ���
        int painted = 0;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (map[x, y] == 0)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                    painted++;

                }
        Debug.Log($"TilemapBuilder: painted floor tiles = {painted}");

        // 4) ������������ ����� ������ ����
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
                    // �������� ������ ����� ����������
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
