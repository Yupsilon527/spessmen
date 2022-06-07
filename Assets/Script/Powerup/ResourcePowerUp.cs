using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePowerUp : PowerupComponent
{
    public enum Type
    {
        gold,
        stone,
        health,
        oxygen
    }
    public Type ResourceType ;
    public float ResourceAmount;
    public override bool OnBuy(Player owningPlayer)
    {
        switch (ResourceType)
        {
            case Type.gold:
                owningPlayer.resources.GiveResource(ResourceController.Resources.gold, ResourceAmount);
                break;
            case Type.stone:
                owningPlayer.resources.GiveResource(ResourceController.Resources.wood, ResourceAmount);
                break;
            case Type.health:
                owningPlayer.health.GiveValue(ResourceAmount);
                break;

        }
        return true;
    }
}
