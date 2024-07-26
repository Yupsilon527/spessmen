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
    public override bool OnBuy(PlayerMob owningPlayer)
    {
        switch (Upgrade)
        {
            case UpgradeType.jetpack:
                owningPlayer.UpgradeJumps();
                break;
            case UpgradeType.mining:
                owningPlayer.UpgradeDigging();
                break;

        }
        return true;
    }
}
