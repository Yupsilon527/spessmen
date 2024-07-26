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
    public enum Edibility
    {
        fruit,
        explosive,
        gold,
        inedible
    }
    public Category category;
    public Edibility ediblecategory;
    public InventoryComponent container;
    public bool RequiresGroundToUse = false;
    public float ThrowSpeed = 10;
    public float GoldValue = 0;
    public virtual void OnCreate()
    {
            SetSuspended(StartSuspended);
    }
    public virtual void OnActivate(PlayerMob user)
    {
        SetSuspended(false);
        Vector2 throwVel = user.GetForwardVector(true) * ThrowSpeed + Vector2.up * ThrowSpeed * user.ThrowStrength;
        if (container!=null)
        container.UnloadItem(this);
        rigidbody.velocity = throwVel.x* user.transform.right + throwVel.y * user.transform.up;
    }
    public override bool IsInside()
    {
        return container!=null;
    }
    public virtual void OnMoveToContainer(InventoryComponent ncontainer)
    {
        if (container!= null && container != ncontainer)
            container.UnloadItem(this);
        container = ncontainer;
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySfx("Pickup Item", 6);
    }
    public void DropFromContainer()
    {
        if (container!=null)
        container.UnloadItem(this);
    }
    public virtual void OnDrop()
    {
        SetSuspended(false);
        gameObject.SetActive(true);
        HandleOrbit(true);
    }
    public override void Kill()
    {
        DropFromContainer();
        base.Kill();
    }
    public virtual void OnSold(PlayerMob sellingPlayer)
    {
        sellingPlayer.resources.GiveResource( ResourceController.Resources.gold ,GoldValue);
    }
    public virtual float GetNutritionalValue()
    {
        return 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (iItemToucher col in collision.transform.GetComponents<iItemToucher>())
        {
            col.OnTouchEnter(this);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (iItemToucher col in collision.transform.GetComponents<iItemToucher>())
        {
            col.OnTouchExit( this);
            
        }
    }
    public bool StartSuspended = false;
public void SetSuspended(bool value)
    {
        suspended = value;
        rigidbody.bodyType = value ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
    }
}
