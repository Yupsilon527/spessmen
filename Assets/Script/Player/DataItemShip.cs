using System.Collections.Generic;

public class DataItemShip : DataItemGrid
{
    public ShipScriptable scriptable;
    public HashSet<DataItemPart> parts = new();
    public bool[,] occupied;

    public DataItemShip(ShipScriptable so)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
         Encode(so.grid.ToOutputGrid());
        ResetOccupancy();
    }


    #region Occupation
    public void ResetOccupancy()
    {
        occupied = new bool[width, height];
    }

    public bool IsInsideBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public bool IsOccupied(int x, int y)
    {
        if (IsInsideBounds(x, y))
            return occupied[x, y];
        return false;
    }

    public void SetOccupied(int x, int y, bool value)
    {
        occupied[x, y] = value;
    }
    #endregion
    #region Placement

    public bool CanPlace(DataItemPart placement, int oX, int oY)
    {
        bool[,] shape = placement.RetrieveRotated(placement.rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = oX + x;
                int py = oY + y;
                if (!Valid(px, py)) return false;
            }
        }

        return true;
    }
    public bool Valid(int px, int py)
    {

        if (!IsInsideBounds(px, py)) return false;
        if (!_grid[px, py]) return false;
        if (IsOccupied(px, py)) return false;
        return true;
    }

    public bool TryPlace(DataItemPart placement, int oX, int oY)
    {
        if (!CanPlace(placement, oX, oY)) return false;

        placement.originX = oX;
        placement.originY = oY;
        RegisteraPart(placement, true);
        parts.Add(placement);

        return true;
    }
    public void RemovePart(DataItemPart part)
    {
        RegisteraPart(part, false);
        parts.Remove(part);
    }
    void RegisteraPart(DataItemPart part, bool value)
    {
        bool[,] shape = part._grid;
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = part.originX + x;
                int py = part.originY + y;
                SetOccupied(px, py, value);
            }
        }
    }

    public bool ValidateAll(List<DataItemPart> placements)
    {
        ResetOccupancy();

        foreach (var placement in placements)
        {
            if (!TryPlace(placement, placement.originX, placement.originY))
                return false;
        }

        return true;
    }
    #endregion
}
