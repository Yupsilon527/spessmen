using System;
using UnityEngine;

public class Racer 
{
    public int id = 0;
   public float distanceRaced;
    public ResourceFloat fuel = new ResourceFloat(1, "gas", false, true);
   public  RacerStatsTable stats;
    public RacerAbilities abilities ;
    public RacerModifiers modifiers ;
    public Racer(int rId)
    {
        id = rId;
    }

    public virtual void OnRaceBegin()
    {
        abilities.OnRaceBegin();
        modifiers.OnRaceBegin();
    }
    public virtual void OnRaceEnd()
    {
        abilities.OnRaceEnd();
        modifiers.OnRaceEnd();
    }
    #region States and Properties
    public virtual bool GetState(ModifierDefines.State State)
    {
        return (modifiers != null && modifiers.GetState(State));
    }
    public virtual float GetPropertyAdditive(ModifierDefines.Property Property)
    {
        float value = 0;
        if (modifiers != null)
            value += modifiers.GetPropertyAdditive(Property);
        return value;
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Property Property)
    {
        float value = 1;
        if (modifiers != null)
            value *= modifiers.GetPropertyMultiplicative(Property);
        return value;
    }
    #endregion
}
