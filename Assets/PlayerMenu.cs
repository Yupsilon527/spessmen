using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerMenu : MobComponent
{
    public Player parent;
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
            lNames.Add(bPrefab.name);
            lActions.Add(() =>
            {
                if (parent.builder.TryBuildBuilding(bPrefab, parent.movement.transform.position + parent.movement.transform.lossyScale.y * Vector3.down))
                source.Kill();
                return true;
            });
        }
        lNames.Add("Close");
        lActions.Add(() => { return true; });
        menuController.OpenAtPosition(lNames.ToArray(), lActions.ToArray(), transform.position);
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
            lNames.Add("Store");
            lActions.Add(() => { OpenBuildingInventoryMenu(inventory); return false; });
        }

        lNames.Add("Exit");
        lActions.Add(() => { parent.movement.ExitBuilding(); return true; });
        menuController.OpenAtPosition(lNames.ToArray(), lActions.ToArray(), transform.position);
    }
    public void OpenBuildingInventoryMenu(InventoryComponent inventory)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        lNames.Add("Back");
        lActions.Add(() => { OpenGeneralMenu(); return false; });
        menuController.OpenAtPosition(lNames.ToArray(), lActions.ToArray(), transform.position);
    }
    public void OpenBuildingShopMenu(ShopComponent store)
    {
        List<string> lNames = new List<string>();
        List<PlayerMenuController.PlayerMenuAction> lActions = new List<PlayerMenuController.PlayerMenuAction>();

        foreach (var entry in store.Shop)
        {
            lNames.Add(entry.Item.name + " ("+ entry.ItemCost + ")");
            lActions.Add(() => { return false; });
        }

        lNames.Add("Back");
        lActions.Add(() => { OpenGeneralMenu(); return false; });
        menuController.OpenAtPosition(lNames.ToArray(), lActions.ToArray(), transform.position);
    }
}