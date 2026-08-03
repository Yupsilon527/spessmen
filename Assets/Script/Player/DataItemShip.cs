using System.Collections.Generic;

public class DataItemShip : DataItemGrid
{
    public ShipScriptable scriptable;
    public HashSet<DataItemPart> stash = new();
    public HashSet<DataItemPart> parts = new();
    public DataItemPart[,] occupied;

    public DataItemShip(ShipScriptable so)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
        ResetOccupancy();

        foreach (var stashed in so.startingParts)
        {
            stash.Add(new DataItemPart(stashed, stashed.GetBasePrice()));
        }
    }


    #region Occupation
    public void ResetOccupancy()
    {
        occupied = new DataItemPart[width, height];
    }
    public bool IsOccupied(int x, int y)
    {
        if (IsInsideBounds(x, y))
            return occupied[x, y]!=null;
        return false;
    }

    public void SetOccupied(int x, int y, DataItemPart part)
    {
        if (IsInsideBounds(x, y))
            occupied[x, y] = part;
    }
    public int CountTilesEmpty()
    {
        int total = 0;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (!mGrid[x, y] && occupied[x, y] == null)
                    total++;
            }
        return total;
    }
    #endregion
    #region Placement

    public bool CanPlace(DataItemPart placement, int oX, int oY, int rotation)
    {
        bool[,] shape = placement.RetrieveRotated(rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);   

        if (placement.scriptable.partType == ItemDefines.PartType.expansion)
        {
            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (!shape[x, y]) continue;

                    int px = oX + x;
                    int py = oY + y;
                    if (!Valid(px, py)) return true;
                }
            }
        }
        else
        {
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
            if (placement.scriptable.attach == ItemDefines.PartCondition.Anywhere) return true;
            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (!shape[x, y]) continue;

                    int px = oX + x;
                    int py = oY + y;
                    if (MeetsCondition(px, py, placement.scriptable.attach)) return true;
                }
            }
        }

        return false;
    }
    public bool Valid(int px, int py)
    {

        if (!IsInsideBounds(px, py)) return false;
        if (!mGrid[px, py]) return false;
        if (IsOccupied(px, py)) return false;
        return true;
    }
    public bool MeetsCondition(int px, int py, ItemDefines.PartCondition condition)
    {
        switch (condition)
        {
            case ItemDefines.PartCondition.Top:
                return IsTopmost( py);
            case ItemDefines.PartCondition.Bottom:
                return IsBottommost( py);
            case ItemDefines.PartCondition.Left:
                return IsLeftmost(px);
            case ItemDefines.PartCondition.Right:
                return IsRightmost(px);
            default:
                return true;
        }
    }

    public bool TryPlace(DataItemPart placement, int oX, int oY, int rotation)
    {
        if (!CanPlace(placement, oX, oY, rotation)) return false;

        placement.originX = oX;
        placement.originY = oY;
        placement.rotation = rotation;
        if (placement.scriptable.partType == ItemDefines.PartType.expansion)
        {
            ExpandGrid(placement);
        }
        else
        {
            RegisteraPart(placement, true);
            parts.Add(placement);
        }
        return true;
    }
    public void RemovePart(DataItemPart part)
    {
        RegisteraPart(part, false);
        parts.Remove(part);
    }
    void RegisteraPart(DataItemPart part, bool value)
    {
        bool[,] shape = part.RetrieveRotated(part.rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = part.originX + x;
                int py = part.originY + y;
                SetOccupied(px, py, value ? part : null);
            }
        }
    }
    void ExpandGrid(DataItemPart part)
    {
        bool[,] shape = part.RetrieveRotated(part.rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = part.originX + x;
                int py = part.originY + y;
                SetValue(px, py, true);
            }
        }
    }

    public bool ValidateAll()
    {
        ResetOccupancy();

        foreach (var placement in parts)
        {
            if (!TryPlace(placement, placement.originX, placement.originY,placement.rotation))
            {
                return false;
            }
        }

        return true;
    }
    #endregion
}
