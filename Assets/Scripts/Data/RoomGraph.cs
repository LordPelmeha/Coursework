// Assets/Scripts/Data/RoomGraph.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomGraph
{
    /// <summary>
    /// ������ ��������� ������� ������ (����� �����).
    /// </summary>
    public List<Vector2Int> Nodes { get; }

    /// <summary>
    /// ������ ����; ������ ����� ��������� ��� ������� �� �� ��������.
    /// </summary>
    public List<Edge> Edges { get; }

    public RoomGraph(List<Vector2Int> nodes, List<Edge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }
}
