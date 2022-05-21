using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryComponent))]
public class PlayerCarryItem : MonoBehaviour
{
    public float PickUpRange = 1;
    public Player parent;
    public InventoryComponent Backpack;
    ItemMob WieldedTool;
    ItemMob HauledItem;

    private void Awake()
    {
        Backpack = GetComponent<InventoryComponent>();
    }
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
            if (Backpack.GetActiveItem()!=null)
            {
                Debug.Log("[PlayerCarryItem] Try activate item " + Backpack.name);
                Backpack.GetActiveItem().OnActivate();
            }
            else
            {
                TryPickItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Backpack.GetActiveItem() != null)
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
            Backpack.LoadItem( item);
            return true;
        }
        return false;
    }
    public void DropItem()
    {
        Debug.Log("[PlayerCarryItem] Drop held item " + Backpack.name);
            Backpack.UnloadItem(Backpack.GetActiveItem());
        
    }
}
