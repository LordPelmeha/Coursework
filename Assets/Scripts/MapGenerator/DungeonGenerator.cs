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
        // ����� ������, ������, ������, ��������
        GameplayPlacer.Place(layout, settings);
    }
    private void GenerateDungeon()
    {
        // ��������� ����� ������
        RoomGraph graph = GraphGenerator.Generate(settings);

        // ���������� ������ �� �����
        layout = RoomPlacer.Place(graph, settings);

        // ���������� ��������� � ������ �� � layout
        var corridors = CorridorConnector.Connect(layout);
        layout.SetCorridors(corridors);

        // ��������� Tilemap (��� � �����)
        TilemapBuilder.Build(layout, corridors);
        Debug.Log(">> TilemapBuilder.Build ������");

        // �������� ��������� � ����������� ������
        DungeonValidator.Validate(layout);

    }

}
