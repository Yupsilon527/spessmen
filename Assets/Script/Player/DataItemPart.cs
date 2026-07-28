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
        value = Encode(so.grid.ToOutputGrid());
    }
    public void Rotate(bool clockwise)
    {
        rotation = (rotation + (clockwise ? 1 : -1)) % 4;
    }
    public override bool[,] Decode()
    {
        return Decode(value, width, width, rotation);
    }
}
