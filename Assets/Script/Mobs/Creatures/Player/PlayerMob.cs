using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMob : Mob
{
    bool FacesRight = false;
    public float WalkSpeed = 10;
    public float JumpTime = .33f;
    public float MaxJumpSpeed = 2;
    public float JumpSpeed = 1;
    public float MaxFallSpeed = 1;
    public float MaxGlideSpeed = 1;
    public float GroundTime = .1f;

    public Player parent;
    public bool CanMove = true;

    protected override void Start()
    {
        parent = new Player(gameObject,0);
        base.Start();
    }
    float LastGroundTime = 0;
    public bool IsGrounded()
    {
        return LastGroundTime > Time.time;
    }

    protected override void Update()
    {
        if (CanMove)
            HandleControls();
        base.Update();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D point in collision.contacts)
        {
            if (IsAbove(point.point.y))
            {
                LastGroundTime = Time.time + GroundTime;
            }
        }
    }
    Vector2 modifiedVelocity;
    void HandleControls()
    {
        modifiedVelocity = parent.rigidbody.velocity;
        Move(Input.GetAxis("Horizontal"));
        HandleFall();
        parent.rigidbody.velocity = modifiedVelocity;
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
            if (Input.GetKeyDown(KeyCode.Space) && JumpCoroutine == null)
            {
                LastGroundTime = 0;
                JumpCoroutine = StartCoroutine(JumpFloat());
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
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

        while (jumpEndTime >= Time.time && Input.GetKey(KeyCode.Space))
        {
            parent.rigidbody.velocity = parent.rigidbody.velocity * Vector2.right + Vector2.up * JumpSpeed;
            yield return new WaitForFixedUpdate();
        }

        parent.rigidbody.velocity = parent.rigidbody.velocity * Vector2.right + Vector2.up * MaxJumpSpeed;
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
        transform.localScale = new Vector3(right ? transform.localScale.x : -transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public bool IsAbove(float y)
    {
        return transform.position.y + (parent.collider.size.y + parent.collider.size.x * .66f) * .5f > y;
    }
    public override Vector2 GetForwardVector()
    {
        return Vector2.right * (FacesRight ? 1 : -1);
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
}
