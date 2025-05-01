// Assets/Scripts/MapGenerator/GameplayPlacer.cs
using UnityEngine;

public static class GameplayPlacer
{
    public static void Place(RoomLayout layout, DungeonSettings settings)
    {
        // 1) Старт
        Vector2 startF = layout.Rooms[0].center;
        Vector2Int start = new Vector2Int(
            Mathf.RoundToInt(startF.x),
            Mathf.RoundToInt(startF.y)
        );
        Vector3 worldStart = settings.groundTilemap
            .CellToWorld(new Vector3Int(start.x, start.y, 0))
            + settings.groundTilemap.cellSize / 2;
        GameObject player = GameObject.Instantiate(settings.playerPrefab, worldStart, Quaternion.identity);

        // Дополнительные действия с игроком, например, установка камеры
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;  // Камера будет следовать за игроком
        }
        // 2) Финал
        int farIdx = 0;
        float maxD = -1f;
        for (int i = 1; i < layout.Rooms.Count; i++)
        {
            Vector2 roomF = layout.Rooms[i].center;
            Vector2Int roomI = new Vector2Int(
                Mathf.RoundToInt(roomF.x),
                Mathf.RoundToInt(roomF.y)
            );
            Vector2Int diff = roomI - start;
            float d = diff.x * diff.x + diff.y * diff.y;
            if (d > maxD)
            {
                maxD = d;
                farIdx = i;
            }
        }
        Vector2 endF = layout.Rooms[farIdx].center;
        Vector2Int end = new Vector2Int(
            Mathf.RoundToInt(endF.x),
            Mathf.RoundToInt(endF.y)
        );
        Vector3 worldEnd = settings.groundTilemap
            .CellToWorld(new Vector3Int(end.x, end.y, 0))
            + settings.groundTilemap.cellSize / 2;
        GameObject.Instantiate(settings.exitPrefab, worldEnd, Quaternion.identity);

        // 3) Враги и сундуки
        //foreach (var room in layout.Rooms)
        //{
        //    if (Random.value < settings.enemySpawnChance)
        //        SpawnRandom(room, settings.enemyPrefab, settings);
        //    if (Random.value < settings.chestSpawnChance)
        //        SpawnRandom(room, settings.chestPrefab, settings);
        //}
    }

    private static void SpawnRandom(RectInt room, GameObject prefab, DungeonSettings settings)
    {
        int x = Random.Range(room.xMin + 1, room.xMax - 1);
        int y = Random.Range(room.yMin + 1, room.yMax - 1);
        Vector3 worldPos = settings.groundTilemap
            .CellToWorld(new Vector3Int(x, y, 0))
            + settings.groundTilemap.cellSize / 2;
        GameObject.Instantiate(prefab, worldPos, Quaternion.identity);
    }

}
