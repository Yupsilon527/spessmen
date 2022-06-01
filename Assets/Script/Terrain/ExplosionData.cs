using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionData
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

    public ExplosionData(Vector2 center, float inner_radius, float middle_radius, float outer_radius, float knockback_force, float shockwave_radius, int inner_damage, int middle_damage, int outer_damage, float creature_damage)
    {
        this.center = center;
        this.inner_radius = inner_radius;
        this.middle_radius = middle_radius;
        this.outer_radius = outer_radius;
        this.shockwave_radius = shockwave_radius;
        this.knockback_force = knockback_force;
        this.inner_damage = inner_damage;
        this.middle_damage = middle_damage;
        this.outer_damage = outer_damage;
        this.creature_damage = creature_damage;
    }

    public ExplosionData(Vector2 center, float inner_radius, float knockback_force, float shockwave_radius, int inner_damage, float creature_damage)
    {
        this.center = center;
        this.inner_radius = 0;
        this.middle_radius = 0;
        this.outer_radius = inner_radius;
        this.shockwave_radius = shockwave_radius;
        this.knockback_force = knockback_force;
        this.inner_damage = 0;
        this.middle_damage = 0;
        this.outer_damage = inner_damage;
        this.creature_damage = creature_damage;
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
