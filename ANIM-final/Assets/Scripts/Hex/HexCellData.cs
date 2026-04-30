using UnityEngine;

public class HexCellData
{
    public HexCoordinates coordinates;
    public Color color;
    public Vector3 Position { get; private set; }

    HexCellData[] neighbors = new HexCellData[6];

    public HexCellData(int x, int z, Color defaultColor)
    {
        coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        color = defaultColor;
        Position = new Vector3(
            (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f),
            0f,
            z * (HexMetrics.outerRadius * 1.5f)
        );
    }

    public HexCellData GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCellData cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}