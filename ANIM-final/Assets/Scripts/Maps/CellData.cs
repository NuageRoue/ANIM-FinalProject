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

  public GameObject getRandomProp() {
    if (propSpawn == 0 || potentialProps.Count == 0)
      return null;
    return potentialProps[Random.Range(0, potentialProps.Count)];
  }


  public CallEvent getRandomEvent() {
    if (eventSpawn == 0 || potentialEvents.Count == 0)
      return null;
    return potentialEvents[Random.Range(0, potentialEvents.Count)];
  }
}


public enum CellType
{
    coast, grass, mountain, tower, Forest
}

[System.Serializable]
public class Cell {

  public Cell(CellData data) {
    LoadFromCellData(data);
  }

  public void ForceEvent(CallEvent selectedEvent) {
    this.selectedEvent = selectedEvent;
    hasEvent = true;
  }
  void LoadFromCellData(CellData data) 
  {
    type = data.type;
    selectedProp = data.getRandomProp();
    selectedEvent = data.getRandomEvent();
    cell = data.cell;
    hasEvent = (selectedEvent != null);
  }

  bool hasEvent;
  public HexCell cell;
  CellType type;

  public GameObject selectedProp;
  public CallEvent selectedEvent;
}

