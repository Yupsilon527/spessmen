    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MobComponent
{
    public Resource Health;
    public float RegenerationPercent = 10;
    public float OxygenDamage = 5;

    protected override void Awake()
    {
        base.Awake();
        Health = new Resource(Owner, 100, "Health", false, true);
        Health.OnValueChanged.AddListener(() =>
        {
            CheckAliveState();
        });
    }
    void CheckAliveState()
    {
        if (Health.GetPercentage() <= 0)
        {
            Owner.Kill();
        }
    }
    private void Update()
    {
        HandleMetabolism();
    }
    void HandleMetabolism()
    {
        if (Owner.IsInside())
            Health.GiveValue(RegenerationPercent * Time.deltaTime);
        if (AtmosphereController.oxygen.GetPercentage() == 0)
            Health.SubstractValue(OxygenDamage * Time.deltaTime);
    }
}