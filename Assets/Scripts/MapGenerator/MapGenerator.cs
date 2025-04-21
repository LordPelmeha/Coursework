using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int width = 100;
    public int height = 70;

    [Header("Room Settings")]
    public int roomCount = 15;
    public int roomMinSize = 5;
    public int roomMaxSize = 15;

    [Header("Tilemaps & Tiles")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public TileBase groundTile;
    public TileBase wallTile;

    [Header("Spawn Settings")]
    public Transform player;        // Ссылка на объект игрока в сцене
    public GameObject exitPrefab;   // Префаб маркера выхода/финиша

    private int[,] map;
    private List<RectInt> rooms;

    void Start()
    {
        GenerateDungeon();
        DrawDungeon();

        PlacePlayerAndExit();
    }

    void GenerateDungeon()
    {
        map = new int[width, height];
        rooms = new List<RectInt>();

        // 1) Заполняем стенами
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 1;

        // 2) Создаём комнаты
        for (int i = 0; i < roomCount; i++)
        {
            int w = Random.Range(roomMinSize, roomMaxSize + 1);
            int h = Random.Range(roomMinSize, roomMaxSize + 1);
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);
            var newRoom = new RectInt(x, y, w, h);

            bool overlaps = false;
            foreach (var room in rooms)
                if (newRoom.Overlaps(room)) { overlaps = true; break; }

            if (!overlaps)
            {
                rooms.Add(newRoom);
                CarveRoom(newRoom);
            }
        }

        // 3) Соединяем комнаты коридорами
        for (int i = 1; i < rooms.Count; i++)
        {
            var prev = rooms[i - 1].center;
            var curr = rooms[i].center;
            var a = new Vector2Int(Mathf.RoundToInt(prev.x), Mathf.RoundToInt(prev.y));
            var b = new Vector2Int(Mathf.RoundToInt(curr.x), Mathf.RoundToInt(curr.y));

            CarveHorizontalCorridor(a.x, b.x, a.y);
            CarveVerticalCorridor(a.y, b.y, b.x);
        }
    }

    void PlacePlayerAndExit()
    {
        if (rooms.Count == 0) return;

        // Стартовая комната — первая
        var startCenterF = rooms[0].center;
        var startCell = new Vector3Int(
            Mathf.RoundToInt(startCenterF.x),
            Mathf.RoundToInt(startCenterF.y),
            0
        );
        var worldStart = groundTilemap.CellToWorld(startCell) + groundTilemap.cellSize / 2;
        player.position = worldStart;

        // Финальная — самая удалённая от старта
        float maxDist = -1f;
        RectInt finalRoom = rooms[0];
        foreach (var room in rooms)
        {
            float d = Vector2.SqrMagnitude((Vector2)room.center - (Vector2)rooms[0].center);
            if (d > maxDist) { maxDist = d; finalRoom = room; }
        }
        var endCenterF = finalRoom.center;
        var endCell = new Vector3Int(
            Mathf.RoundToInt(endCenterF.x),
            Mathf.RoundToInt(endCenterF.y),
            0
        );
        var worldEnd = groundTilemap.CellToWorld(endCell) + groundTilemap.cellSize / 2;
        Instantiate(exitPrefab, worldEnd, Quaternion.identity, transform);
    }

    void CarveRoom(RectInt room)
    {
        for (int x = room.xMin; x < room.xMax; x++)
            for (int y = room.yMin; y < room.yMax; y++)
                map[x, y] = 0;
    }

    void CarveHorizontalCorridor(int x1, int x2, int y)
    {
        int start = Mathf.Min(x1, x2), end = Mathf.Max(x1, x2);
        for (int x = start; x <= end; x++)
            map[x, y] = 0;
    }

    void CarveVerticalCorridor(int y1, int y2, int x)
    {
        int start = Mathf.Min(y1, y2), end = Mathf.Max(y1, y2);
        for (int y = start; y <= end; y++)
            map[x, y] = 0;
    }

    void DrawDungeon()
    {
        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        // Рисуем пол
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }

        // Направления для проверки соседей (4-соседство)
        Vector2Int[] dirs = new Vector2Int[]
        {
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1)
        };

        // Рисуем стены вокруг пола
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    foreach (var dir in dirs)
                    {
                        int nx = x + dir.x;
                        int ny = y + dir.y;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height && map[nx, ny] == 1)
                        {
                            wallTilemap.SetTile(new Vector3Int(nx, ny, 0), wallTile);
                        }
                    }
                }
            }
        }
    }

}
