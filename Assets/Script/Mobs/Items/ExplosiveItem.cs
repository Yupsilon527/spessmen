using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveItem : ItemMob
{
    public GameObject ExplosionEffect;
    public float DetonationDelay;
    public float DetonationForce;
    public ExplosionTable explosionData;
    public override void OnCreate()
    {
        base.OnCreate();
        StopDetonation();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.sqrMagnitude * collision.otherRigidbody.mass > DetonationForce)
        {
            Detonate();
        }
    }
    public void Detonate()
    {
        if (DetonationDelay > 0)
        detonationCoroutine = StartCoroutine(DelayedDetonate(DetonationDelay));
        else
            Explode();
    }
    Coroutine detonationCoroutine;
    public IEnumerator DelayedDetonate(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        Explode();
    }
    void Explode()
    {
        //SFX explosion audio
        Kill();
        ExplosionData boom = new ExplosionData((Vector2)transform.position + rigidbody.velocity, explosionData);
        boom.Explode();
        if (ExplosionEffect!= null &&  WorldController.active.EffectPool!=null)
        {
            var expEff = WorldController.active.EffectPool.PoolItem(ExplosionEffect);
            if (expEff!=null)
            {
                expEff.transform.position = transform.position;
                expEff.transform.rotation = transform.rotation;
            }
        }
    }
    public override void Kill()
    {
        base.Kill();
    }
    void StopDetonation()
    {
        if (detonationCoroutine != null)
            StopCoroutine(detonationCoroutine);
        detonationCoroutine = null;
    }
}
