using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePowerup : PowerupComponent
{
    public enum UpgradeType
    {
        mining,
        jetpack
    }
    public UpgradeType Upgrade;
    public override bool OnBuy(Player owningPlayer)
    {
        switch (Upgrade)
        {
            case UpgradeType.jetpack:
                owningPlayer.stats.UpgradeJumps();
                break;
            case UpgradeType.mining:
                owningPlayer.stats.UpgradeDigging();
                break;

        }
        return true;
    }
}
