using System;
using UnityEngine;

public class PropertyComponent : RacerComponent
{
    public PropertyComponent(Racer racer) : base(racer)
    {
    }

    #region States
    [NonSerialized] protected int[] states = new int[(int)ModifierDefines.State.Total];

    public virtual bool GetState(ModifierDefines.State State)
    {
        return states[(int)State] > 0;
    }
    public void UpdateState(ModifierDefines.State State, int value)
    {
        if ((int)State >= 0 && (int)State < (int)ModifierDefines.State.Total)
            return;

        if (Mathf.Abs(states[(int)State]) < Mathf.Abs(value))
        {
            states[(int)State] += value;
        }
    }

    #endregion
    #region Properties
    [NonSerialized] protected float[] properties = new float[(int)ModifierDefines.Property.total];

    public float GetPropertyAdditive(ModifierDefines.Property Property)
    {
        return GetPropertyAdditive((int)Property);
    }

    public virtual float GetPropertyAdditive(int Property)
    {
        return properties[Property];
    }
    public virtual float GetPropertyMultiplicative(ModifierDefines.Property Property)
    {
        return 1 + properties[(int)Property];
    }
    public virtual void UpdateProperty(ModifierDefines.Property Property, float value)
    {
        if ((int)Property < 0 && (int)Property >= (int)ModifierDefines.Property.total)
            return;
        if (ModifierDefines.IsPropertyMultiplicative(Property))
        {
            properties[(int)Property] = (1+ properties[(int)Property]) * (1+value) - 1;
        }
        else
        {
            properties[(int)Property] += value;
        }
    }
    #endregion
}
