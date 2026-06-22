using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetoidController : Initializable
{
    public float gravity = 3;
    public float gravityRange = 3;
    protected override void Initialize()
    {
        base.Initialize();
        WorldController.active.planets.Add(this);
    }
}
