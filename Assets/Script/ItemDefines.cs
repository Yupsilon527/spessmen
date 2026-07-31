
using UnityEngine;

public static class ItemDefines
{
    public static float baseSpawnWeight = 4;
    public static float raritySpawnWeight = -1;

    public static float chaosPerRace = 20;
    public static float chaosPerShopReset = 10;
    public static float chaosFromCommon = 10;
    public static float chaosFromRare = 20;
    public static float chaosFromEpic = 33;
    public static float chaosFromLegendary = 50;

    public static float chaosPlus = 50;
    public static float chaosMinus = 30;
    public static float luckPlus = 25;

    public static int RerollTriesAmount = 15;
    public enum BoonRarity
    {
        common = 0,
        rare = 1,
        epic = 2,
        legendary = 3,
    }
    public enum PartType
    {
        other = 0,
        wheel=1,
        tank = 2,
        engine = 3,
        active = 4,
    }

    public static float LuckNumber(DataItemPlayer player)
    {
        if (player != null)
            return LuckNumber(player.score.playerChaos);
        return Random.value;
    }

    public static float LuckNumber(float luck)
    {
        if (luck > 0)
            return Random.value * (luckPlus + luck) / luckPlus;
        else
            return (Random.value + Random.value * luckPlus / (luckPlus + luck)) * -.5f;
    }
}