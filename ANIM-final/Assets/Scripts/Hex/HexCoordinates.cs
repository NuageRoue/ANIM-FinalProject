using System;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{

    [SerializeField]
    private int x, z;

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Z
    {
        get
        {
            return z;
        }
    }

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public override string ToString()
    {
        return "(" +
            X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }


        return new HexCoordinates(iX, iZ);
    }

    internal Vector3Int ToVector()
    {
        return new Vector3Int(X, Y, Z);
    }

    public static Vector3 CoordsToWorldPosition(Vector3Int coords)
    {
        int z = coords.z;
        int x = coords.x + z / 2;
        return new Vector3(
            (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f),
            0f,
            z * (HexMetrics.outerRadius * 1.5f)
        );
    }
}