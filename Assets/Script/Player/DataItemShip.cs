using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DataItemShip : DataItemGrid
{
    public ShipScriptable scriptable;
    public bool[,] occupied;
    #region Occupation
    public void ResetOccupancy()
    {
        occupied = new bool[ShipDefines.shipSize, ShipDefines.shipSize];
    }

    public bool IsInsideBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < ShipDefines.shipSize && y < ShipDefines.shipSize;
    }

    public bool IsOccupied(int x, int y)
    {
        return occupied[x, y];
    }

    public void SetOccupied(int x, int y, bool value)
    {
        occupied[x, y] = value;
    }
    #endregion
    #region Placement

    public bool CanPlace(DataItemPart placement)
    {
        bool[,] shape = placement.Decode();
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = placement.originX + x;
                int py = placement.originY + y;

                if (!IsInsideBounds(px, py)) return false;
                if (IsOccupied(px, py)) return false;
            }
        }

        return true;
    }

    public bool TryPlace(DataItemPart placement)
    {
        if (!CanPlace(placement)) return false;

        bool[,] shape = placement.Decode();
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = placement.originX + x;
                int py = placement.originY + y;
                SetOccupied(px, py, true);
            }
        }

        return true;
    }

    public  bool ValidateAll(List<DataItemPart> placements)
    {
        ResetOccupancy();

        foreach (var placement in placements)
        {
            if (!TryPlace( placement))
                return false;
        }

        return true;
    }
    #endregion
}
