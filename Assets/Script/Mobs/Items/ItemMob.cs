using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMob : Mob
{
    public enum Category
    {
        small,
        tool,
        large
    }
    public Category category;
    PlayerCarryItem carrier;
    public virtual void OnPickupByCreature(PlayerCarryItem actor)
    {
        carrier = actor;
        gameObject.SetActive(false);
    }
    public virtual void OnDroppedByCreature()
    {
        transform.position = carrier.transform.position;
        carrier = null;
        gameObject.SetActive(true);
    }
    public virtual void OnActivate()
    {
        Vector2 throwVel = carrier.parent.movement.GetForwardVector() * 5 + Vector2.up * 3;
        carrier.DropItem();
        rigidbody.velocity = throwVel;
    }
}
