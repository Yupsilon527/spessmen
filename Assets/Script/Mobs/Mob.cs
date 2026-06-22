using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ConstantForce2D))] 
public abstract class Mob : SpaceObject
{
    protected override void Initialize()
    {
        FindComponent(ref rigidbody);
        FindComponent(ref gravity);
        base.Initialize();
    }
    protected virtual void Start()
    {
        Register(); 
    }
    protected bool suspended = false;
    public bool IsSuspended()
    {
        return suspended;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
         if (!suspended)
        {
            HandleOrbit(false);
        }
    }
    public virtual void Register()
    {
        Debug.Log("[Mob] Register "+name);
        TryTieToPlanet();
    }
    public virtual Vector2 GetForwardVector(bool absolute)
    {
        return Vector2.zero;
    }

    public virtual bool IsInside()
    {
        return false;
    }
    public virtual bool WasKilled()
    {
        return false;
    }
    public void TryTieToPlanet()
    {
        TieToPlanet(WorldController.active.GetClosestPlanetToPoint(transform.position));
    }
    protected void TieToPlanet(PlanetoidController p)
    {
        planet = p;
        if (rigidbody.gravityScale != 0)
        {
            rigidbody.gravityScale = 0;
        }
        gravity.enabled = true;
        HandleOrbit(true);
    }
    public void DetachFromPlanet()
    {
        gravity.enabled = false;
        planet = null;
    }
    public string MobName = "";
    public virtual string GetMobName()
    {
        return MobName;
    }
}