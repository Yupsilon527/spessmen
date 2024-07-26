using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWanderComponent : AnimalComponent, iItemToucher
{
    public float UpdateTime = 1f;
    float LastWalk = 0;
    float WalkDir = 0;

    void FixedUpdate()
    {
        //SFX maybe creature noise
        if (LastWalk < Time.time)
        {
            LastWalk = Time.time + UpdateTime;
            if ( Random.value < 1 - parent.hunger.Hunger.GetPercentage())
            {
                WalkDir = Random.value < .5f ? 1 : -1;
            }
            else
            {
                WalkDir = 0;
            }
        }
        HandleMovement();
    }
    void HandleMovement()
    {
        if (parent.CanMove)
            parent.movement.Move(WalkDir);
    }
}
