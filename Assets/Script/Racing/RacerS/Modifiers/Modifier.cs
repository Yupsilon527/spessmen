using System.Collections.Generic;

public class Modifier : Countdown
{
    Racer racer;
    public ModifierScriptable data;
    public string ModifierName;
    public bool dead = false;
    public int stacks = 1;

    public ModifierDefines.Priority priority;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public bool IsExpired()
    {
        return false;
    }
    public Modifier(Racer racer,ModifierScriptable data, int stacks=1)
    {
        this.racer = racer;
        this.data = data;
        ModifierName = data.InternalName;
        SetStackCount( stacks);
    }
    #region States
    public List<ModifierDefines.State> states = new List<ModifierDefines.State>();
    public void SetState(ModifierDefines.State state, bool value)
    {
        if (value)
        {
            if (!states.Contains(state))
                states.Add(state);
        }
        else
        {
            if (states.Contains(state))
                states.Remove(state);
        }
    }
    public bool GetState(ModifierDefines.State state)
    {
        return states.Contains(state);
    }
    public bool GetState(int state)
    {
        return GetState((ModifierDefines.State)state);
    }
    #endregion
    #region Properties
    public Dictionary<ModifierDefines.Property, float> properties = new Dictionary<ModifierDefines.Property, float>();
    public void SetProperty(ModifierDefines.Property prop, float value)
    {
        if (properties.ContainsKey(prop))
        {
            properties[prop] = value;
        }
        else
        {
            properties.Add(prop, value);
        }
    }
    public float GetProperty(ModifierDefines.Property property)
    {
        if (!properties.ContainsKey(property))
            return 0;
        return properties[property];
    }

    #endregion
    #region Stacks
    public int GetStackCount()
    {
        return stacks;
    }
    public void SetStackCount(int value)
    {
        stacks = value;
        UpdateFromLevel();
    }
    public void IncrementStackCount()
    {
        SetStackCount(stacks + 1);
    }
    public void DecrementStackCount()
    {
        SetStackCount(stacks - 1);
    }
    public void UpdateFromLevel()
    {
        properties.Clear();

        HashSet<ModifierDefines.Property> props = new();
        foreach (ModifierDefines.PropertyData prop in data.properties)
        {
            props.Add(prop.Property);
        }
        foreach (ModifierDefines.RelativeStatData prop in data.relative)
        {
            props.Add(prop.Property);
        }

        foreach (var prop in props)
        {
            SetProperty(prop, GetProperty(prop) + data.GetProperty(prop,stacks - 1) + data.GetPropertyForRacer(prop,racer));
        }
       
        if (properties.Count > 0)
            racer.modifiers.RefreshModifier(this);
    }
    #endregion

    public virtual void Die(bool expire)
    {
        if (!dead)
        {
            dead = true;
            racer.modifiers.RefreshModifier(this);
            racer.modifiers.Refresh(false);
        }
    }
}
