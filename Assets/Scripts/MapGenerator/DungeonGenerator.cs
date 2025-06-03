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
        if (!runtimeSettings.isSeedConstant)
            runtimeSettings.seed = DateTime.Now.GetHashCode();
        runtimeSettings.groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        runtimeSettings.wallTilemap = GameObject.Find("Wall").GetComponent<Tilemap>();

        GenerateDungeon(runtimeSettings);
    }

    void Start()
    {
        var rng = new System.Random(runtimeSettings.seed);

        float r = (float)rng.NextDouble();
        float g = (float)rng.NextDouble();
        float b = (float)rng.NextDouble();

        Camera.main.backgroundColor = new Color(r, g, b);
    }

    public void GenerateDungeon(DungeonSettings runtimeSettings)
    {
        bool isValid = false;
        while (!isValid)
        {
            // ���������� ���� � �������
            var graph = GraphGenerator.Generate(runtimeSettings);
            layout = RoomPlacer.Place(graph, runtimeSettings);

            // 2) ���������� ��������
            var corridors = DrunkardWalkConnector.Connect(layout, runtimeSettings);
            layout.SetCorridors(corridors);

            // 3) ����-��������� �����
            PostProcessor.Process(layout, runtimeSettings);

            // 3) ��������� �������
            isValid = DungeonValidator.Validate(layout);
            if (!isValid)
            {
                runtimeSettings.seed++;
                continue;
            }

            // 4) ������ Tilemap
            TilemapBuilder.Build(layout, corridors);

            // 5) ������� ������ � �����
            GameplayPlacer.Place(layout, runtimeSettings);
        }
    }
}
