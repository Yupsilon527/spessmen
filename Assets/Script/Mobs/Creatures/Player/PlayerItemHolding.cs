using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryComponent))]
public class PlayerItemHolding : MonoBehaviour
{
    public float PickUpRange = 1;
    public Player parent;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            ItemMob item = parent.backpack.GetActiveItem();
            if (item != null)
            {
                if (!item.RequiresGroundToUse || parent.movement.IsGrounded())
                {
                    Debug.Log("[PlayerCarryItem] Try activate item " + item.name);
                    item.OnActivate(parent.movement);
                }
            }
            else
            {
                TryPickItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (parent.backpack.GetActiveItem() != null)
            {
                DropItem();
            }
        }
    }
    void TryPickItem()
    {
        Debug.Log("[PlayerCarryItem] Try pick up items");
        foreach (RaycastHit2D rch in Physics2D.CircleCastAll(transform.position,PickUpRange,Vector2.zero))
        {
            if (rch.transform.tag == "Item" && rch.transform.TryGetComponent(out ItemMob item))
            {
                if (PickUpItem(item))
                    break;
            }
        }
    }
    bool PickUpItem(ItemMob item)
    {
        if (item.category == ItemMob.Category.small)
        {
            Debug.Log("[PlayerCarryItem] Pick up " + item.name);
            
            return parent.backpack.LoadItem(item);
        }
        return false;
    }
    public void DropItem()
    {
        Debug.Log("[PlayerCarryItem] Drop held item " + parent.backpack.name);
        parent.backpack.UnloadItem(parent.backpack.GetActiveItem());
        
    }
}
