using System;
using System.Collections;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DungeonSettings settings;

    private RoomLayout layout;

    private void Awake()
    {
        settings.seed = DateTime.Now.GetHashCode();
        GenerateDungeon();
    }
    private void Start()
    {
        // Спавн игрока, выхода, врагов, сундуков
        GameplayPlacer.Place(layout, settings);
    }
    private void GenerateDungeon()
    {
        // Генерация графа комнат
        RoomGraph graph = GraphGenerator.Generate(settings);

        // Размещение комнат по графу
        layout = RoomPlacer.Place(graph, settings);

        // Построение коридоров и запись их в layout
        var corridors = CorridorConnector.Connect(layout);
        layout.SetCorridors(corridors);

        // Отрисовка Tilemap (пол и стены)
        TilemapBuilder.Build(layout, corridors);
        Debug.Log(">> TilemapBuilder.Build вызван");

        // Проверка связности и целостности уровня
        DungeonValidator.Validate(layout);

    }

}
