using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CreatureMovement))]
[RequireComponent(typeof(HealthController))]

public class CreatureMob : Mob
{
    public SpriteRenderer renderer;
    public bool CanMove = true;

    public float WalkSpeed = 10;

    #region Jumps
    [Header("Jumping/Flying")]
    public float JumpTime = .33f;
    public float MaxJumpSpeed = 2;
    public float JumpSpeed = 1;
    public float JumpTimeUpgrade = .33f;
    public int JumpUpgradesMax = 0;
    public float FallDeceleration = .33f;
    public float MaxFallSpeed = 1;
    public float MaxGlideSpeed = 1;

    protected int JumpUpgrades = 0;
    public void UpgradeJumps()
    {
        JumpUpgrades++;
    }
    public float GetJumpTime()
    {
        return JumpTime + JumpTimeUpgrade * JumpUpgrades;
    }
    #endregion
    public HealthController health;
    public CreatureMovement movement;
    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<HealthController>();
        movement = GetComponent<CreatureMovement>();
    }
    #region Grounded
    public float GroundTime = .1f;
    [HideInInspector]
    public float LastGroundTime = 0;
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
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGrounded(collision);
    }
    #endregion
    #region IsAbove
    public float BelowAngle = 75f;
    public bool IsAbove(Vector2 point)
    {
        Vector2 delta = point - (Vector2)transform.position;

        float angleDiff = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float deltaAng = Mathf.DeltaAngle(angleDiff, transform.rotation.eulerAngles.z - 90);
        deltaAng += 1;
        return Mathf.Abs(deltaAng) < BelowAngle;
    }
    #endregion

    protected bool FacesRight = false;
    public void SetFacing(bool right)
    {
        FacesRight = right;
        if (renderer!=null)
        {
            renderer.flipX = !FacesRight;
        }
    }
    public override Vector2 GetForwardVector(bool absolute)
    {
        return (absolute ? Vector2.right : (Vector2)transform.right) * (FacesRight ? 1 : -1);
    }
    public override void Register()
    {
        base.Register();
        health.Health.SetPercentage(1);
    }
}
