using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BuildingKitItem : ItemMob
{
    public GameObject[] AllowedBuildings;
    public override void OnActivate(PlayerMob user)
    {
        if (AllowedBuildings.Length == 1)
        {

            if (user.builder.TryBuildBuilding(AllowedBuildings[0], user.movement.transform.position))
            {
                Erase();
            }
        }
        else 
         user.menu.OpenConstructionMenu(this);
    }
}
