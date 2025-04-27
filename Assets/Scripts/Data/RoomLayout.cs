// Assets/Scripts/Data/RoomLayout.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomLayout
{
    public RoomGraph Graph { get; }
    public List<RectInt> Rooms { get; }
    public List<RectInt> Corridors { get; private set; }
    public DungeonSettings Settings { get; }   

    public RoomLayout(RoomGraph graph, List<RectInt> rooms, DungeonSettings settings)
    {
        Graph = graph;
        Rooms = rooms;
        Corridors = new List<RectInt>();
        Settings = settings;                  
    }

    public void SetCorridors(List<RectInt> corridors)
    {
        Corridors = corridors;
    }
}
