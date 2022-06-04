using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupComponent : MonoBehaviour
{
    public virtual bool OnBuy(Player owningPlayer)
    {
        return true;
    }
}
