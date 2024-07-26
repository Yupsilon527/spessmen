using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryComponent))]
public class PlayerItemHolding : PlayerComponent, iItemToucher
{
    public float PickUpRange = 1;
    ItemMob WieldedTool;
    ItemMob HauledItem;

    void Update()
    {
        HandleItemHauling();
        HandleItemPickup();
    }
    void HandleItemHauling()
    {

    }
    void HandleItemPickup()
    {
        if (Input.GetButtonDown("Use"))
        {
            if (TryPickItem())
            {
                ItemMob item = parent.backpack.GetActiveItem();
                if (item != null)
                {
                    if (!item.RequiresGroundToUse || parent.IsGrounded())
                    {
                        Debug.Log("[PlayerCarryItem] Try activate item " + item.name);
                        item.OnActivate(parent);
                        
                    }
                }
            }
        }
        if (Input.GetButtonDown("Dig"))
        {
            if (parent.backpack.GetActiveItem() != null)
            {
                DropItem();
            }
        }
        if (Input.GetButtonDown("CycleRight"))
        {
            parent.backpack.CycleItemRight();
        }
        if (Input.GetButtonDown("CycleLeft"))
        {
            parent.backpack.CycleItemLeft();
        }
    }

    public List<ItemMob> TouchedItems = new List<ItemMob>();
    public void OnTouchEnter(ItemMob item)
    {
        if (!TouchedItems.Contains(item))
            TouchedItems.Add(item);
        



    }
    public void OnTouchExit(ItemMob item)
    {
        if (TouchedItems.Contains(item))
            TouchedItems.Remove(item);

    }
    public ItemMob GetTouchedItem()
    {
        if (TouchedItems.Count > 0)
            return TouchedItems[0];
        return null;
        

    }
    bool TryPickItem()
    {
        Debug.Log("[PlayerCarryItem] Try pick up items");
        if (GetTouchedItem() != null) 
            return !PickUpItem(GetTouchedItem());

        /*foreach (RaycastHit2D rch in Physics2D.CircleCastAll(transform.position,PickUpRange,Vector2.zero))
        {
            if (rch.transform.tag == "Item" && rch.transform.TryGetComponent(out ItemMob item))
            {
                if (PickUpItem(item))
                    break;
            }
        }*/
        return true;
    }
    bool PickUpItem(ItemMob item)
    {
        if (item.category == ItemMob.Category.small)
        {
            Debug.Log("[PlayerCarryItem] Pick up " + item.name);            
            return parent.backpack.LoadItem(item);
            

        }
        return true;
        
    }
    public void DropItem()
    {
        Debug.Log("[PlayerCarryItem] Drop held item " + parent.backpack.name);
        parent.backpack.UnloadItem(parent.backpack.GetActiveItem());
        

    }
}
