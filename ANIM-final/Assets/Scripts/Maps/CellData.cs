using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "CellData", menuName = "Scriptable Objects/Map/CellData")]
public class CellData : ScriptableObject
{
    /*public CellType type;
    public bool isTraversable;

    public float propSpawn = 1, eventSpawn = 0;
    public List<GameObject> potentialProps;
    public List<CallEvent> potentialEvents;

    public HexCell cell;
    public bool isFreeForEvents;

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
    */

    #region parameters
    [Header("parameters:")]
    public CellType cellType;
    public bool isTraversable, canHaveEvent;
    public HexCell cell;

    [Header("props & events:")]
    public float propSpawn = 1, eventSpawn = 0; // probability of spawning an event and a prop
    public List<GameObject> potentialProps;
    public List<CallEvent> potentialEvents;
    #endregion

    public GameObject GetRandomProp()
    {
        if (propSpawn < Random.value || potentialProps.Count == 0)
            return null;
        return potentialProps[Random.Range(0, potentialProps.Count)];
    }


    public CallEvent GetRandomEvent()
    {
        if (eventSpawn < Random.value || potentialEvents.Count == 0)
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

    /*public Cell(CellData data) {
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

      internal void SetAsStartingPoint()
      {
          isStartingPoint = true;
      }

      public bool hasEvent;
      public bool isStartingPoint = false;
    public HexCell cell;
    CellType type;

    public GameObject selectedProp;
    public CallEvent selectedEvent;
    */

    public HexCell cell;
    CellType cellType;
    
    public CallEvent selectedEvent;
    public GameObject selectedProp;

    public bool hasEvent = false, isStartPos = false, isTraversable = true;
    public Cell(CellData data) 
    { 
        LoadFromCellData(data);
    }

    void LoadFromCellData(CellData data)
    {
        cell = data.cell;
        cellType = data.cellType;

        selectedEvent = data.GetRandomEvent();
        selectedProp = data.GetRandomProp();

        hasEvent = selectedEvent != null;

        isTraversable = data.isTraversable;
    }

    public void SetAsStartPos() { 
        isStartPos = true;
    }
    public void SetAsRaftPos() {
        //TODO: pass the right event to it.
        hasEvent = true;
    }

    public void SetEvent(CallEvent _event) { 
            selectedEvent = _event;
        hasEvent = true;
    }

    public HexCell Instantiate(Vector3 pos, Transform parent) { 
        
        var instance = GameObject.Instantiate(cell, pos, Quaternion.identity, parent); 
        instance.Setup(selectedProp, selectedEvent, isTraversable, isStartPos);
        return instance;
    } 
}

