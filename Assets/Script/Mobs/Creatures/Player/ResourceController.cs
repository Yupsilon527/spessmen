using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MobComponent
{
    public enum Resources
    {
        gold = 0,
        wood = 1,
        max = 2
    }

    public float[] storage = new float[(int)Resources.max];

    public float GetResource(Resources res)
    {
        return storage[(int)res];
    }

    public void GiveResource(Resources res, float value)
    {
        if (value != 0)
        {
            SetValue(res, GetResource(res) + value);
        }
    }

    public virtual void SetValue(Resources res,  float value)
    {
            storage[(int)res] = Mathf.Max(0, value);
    }

    public void SubstractValue(Resources res, float value)
    {
        GiveResource(res ,- Mathf.Max(GetResource(res), value));
    }

    public bool ChargeValue(Resources res, float value)
    {
        if (value == 0)
        {
            return true;
        }
        if (GetResource(res) < value)
        {
            return false;
        }
        GiveResource(res ,- value);
        return true;
    }
}
