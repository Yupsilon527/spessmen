using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetoidData : Initializable
{
    public float planetGrav = 1;
    public GameObject planetPrefab;
    public Texture2D mapTexture;
    public SpriteRenderer renderComp;
    private void Start()
    {
        PlixelMapMob tileset = PlixelMapMob.LoadFromTexture(planetPrefab, mapTexture);
        tileset.transform.position = transform.position;
        if (tileset.TryGetComponent(out PlanetoidController planet))
        {
            planet.gravity = planetGrav;
        }
        Destroy(gameObject);
    }

}
