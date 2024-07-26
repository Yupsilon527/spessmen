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
            if (DiggingCoroutine==null && parent.IsGrounded())
            {
                StartCoroutine(DiggingTask());
            }
            
        }
    }
    Coroutine DiggingCoroutine;
    Vector2 digVector = Vector2.zero;
    float lastDigTime = 0;
    IEnumerator DiggingTask()
    {
        parent.CanMove = false;
        while (Input.GetButton("Dig"))
        {
            digVector = parent.input.moveInput;
            MoveDigDirection();
            DigDirection();
            yield return new WaitForEndOfFrame();
        }
        StopDigging();
    }
    void DigDirection()
    {
        digVector = digVector.y * transform.up + digVector.x * transform.right;
        if (digVector.sqrMagnitude > 0 && lastDigTime < Time.time)
        {
            //SFX player digs 
            AudioManager.Instance.PlaySfx("Dig", 2);
            lastDigTime = Time.time + parent.GetDigTime();
            new ExplosionData((Vector2)transform.position + digVector * parent.DigRange, parent.DigRadius, 0, 0, parent.GetDigDamage(), 0).Explode();            
        }
    }
    void MoveDigDirection()
    {
        parent.gravity.relativeForce = parent.WalkSpeed * digVector.x * parent.MoveSpeedMultiplier * Vector2.right;
    }
    public void StopDigging()
    {
        if (DiggingCoroutine != null)
            StopCoroutine(DiggingCoroutine);
        DiggingCoroutine = null;
        parent.CanMove = true;
    }
    private void OnDisable()
    {
        StopDigging();
    }
}
