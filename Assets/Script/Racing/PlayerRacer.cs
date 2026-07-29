using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRacer : Racer
{
    public void FromPlayerData(DataItemShip playerShip)
    {
        foreach (var part in playerShip.parts)
        {
            modifiers.Add(new Modifier(this,part.scriptable));
        }
    }
}
