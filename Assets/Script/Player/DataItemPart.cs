using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataItemPart : DataItemGrid
{
    public PartScriptable scriptable;
    public int rotation = 0;
    public int originX;
    public int originY;

    public DataItemPart(PartScriptable so)
    {
        scriptable = so;
        width = so.grid.width;
        height = so.grid.height;
        Encode(so.grid.ToOutputGrid());
    }
    public PartAbility GetAbility()
    {
        return scriptable.ability;
    }
    public void Rotate(bool clockwise)
    {
        rotation = (rotation + (clockwise ? 1 : -1)) % 4;
    }
    public bool CanBeDiscarded()
    {
        return true;
    }
}
