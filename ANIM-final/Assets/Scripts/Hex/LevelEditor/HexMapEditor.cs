using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HexMapEditor : MonoBehaviour
{
    /*
    public int width = 6;
    public int height = 6;
    public Color[] colors;
    public HexMesh hexMesh;

    public List<CellData> data;
    public Transform waypoint;


    HexMeshCell[] cells;
    Color activeColor;
    int activeIndex;

    [SerializeField] TMP_InputField inputField;

    public Dictionary<Vector3Int, int> posDict;
    Vector3Int[] raftParts = new Vector3Int[3];
    Transform[] waypoints = new Transform[3];
    Transform startingWaypoint;
    Vector3Int startingPoint;

    string mapName = "default";

    void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        SelectColor(0);
        posDict = new Dictionary<Vector3Int, int>();
        cells = new HexMeshCell[width * height];

        for (int z = 0, i = 0; z < height; z++)
            for (int x = 0; x < width; x++)
                CreateCell(x, z, i++);
    }

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i)
    {
        HexMeshCell cell = cells[i] = new HexMeshCell(x, z, activeColor);
        // posDict.Add(HexCoordinates.FromOffsetCoordinates(x, z).ToVector(), 0);

        if (x > 0)
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);

        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            HandleInput();
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
            ColorCell(hit.point);
    }

    void ColorCell(Vector3 position)
    {
        
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        if (activeIndex == -1)
        {
            if (posDict.ContainsKey(coordinates.ToVector()) && (CellType)(posDict[coordinates.ToVector()] - 1) == CellType.coast) { 
                startingPoint = coordinates.ToVector();
                if (startingWaypoint != null)
                    Destroy(startingWaypoint.gameObject);
                startingWaypoint = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(coordinates.ToVector()), Quaternion.identity, transform);
            }// starting point
            return;
        }

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexMeshCell cell = cells[index];
        cell.color = activeColor;

        if (activeIndex == 0 && posDict.ContainsKey(coordinates.ToVector()))
          posDict.Remove(coordinates.ToVector());
        else if (activeIndex != 0)
          posDict[coordinates.ToVector()] = activeIndex; 
        

        hexMesh.Triangulate(cells);
    }

    private bool IsFreeForEvents(Vector3Int pos)
    {
        return (data[posDict[pos] - 1].isFreeForEvents && startingPoint != pos && !raftParts.Contains(pos));
    }

    public void SelectColor(int index)
    {
        if (index >= 0)
            activeColor = colors[index];
        activeIndex = index;
    }

    void GetNewName() { 
        if (inputField.text.Length != 0)
            mapName = inputField.text;
    }

    public void RandomRaftParts() {
      if (posDict.Count < 3)
      {
        Debug.Log("not enough tiles");
        return;
      }
      Vector3Int posA, posB, posC;

      do {
        posA = posDict.Keys.ToList()[Random.Range(0, posDict.Count)];
        posB = posDict.Keys.ToList()[Random.Range(0, posDict.Count)];
        posC = posDict.Keys.ToList()[Random.Range(0, posDict.Count)];
      } while (!differentEnough(posA, posB, posC));

      raftParts[0] = posA;
      raftParts[1] = posB;
      raftParts[2] = posC;

      Debug.Log($"position des pièces du radeau : {posA}, {posB} et {posC}");

      for (int i = 0; i < waypoints.Count(); i++) {

        if (waypoints[i] != null) {
          Destroy(waypoints[i].gameObject);
          waypoints[i] = null;
        }
      }
      waypoints[0] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posA), Quaternion.identity, transform);
      waypoints[1] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posB), Quaternion.identity, transform);
      waypoints[2] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posC), Quaternion.identity, transform);
    }

    bool differentEnough(Vector3Int posA, Vector3Int posB, Vector3Int posC) {
        return (posA != posB && posA != posC && posB != posC) && (IsFreeForEvents(posA) && IsFreeForEvents(posB) && IsFreeForEvents(posC));
    }

    public void SaveMap() {
#if UNITY_EDITOR
        GetNewName();
        string path = $"Assets/Maps/{mapName}.asset";
        IslandMapData instance = ScriptableObject.CreateInstance<IslandMapData>();

        instance.SetCells(posDict, data);
        instance.SetRaft(raftParts);
        instance.SetStartingPos(startingPoint);

        AssetDatabase.CreateAsset(instance, path);
        AssetDatabase.SaveAssets();
#else
        Debug.Log("not in the editor: how are you working here?");
#endif
    }

    */

    #region parameters
    public int width = 30;
    public int height = 30;

    [Header("parameters:")]
    public Color[] colors;
    public List<CellData> data;


    [Header("other gameobjects:")]
    public HexMesh hexMesh;
    public Transform waypoint;
    public TMP_InputField inputField;
    #endregion

    #region values
    string mapName = "default";
    Vector3Int[] raftPos = new Vector3Int[3];
    Vector3Int startingPos;
    Dictionary<Vector3Int, CellType> cellPositions;
    #endregion

    int activeIndex;
    Transform[] raftWaypoints = new Transform[3];
    Transform startingWaypoint;
    HexMeshCell[] cells;

    void Update()
    {
        if (Input.GetMouseButton(0))
            HandleInput();
    }

    #region start
    void Awake()
    {
        ChangeCursor(0);
        cellPositions = new Dictionary<Vector3Int, CellType>();
        SetMesh();
    }
    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void CreateCell(int x, int z, int i)
    {
        HexMeshCell cell = cells[i] = new HexMeshCell(x, z, colors[0]);
        // posDict.Add(HexCoordinates.FromOffsetCoordinates(x, z).ToVector(), 0);

        if (x > 0)
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);

        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
            }
        }
    }
    void SetMesh() 
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexMeshCell[width * height];

        for (int z = 0, i = 0; z < height; z++)
            for (int x = 0; x < width; x++)
                CreateCell(x, z, i++);
    }

    #endregion

    public void ChangeCursor(int index) 
    {
        activeIndex = index;
    }

    #region update
    public void HandleInput() 
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
            UpdateCell(hit.point);
    }
    void UpdateCell(Vector3 pos) 
    {
        if (activeIndex == -1)
            UpdateStartPos(pos);
        else {
            UpdateCellType(pos);
            UpdateMesh(pos);
        }
            
    }
    void UpdateMesh(Vector3 pos) 
    {
        Vector3 position = transform.InverseTransformPoint(pos);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexMeshCell cell = cells[index];
        cell.color = colors[activeIndex];

        hexMesh.Triangulate(cells);
    }
    void UpdateCellType(Vector3 pos)
    {
        Vector3 position = transform.InverseTransformPoint(pos);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        if (activeIndex == 0 && cellPositions.ContainsKey(coordinates.ToVector()))
            cellPositions.Remove(coordinates.ToVector());
        else if (activeIndex != 0)
            cellPositions[coordinates.ToVector()] = (CellType)(activeIndex - 1);
    }
    void UpdateStartPos(Vector3 pos) 
    {
        Vector3 position = transform.InverseTransformPoint(pos);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        
        if (IsValidStartPos(coordinates.ToVector())) {
            Debug.Log($"starting pos: {coordinates.ToVector()}");
            UpdateStartingWaypoint(coordinates.ToVector());
            startingPos = coordinates.ToVector();
        }
    }

    bool IsValidStartPos(Vector3Int pos) 
    {
        foreach (var raftPos in raftPos) {
            if (raftPos != null && pos == raftPos)
                return false;
        }

        return (cellPositions.ContainsKey(pos) && cellPositions[pos] == CellType.coast);
    }
    void UpdateStartingWaypoint(Vector3Int pos) {
        if (startingWaypoint != null)
            Destroy(startingWaypoint.gameObject);
        startingWaypoint = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(pos), Quaternion.identity, transform);
    }

    public void SetRaftParts() 
    {
        if (cellPositions.Count < 3)
        {
            Debug.Log("not enough tiles");
            return;
        }

        Vector3Int posA, posB, posC;

        do
        {
            posA = cellPositions.Keys.ToList()[Random.Range(0, cellPositions.Count)];
            posB = cellPositions.Keys.ToList()[Random.Range(0, cellPositions.Count)];
            posC = cellPositions.Keys.ToList()[Random.Range(0, cellPositions.Count)];
        } while (!DifferentEnough(posA, posB, posC) || !IsValid(posA) || !IsValid(posB) || !IsValid(posC));

        raftPos[0] = posA;
        raftPos[1] = posB;
        raftPos[2] = posC;

        Debug.Log($"position des pièces du radeau : {posA}, {posB} et {posC}");

        for (int i = 0; i < raftWaypoints.Count(); i++)
        {

            if (raftWaypoints[i] != null)
            {
                Destroy(raftWaypoints[i].gameObject);
                raftWaypoints[i] = null;
            }
        }
        raftWaypoints[0] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posA), Quaternion.identity, transform);
        raftWaypoints[1] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posB), Quaternion.identity, transform);
        raftWaypoints[2] = Instantiate(waypoint, HexCoordinates.CoordsToWorldPosition(posC), Quaternion.identity, transform);
    }

    bool DifferentEnough(Vector3Int posA, Vector3Int posB, Vector3Int posC)
    {
        return (!posA.Equals(posB) && !posA.Equals(posC) && !posC.Equals(posB)) 
            && 
            (startingPos == null || (!posA.Equals(startingPos) && !posB.Equals(startingPos) && !posC.Equals(startingPos)));
    }

    bool IsValid(Vector3Int pos) {
        Debug.Log($"- trying to check the type {(int)cellPositions[pos]}");
        return (cellPositions.ContainsKey(pos) && data[(int)cellPositions[pos]].canHaveEvent);
    }


    #endregion

    #region save

    void SetMapName()
    {
        if (inputField.text.Length != 0)
            mapName = inputField.text;
    }
    public void SaveMap() 
    {
#if UNITY_EDITOR
        SetMapName();
        string path = $"Assets/Maps/{mapName}.asset";
        IslandMapData instance = ScriptableObject.CreateInstance<IslandMapData>();

        instance.SetCells(cellPositions, data);
        instance.SetRaftPos(raftPos);
        instance.SetStartingPos(startingPos);

        AssetDatabase.CreateAsset(instance, path);
        AssetDatabase.SaveAssets();
#else
        Debug.Log("not in the editor: how are you working here?");
#endif
    }

    #endregion
}
