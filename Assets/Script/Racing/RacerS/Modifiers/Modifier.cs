using System.Collections.Generic;

public class Modifier : Countdown
{
   protected Racer racer;
    public string ModifierName;
    public bool dead = false;
    public int stacks = 1;
    public List<ModifierDefines.State> states = new List<ModifierDefines.State>();
    public Dictionary<ModifierDefines.Property, float> properties = new Dictionary<ModifierDefines.Property, float>();

    public ModifierDefines.Priority priority;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public Modifier(Racer owner, int level = 0)
    {
        this.racer = owner;
        this.stacks = level;
    }

    public Modifier(Racer owner, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Nothing, ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique, List<ModifierDefines.State> states = new(), Dictionary<ModifierDefines.Property, float> properties = new())
    {
        this.racer = owner;
        this.states = states;
        this.properties = properties;
        this.priority = priority;
        this.flag = flag;
        this.behavior = behavior;
    }

    public bool IsExpired()
    {
        return !IsRunning();
    }
    #region States
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
    public void SetProperty(ModifierDefines.Property prop, float value)
    {
        if (properties.ContainsKey(prop))
        {
            properties[prop] = (1+ properties[prop]) *(1+ value)-1;
        }
        else
        {
            properties.Add(prop, value);
        }
    }
    public float GetProperty(ModifierDefines.Property property)
    {
        if (!properties.ContainsKey(property))
            return ModifierDefines.IsPropertyMultiplicative(property) ? 1 : 0;
        return (ModifierDefines.IsPropertyMultiplicative(property) ? 1 : 0) + properties[property];
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
    public virtual void UpdateFromLevel() { }
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
