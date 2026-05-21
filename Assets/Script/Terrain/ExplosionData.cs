using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionData : ExplosionTable
{
    public Vector2 center;
    public float inner_radius;
    public int inner_damage;
    public float middle_radius;
    public int middle_damage;
    public float outer_radius;
    public int outer_damage;
    public float shockwave_radius;
    public float knockback_force;
    public float creature_damage;


    public ExplosionData(Vector2 center, float inner_radius, int inner_damage, float middle_radius, int middle_damage, float outer_radius, int outer_damage, float shockwave_radius, float knockback_force, float creature_damage)
    {
        this.center = center;
        this.inner_radius = inner_radius;
        this.inner_damage = inner_damage;
        this.middle_radius = middle_radius;
        this.middle_damage = middle_damage;
        this.outer_radius = outer_radius;
        this.outer_damage = outer_damage;
        this.shockwave_radius = shockwave_radius;
        this.knockback_force = knockback_force;
        this.creature_damage = creature_damage;
    }
    public ExplosionData(Vector2 center, ExplosionTable table) : this(center, table.inner_radius,table.inner_damage,table.middle_radius,table.middle_damage,table.outer_radius,table.outer_damage,table.shockwave_radius,table.knockback_force,table.creature_damage)
    {
    }

    public void Explode()
    {
        Debug.Log("[ExplosionData] Explode at position " + center);
        WorldController.active.StartCoroutine(WorldController.active.MakePhysicsExplosion(this));
        if (shockwave_radius>0)
      foreach (RaycastHit2D check in Physics2D.CircleCastAll(center,shockwave_radius, Vector2.zero))
        {
            if (check.collider.TryGetComponent(out Mob hit))
            {
                hit.HandleShockwave(this);
            }
        }
    }
}
[System.Serializable]
public class ExplosionTable
{
    public float inner_radius;
    public int inner_damage;
    public float middle_radius;
    public int middle_damage;
    public float outer_radius;
    public int outer_damage;
    public float shockwave_radius;
    public float knockback_force;
    public float creature_damage;
}
