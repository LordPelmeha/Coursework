using System.Collections.Generic;
using UnityEngine;

public static class CorridorConnector
{
    /// <summary>
    /// ��������� ������� ���������� �� ������ ����� � �� �������������� ������������.
    /// ��� ������� ����� ����� ������������ L-�������� �������:
    /// ������� �� X, ����� �� Y (��� ��������), ������� ��������� � �����.
    /// </summary>
    public static List<RectInt> Connect(RoomLayout layout)
    {
        var corridors = new List<RectInt>();
        var nodes = layout.Graph.Nodes;
        var edges = layout.Graph.Edges;

        foreach (var edge in edges)
        {
            Vector2 centerA = layout.Rooms[edge.a].center;
            Vector2 centerB = layout.Rooms[edge.b].center;
            Vector2Int aCenter = new Vector2Int(
                Mathf.RoundToInt(centerA.x),
                Mathf.RoundToInt(centerA.y)
            );
            Vector2Int bCenter = new Vector2Int(
                Mathf.RoundToInt(centerB.x),
                Mathf.RoundToInt(centerB.y)
            );
            // ������ �����������: 50% ������� ������� �������������, 50% � �����������
            bool horizontalFirst = (Random.value > 0.5f);

            if (horizontalFirst)
            {
                // �������������� �������
                int xMin = Mathf.Min(aCenter.x, bCenter.x);
                int xMax = Mathf.Max(aCenter.x, bCenter.x);
                corridors.Add(new RectInt(xMin, aCenter.y, xMax - xMin + 1, 1));

                // ������������ �������
                int yMin = Mathf.Min(aCenter.y, bCenter.y);
                int yMax = Mathf.Max(aCenter.y, bCenter.y);
                corridors.Add(new RectInt(bCenter.x, yMin, 1, yMax - yMin + 1));
            }
            else
            {
                // ������������ �������
                int yMin = Mathf.Min(aCenter.y, bCenter.y);
                int yMax = Mathf.Max(aCenter.y, bCenter.y);
                corridors.Add(new RectInt(aCenter.x, yMin, 1, yMax - yMin + 1));

                // �������������� �������
                int xMin = Mathf.Min(aCenter.x, bCenter.x);
                int xMax = Mathf.Max(aCenter.x, bCenter.x);
                corridors.Add(new RectInt(xMin, bCenter.y, xMax - xMin + 1, 1));
            }
        }

        return corridors;
    }
}
