using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DungeonSettings", menuName = "Generation/Dungeon Settings")]
public class DungeonSettings : ScriptableObject
{
    [Header("Map Dimensions")]
    public int mapWidth = 100;
    public int mapHeight = 70;

    [Header("Graph Generation")]
    public int roomCount = 20;
    public int extraBranches = 5;
    public int neighborCount = 2;
    public int seed = 12345;

    [Header("Room Placement")]
    public int roomMinSize = 5;
    public int roomMaxSize = 15;
    public int maxRoomPlacementAttempts = 50;

    [Header("Corridor Settings")]
    [Tooltip("If true, corridors carve horizontally then vertically; otherwise vertically then horizontally.")]
    public bool randomCorridorOrientation = true;

    [Header("Drunkard’s Walk Corridors")]
    public int drunkardWalkLength = 100;
    public float drunkardTurnAngle = 45f;   
    public int corridorRadius = 1;

    [Header("Tilemaps & Tiles")]
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public TileBase groundTile;
    public TileBase wallTile;

    [Header("Post-Processing")]
    public int iterations = 3;
    public int birthLimit = 4;
    public int deathLimit = 3;

    [Header("Gameplay Prefabs")]
    public GameObject playerPrefab;
    public GameObject exitPrefab;
   // public GameObject enemyPrefab;
    public float enemySpawnChance = 0.1f;
   // public GameObject chestPrefab;
    public float chestSpawnChance = 0.05f;
}
