using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public abstract class Resource
{
    public enum LimitRule
    {
        leave_value,
        give_difference,
        percent_value,
        substract_total,
        fullheal_value,
        empty_value
    }
    public LimitRule LimitUnder = LimitRule.leave_value;
    public LimitRule LimitOver = LimitRule.give_difference;
    protected static bool resourceDebug = false;
    protected bool canNegative;
    protected string name;
    protected bool hasHardLimit = false;
    public UnityEvent OnValueChanged;
    public abstract float GetValue();
    public abstract float GetPercentage();

    public abstract float GetLimit(bool baseLimit = false);
    public abstract void ResetLimit(LimitRule rule);
    public abstract float GetDifference();


    public abstract void SetValue(float value);
    public virtual void GiveValue(float value)
    {
        if (resourceDebug) Debug.Log($"[{name}]  Give " + value);
        if (value != 0)
        {
            SetValue(GetValue() + value);
        }
    }

    public void SetPercentage(float value)
    {
        if (hasHardLimit)
            SetValue(value * GetLimit(false));
        else
            SetValue(value * GetValue());

    }
    public void SetLimit(float value)
    {
        SetLimit(value, LimitUnder, LimitOver);
    }
    public void SetLimit(float value, LimitRule under, LimitRule over)
    {
        SetLimit(value, value < GetLimit(false) ? under : over, false);
    }

    public abstract void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false);

    public virtual bool ChargeValue(float value)
    {
        if (resourceDebug) Debug.Log($"[{name}]  Charge " + value);
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
    public bool ChargePercentage(float value,bool total)
    {
     return ChargeValue((total ? GetLimit(false) : GetValue() )* value);
    }

    public virtual float SubstractedValue(float value)
    {
        if (resourceDebug) Debug.Log("[" + name + "] Substract " + value);
        value = Mathf.Abs(value);
        if (!canNegative)  value = Mathf.Min(GetValue(), value);
        GiveValue(-value);
        return value;
    }
    public virtual float RemainingValue(float value)
    {
        if (resourceDebug) Debug.Log("[" + name + "] Substract " + value);
        GiveValue(-value);
        return GetValue();
    }
}
public class ResourceFloat : Resource
{
    public override string ToString()
    {
        return $"{name} ({values[0]},{values[1]})";
    }
    float[] values;
    public ResourceFloat(float limit, string name, bool negative, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = new float[] { limit, limit, limit };
        canNegative = negative;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public override float GetValue()
    {
        return values[0];
    }
    public override float GetPercentage()
    {
        float value = values[0] / values[1];
        if (float.IsNaN(value))
            return 1;
        return value;
    }
    public override float GetLimit(bool baseLimit = false)
    {
        return values[baseLimit ? 2 : 1];
    }
    public override void ResetLimit(LimitRule rule)
    {
        SetLimit(values[2], rule);
    }
    public override float GetDifference()
    {
        return values[1] - values[0];
    }
    public float GetValueRounded(int d = 1)
    {
        d = (int)Mathf.Max(1, Mathf.Pow(10, d));
        return Mathf.Round(values[0] * d) / d;
    }
    public override void SetValue(float value)
    {
        float oldlife = values[0];
        if (hasHardLimit)
            values[0] = Mathf.Min(value, values[1]);
        else
            values[0] = value;

        if (!canNegative && values[0] < 0)
        {
            values[0] = Mathf.Max(0, values[0]);
        }
        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values[0]);
        OnValueChanged.Invoke();

    }

    public override void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false)
    {
        // display?.SetMaximum(values[1]);
        switch (rule)
        {
            case LimitRule.leave_value:
                values[1] = value;
                SetValue(values[0]);
                break;
            case LimitRule.give_difference:
                float difference = value - values[1];
                values[1] = value;
                SetValue(values[0] + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values[1] = value;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values[1] = value;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values[1] = value;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values[0] -= values[1];
                values[1] = value;
                SetPercentage(0);
                break;
        }
        if (hard)
        {
            values[2] = value;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value);
    }
}
public class ResourceInt : Resource
{
    public int GetValueRaw()
    {
        return values[0];
    }
    public override string ToString()
    {
        return $"{name} ({values[0]},{values[1]})";
    }
    int[] values;
    public ResourceInt(int limit, string name, bool negative, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = new int[] { limit, limit, limit };
        canNegative = negative;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public override float GetValue()
    {
        return values[0];
    }
    public override float GetPercentage()
    {
        float value = (float)values[0] / values[1];
        if (float.IsNaN(value))
            return 1;
        return value;
    }
    public override float GetLimit(bool baseLimit = false)
    {
        return values[baseLimit ? 2 : 1];
    }
    public override void ResetLimit(LimitRule rule)
    {
        SetLimit(values[2], rule);
    }
    public override float GetDifference()
    {
        return values[1] - values[0];
    }
    public float GetValueRounded(int d = 1)
    {
        d = (int)Mathf.Max(1, Mathf.Pow(10, d));
        return Mathf.Round(values[0] * d) / d;
    }
    public override void SetValue(float value)
    {
        float oldlife = values[0];
        if (hasHardLimit)
            values[0] = Mathf.RoundToInt( Mathf.Min(value, values[1]));
        else
            values[0] = Mathf.RoundToInt(value);

        if (!canNegative && values[0]<0)
        {
            values[0] = Mathf.Max(0, values[0]);
        }
        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values[0]);
        OnValueChanged.Invoke();

    }

    public override void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false)
    {
        int ivalue = Mathf.RoundToInt(value);
        switch (rule)
        {
            case LimitRule.leave_value:
                values[1] = ivalue;
                SetValue(values[0]);
                break;
            case LimitRule.give_difference:
                float difference = value - values[1];
                values[1] = ivalue;
                SetValue(values[0] + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values[1] = ivalue;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values[1] = ivalue;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values[1] = ivalue;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values[0] -= values[1];
                values[1] = ivalue;
                SetPercentage(0);
                break;
        }
        if (hard)
        {
            values[2] = ivalue;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value);
    }
    public override void GiveValue(float value)
    {
        base.GiveValue(Mathf.Floor(value));
    }
    public override float RemainingValue(float value)
    {
        return base.RemainingValue(Mathf.Ceil(value));
    }
    public override float SubstractedValue(float value)
    {
        return base.SubstractedValue(Mathf.Ceil(value));
    }
    public override bool ChargeValue(float value)
    {
        return base.ChargeValue(Mathf.Ceil(value));
    }
}