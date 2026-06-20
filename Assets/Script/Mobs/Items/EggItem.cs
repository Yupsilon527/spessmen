using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

public class EggItem : ItemMob
{
    public GameObject creaturePrefab;
    public float breakForce;
    public override void OnCreate()
    {
        base.OnCreate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.sqrMagnitude * collision.otherRigidbody.mass > breakForce)
        {
            Spawn();
        }
    }
    public override void OnActivate(PlayerMob user)
    {
        Spawn();
    }
    void Spawn()
    {
        GameObject creature = WorldController.active.MobPool.PoolItem(creaturePrefab);
        creature.transform.position = transform.position;
        Erase();
    }
}
