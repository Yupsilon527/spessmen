using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePowerUp : PowerupComponent
{
    public enum ResType
    {
        gold,
        stone,
        health,
        oxygen
    }
    public ResType ResourceType ;
    public float ResourceAmount;
    public override bool OnBuy(Player owningPlayer)
    {
        switch (ResourceType)
        {
            case ResType.gold:
                owningPlayer.resources.GiveResource(ResourceController.Resources.gold, ResourceAmount);
                break;
            case ResType.stone:
                owningPlayer.resources.GiveResource(ResourceController.Resources.wood, ResourceAmount);
                break;
            case ResType.health:
                owningPlayer.health.Health.GiveValue(ResourceAmount);
                break;

        }
        return true;
    }
}
