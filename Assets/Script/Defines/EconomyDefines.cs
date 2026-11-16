
using UnityEngine;

public static class EconomyDefines
{
    //income
    public const float constantGoldForRace = 15;
    public const float constantGoldPerPosition = 2;
    //interest
    public const float interestGoldCap = 100;
    public const float constantGoldInterest = .2f;

    //performance
    public const float performanceGoldCap = 50;
    public const float constantGoldPerDistance = .005f;
    public const float goldPerRaceIncrease = 1.15f;

    public static int goldPerRaceLimit = 9;

    //resell

    public const float PartPriceBase = 20f;
    public const float partResellPrice = 0.6f;

    public static string FormatGold(float value)
    {
        int playerGoldvalue = Mathf.CeilToInt(value);
        return playerGoldvalue < 100 ? (playerGoldvalue.ToString("F0") + LanguageController.main.Translate("UI Table", "Cent unit")) : ((playerGoldvalue * .01f).ToString("F2") + LanguageController.main.Translate("UI Table", "Dollar unit"));
    }
}
