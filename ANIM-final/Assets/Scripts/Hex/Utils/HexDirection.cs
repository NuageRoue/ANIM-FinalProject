using System;
using UnityEngine;

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}


public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }

    public static Vector3Int ToCoords(this HexDirection direction)
    {
        switch (direction)
        {
            case HexDirection.NE:
                return new Vector3Int(0, -1, 1);
            case HexDirection.E:
                return new Vector3Int(1, -1, 0);
            case HexDirection.SE:
                return new Vector3Int(1, 0, -1);
            case HexDirection.SW:
                return new Vector3Int(0, 1, -1);
            case HexDirection.W:
                return new Vector3Int(-1, 1, 0);
            case HexDirection.NW:
                return new Vector3Int(-1, 0, 1);
            default:
                throw new IndexOutOfRangeException();
        }
    }
}