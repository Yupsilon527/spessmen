using UnityEngine;

public class SpaceObject : Initializable
{
    protected int eid = 0;
    public Rigidbody2D rigidbody;
    public ConstantForce2D gravity;
    public PlanetoidController planet;

    public bool FreeRotation = false;
    public float Mass = 10;
    float nextPosUpdate = 0f;
    public float posUpdateInterval = .1f;
    public Vector2 vectorUp = Vector2.zero;

    public static int geid = 0;
    protected override void Initialize()
    {
        base.Initialize();
        eid = geid++;
    }
    public virtual void ApplyForce(Vector2 force, Vector2 center)
    {
        // rigidbody.AddForceAtPosition(force, center);
        rigidbody.AddForce(force);
        Debug.Log("[Mob] Apply " + force + " force to " + name + " at point " + center);

    }
    protected void OrbitPoint(Vector3 point)
    {
        vectorUp = (transform.position - point).normalized;
        if (Mathf.Abs(vectorUp.x) > .01f)
            transform.up = vectorUp;

    }
    protected virtual void HandleOrbit(bool force)
    {
        if (planet != null)
        {
            gravity.force = -vectorUp * Mass * planet.gravity;
            if (force || (!FreeRotation && nextPosUpdate < Time.time))
            {
                OrbitPoint(planet.transform.position);
                nextPosUpdate = Time.time + posUpdateInterval;
            }
        }
        else
        {
            gravity.force = Vector3.zero;
        }
    }
    public void HandleShockwave(ExplosionData eData)
    {
        HandleShockwave(eData.center, eData.shockwave_radius * .5f, eData.shockwave_radius, eData.knockback_force, eData.creature_damage);
    }
    public virtual void HandleShockwave(Vector2 center, Vector2 dir, float force_delta, float force, float damage)
    {
        if (force_delta > 0)
        {
            ApplyForce(dir * force_delta * force, center);
        }
    }
    public void HandleShockwave(Vector2 center, float explosion_inradius, float explosion_outradius, float explosion_force, float explosion_damage)
    {
        Debug.Log("[Mob] " + name + " Handle explosion at  " + center);
        Vector2 force_position = transform.position;
        Vector2 vector_position = force_position - center;

        float force_delta = 1;
        if (vector_position.sqrMagnitude > explosion_outradius * explosion_outradius)
        {
            force_delta = 0;
        }
        else if (vector_position.sqrMagnitude > explosion_inradius * explosion_inradius)
        {
            force_delta = (vector_position.magnitude - explosion_inradius) / (explosion_outradius - explosion_inradius);
        }
        HandleShockwave(center, vector_position.normalized, force_delta, explosion_force, explosion_damage);
    }
    public virtual void Erase()
    {
        WorldController.active.MobPool.DeactivateObject(gameObject);
    }
}
