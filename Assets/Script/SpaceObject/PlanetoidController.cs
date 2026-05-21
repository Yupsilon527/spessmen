using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetoidController : MonoBehaviour
{
    public static PlanetoidController mainPlanet;
    private void Awake()
    {
        mainPlanet = this;
    }
}
