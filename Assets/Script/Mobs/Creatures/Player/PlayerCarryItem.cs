using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarryItem : MonoBehaviour
{
    public float PickUpRange = 1;
    public Player parent;
    ItemMob CarriedItem;
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
            if (CarriedItem != null)
            {
                Debug.Log("[PlayerCarryItem] Try activate item " + CarriedItem.name);
                CarriedItem.OnActivate();
            }
            else
            {
                TryPickItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CarriedItem != null)
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
            CarriedItem = item;
            CarriedItem.OnPickupByCreature(this);
            return true;
        }
        return false;
    }
    public void DropItem()
    {
        Debug.Log("[PlayerCarryItem] Drop held item " + CarriedItem.name);
        CarriedItem.OnDroppedByCreature();
        CarriedItem = null;
    }
}
