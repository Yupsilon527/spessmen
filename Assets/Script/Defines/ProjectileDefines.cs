using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDefines
{
    public static GameObject ProjectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Projectile");
    public enum Behavior
    {
        nothing = 0,
        explode = 1,
        delete = 2,
        inflict_damage = 3,
    }
    public static float zLayer = -3;
}
