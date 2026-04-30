using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IslandMapData", menuName = "Scriptable Objects/IslandMapData")]
public class IslandMapData : ScriptableObject
{
    public int landCounter = 0;
    public List<Vector3Int> keys = new();
    public List<CellType> values = new();

    public void SetCells(Dictionary<Vector3Int, int> indexes)
    {
        keys.Clear();
        values.Clear();
        landCounter = 0;
        foreach (var kvp in indexes)
        {
            if (kvp.Value != 0)
            {
                keys.Add(kvp.Key);
                values.Add((CellType)kvp.Value);
                    if (kvp.Value != 0)
                        landCounter++;
            }        
        }
    }

    public Dictionary<Vector3Int, CellType> GetDict()
    {
        Dictionary<Vector3Int, CellType> dict = new();
        for (int i = 0; i < keys.Count; i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }

    public List<GameObjectList> props;
}

public enum CellType
{
    water, coast, grass, mountain, tower, Forest
}

[System.Serializable]
public class GameObjectList
{
    public List<GameObject> items = new List<GameObject>();
    public int Count { get { return items.Count;  } }

    public GameObject Get(int id) { 
        return items[id];
    }
}