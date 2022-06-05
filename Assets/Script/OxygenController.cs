using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class OxygenController : PropertyController
{
    public float DamageAtEmpty = 5;
    public PropertyController health;
    protected override void Awake()
    {
        base.Awake();
        if (health == null)
            health = GetComponent<HealthController>();
    }
    private void Update()
    {
        if (GetValue() <= 0)
            health.SubstractValue(DamageAtEmpty * Time.deltaTime);
    }
}
