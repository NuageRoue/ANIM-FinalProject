using System;
using UnityEngine;


public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    public float height = 0;
    GameObject prop;

    [SerializeField]
    HexCell[] neighbors;

    private void Awake()
    {
        neighbors = new HexCell[6];
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        // cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void SetAsRaftPart() // temporary until I fix CallEvent
    {
        name += " (raft part)";
    }

    public void SetAsStartingPos()
    {
        name += " (starting pos)";
    }
    internal void Setup(GameObject selectedProp, CallEvent selectedEvent)
    {
        if (selectedProp != null)
            prop = GameObject.Instantiate(selectedProp, transform.position + Vector3.up * height, Quaternion.identity, transform);
        //TODO: take care of the callEVent
    }
}
