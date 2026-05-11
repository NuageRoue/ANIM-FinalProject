using UnityEngine;
using UnityEngine.UI;


public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    public float height = 0;
    float scale = 7;
    GameObject prop;

    [SerializeField]
    Image eventUI;
    Image _eventUI;

    public float UIOffset;

    [SerializeField]
    Cloud cloud;
    Cloud _cloud;

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

        hasBeenExplored = false ;
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
    internal void Setup(GameObject selectedProp, CallEvent selectedEvent, bool isTraversable, Canvas canvas, bool startingCell = false)
    {
        if (selectedProp != null)
            prop = GameObject.Instantiate(selectedProp, transform.position + Vector3.up * height, Quaternion.identity, transform);

        if (selectedEvent != null) {
            name += $"(event: {selectedEvent.name})";
            callEvent = selectedEvent;
            if (callEvent.sprite != null)
            {
                _eventUI = Instantiate(eventUI, new Vector3(transform.position.x, UIOffset, transform.position.z), Quaternion.Euler(50, 0, 0), canvas.transform);
                _eventUI.sprite = callEvent.sprite;
            }
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

        _cloud = Instantiate(cloud, transform);
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
        for (int j = 0; j < coordOccupied.Length; j++)
        {
            if (survivorOnTiles[j] == s)
                return GetSnappingPoint(j);
        }
        s.currentCell?.FreePosition(s);
        int i = GetAvailablePosition();
        survivorOnTiles[i] = s;
        s.currentCell = this;

        CleanFog(s.visionRadius);
        return GetSnappingPoint(i);
    }

    private void FreePosition(Survivor s)
    {
        Debug.Log("freeing the pos");
        for (int i = 0; i < 3; i++)
        {
            if (survivorOnTiles[i] == s)
            {
                survivorOnTiles[i] = null;
                coordOccupied[i] = false;

                return;
            }
        }
    }


    public Vector3 GetSnappingPoint(int i) 
    { 
        return transform.position + scale * (SurvivorCoordinates[i] + Vector3.up * height);
    }


    public void SetHighlight(int layer) 
    {
        gameObject.layer = layer;
        foreach (Transform child in transform)
            child.gameObject.layer = layer;
    }

    public bool HasEvent() 
    { 
        return callEvent != null;
    }

    public void CleanFog(int dist) 
    {
        if (dist < 0) return;
        if (_cloud != null) _cloud.CleanCloud();

        foreach (var neighbor in neighbors)
        {
            neighbor?.CleanFog(dist - 1);
        }
    }

    public void ClearEvent() 
    {
        callEvent = null;
        Destroy(_eventUI);
    }

    internal void CallEvent(Survivor s, System.Action<bool> unloadEvent)
    {
        callEvent.Trigger(s, unloadEvent);
    }
}


