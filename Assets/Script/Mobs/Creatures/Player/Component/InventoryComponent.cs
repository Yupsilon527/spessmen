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
        return CarryLimit>0 && Inventory.Count < CarryLimit;
    }
    public bool LoadItem(ItemMob item)
    {
        if (CanLoadItem())
        {
            Inventory.Add(item);
            item.OnMoveToContainer(this);
            return true;
            
        }
        return false;
    }
    public void UnloadItem(ItemMob item)
    {
        UnloadItemAtPosition(item, transform.position);
    }
    public void UnloadItemAtPosition(ItemMob item, Vector2 position)
    {
        if (item == null) return;
        item.transform.position = position;
        if (Inventory.IndexOf(item) < ActiveItem)
            CycleItemLeft();
        Inventory.Remove(item);
        item.container = null;
        item.OnDrop();
        AudioManager.Instance.PlaySfx("Drop Item", 10);
    }
    public void TransferItem(InventoryComponent otherInventory)
    {
        TransferItem(GetActiveItem(), otherInventory);
    }
    public ItemMob GetFirstItemByName(string itemName)
    {
        foreach (ItemMob i in Inventory)
        {
            if (i.GetMobName() == itemName)
            {
                return i;
            }
        }
        return null;
    }
    public void TransferItem(string itemName, InventoryComponent otherInventory)
    {
        TransferItem(GetFirstItemByName(itemName), otherInventory);
    }
    public void TransferItem(ItemMob item, InventoryComponent otherInventory)
    {
        if (item == null)
            return;

            if (!otherInventory.LoadItem(item))
        {
            UnloadItem(item);
        }
        
    }
    public ItemMob GetActiveItem()
    {
        if (Inventory.Count == 0)
            return null;
         return Inventory[ActiveItem];
    }
    public InventoryEntry[] GetInventoryList()
    {
        List<InventoryEntry> entries = new List<InventoryEntry>();
        foreach (ItemMob i in Inventory)
        {
            bool accounted = false;
            foreach (InventoryEntry e in entries)
            {
                if (e.itemName == i.GetMobName())
                {
                    accounted = true;
                    e.itemCount++;
                    break;
                }
            }
            if (!accounted)
            {
                entries.Add(new InventoryEntry( i.GetMobName(), 1));
            }
        }
        return entries.ToArray();
    }
    public class InventoryEntry
    {
        public string itemName;
        public int itemCount;

        public InventoryEntry(string itemName,  int itemCount)
        {
            this.itemName = itemName;
            this.itemCount = itemCount;
        }
    }
    public bool IsShop = false;
    public void SellItem(PlayerMob sellingPlayer, ItemMob item)
    {
        if (Inventory.Contains(item))
        {
            if (Inventory.IndexOf(item) < ActiveItem)
                CycleItemLeft();
            Inventory.Remove(item);
            item.OnSold(sellingPlayer);
            item.Kill();
        }
    }
    public void CycleItemLeft()
    {
        CycleItem(-1);
    }
    public void CycleItemRight()
    {
        CycleItem(1);
    }
    void CycleItem(int dir)
    {
        ActiveItem = Inventory.Count > 0 ? ((ActiveItem + dir) % Inventory.Count) : 0;
        if (ActiveItem < 0)
            ActiveItem += Inventory.Count;
    }
}
