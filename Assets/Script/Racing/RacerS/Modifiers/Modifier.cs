using System.Collections.Generic;

public class Modifier : Countdown
{
    protected Racer racer;
    public string ModifierName="UNASSIGNED";
    public bool dead = false;
    public int stacks = 1;
    public List<ModifierDefines.State> states = new List<ModifierDefines.State>();
    public Dictionary<ModifierDefines.Property, float> properties = new Dictionary<ModifierDefines.Property, float>();

    public ModifierDefines.ExpireType expire = ModifierDefines.ExpireType.Permanent;
    public ModifierDefines.Priority priority;
    public ModifierDefines.Flag flag;
    public ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique;
    public Modifier(Racer owner, int level = 0)
    {
        this.racer = owner;
        this.stacks = level;
    }

    public Modifier(Racer owner, ModifierDefines.Priority priority = ModifierDefines.Priority.normal, ModifierDefines.Flag flag = ModifierDefines.Flag.Nothing, ModifierDefines.Behavior behavior = ModifierDefines.Behavior.Unique, List<ModifierDefines.State> states = null , Dictionary<ModifierDefines.Property, float> properties = null)
    {
        this.racer = owner;
        this.states = states == null ? new() : states;
        this.properties = properties == null ? new() : properties;
        this.priority = priority;
        this.flag = flag;
        this.behavior = behavior;
    }

    public bool IsExpired()
    {
        return false;
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
    #region Functions
    public Dictionary<ShipDefines.PartEvent, ModifierDefines.ModifierAction> functions = new Dictionary<ShipDefines.PartEvent, ModifierDefines.ModifierAction>();

    public void AddFunction(ShipDefines.PartEvent evt, ModifierDefines.ModifierAction execution)
    {
        if (execution == null)
        {
            return;
        }

        functions.Add(evt, execution);

    }

    public void ExecuteFunction(ShipDefines.PartEvent act)
    {
        ExecuteEvent(act);
    }

    public void ExecuteEvent(ShipDefines.PartEvent act)
    {
        if (functions.TryGetValue(act, out ModifierDefines.ModifierAction func))
            func.Invoke(this);
    }

    #endregion
    #region Parameters
    public Dictionary<string, float> parameters = new Dictionary<string, float>();
    public void SetParameter(string name, float value)
    {
        if (parameters.ContainsKey(name))
        {
            parameters[name] = value;
        }
        else
        {
            parameters.Add(name, value);
        }
    }
    public float GetParameter(string name)
    {
        if (parameters.TryGetValue(name, out var value))
            return value;
        return 0;
    }
    #endregion
}
