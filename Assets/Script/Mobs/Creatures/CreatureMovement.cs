using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMovement : CreatureComponent
{

    

    protected void FixedUpdate()
    {
            HandleMovement();
    }
    void HandleMovement()
    {
        HandleGravity();
       parent.gravity.relativeForce = modifiedVelocity;
    }
    #region Movement
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void MovePosition(Vector3 newPos)
    {
        parent.rigidbody.position = newPos;
    }

    public void MoveDirection(Vector3 direction)
    {
        MovePosition(transform.position + direction);
    }

    #endregion
    #region Movement
    Vector2 modifiedVelocity;
    void HandleGravity()
    {
        modifiedVelocity.y = Mathf.Max(-parent.MaxFallSpeed, modifiedVelocity.y - parent.FallDeceleration);
    }
    public void Move(float dir)
    {
        modifiedVelocity.x = dir * parent.WalkSpeed;
        if (dir != 0 && parent.IsGrounded())
        {
            parent.SetFacing(dir > 0);
        }
    }

    #endregion
    public void HandleFall()
    {
        if (parent.IsGrounded() && parent.CanMove)
        {

            
            if (Input.GetButton("Jump") && JumpCoroutine == null)
            {
                parent.LastGroundTime = 0;
                JumpCoroutine = StartCoroutine(JumpFloat());

                AudioManager.Instance.PlaySfx("Jump", 0);





            }
        }
        else
        {
            if (Input.GetButton("Jump"))
            {

                

                if (modifiedVelocity.y < -parent.MaxGlideSpeed)
                {
                    modifiedVelocity.y = -parent.MaxGlideSpeed;

                }

                
            }
            else if (modifiedVelocity.y < -parent.MaxFallSpeed)
            {
                modifiedVelocity.y = -parent.MaxFallSpeed;
            }
        }
    }
    public ParticleSystem jetpackParticles;
    Coroutine JumpCoroutine;
    IEnumerator JumpFloat()
    {
        float jumpEndTime = Time.time + parent.GetJumpTime();

        if (jetpackParticles != null) 
            jetpackParticles.Play();
        

        while (jumpEndTime >= Time.time && Input.GetButton("Jump"))
        {
            
            modifiedVelocity.y = parent.JumpSpeed;
            yield return new WaitForFixedUpdate();
        }
        modifiedVelocity.y = parent.MaxJumpSpeed;

        if (jetpackParticles != null)
            jetpackParticles.Stop();
        JumpCoroutine = null;
    }
}
