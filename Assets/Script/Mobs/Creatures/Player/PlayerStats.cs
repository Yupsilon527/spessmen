using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float WalkSpeed = 10;

    #region Jumps
    [Header("Jumping/Flying")]
    public float JumpTime = .33f;
    public float MaxJumpSpeed = 2;
    public float JumpSpeed = 1;
    public float JumpTimeUpgrade = .33f;
    public int JumpUpgradesMax = 0;
    public float FallDeceleration = .33f;
    public float MaxFallSpeed = 1;
    public float MaxGlideSpeed = 1;

    protected int JumpUpgrades = 0;
    public void UpgradeJumps()
    {
        JumpUpgrades++;
    }
    public float GetJumpTime()
    {
        return JumpTime + JumpTimeUpgrade * JumpUpgrades;
    }
    #endregion
    #region Digging
    [Header("Digging")]
    public float DigRange = 3;
    public float DigRadius = 1;
    public float DigCooldown = 1;
    public float DigCooldownUpgrades = .1f;
    public float MoveSpeedMultiplier = 1f;
    public int DigStrengthUpgrades = 5;

    protected int DiggingUpgrades = 0;
    public void UpgradeDigging()
    {
        DiggingUpgrades++;
    }
    public float GetDigTime()
    {
        return DigCooldown + DigCooldownUpgrades * DiggingUpgrades;
    }
    public int GetDigDamage()
    {
        return DiggingUpgrades  >= DigStrengthUpgrades ? 2 : 1;
    }
    #endregion
}
