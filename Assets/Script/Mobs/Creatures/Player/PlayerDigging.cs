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
        if (Input.GetKeyDown(KeyCode.Q))
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
        parent.movement.enabled = false;
        while (Input.GetKey(KeyCode.Q))
        {
            digVector.x = Input.GetAxis("Horizontal");
            digVector.y = Input.GetAxis("Vertical");
            digVector = digVector.normalized;
            if (digVector.sqrMagnitude > 0 && lastDigTime < Time.time)
            {
                lastDigTime = Time.time + DigCooldown;
                new ExplosionData((Vector2)transform.position + digVector * DigRange,DigRadius,0,0,1,0).Explode();
                if (!parent.movement.IsGrounded())
                {
                    break;
                }
            }
            parent.rigidbody.velocity = digVector * MoveSpeed;
            yield return new WaitForEndOfFrame();
        }
        StopDigging();
    }
    public void StopDigging()
    {
        if (DiggingCoroutine != null)
            StopCoroutine(DiggingCoroutine);
        DiggingCoroutine = null;
        parent.movement.enabled = true;
    }
    private void OnDisable()
    {
        StopDigging();
    }
}
