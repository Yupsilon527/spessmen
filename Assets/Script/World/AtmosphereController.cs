using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    public static Resource oxygen;

    public float TotalValue = 100;
    public float OxygenLossPerSecond = 3;

    private void Awake()
    {
        oxygen = new Resource(null, TotalValue, "Atmosphere",false,false );
    }
    private void Update()
    {
        oxygen.SubstractValue(OxygenLossPerSecond * Time.deltaTime);
    }
}
