using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDigging : MonoBehaviour
{

    public Player parent;
    void Update()
    {
        if (Input.GetButtonDown("Dig") && parent.backpack.GetActiveItem() == null)
        {
            if (DiggingCoroutine==null && parent.movement.IsGrounded())
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
        parent.movement.CanMove = false;
        while (Input.GetButton("Dig"))
        {
            DigDirection();
            parent.movement.gravity.relativeForce = parent.stats.WalkSpeed * digVector * parent.stats.MoveSpeedMultiplier;
            yield return new WaitForEndOfFrame();
        }
        StopDigging();
    }
    void DigDirection()
    {
        digVector = parent.moveInput;
        digVector = digVector.y * transform.up + digVector.x * transform.right;
        if (digVector.sqrMagnitude > 0 && lastDigTime < Time.time)
        {
            lastDigTime = Time.time + parent.stats.GetDigTime();
            new ExplosionData((Vector2)transform.position + digVector * parent.stats.DigRange, parent.stats.DigRadius, 0, 0, parent.stats.GetDigDamage(), 0).Explode();
            /*if (!parent.movement.IsGrounded())
            {
                break;
            }*/
        }
    }
    public void StopDigging()
    {
        if (DiggingCoroutine != null)
            StopCoroutine(DiggingCoroutine);
        DiggingCoroutine = null;
        parent.movement.CanMove = true;
    }
    private void OnDisable()
    {
        StopDigging();
    }
}
