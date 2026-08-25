
using UnityEngine;

public static class ItemDefines
{
    public const float commonSpawnWeight = 70;
    public const float raritySpawnWeight = 7;

    public const float chaosPerRace = 20;
    public const float chaosPerShopReset = 10;
    public const float chaosFromCommon = 10;
    public const float chaosFromRare = 20;
    public const float chaosFromEpic = 33;
    public const float chaosFromLegendary = 50;

    public const float chaosPlus = 50;
    public const float chaosMinus = 30;
    public const float luckPlus = 7;

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
        Back,
        Front,
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
    public static Color32 GetColorForRarity(BoonRarity rarity)
    {
        switch (rarity)
        {
            default:
                return Color.black;
            case BoonRarity.common:
                return new Color(82f/255f, 129f / 255f, 74f / 255f);
            case BoonRarity.rare:
                return new Color(53f / 255f, 159f / 255f, 209f / 255f);
            case BoonRarity.epic:
                return new Color(175f / 255f, 66f / 255f, 183f / 255f);
            case BoonRarity.legendary:
                return new Color(238f / 255f, 231f / 255f, 65f / 255f);

        }
    }
}