using UnityEngine;

[CreateAssetMenu(fileName = "TerrainSettings", menuName = "Settings/TerrainSettings", order = 1)]
public class TerrainSettings : ScriptableObject
{
    [Header("Grid Settings")]
    public int GridWidth = 10; // Number of tiles in the X direction
    public int GridHeight = 10; // Number of tiles in the Z direction
    public float TileSize = 1.0f; // Size of each tile

    [Header("Tile Colors")]
    public Color DefaultColor = Color.white;
    public Color HighlightColor = Color.yellow;
}
