using UnityEngine;
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

    public abstract float GetLimit();
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
            SetValue(value * GetLimit());
        else
            SetValue(value * GetValue());

    }
    public void SetLimit(float value)
    {
        SetLimit(value, LimitUnder, LimitOver);
    }
    public void SetLimit(float value, LimitRule under, LimitRule over)
    {
        SetLimit(value, value < GetLimit() ? under : over, false);
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
     return ChargeValue((total ? GetLimit() : GetValue() )* value);
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
        return $"{name} ({values.Item1},{values.Item2})";
    }
    (float,float) values;
    public ResourceFloat(float limit, string name, bool negative, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = (limit,limit);
        canNegative = negative;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public override float GetValue()
    {
        return values.Item1;
    }
    public override float GetPercentage()
    {
        float value = values.Item1 / values.Item2;
        if (float.IsNaN(value))
            return 1;
        return value;
    }
    public override float GetLimit()
    {
        return values.Item2;
    }
    public override float GetDifference()
    {
        return values.Item2 - values.Item1;
    }
    public float GetValueRounded(int d = 1)
    {
        d = (int)Mathf.Max(1, Mathf.Pow(10, d));
        return Mathf.Round(values.Item1 * d) / d;
    }
    public override void SetValue(float value)
    {
        float oldlife = values.Item1;
        if (hasHardLimit)
            values.Item1 = Mathf.Min(value, values.Item2);
        else
            values.Item1 = value;

        if (!canNegative && values.Item1 < 0)
        {
            values.Item1 = Mathf.Max(0, values.Item1);
        }
        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values.Item1);
        OnValueChanged.Invoke();

    }

    public override void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false)
    {
        // display?.SetMaximum(values.Item2);
        switch (rule)
        {
            case LimitRule.leave_value:
                values.Item2 = value;
                SetValue(values.Item1);
                break;
            case LimitRule.give_difference:
                float difference = value - values.Item2;
                values.Item2 = value;
                SetValue(values.Item1 + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values.Item2 = value;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values.Item2 = value;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values.Item2 = value;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values.Item1 -= values.Item2;
                values.Item2 = value;
                SetPercentage(0);
                break;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value);
    }
}
public abstract class ResourceSimple : Resource
{
    public override float GetPercentage()
    {
        float value = (float)GetValue() / GetLimit();
        if (float.IsNaN(value))
            return 1;
        return value;
    }
    public override float GetDifference()
    {
        return GetLimit() - GetValue();
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
    public float GetValueRounded(int d = 1)
    {
        return GetValue();
    }
}
public class ResourceInt : ResourceSimple
{
    (int, int) values;
    public override string ToString()
    {
        return $"{name} ({values.Item1},{values.Item2})";
    }
    public ResourceInt(int limit, string name, bool negative, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = (limit, limit);
        canNegative = negative;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public override float GetValue()
    {
        return values.Item1;
    }
    public override float GetLimit()
    {
        return values.Item2;
    }
    public override void SetValue(float value)
    {
        var oldlife = values.Item1;
        if (hasHardLimit)
            value = Mathf.Min(value, values.Item2);
        values.Item1 = Mathf.RoundToInt(value);

        if (!canNegative && values.Item1 < 0)
        {
            values.Item1 = Mathf.Max(0, values.Item1);
        }
        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values.Item1);
        OnValueChanged.Invoke();
    }

    public override void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false)
    {
        int ivalue = Mathf.RoundToInt(value);
        // display?.SetMaximum(values.Item2);
        switch (rule)
        {
            case LimitRule.leave_value:
                values.Item2 = ivalue;
                SetValue(values.Item1);
                break;
            case LimitRule.give_difference:
                float difference = value - values.Item2;
                values.Item2 = ivalue;
                SetValue(values.Item1 + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values.Item2 = ivalue;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values.Item2 = ivalue;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values.Item2 = ivalue;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values.Item1 -= values.Item2;
                values.Item2 = ivalue;
                SetPercentage(0);
                break;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value);
    }
}
public class ResourceUint : ResourceSimple
{
    public override string ToString()
    {
        return $"{name} ({values.Item1},{values.Item2})";
    }
    (uint, uint) values;
    public ResourceUint(uint limit, string name, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = (limit, limit);
        canNegative = false;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public override float GetValue()
    {
        return values.Item1;
    }
    public override float GetLimit()
    {
        return values.Item2;
    }
    public override void SetValue(float value)
    {
        var oldlife = values.Item1;
        float clamped = Mathf.Max(0, value);
        if (hasHardLimit)
            clamped = Mathf.Min((float)value, values.Item2);
        values.Item1 = (uint)Mathf.RoundToInt(clamped);

        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values.Item1);
        OnValueChanged.Invoke();

    }

    public override void SetLimit(float value, LimitRule rule = LimitRule.leave_value, bool hard = false)
    {
        uint ivalue = (uint) Mathf.RoundToInt(value);
        // display?.SetMaximum(values.Item2);
        switch (rule)
        {
            case LimitRule.leave_value:
                values.Item2 = ivalue;
                SetValue(values.Item1);
                break;
            case LimitRule.give_difference:
                float difference = value - values.Item2;
                values.Item2 = ivalue;
                SetValue(values.Item1 + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values.Item2 = ivalue;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values.Item2 = ivalue;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values.Item2 = ivalue;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values.Item1 -= values.Item2;
                values.Item2 = ivalue;
                SetPercentage(0);
                break;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value);
    }
}