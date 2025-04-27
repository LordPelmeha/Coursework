using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private DungeonSettings settings;

    private RoomLayout layout;

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
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

        // ����� ������, ������, ������, ��������
        GameplayPlacer.Place(layout, settings);

        // �������� ��������� � ����������� ������
        DungeonValidator.Validate(layout);
    }
}
