using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDigging : MonoBehaviour
{
    public float DigRange = 3;
    public float DigRadius = 1;
    public float DigCooldown = 1;
    public float MoveSpeed = 1f;

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
            digVector = parent.moveInput;
            digVector = digVector.normalized;
            digVector = digVector.y * transform.up + digVector.x * transform.right;
            if (digVector.sqrMagnitude > 0 && lastDigTime < Time.time)
            {
                lastDigTime = Time.time + DigCooldown;
                new ExplosionData((Vector2)transform.position + digVector * DigRange,DigRadius,0,0,1,0).Explode();
                /*if (!parent.movement.IsGrounded())
                {
                    break;
                }*/
            }
            parent.movement.gravity.relativeForce = parent.movement.WalkSpeed * digVector * MoveSpeed;
            yield return new WaitForEndOfFrame();
        }
        StopDigging();
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
