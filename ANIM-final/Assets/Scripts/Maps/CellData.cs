using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "CellData", menuName = "Scriptable Objects/CellData")]
public class CellData : ScriptableObject
{
  public CellType type;

  public float propSpawn = 1, eventSpawn = 0;
  public List<GameObject> potentialProps;
  public List<CallEvent> potentialEvents;

  public HexCell cell;
}


public enum CellType
{
    coast, grass, mountain, tower, Forest
}

[System.Serializable]
public class Cell {

  public Cell(CellData data) {
    this.data = data;
  }


  public CellData data;

  public GameObject forcedProp;
  public CallEvent forcedEvent;
}
