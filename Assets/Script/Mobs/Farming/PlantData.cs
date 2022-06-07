using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Data",menuName = "Scriptables/Plant Data")]
public class PlantData : ScriptableObject
{
    public string PlantName = "Plant";
    public float PlantGrowthTime = 100f;

    public float PlantHunger = 10;
    public float PlantHealth = 100;
    public float PlantAdultCycles = 5;

    public float OxygenProduction = 0;
    public float FruitChance = 0;
    public GameObject FruitPrefab;

    public GameObject SeedPrefab;
    public int SeedCount = 0;

    public Sprite SpriteSeedling;
    public Sprite SpriteMiddle;
    public Sprite SpriteAdult;
    public Sprite SpriteDry;
}
