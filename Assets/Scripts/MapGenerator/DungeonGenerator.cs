using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DungeonSettings settings;
    private RoomLayout layout;
    private DungeonSettings runtimeSettings;

    private void Awake()
    {
        runtimeSettings = Instantiate(settings);
        runtimeSettings.seed = DateTime.Now.GetHashCode();
        runtimeSettings.groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        runtimeSettings.wallTilemap = GameObject.Find("Wall").GetComponent<Tilemap>();

        // теперь можно править тайлмапы в runtimeSettings,
        // а оригинальный ассет останется целым

        GenerateDungeon(runtimeSettings);
    }

    public void GenerateDungeon(DungeonSettings runtimeSettings)
    {
        // 1) Генерим граф и комнаты
        var graph = GraphGenerator.Generate(runtimeSettings);
        layout = RoomPlacer.Place(graph, runtimeSettings);

        // Логим все комнаты
        for (int i = 0; i < layout.Rooms.Count; i++)
        {
            RectInt r = layout.Rooms[i];
            Debug.Log($"DungeonGenerator: Room[{i}] = x[{r.xMin}..{r.xMax}) y[{r.yMin}..{r.yMax})");
        }

        // 2) Генерим коридоры
        var corridors = CorridorConnector.Connect(layout);
        layout.SetCorridors(corridors);
        Debug.Log($"DungeonGenerator: Corridors.Count = {corridors.Count}");

        // 3) Строим Tilemap
        TilemapBuilder.Build(layout, corridors);
        Debug.Log("DungeonGenerator: TilemapBuilder.Build завершён");

        // 4) Проверяем уровень
        DungeonValidator.Validate(layout);

        // 5) Спавним игрока и выход
        GameplayPlacer.Place(layout, runtimeSettings);
    }

}
