using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class OxygenController : PropertyController
{
    HealthController health;
    protected override void Awake()
    {
        base.Awake();
        if (health == null)
            health = GetComponent<HealthController>();
    }
}
