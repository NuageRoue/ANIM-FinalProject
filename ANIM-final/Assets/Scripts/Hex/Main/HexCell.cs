using UnityEngine;


public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    public float height = 0;
    float scale = 7;
    GameObject prop;
    bool isTraversable = true;
    public bool IsTraversable { get { return isTraversable; } }

    public bool hasBeenExplored = false;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell[] Neighbors { get { return neighbors; } }

    [SerializeField]
    public Vector3[] SurvivorCoordinates = new Vector3[3]
    {
        new Vector3(0.75f, 0, 0.45f),
        new Vector3(-0.75f, 0, 0.45f),
        new Vector3(0, 0, -0.75f)
    };

    public bool[] coordOccupied = new bool[3]
    {
        false, false, false
    };

    public Survivor[] survivorOnTiles = new Survivor[3] { null, null, null };

    bool isStartingPoint = false;
    public bool IsStartingCell
    {
        get { return isStartingPoint; }}

    [SerializeField]
    public CallEvent callEvent;
    private void Awake()
    {
        neighbors = new HexCell[6];

        for (int i = 0; i < coordOccupied.Length; i++) {
            if (coordOccupied[i])
            {
                Debug.Log("why is it occupied");
            }
        }
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
        isStartingPoint = true;
    }
    internal void Setup(GameObject selectedProp, CallEvent selectedEvent, bool isTraversable, bool startingCell = false)
    {
        if (selectedProp != null)
            prop = GameObject.Instantiate(selectedProp, transform.position + Vector3.up * height, Quaternion.identity, transform);

        if (selectedEvent != null) {
            name += $"(event: {selectedEvent.name})";
            callEvent = selectedEvent;
        }

        if (startingCell) 
        {
            SetAsStartingPos();
        }

        if (!isTraversable)
        {
            this.isTraversable = false;
        }
        else
            this.isTraversable = true;


        hasBeenExplored = false;
}

    int GetAvailablePosition() 
    {
        bool nothingAvailable = true;
        foreach (bool b in coordOccupied) 
        { 
            if (!b)
                nothingAvailable = false;
        }
        if (nothingAvailable)
        {
            Debug.Log("why");
            return -1;
        }
        int i;
        do { i = Random.Range(0, 3); } while (coordOccupied[i]);

        coordOccupied[i] = true;
        hasBeenExplored = true;
        return i; // scale * (SurvivorCoordinates[i] + Vector3.up * height);
    }
    public void PlaceSurvivors(Survivor[] s) 
    {
        Debug.Log("placing survivors");
        foreach (Survivor survivor in s)
        {
            survivor.transform.position = PlaceSurvivor(survivor);
        }
    }

    public Vector3 PlaceSurvivor(Survivor s) 
    {
        int i = GetAvailablePosition();
        survivorOnTiles[i] = s;
        s.currentCell?.FreePosition(i);
        s.currentCell = this;
        return GetSnappingPoint(i);
    }

    private void FreePosition(int i)
    {
                survivorOnTiles[i] = null;
        coordOccupied[i] = false;
    }


    public Vector3 GetSnappingPoint(int i) 
    { 
        return transform.position + scale * (SurvivorCoordinates[i] + Vector3.up * height);
    }
}


