using UnityEngine;

public class PlayerChaosController : PlayerComponent
{
    public float playerChaos = 0;

    public override void Setup()
    {
        base.Setup();
        playerChaos = 0;
    }

    public void GiveLuck(ItemDefines.BoonRarity rarity)
    {
        switch (rarity)
        {
            case ItemDefines.BoonRarity.common:
                RemoveChaos(ItemDefines.chaosFromCommon);
                break;
            case ItemDefines.BoonRarity.rare:
                RemoveChaos(ItemDefines.chaosFromRare);
                break;
            case ItemDefines.BoonRarity.epic:
                RemoveChaos(ItemDefines.chaosFromEpic);
                break;
            case ItemDefines.BoonRarity.legendary:
                RemoveChaos(ItemDefines.chaosFromLegendary);
                break;
        }
    }

    public void GiveChaos(float value)
    {
        playerChaos += value;
    }

    public void RemoveChaos(float value)
    {
        playerChaos -= value;
    }
}
