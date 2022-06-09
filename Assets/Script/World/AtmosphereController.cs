using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    public static PropertyController oxygen;

    public float TotalValue = 100;
    public float OxygenLossPerSecond = 3;

    private void Awake()
    {
        oxygen = gameObject.AddComponent<PropertyController>();
        oxygen.ResetLimit(TotalValue);
    }
    private void Update()
    {
        oxygen.SubstractValue(OxygenLossPerSecond * Time.deltaTime);
    }
}
