using System.Linq;
using UnityEngine;

public class ExplosionData : ExplosionTable
{
    public Vector2 center;


    public ExplosionData(Vector2 center, ExplosionRadius[] terrain_damage, float shockwave_radius, float knockback_force, float creature_damage)
    {
        this.center = center;
        this.terrain_damage = terrain_damage;
        this.shockwave_radius = shockwave_radius;
        this.knockback_force = knockback_force;
        this.creature_damage = creature_damage;
    }
    public ExplosionData(Vector2 center, ExplosionTable table) : this(center, table.terrain_damage, table.shockwave_radius, table.knockback_force, table.creature_damage)
    {
    }
    
    public ExplosionData(Vector2 center, ExplosionRadius simple) : this(center, new ExplosionRadius[] {simple},0,0,0)
    {
    }

    public void Explode()
    {
        Debug.Log("[ExplosionData] Explode at position " + center);
        if (terrain_damage.Length == 0 || !terrain_damage.Any(r => r.radius > 0)) return;

        foreach (PlixelMapMob Zim in WorldController.active.terrainmobs)
        {
            Zim.HandleExplosion(this);
        }


        if (shockwave_radius > 0)
        {
            foreach (PlixelMapMob Zim in WorldController.active.terrainmobs)
            {
                if (Zim != null)
                {
                    Zim.HandleShockwave(this);
                }
            }
            foreach (RaycastHit2D check in Physics2D.CircleCastAll(center, shockwave_radius, Vector2.zero))
            {
                if (check.collider.TryGetComponent(out Mob hit))
                {
                    hit.HandleShockwave(this);
                }
            }
        }
    }
}
[System.Serializable]
public class ExplosionTable
{

    [System.Serializable]
    public class ExplosionRadius
    {
        public float radius;
        public int damage;
    }
    public ExplosionRadius[] terrain_damage;
    public float shockwave_radius;
    public float knockback_force;
    public float creature_damage;
}
