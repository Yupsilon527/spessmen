using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Mob : MonoBehaviour
{
    protected int eid = 0;
    public Rigidbody2D rigidbody;
    protected bool underwater = false;
    protected virtual void Awake()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();
    }
    protected virtual void Start()
    {
        Register(); 
    }
    protected virtual void Update()
    {
        if (IsBelowWaterLevel())
        {
            TouchWater();
        }
        if (IsBelowDeathLevel())
        {
            Kill();
        }
    }
    public virtual void Register()
    {
        eid = WorldController.active.nEntitites;
        gameObject.name = gameObject.name.Substring(0, gameObject.name.Length-7) +"-"+ eid;
        WorldController.active.nEntitites++;
        Debug.Log("[Mob] Register "+name);
    }
    public virtual Vector2 GetForwardVector()
    {
        return Vector2.zero;
    }
    public virtual bool IsInMotion()
    {
        return gameObject.activeInHierarchy && rigidbody.IsAwake();
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
        rigidbody.AddForce (force);
        WorldController.active.MobsInMotion.Add(this);
        Debug.Log("[Mob] Apply " + force + " force to " + name + " at point " + center);
       
    }
    public virtual bool IsBelowWaterLevel()
    {
        return underwater || transform.position.y < -WorldController.active.waterLevel;
    }
    public virtual bool IsBelowDeathLevel()
    {
        return transform.position.y < -WorldController.active.bottomLevel * 2;
    }

    public virtual void TouchWater()
    {
        underwater = true;
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
}