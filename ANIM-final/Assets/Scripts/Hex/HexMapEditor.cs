using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public Color[] colors;
    public HexMesh hexMesh;

    HexCellData[] cells;
    Color activeColor;
    int activeIndex;

    [SerializeField] TMP_InputField inputField;

    public Dictionary<Vector3Int, int> posDict;
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
        posDict.Add(HexCoordinates.FromOffsetCoordinates(x, z).ToVector(), 0);

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

    public void SaveMap() {
#if UNITY_EDITOR
        GetNewName();
        string path = $"Assets/Maps/{mapName}.asset";
        IslandMapData instance = ScriptableObject.CreateInstance<IslandMapData>();

        instance.SetCells(posDict);

        AssetDatabase.CreateAsset(instance, path);
        AssetDatabase.SaveAssets();
#else
        Debug.Log("not in the editor: how are you working here?");
#endif
    }
}