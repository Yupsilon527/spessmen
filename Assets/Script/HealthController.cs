    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : PropertyController
{
    public override void SetValue(float value)
    {
        base.SetValue(value);
        if (current <= 0 )
        {
            Owner.Kill();
        }    
    }
}