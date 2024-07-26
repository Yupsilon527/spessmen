using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : PlayerComponent
{
    public Vector3 homePosition;
    private void OnValidate()
    {
        homePosition = transform.position;
    }
    float ResetTime = 0;
    private void Update()
    {
        TryReset();

    }

    void TryReset()
    {
        if (Input.GetButton("Reset"))
            {
            ResetTime += Time.deltaTime;
            if (ResetTime>1)
            {
                ResetPlayer();
            }
        }
        else
        {
            ResetTime = 0;
        }
    }
    void ResetPlayer()
    {
        //SFX teleport sound
        parent.hauler.DropItem();
        transform.position = homePosition;
        ResetTime = 0;
    }
}
