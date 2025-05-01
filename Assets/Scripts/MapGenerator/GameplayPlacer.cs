// Assets/Scripts/MapGenerator/GameplayPlacer.cs
using UnityEngine;

public static class GameplayPlacer
{
    public static void Place(RoomLayout layout, DungeonSettings settings)
    {
        // 1) Спавн игрока внутри стартовой комнаты
        RectInt startRoom = layout.Rooms[0];
        // выбираем случайную точку внутри комнаты, не на самой границе
        int spawnX = Random.Range(startRoom.xMin + 1, startRoom.xMax - 1);
        int spawnY = Random.Range(startRoom.yMin + 1, startRoom.yMax - 1);

        spawnX = Mathf.Clamp(spawnX, 0, settings.mapWidth - 1);
        spawnY = Mathf.Clamp(spawnY, 0, settings.mapHeight - 1);

        Vector3Int spawnCell = new Vector3Int(spawnX, spawnY, 0);

        // переводим в мировые координаты и немного смещаем по Z, чтобы игрок был перед полом
        Vector3 worldStart = settings.groundTilemap.CellToWorld(spawnCell);
        worldStart.z = -1f;

        GameObject player = GameObject.Instantiate(
            settings.playerPrefab,
            worldStart,
            Quaternion.identity
        );

        // подключаем камеру к игроку
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
            cameraFollow.target = player.transform;

        // 2) Спавн выхода — на максимально удалённой комнате
        int farIdx = 0;
        float maxDist2 = -1f;
        // пересчитываем центр старой комнаты как Vector2Int, чтобы сравнивать правильно
        Vector2Int startCell = new Vector2Int(spawnX, spawnY);

        for (int i = 1; i < layout.Rooms.Count; i++)
        {
            RectInt room = layout.Rooms[i];
            Vector2Int roomCenter = new Vector2Int(
                Mathf.FloorToInt((room.xMin + room.xMax) * 0.5f),
                Mathf.FloorToInt((room.yMin + room.yMax) * 0.5f)
            );

            Vector2Int delta = roomCenter - startCell;
            float dist2 = delta.x * delta.x + delta.y * delta.y;
            if (dist2 > maxDist2)
            {
                maxDist2 = dist2;
                farIdx = i;
            }
        }

        RectInt endRoom = layout.Rooms[farIdx];
        int exitX = Random.Range(endRoom.xMin + 1, endRoom.xMax - 1);
        int exitY = Random.Range(endRoom.yMin + 1, endRoom.yMax - 1);
        Vector3Int exitCell = new Vector3Int(exitX, exitY, 0);

        Vector3 worldEnd = settings.groundTilemap.CellToWorld(exitCell);
        worldEnd.z = -1f;

        if (settings.exitPrefab != null)
            GameObject.Instantiate(
                settings.exitPrefab,
                worldEnd,
                Quaternion.identity
            );

        // 3) Спавн врагов и сундуков (по желанию)
        //foreach (var room in layout.Rooms)
        //{
        //    if (settings.enemyPrefab != null && Random.value < settings.enemySpawnChance)
        //        SpawnRandom(room, settings.enemyPrefab, settings);
        //    if (settings.chestPrefab != null && Random.value < settings.chestSpawnChance)
        //        SpawnRandom(room, settings.chestPrefab, settings);
        //}
    }

    private static void SpawnRandom(RectInt room, GameObject prefab, DungeonSettings settings)
    {
        int x = Random.Range(room.xMin + 1, room.xMax - 1);
        int y = Random.Range(room.yMin + 1, room.yMax - 1);
        Vector3Int cell = new Vector3Int(x, y, 0);
        Vector3 worldPos = settings.groundTilemap.CellToWorld(cell);
        worldPos.z = -1f;
        GameObject.Instantiate(prefab, worldPos, Quaternion.identity);
    }
}
