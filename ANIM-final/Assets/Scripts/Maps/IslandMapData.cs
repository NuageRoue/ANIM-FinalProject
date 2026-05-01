using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IslandMapData", menuName = "Scriptable Objects/IslandMapData")]
public class IslandMapData : ScriptableObject
{
    public int landCounter = 0;
    public List<Vector3Int> keys = new();
    public List<Cell> values = new();

    public void SetCells(Dictionary<Vector3Int, int> indexes, List<CellData> data)
    {
        keys.Clear();
        values.Clear();
        landCounter = 0;
        foreach (var kvp in indexes)
        {
            if (kvp.Value != 0)
            {
                keys.Add(kvp.Key);
                Debug.Log(kvp.Value - 1);
                values.Add(new Cell(data[kvp.Value - 1]));
                if (kvp.Value != 0)
                    landCounter++;
            }        
        }
    }

    public Dictionary<Vector3Int, Cell> GetDict()
    {
        Dictionary<Vector3Int, Cell> dict = new();
        for (int i = 0; i < keys.Count; i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }
}

