using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public IslandMapData mapData;
    public HexCell[] cellPrefabs; // index = CellType value, 0 = null (mer)

    Dictionary<Vector3Int, HexCell> cellMap = new();

    static readonly Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        new Vector3Int(1, -1, 0),  // NE
        new Vector3Int(1, 0, -1),  // E
        new Vector3Int(0, 1, -1),  // SE
        new Vector3Int(-1, 1, 0),  // SW
        new Vector3Int(-1, 0, 1),  // W
        new Vector3Int(0, -1, 1),  // NW
    };

    void Awake()
    {
        if (mapData == null)
            Debug.Log("no MapData");
        else
            Debug.Log("mapData");
        foreach (var kvp in mapData.GetDict())
        {
            Vector3Int coords = kvp.Key;
            CellType type = kvp.Value;

            if (type == CellType.water) continue;
            
            Vector3 worldPos = HexCoordinates.CoordsToWorldPosition(coords);
            HexCell cell = Instantiate(cellPrefabs[(int)type], worldPos, Quaternion.identity, transform);
            cell.coordinates = new HexCoordinates(coords.x, coords.z);
            cellMap[coords] = cell;
            cell.SpawnProps(mapData.props[(int)type]);
            
        }
        foreach (var kvp in cellMap)
        {
            Vector3Int coords = kvp.Key;
            HexCell cell = kvp.Value;
            for (int i = 0; i < 6; i++)
            {
                Vector3Int neighborCoords = coords + neighborOffsets[i];
                if (cellMap.TryGetValue(neighborCoords, out HexCell neighbor))
                    cell.SetNeighbor((HexDirection)i, neighbor);
            }
        }
    }

    
}