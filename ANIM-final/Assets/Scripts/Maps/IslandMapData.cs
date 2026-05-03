using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CreateAssetMenu(fileName = "IslandMapData", menuName = "Scriptable Objects/IslandMapData")]
public class IslandMapData : ScriptableObject
{
    public List<Vector3Int> keys = new();
    public List<Cell> values = new();

    public Dictionary<Vector3Int, Cell> GetDict()
    {
        Dictionary<Vector3Int, Cell> dict = new();
        for (int i = 0; i < keys.Count; i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }

    #region build
    public void SetCells(Dictionary<Vector3Int, CellType> indexes, List<CellData> data)
    {
        keys.Clear();
        values.Clear();
        
        foreach (var kvp in indexes)
        {
                keys.Add(kvp.Key);
                values.Add(new Cell(data[(int)kvp.Value]));        
        }
    }
    public void SetRaftPos(Vector3Int[] raftPos) {
        foreach (Vector3Int pos in raftPos)
        {
            Debug.Log($"raft pos: {pos}");
            if (!keys.Contains(pos))
                Debug.Log("fak");
            else
            values[keys.IndexOf(pos)].SetAsRaftPos();
        }
    }
    public void SetStartingPos(Vector3Int startingPoint)
    {
        Debug.Log($"starting pos: {startingPoint}");
        if (!keys.Contains(startingPoint))
            Debug.Log("fak");
        if (startingPoint != null)
            values[keys.IndexOf(startingPoint)].SetAsStartPos();
    }

    #endregion
}