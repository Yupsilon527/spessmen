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
        Quantity = nQuantity;
        OnCreate();
    }
    public override void OnCreate()
    {
        UpdateVisual();
    }
    public void SetQuantity(int nQ)
    {
        if (nQ == 0)
        {
            Kill();
            return;
        }
        Quantity = nQ;
        UpdateVisual();
    }
    public override void OnActivate(PlayerMob user)
    {
        if (GetNutritionalValue()>0 && user.parent.farmer.FarmingSpot !=null)
        {
            if (!user.parent.farmer.FarmingSpot.FeedItem(this))
                base.OnActivate(user);
        }
        else
        {
            base.OnActivate(user);
        }
    }
    public override float GetNutritionalValue()
    {
        if (Element == TerrainDefines.Element.fertilizer)
            return Quantity;
        return 0;
    }
    public virtual bool OnStoredInBuilding()
    {
        return true;
    }
    private void OnValidate()
    {
        UpdateVisual();
    }
    void UpdateVisual()
    {
        GetComponent<SpriteRenderer>().color = TerrainDefines.ElementColors[(int)Element];
        transform.localScale = Vector3.one *( .1f + Quantity * 0.001f);
    }
    public override void OnMoveToContainer(InventoryComponent ncontainer)
    {
        if (Element == TerrainDefines.Element.gold || Element == TerrainDefines.Element.rock)
        {
            if (ncontainer.IsShop)
            {
                PlayerMob playerOwner = (PlayerMob)container.Owner;
                if (playerOwner != null)
                {
                    ResourceController res = playerOwner.parent.resources;

                    switch (Element)
                    {
                        case TerrainDefines.Element.gold:
                            res.GiveResource(ResourceController.Resources.gold,Quantity);
                            break;
                        case TerrainDefines.Element.rock:
                            res.GiveResource(ResourceController.Resources.wood, Quantity);
                            break;
                    }
                    ncontainer.SellItem(playerOwner.parent,this);
                    return;
                }
            }
        }
        base.OnMoveToContainer(ncontainer);
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
    public override string GetMobName()
    {
        return Element.ToString() + " " + Quantity ;
    }
}
