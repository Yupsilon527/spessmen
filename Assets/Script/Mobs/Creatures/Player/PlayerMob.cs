using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMob : Mob
{
    bool FacesRight = false;
    public float WalkSpeed = 10;
    public float JumpTime = .33f;
    public float MaxJumpSpeed = 2;
    public float JumpSpeed = 1;
    public float FallDeceleration = .33f;
    public float MaxFallSpeed = 1;
    public float MaxGlideSpeed = 1;
    public float GroundTime = .1f;

    public Player parent;
    public bool CanMove = true;

    protected override void Start()
    {
        parent = new Player(gameObject,0);
        base.Start();
        if (SidewaysCamera.active!=null)
        SidewaysCamera.active.FollowMob(this);
    }
    float LastGroundTime = 0;
    public bool IsGrounded()
    {
        return LastGroundTime > Time.time;
    }

    protected override void Update()
    {
        base.Update();
        if (CanMove)
            HandleControls();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D point in collision.contacts)
        {
            if (IsAbove(point.point))
            {
                LastGroundTime = Time.time + GroundTime;
            }
        }
    }
    Vector2 modifiedVelocity;
    void HandleControls()
    {
        parent.HandleControls();
        modifiedVelocity.y = Mathf.Max(-MaxFallSpeed, modifiedVelocity.y - FallDeceleration) ;
        Move(parent.moveInput.x);
        HandleFall();
        // parent.rigidbody.velocity +=(Vector2)(modifiedVelocity.x * transform.right + modifiedVelocity.y * transform.up) ;
        parent.movement.gravity.relativeForce = modifiedVelocity;
    }
    void Move(float dir)
    {
        modifiedVelocity.x = dir * WalkSpeed;
        if (dir != 0 && IsGrounded())
        {
            SetFacing(dir > 0);
        }
    }
    void HandleFall()
    {
        if (IsGrounded())
        {
            if (Input.GetButton("Jump") && JumpCoroutine == null)
            {
                LastGroundTime = 0;
                JumpCoroutine = StartCoroutine(JumpFloat());
            }
        }
        else
        {
            if (Input.GetButton("Jump"))
            {
                if (modifiedVelocity.y < -MaxGlideSpeed)
                {
                    modifiedVelocity.y = -MaxGlideSpeed;
                }
            }
            else if (modifiedVelocity.y < -MaxFallSpeed)
            {
                modifiedVelocity.y = -MaxFallSpeed;
            }
        }
    }
    Coroutine JumpCoroutine;
    IEnumerator JumpFloat()
    {
        float jumpEndTime = Time.time + JumpTime;

        while (jumpEndTime >= Time.time && Input.GetButton("Jump"))
        {
            modifiedVelocity.y = JumpSpeed;
            yield return new WaitForFixedUpdate();
        }
        modifiedVelocity.y = MaxJumpSpeed;
        JumpCoroutine = null;
    }

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

    public void SetFacing(bool right)
    {
        FacesRight = right;
        //transform.localScale = new Vector3(right ? transform.localScale.x : -transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public bool IsAbove(Vector2 point)
    {
        Vector2 delta = point - (Vector2)transform.position;

        float angleDiff = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float deltaAng = Mathf.DeltaAngle(angleDiff,transform.rotation.eulerAngles.z-90);
        deltaAng += 1;
        return Mathf.Abs(deltaAng) < 30f;
    }
    public override Vector2 GetForwardVector(bool absolute)
    {
        return (absolute ? Vector2.right : (Vector2)transform.right) * (FacesRight ? 1 : -1);
    }

    public CompartimentComponent indoor;
    public override bool IsInside()
    {
        return indoor!=null;
    }
    public virtual void OnEnterBuilding()
    {
        gameObject.SetActive(false);
    }
    public void ExitBuilding()
    {
        indoor.UnloadMob(this);
    }
    public virtual void OnExitBuilding()
    {
        gameObject.SetActive(true);
    }
    public override void HandleShockwave(Vector2 center, Vector2 dir, float force_delta, float force, float damage)
    {
        base.HandleShockwave(center, dir, force_delta, force, damage);
        parent.health.SubstractValue(force_delta * damage);
    }
}
