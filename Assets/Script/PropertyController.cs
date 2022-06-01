using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController : MobComponent
{
    public bool canNegative = false;
    public float current = 100;
    public float maximum = 100;

    public float GetValue()
    {
        return current;
    }
    public float GetPercentage()
    {
        return current / maximum;
    }

    public float GetLimit()
    {
        return maximum;
    }

    public void GiveValue(float value)
    {
        if (value != 0)
        {
            SetValue(current + value);
        }
    }

    public virtual void SetValue(float value)
    {
        current = Mathf.Min(value, maximum);
        if (!canNegative)
        {
            current = Mathf.Max(0, current);
        }
    }

    public void SetLimit(float value)
    {
        maximum = value;
        SetValue(current);
    }

    public bool ChargeValue(float value)
    {
        if (value == 0)
        {
            return true;
        }
        if (GetValue() < value)
        {
            return false;
        }
        GiveValue(-value);
        return true;
    }

    public void SubstractValue(float value)
    {
        GiveValue(-Mathf.Min(GetValue(), value));
    }
}
