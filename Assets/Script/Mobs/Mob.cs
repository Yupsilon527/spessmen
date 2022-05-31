using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ConstantForce2D))] 
public abstract class Mob : MonoBehaviour
{
    protected int eid = 0;
    public Rigidbody2D rbody;
    protected virtual void Awake()
    {
        if (rbody == null)
            rbody = GetComponent<Rigidbody2D>();
        if (gravity == null)
            gravity = GetComponent<ConstantForce2D>();
    }
    protected virtual void Start()
    {
        Register(); 
    }
    protected virtual void Update()
    {
        if (Planet ==null)
        {
            TieToPlanet(PlanetoidController.mainPlanet);
        }
        else
        {
            HandlePlayerOrbit();
        }
    }
    public virtual void Register()
    {
        Debug.Log("[Mob] Register "+name);
        TieToPlanet(PlanetoidController.mainPlanet);
    }
    public virtual Vector2 GetForwardVector()
    {
        return Vector2.zero;
    }
    public virtual bool IsInMotion()
    {
        return gameObject.activeInHierarchy && rbody.IsAwake();
    }
    public virtual void HandleShockwave(Vector2 center, float explosion_inradius, float explosion_outradius, float explosion_force)
    {
        Debug.Log("[Mob] "+name+" Handle explosion at  " + center);
        Vector2 force_position = transform.position;
        Vector2 vector_position = force_position - center;

        float force_delta = 1;
        if (vector_position.sqrMagnitude > explosion_outradius * explosion_outradius)
        {
            force_delta = 0;
        }
        else if(vector_position.sqrMagnitude > explosion_inradius * explosion_inradius)
        {
            force_delta = (vector_position.magnitude - explosion_inradius) / (explosion_outradius - explosion_inradius);
        }
        if (force_delta > 0)
        {
            ApplyForce(vector_position.normalized * explosion_force * force_delta, force_position);
        }
        else
        {
            Debug.Log("[Mob] " + name + " is outside explosion range "+ force_position+"~"+ center);
        }
    }
    public virtual void ApplyForce(Vector2 force, Vector2 center)
    {
        // rigidbody.AddForceAtPosition(force, center);
        rbody.AddForce (force);
        WorldController.active.MobsInMotion.Add(this);
        Debug.Log("[Mob] Apply " + force + " force to " + name + " at point " + center);
       
    }

    public virtual bool IsInside()
    {
        return false;
    }
    public virtual void Kill()
    {
        Debug.Log("[Mob] Kill " + name);
        gameObject.SetActive(false);
    }
    public virtual bool WasKilled()
    {
        return false;
    }
    public ConstantForce2D gravity;
    public PlanetoidController Planet;
    protected void TieToPlanet(PlanetoidController p)
    {
        Planet = p;
        if (rbody.gravityScale != 0)
        {
            rbody.gravityScale = 0;
        }
        HandlePlayerOrbit();
    }
    protected void HandlePlayerOrbit()
    {
        if (Planet != null)
        {
            Vector2 vectorUp = (transform.position - Planet.transform.position);
            transform.up = vectorUp;
            rbody.velocity = -vectorUp.normalized * 2 ;
        }
    }
}