    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : PropertyController
{
    public float RegenerationPercent = 10;
    public override void SetValue(float value)
    {
        base.SetValue(value);
        if (current <= 0 )
        {
            Owner.Kill();
        }    
    }
    private void Update()
    {
        if (Owner.IsInside())
            GiveValue(RegenerationPercent * Time.deltaTime);
    }
}