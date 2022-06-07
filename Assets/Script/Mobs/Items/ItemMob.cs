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
        Vector2 throwVel = user.GetForwardVector(true) * ThrowSpeed + Vector2.up * ThrowSpeed;
        if (container!=null)
        container.UnloadItem(this);
        rbody.velocity = throwVel.x* user.transform.right + throwVel.y * user.transform.up;
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
    }
    public void DropFromContainer()
    {
        if (container!=null)
        container.UnloadItem(this);
    }
    public virtual void OnDrop()
    {
        gameObject.SetActive(true);
        HandleOrbit();
    }
    public override void Kill()
    {
        DropFromContainer();
        base.Kill();
    }
    public virtual void OnSold(Player sellingPlayer)
    {
        sellingPlayer.resources.GiveResource( ResourceController.Resources.gold ,GoldValue);
    }
    public virtual float GetNutritionalValue()
    {
        return 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.TryGetComponent(out PlayerItemHolding player))
        {
            player.OnTouchItem(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out PlayerItemHolding player))
        {
                player.OnTouchExit( this);
            
        }
    }
    public bool StartSuspended = false;
public void SetSuspended(bool value)
    {
        rbody.bodyType = value ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
    }
    public bool IsSuspended()
    {
        return rbody.bodyType == RigidbodyType2D.Static;
    }
}
