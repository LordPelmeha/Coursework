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

        // ������ ����� ������� �������� � runtimeSettings,
        // � ������������ ����� ��������� �����

        GenerateDungeon(runtimeSettings);
    }

    public void GenerateDungeon(DungeonSettings runtimeSettings)
    {
        // 1) ������� ���� � �������
        var graph = GraphGenerator.Generate(runtimeSettings);
        layout = RoomPlacer.Place(graph, runtimeSettings);

        // ����� ��� �������
        for (int i = 0; i < layout.Rooms.Count; i++)
        {
            RectInt r = layout.Rooms[i];
            Debug.Log($"DungeonGenerator: Room[{i}] = x[{r.xMin}..{r.xMax}) y[{r.yMin}..{r.yMax})");
        }

        // 2) ������� ��������
        var corridors = CorridorConnector.Connect(layout);
        layout.SetCorridors(corridors);
        Debug.Log($"DungeonGenerator: Corridors.Count = {corridors.Count}");

        // 3) ������ Tilemap
        TilemapBuilder.Build(layout, corridors);
        Debug.Log("DungeonGenerator: TilemapBuilder.Build ��������");

        // 4) ��������� �������
        DungeonValidator.Validate(layout);

        // 5) ������� ������ � �����
        GameplayPlacer.Place(layout, runtimeSettings);
    }

}
