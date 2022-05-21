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
    public InventoryComponent container;
    public bool RequiresGround = false;
    public virtual void OnActivate(PlayerMob user)
    {
        Vector2 throwVel = user.GetForwardVector() * 5 + Vector2.up * 3;
        container.UnloadItem(this);
        rigidbody.velocity = throwVel;
    }
    public override bool IsInside()
    {
        return container!=null;
    }
    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }
    public void DropFromContainer()
    {
        if (container!=null)
        container.UnloadItem(this);
    }
    public virtual void OnDrop()
    {
        gameObject.SetActive(true);
    }
    public override void Kill()
    {
        DropFromContainer();
        base.Kill();
    }
}
