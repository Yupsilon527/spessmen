using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DriveComponent : BuildingComponent
{
    public float angularAcceleration = 1;
    public float forwardAcceleration = 1;
    public float backwardAcceleration = 1;
    bool driveMode = false;
    protected override void Initialize()
    {
        base.Initialize();
        ExitDriveMode();
    }
    public bool IsDriveMode()
    {
        return driveMode;
    }
    private void Update()
    {
            if (Input.GetButtonDown("Build/Enter"))
            {
                ExitDriveMode();
        }
    }
    private void FixedUpdate()
    {
        Rotate(Input.GetAxisRaw("Horizontal"), Time.fixedDeltaTime);
        Accelerate(Input.GetAxisRaw("Vertical"), Time.fixedDeltaTime);
    }
    public void EnterDriveMode()
    {
        ToggleMode(true);
    }
    public void ExitDriveMode()
    {
        ToggleMode(false);
    }
    void ToggleMode(bool value)
    {
        enabled = value;
        driveMode =value;
        if (value)
        {
            parentMob.rigidbody.bodyType = RigidbodyType2D.Dynamic;
            parentMob.DetachFromPlanet();
            parentMob.rigidbody.constraints = RigidbodyConstraints2D.None;
            if (SidewaysCamera.active != null)
                SidewaysCamera.active.FollowMob(parentMob);
        }
        else
        {
            parentMob.rigidbody.velocity *= 0;
            parentMob.rigidbody.angularVelocity *= 0;
            parentMob.rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            parentMob.TryTieToPlanet();
        }
        parentMob.transform.up = parentMob.vectorUp;
    }
    public void Rotate(float x, float delta)
    {
        parentMob.rigidbody.AddTorque(x * angularAcceleration * -delta);
    }
    public void Accelerate(float y, float delta)
    {
        if (y > 0)
            parentMob.rigidbody.AddForce(parentMob.transform.up * forwardAcceleration *delta);
        else if (y<0)
            parentMob.rigidbody.AddForce(parentMob.transform.up * backwardAcceleration * -delta);
    }
}
