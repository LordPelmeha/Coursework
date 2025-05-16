using System.Collections.Generic;
using UnityEngine;

public static class GameplayPlacer
{
    public static void Place(RoomLayout layout, DungeonSettings settings)
    {
        // 1) ����� ������ ������ ��������� �������
        RectInt startRoom = layout.Rooms[0];
        Debug.Log($"GameplayPlacer: StartRoom = x[{startRoom.xMin}..{startRoom.xMax}) y[{startRoom.yMin}..{startRoom.yMax})");

        // 2) ���� spawnCell
        Vector3Int spawnCell = FindValidGroundCell(startRoom, layout);
        Debug.Log($"GameplayPlacer: spawnCell (cell coords) = {spawnCell}");

        Vector3 worldStart = settings.groundTilemap.GetCellCenterWorld(spawnCell);
        Debug.Log($"GameplayPlacer: worldStart (world coords) = {worldStart}");
        worldStart.z = -1f;
        Debug.Log($"Ground Tilemap transform.position = {settings.groundTilemap.transform.position}");

        var player = GameObject.Instantiate(settings.playerPrefab, worldStart, Quaternion.identity);
        Debug.Log($"GameplayPlacer: Player instantiated at {worldStart}");
        ;

        // ����������� ������
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;
            Debug.Log("GameplayPlacer: Camera target assigned");

        }

        // 2) ����� ������ � � ����� �������� ������� �� spawnCell
        Vector2Int startCell2D = new Vector2Int(spawnCell.x, spawnCell.y);
        int farIdx = 0;
        float maxDist2 = -1f;

        for (int i = 1; i < layout.Rooms.Count; i++)
        {
            RectInt room = layout.Rooms[i];
            Vector2Int center = new Vector2Int(
                (room.xMin + room.xMax) / 2,
                (room.yMin + room.yMax) / 2
            );
            Vector2Int delta = center - startCell2D;
            float dist2 = delta.x * delta.x + delta.y * delta.y;
            if (dist2 > maxDist2)
            {
                maxDist2 = dist2;
                farIdx = i;
            }
        }

        RectInt endRoom = layout.Rooms[farIdx];
        Debug.Log($"GameplayPlacer: EndRoom index = {farIdx}, x[{endRoom.xMin}..{endRoom.xMax}) y[{endRoom.yMin}..{endRoom.yMax})");

        Vector3Int exitCell = FindValidExitCell(endRoom, layout, settings);
        Debug.Log($"GameplayPlacer: exitCell (cell coords) = {exitCell}");

        Vector3 worldEnd = settings.groundTilemap.GetCellCenterWorld(exitCell);
        Debug.Log($"GameplayPlacer: worldEnd (world coords) = {worldEnd}");

        worldEnd.z = -0.1f;

        if (settings.exitPrefab != null)
        {
            GameObject.Instantiate(settings.exitPrefab, worldEnd, Quaternion.identity);
            Debug.Log("GameplayPlacer: Exit instantiated");
        }


        // 3) (�����������) ����� ������/�������� ����������� ��������:
        // foreach (var room in layout.Rooms) {
        //     Vector3Int cell = FindValidGroundCell(room, layout.MapData);
        //     GameObject.Instantiate(settings.enemyPrefab, settings.groundTilemap.CellToWorld(cell) + Vector3.back, Quaternion.identity);
        // }
    }

    private static Vector3Int FindValidGroundCell(RectInt room,RoomLayout layout )
    {
        var map = layout.MapData;
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        List<Vector3Int> candidates = new List<Vector3Int>();

        // === ��� ������ ������� ===
        Debug.Log($"[DEBUG] Checking room bounds: x[{room.xMin}..{room.xMax}), y[{room.yMin}..{room.yMax})");

        for (int x = room.xMin + 1; x < room.xMax - 1; x++)
        {
            for (int y = room.yMin + 1; y < room.yMax - 1; y++)
            {
                if (x >= 0 && x < w && y >= 0 && y < h)
                {
                    // === ��� ����������� ������ ===
                    if (map[x, y] == 0)
                    {
                        candidates.Add(new Vector3Int(x, y, 0));
                        Debug.Log($"[DEBUG] Valid ground tile found at ({x},{y})");
                    }
                    else
                    {
                        Debug.Log($"[DEBUG] Tile at ({x},{y}) is wall");
                    }
                }
                else
                {
                    Debug.Log($"[DEBUG] Tile at ({x},{y}) is out of bounds");
                }
            }
        }

        if (candidates.Count > 0)
        {
            var chosen = candidates[Random.Range(0, candidates.Count)];
            Debug.Log($"[DEBUG] Chosen cell: {chosen}");
            return chosen;
        }

        Debug.LogWarning("[DEBUG] No valid ground cells found in room � fallback to room origin");

        int fx = Mathf.Clamp(room.xMin, 0, w - 1);
        int fy = Mathf.Clamp(room.yMin, 0, h - 1);
        return new Vector3Int(fx, fy, 0);
    }
    // � ������ ������ ������ �������� ���� �����:
    private static Vector3Int FindValidExitCell(RectInt room, RoomLayout layout, DungeonSettings settings)
    {
        var map = layout.MapData;
        var groundMap = settings.groundTilemap;
        var wallMap = settings.wallTilemap;
        int w = map.GetLength(0), h = map.GetLength(1);
        var candidates = new List<Vector3Int>();

        // �������� �� ���� ����������� ������� �������
        for (int x = room.xMin + 1; x < room.xMax - 1; x++)
            for (int y = room.yMin + 1; y < room.yMax - 1; y++)
            {
                if (x < 0 || x >= w || y < 0 || y >= h) continue;

                var cell = new Vector3Int(x, y, 0);
                // ������� 1: ��� ������������� ���
                bool isFloor = map[x, y] == 0
                               && groundMap.HasTile(cell);
                // ������� 2: � ���� ������ ��� �����
                bool noWall = !wallMap.HasTile(cell);

                if (isFloor && noWall)
                    candidates.Add(cell);
            }

        if (candidates.Count > 0)
            return candidates[Random.Range(0, candidates.Count)];

        // ������� �� ����� ����� ������� (��� ����� ����, ���� ������)
        int fx = Mathf.Clamp((room.xMin + room.xMax) / 2, 0, w - 1);
        int fy = Mathf.Clamp((room.yMin + room.yMax) / 2, 0, h - 1);
        return new Vector3Int(fx, fy, 0);
    }


}
