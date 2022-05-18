using UnityEditor;
using UnityEngine;


public static class MobDefines {

    public static GameObject CritterPrefab = Resources.Load<GameObject>("Prefabs/Critters/CritterPrefab");

    public enum DamageTypes
    {
        Brute = 0,
        Toxin = 1,
        Blood = 2
    }
    public static Vector2 defaultPlayerSize = Vector2.one * 1.33f * .5f;
    public static float MinimumMotion = 12;
    public static float zlayer = -5;

    public static float Collision_Tresspass = 10;
    public static float Collision_Damage = 2;

    public static float AimSpeed = 1f;
    public static float CritterSpeed = 1.75f;
    public static Vector2 LongJump = new Vector2(4,4);
    public static Vector2 TallJump = new Vector2(1, 7);

    public static GameObject HealthBarPrefab = Resources.Load<GameObject>("Prefabs/InGameUI/HealthBarTabs");
    public static GameObject DamageIndicatorPrefab = Resources.Load<GameObject>("Prefabs/InGameUI/DamageIndicator");
    public static float ReticleRange = 3f;
    public static float UIZlayer = -10f;
}