using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerMob : Mob
{
    bool FacesRight = false;

    public Player parent;
    public bool CanMove = true;

    protected override void Start()
    {
        parent = new Player(gameObject,0);
        base.Start();
        if (SidewaysCamera.active!=null)
        SidewaysCamera.active.FollowMob(this);
    }

    protected override void Update()
    {
        base.Update();
        if (CanMove)
            HandleMovement();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGrounded(collision);
    }
    #region Grounded
    public float GroundTime = .1f;
    float LastGroundTime = 0;
    public bool IsGrounded()
    {
        return LastGroundTime > Time.time;
    }
    void CheckGrounded(Collision2D collision)
    {
        foreach (ContactPoint2D point in collision.contacts)
        {
            if (IsAbove(point.point))
            {
                LastGroundTime = Time.time + GroundTime;
            }
        }
    }
    #endregion
    Vector2 modifiedVelocity;
    void HandleMovement()
    {
        parent.HandleControls();
        HandleGravity();
        Move(parent.moveInput.x);
        HandleFall();
        // parent.rigidbody.velocity +=(Vector2)(modifiedVelocity.x * transform.right + modifiedVelocity.y * transform.up) ;
        parent.movement.gravity.relativeForce = modifiedVelocity;
    }
    void HandleGravity()
    {
        modifiedVelocity.y = Mathf.Max(-parent.stats.MaxFallSpeed, modifiedVelocity.y - parent.stats.FallDeceleration);
    }
    void Move(float dir)
    {
        modifiedVelocity.x = dir * parent.stats.WalkSpeed;
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
                if (modifiedVelocity.y < -parent.stats.MaxGlideSpeed)
                {
                    modifiedVelocity.y = -parent.stats.MaxGlideSpeed;
                }
            }
            else if (modifiedVelocity.y < -parent.stats.MaxFallSpeed)
            {
                modifiedVelocity.y = -parent.stats.MaxFallSpeed;
            }
        }
    }
    Coroutine JumpCoroutine;
    IEnumerator JumpFloat()
    {
        float jumpEndTime = Time.time + parent.stats.GetJumpTime();

        while (jumpEndTime >= Time.time && Input.GetButton("Jump"))
        {
            modifiedVelocity.y = parent.stats.JumpSpeed;
            yield return new WaitForFixedUpdate();
        }
        modifiedVelocity.y = parent.stats.MaxJumpSpeed;
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
    public float BelowAngle = 75f;
    public bool IsAbove(Vector2 point)
    {
        Vector2 delta = point - (Vector2)transform.position;

        float angleDiff = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float deltaAng = Mathf.DeltaAngle(angleDiff,transform.rotation.eulerAngles.z-90);
        deltaAng += 1;
        return Mathf.Abs(deltaAng) < BelowAngle;
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
        parent.health.Health.ChargeValue(force_delta * damage);
    }
}
