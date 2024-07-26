using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMob : CreatureMob
{
    
    public AnimalWanderComponent ai;
    public AnimalHungerComponent hunger;
    protected override void Awake()
    {
        base.Awake();

        ai = GetComponent<AnimalWanderComponent>();
        hunger = GetComponent<AnimalHungerComponent>();

    }
    public override void Register()
    {
        //SFX creature comes out of the house
        base.Register();
        hunger.Hunger.SetPercentage(1);
        RandomzieColor();
    }
    [Header("Colors")]
    public Color[] possibleColors = new Color[0];
    void RandomzieColor()
    {
        if (renderer!=null && possibleColors.Length>0)
        {
            renderer.color = possibleColors[Random.Range(0, possibleColors.Length)];
        }
    }
}
