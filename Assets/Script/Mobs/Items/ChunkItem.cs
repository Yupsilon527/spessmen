using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkItem : ItemMob
{
    public int Quantity;
    public TerrainDefines.Element Element;

    public void ChangeElement(TerrainDefines.Element nElement, int nQuantity)
    {
        if (nElement == TerrainDefines.Element.nothing || nQuantity == 0)
        {
            Kill();
            return;
        }
        Element = nElement;
        GetComponent<SpriteRenderer>().color = TerrainDefines.ElementColors[(int)Element];
        Quantity = nQuantity;
    }

    public void IncreaseQuantity(int q)
    {
        Quantity += q;
    }
    public void DecreaseQuantity(int q)
    {
        Quantity -= q;
        if (Quantity<=0)
        {
            Kill();
        }
    }
    public override void Kill()
    {
        Element = TerrainDefines.Element.nothing;
        base.Kill();
    }
}
