using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public Color[] colors;
    public HexMesh hexMesh;

    public List<CellData> data;
    public Transform waypoint;

    HexCellData[] cells;
    Color activeColor;
    int activeIndex;

    [SerializeField] TMP_InputField inputField;

    public Dictionary<Vector3Int, int> posDict;
    Vector3Int[] raftParts = new Vector3Int[3];
    Transform[] waypoints = new Transform[3];


    string mapName = "default";

    void Awake()
    {
        hexMesh = GetComponentInChildren<HexMesh>();

        SelectColor(0);
        posDict = new Dictionary<Vector3Int, int>();
        cells = new HexCellData[width * height];

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
        HexCellData cell = cells[i] = new HexCellData(x, z, activeColor);
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

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCellData cell = cells[index];
        cell.color = activeColor;

        if (activeIndex == 0 && posDict.ContainsKey(coordinates.ToVector()))
          posDict.Remove(coordinates.ToVector());
        else if (activeIndex != 0)
          posDict[coordinates.ToVector()] = activeIndex; 
        

        hexMesh.Triangulate(cells);
    }

    public void SelectColor(int index)
    {
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
        return (posA != posB && posA != posC && posB != posC) && (posDict[posA] == 2 && posDict[posB] == 2 && posDict[posC] == 2);
    }

    public void SaveMap() {
#if UNITY_EDITOR
        GetNewName();
        string path = $"Assets/Maps/{mapName}.asset";
        IslandMapData instance = ScriptableObject.CreateInstance<IslandMapData>();

        instance.SetCells(posDict, data);
        instance.SetRaft(raftParts);

        AssetDatabase.CreateAsset(instance, path);
        AssetDatabase.SaveAssets();
#else
        Debug.Log("not in the editor: how are you working here?");
#endif
    }


}
