using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataItemPart : DataItemGrid
{
    public ComponentScriptable scriptable;
    public int rotation = 0;
    public int originX;
    public int originY;
    public override bool[,] Decode()
    {
        return Decode(value, length, length, rotation);
    }
}
