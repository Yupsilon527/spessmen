using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    public int CarryLimit = 1;
    int ActiveItem = 0;
    public Mob Owner;
    public List<ItemMob> Inventory;
    private void Awake()
    {
        Inventory = new List<ItemMob>();
        Owner = GetComponent<Mob>();
    }

    public bool CanLoadItem()
    {
        return Inventory.Count < CarryLimit;
    }
    public void LoadItem(ItemMob item)
    {
        Inventory.Add(item);
        item.container = this;
        item.OnPickup();
    }
    public void UnloadItem(ItemMob item)
    {
        UnloadItemAtPosition(item, transform.position);
    }
    public void UnloadItemAtPosition(ItemMob item, Vector2 position)
    {
        item.transform.position = position;
        Inventory.Remove(item);
        item.container = null;
        item.OnDrop();
    }
    public ItemMob GetActiveItem()
    {
        if (Inventory.Count == 0)
            return null;
        return Inventory[ActiveItem];
    }
}
