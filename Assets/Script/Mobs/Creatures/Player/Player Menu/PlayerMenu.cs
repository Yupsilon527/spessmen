using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerMenu : PlayerComponent
{
    public PlayerMenuController menuController;
    private void Start()
    {
        menuController.Close();
    }

    public void OpenConstructionMenu(BuildingKitItem source)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        foreach (GameObject bPrefab in source.AllowedBuildings)
        {
            if (bPrefab.TryGetComponent(out Mob mob))
            {
                lNames.Add(mob.GetMobName());
                lActions.Add(() =>
                {
                    if (parent.builder.TryBuildBuilding(bPrefab, parent.movement.transform.position))
                    {
                        //SFX build a new building sound
                       // AudioManager.Instance.PlaySfx("Build", 7);
                        source.Kill();
                    }
                    return true;
                });
            }
        }
        lNames.Add("Close");
        lActions.Add(() => { return true; });
        menuController.OpenAtTarget(lNames.ToArray(), lActions.ToArray(), transform);
    }
    public void CloseMenu()
    {
        indoorHouse = null;
        menuController.Close();
    }

    HouseMob indoorHouse;
    public void OpenIndoorsMenu(HouseMob house)
    {
        indoorHouse = house;
        OpenGeneralMenu();
    }
    void OpenGeneralMenu()
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        if (indoorHouse.TryGetComponent(out ShopComponent store))
        {
            lNames.Add("Buy");
            lActions.Add(() => { OpenBuildingShopMenu(store); return false; });

        }

        if (indoorHouse.TryGetComponent(out InventoryComponent inventory))
        {
            if (inventory.IsShop)
            {
                lNames.Add("Sell");
                lActions.Add(() => { OpenSellMenu(inventory); return false; });
            }

            lNames.Add("Inventory");
            lActions.Add(() => { OpenWithdrawMenu(inventory); return false; });
        }

        lNames.Add("Deposit Item");
        lActions.Add(() => {
            if (parent.backpack.GetActiveItem()!= null && parent.IsInside() && parent.backpack.GetActiveItem() != null && parent.indoor.TryGetComponent(out InventoryComponent inventory))
            {
                inventory.LoadItem(parent.backpack.GetActiveItem());
            }
            return false;
        });

        lNames.Add("Exit");
        lActions.Add(() => { parent.ExitBuilding(); return true; });
        menuController.OpenAtTarget(lNames.ToArray(), lActions.ToArray(), indoorHouse.transform);
    }
    public void OpenWithdrawMenu(InventoryComponent inventory)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        foreach (InventoryComponent.InventoryEntry item in inventory.GetInventoryList())
        {
            lNames.Add(item.itemName + " (" + item.itemCount + ")");
            lActions.Add(() => { inventory.TransferItem(item.itemName, parent.backpack); OpenWithdrawMenu(inventory); return false; });
        }


        lNames.Add("Back");
        lActions.Add(() => { OpenGeneralMenu(); return false; });
        menuController.OpenAtTarget(lNames.ToArray(), lActions.ToArray(), inventory.transform);
    }
    public void OpenSellMenu(InventoryComponent inventory)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        foreach (InventoryComponent.InventoryEntry item in inventory.GetInventoryList())
        {
            lNames.Add(item.itemName + " (" + item.itemCount + ")");
            lActions.Add(() => {
                //SFX sold an item
                AudioManager.Instance.PlaySfx("Sell", 9);
                inventory.SellItem(parent, inventory.GetFirstItemByName(item.itemName)); 
                OpenSellMenu(inventory); 
                return false; 
            });
        }


        lNames.Add("Back");
        lActions.Add(() => { OpenGeneralMenu(); return false; });
        menuController.OpenAtTarget(lNames.ToArray(), lActions.ToArray(), inventory.transform);
    }
    public void OpenBuildingShopMenu(ShopComponent store)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        foreach (ShopComponent.ShopEntry entry in store.Shop)
        {
            if (entry.Item.TryGetComponent(out Mob mobble))
                lNames.Add(mobble.GetMobName() + " (" + entry.Cost + "g)");
            else 
                lNames.Add(entry.Item.name + " ("+ entry.Cost + "g)");
            lActions.Add(() => { if (store.CanPlayerBuyItem(parent,entry)) { store.BuyItemForPlayer(parent, entry); } return false; });
        }

        lNames.Add("Back");
        lActions.Add(() => { OpenGeneralMenu(); return false; });
        menuController.OpenAtTarget(lNames.ToArray(), lActions.ToArray(), store.transform);
    }
}