using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDigging : PlayerComponent
{
    void Update()
    {
        if (Input.GetButtonDown("Dig") && parent.backpack.GetActiveItem() == null)
        {
            if (parent.IsGrounded())
            {
                StartDigging();
            }
        }
        else if (Input.GetButtonUp("Dig"))
        {
            StopDigging();
        }
    }
    float nextUpdateTime = 0;
    public float updateTime = .1f;
    void FixedUpdate()
    {
        if (digging)
        {
            digVector = parent.input.moveInput;
            parent.CanMove = false;
            if (nextUpdateTime < Time.time)
            {
                if (digVector.sqrMagnitude > 0)
                {
                    MoveDigDirection();
                    DigDirection();
                }
                nextUpdateTime = Time.time + updateTime;
            }
        }
    }
    bool digging = false;
    void StartDigging()
    {
        digging = true;
    }
    Vector2 digVector = Vector2.zero;
    float lastDigTime = 0;
    void DigDirection()
    {
        digVector = digVector.y * transform.up + digVector.x * transform.right;
        if (digVector.sqrMagnitude > 0 && Time.time > lastDigTime)
        {
            //SFX player digs 
            AudioManager.Instance.PlaySfx("Dig", 2);
            lastDigTime = Time.time + parent.GetDigTime();
            var dig = new ExplosionData((Vector2)transform.position + digVector * parent.DigRange, new ExplosionTable.ExplosionRadius() { radius = parent.DigRadius, damage = parent.GetDigDamage() });
            dig.Explode();
        }
    }
    void MoveDigDirection()
    {
        parent.gravity.relativeForce = parent.WalkSpeed * digVector.x * parent.MoveSpeedMultiplier * Vector2.right;
    }
    public void StopDigging()
    {
        digging = false;
        parent.CanMove = true;
    }
    private void OnDisable()
    {
        StopDigging();
    }
    private void OnDrawGizmos()
    {
        if (digging)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + digVector * parent.DigRange, parent.DigRadius);
        }
    }
}
