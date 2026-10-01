
using UnityEngine;

public static class DifficultyDefines
    {
    public static int eliteRaceInterval = 3;
    public const float eliteRaceMultiplier = 1.5f;
    public const float eliteRaceScoreMultiplier = 2;

    public const float enemyMinSpeed = 4;
    public const float enemyBaseSpeed = 8;
    public const float enemyWheelSpeed = 6;
    public const float enemyEngineSpeed = 5;
    public const float enemyEngineCooldown = 2.5f;
    public const float enemyEngineDelta = 1.25f;
    public const float enemyGasUse = 10;
    public const float enemyTankBonus = 10;

    public const float lapDistanceBase = 200;
    public const float lapDistanceAdd = 20;

    public const float enemyStartDistance = 2;
    public const float aiUseAbilityChance = .2f;

    public static int qualifiedPosition = 0;

    public static float GetEnemyWheelSpeedAtLevel(int level, float rnval = 1)
    {
        return Mathf.Max(enemyMinSpeed, enemyBaseSpeed + enemyWheelSpeed * (level + 1) * rnval);
    }
}
