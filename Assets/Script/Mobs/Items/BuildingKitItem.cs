using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingKitItem : ItemMob
{
    public GameObject[] AllowedBuildings;
    public override void OnActivate(PlayerMob user)
    {
        user.menu.OpenConstructionMenu(this);
        
    }
}
