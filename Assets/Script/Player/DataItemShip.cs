using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DataItemShip : DataItemGrid
{
    public ShipScriptable scriptable;
    public HashSet<DataItemPart> stash = new();
    public HashSet<DataItemPart> parts = new();
    public DataItemPart[,] occupied;

    public DataItemPart lastAppliedExpansion;
    public HashSet<Vector2Int> lastExpansionSlots = new();
    public DataItemShip(ShipScriptable so, bool giveStart)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
        ResetOccupancy();
        if (giveStart)
        {
            foreach (var stashed in so.startingParts)
            {
                stash.Add(new DataItemPart(stashed, stashed.GetBasePrice()));
            }
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
            return occupied[x, y] != null;
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

                    if (!GetValue(px, py)
                            && (GetValue(px + 1, py)
                            || GetValue(px - 1, py)
                            || GetValue(px, py + 1)
                            || GetValue(px, py - 1))) return true;
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

        if (!GetValue(px, py)) return false;
        if (IsOccupied(px, py)) return false;
        return true;
    }
    public bool MeetsCondition(int px, int py, ItemDefines.PartCondition condition)
    {
        switch (condition)
        {
            case ItemDefines.PartCondition.Top:
                return IsTopmost(py);
            case ItemDefines.PartCondition.Bottom:
                return IsBottommost(py);
            case ItemDefines.PartCondition.Back:
                return IsLeftmost(px);
            case ItemDefines.PartCondition.Front:
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
            ExpandGrid(placement, true);
        }
        else
        {
            RegisterPart(placement, true);
            parts.Add(placement);
        }
        return true;
    }
    public DataItemPart[] GetPartsOccupying(DataItemPart placement, int oX, int oY, int rotation)
    {
        HashSet<DataItemPart> parts = new();

        bool[,] shape = placement.RetrieveRotated(placement.rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = placement.originX + x;
                int py = placement.originY + y;

                if (occupied[x, y] != null)
                    parts.Add(occupied[x, y]);
            }
        }
        return parts.ToArray();
    }
    #endregion
    #region Place/Remove Parts
    public void RemovePart(DataItemPart part)
    {
        RegisterPart(part, false);
        parts.Remove(part);
    }
    void RegisterPart(DataItemPart part, bool value)
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

    public bool ValidateAll()
    {
        ResetOccupancy();

        foreach (var placement in parts)
        {
            if (!TryPlace(placement, placement.originX, placement.originY, placement.rotation))
            {
                return false;
            }
        }

        return true;
    }
    #endregion
    #region Undo
    void ExpandGrid(DataItemPart part, bool registerExpansion)
    {
        bool[,] shape = part.RetrieveRotated(part.rotation);
        int shapeWidth = shape.GetLength(0);
        int shapeHeight = shape.GetLength(1);

        if (registerExpansion)
        {
            ClearUndo();
            lastAppliedExpansion = part;
        }

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                if (!shape[x, y]) continue;

                int px = part.originX + x;
                int py = part.originY + y;
                if (GetValue(px, py)) continue;
                SetValue(px, py, true);
                if (registerExpansion)
                    lastExpansionSlots.Add(new Vector2Int(px, py));
            }
        }
    }
    public void UndoLastExpansion()
    {
        lastAppliedExpansion.deleted = false;

        foreach (var t in lastExpansionSlots)
        {
            SetValue(t.x, t.y, false);
        }
        ClearUndo();
    }
    public bool CanUndoExpansion()
    {
        return lastAppliedExpansion != null;
    }
    public void ClearUndo()
    {
        lastAppliedExpansion = null;
        lastExpansionSlots.Clear();
    }
    #endregion
}
