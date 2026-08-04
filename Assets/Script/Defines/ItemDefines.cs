
using UnityEngine;

public static class ItemDefines
{
    public static float commonSpawnWeight = 100;
    public static float raritySpawnWeight = 60;

    public static float chaosPerRace = 20;
    public static float chaosPerShopReset = 10;
    public static float chaosFromCommon = 10;
    public static float chaosFromRare = 20;
    public static float chaosFromEpic = 33;
    public static float chaosFromLegendary = 50;

    public static float chaosPlus = 50;
    public static float chaosMinus = 30;
    public static float luckPlus = 7;

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
        gadget = 4,
        nitro = 5,
        decal = 6,
        expansion = 7,
    }
    public enum PartCondition
    {
        Anywhere,
        Bottom,
        Left,
        Right,
        Top
    }

    public static float LuckNumber(float luck)
    {
        if (luck >= 0)
            return   (luck + luckPlus) / luckPlus;
        else
            return  - luckPlus / (luck - luckPlus);
    }
    public static float ChaosNumber(float chaos)
    {
        if (chaos >= 0)
            return  (chaos + chaosPlus) / chaosPlus;
        else
            return  - chaosMinus / (chaos - chaosMinus);
    }
    public static Color GetColorForRarity(BoonRarity rarity)
    {
        switch (rarity)
        {
            default:
                return Color.black;
            case BoonRarity.common:
                return new Color(82, 129, 74);
            case BoonRarity.rare:
                return new Color(53, 159, 209);
            case BoonRarity.epic:
                return new Color(175, 66, 183);
            case BoonRarity.legendary:
                return new Color(238, 231, 65);

        }
    }
}