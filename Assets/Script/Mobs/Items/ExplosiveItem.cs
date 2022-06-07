using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveItem : ItemMob
{
    public float DetonationForce = 10;
    public float DetonationDuration = 1;
    public float[] ExplosionRadius = new float[3];
    public int[] ExplosionDamage = new int[3];

    public float CreatureDamage = 1;
    public float KnockbackForce = 1;
    public float KnockbackRange = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.sqrMagnitude * collision.otherRigidbody.mass > DetonationForce)
        {
            Detonate();
        }
    }
    public void Detonate()
    {
        detonationCoroutine = StartCoroutine(DetonateAfterDuration(DetonationDuration));
    }
    Coroutine detonationCoroutine;
    public IEnumerator DetonateAfterDuration(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        Explode();
    }
    void Explode()
    {
        Kill();
        ExplosionData boom = new ExplosionData((Vector2)transform.position + rbody.velocity, ExplosionRadius[0], ExplosionRadius[1], ExplosionRadius[2], KnockbackForce, KnockbackRange, ExplosionDamage[0], ExplosionDamage[1], ExplosionDamage[2], CreatureDamage);
        boom.Explode();
    }
    public override void Kill()
    {
        base.Kill();
        if (detonationCoroutine!=null)
            StopCoroutine(detonationCoroutine);
        detonationCoroutine = null;
    }
}
