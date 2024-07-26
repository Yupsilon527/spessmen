using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Data",menuName = "Scriptables/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("General Info")]
    public string PlantName = "Plant";
    public float PlantGrowthTime = 100f;

    [Header("Growth")]
    public float PlantHunger = 10;
    public float PlantHealth = 100;
    public float PlantAdultCycles = 5;

    [Header("Production")]
    public float OxygenProduction = 0;
    public float FruitChance = 0;
    public int FruitDropChance = 15;
    public GameObject FruitPrefab;

    [Header("Seeds")]
    public GameObject SeedPrefab;
    public int SeedCount = 0;

    [Header("Sprite")]
    public Sprite SpriteSeedling;
    public Sprite SpriteMiddle;
    public Sprite SpriteAdult;
    public Sprite SpriteDry;
}
