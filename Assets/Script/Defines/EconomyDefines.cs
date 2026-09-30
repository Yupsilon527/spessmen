
using UnityEngine;

public static class EconomyDefines
{
    public const float constantGoldForRace = 15;
    public const float interestGoldCap = 100;
    public const float performanceGoldCap = 10;
    public const float constantGoldPerPosition = 2;
    public const float constantGoldPerDistance = .003f;
    public const float constantGoldInterest = .2f;
    public const float goldPerRaceIncrease = 1.15f;
    public static int goldPerRaceLimit = 9;

    public const float PartPriceBase = 20f;
    public const float partResellPrice = 0.6f;

    public static string FormatGold(float value)
    {
        int playerGoldvalue = Mathf.CeilToInt(value);
        return playerGoldvalue < 100 ? (playerGoldvalue.ToString("F0") + LanguageController.main.Translate("UI Table", "Cent unit")) : ((playerGoldvalue * .01f).ToString("F2") + LanguageController.main.Translate("UI Table", "Dollar unit"));
    }
}
