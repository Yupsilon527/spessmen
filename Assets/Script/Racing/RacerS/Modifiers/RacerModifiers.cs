using System;
using System.Collections.Generic;
using UnityEngine;

public class RacerModifiers : PropertyComponent
{
    [NonSerialized] protected List<Modifier> modifiers = new List<Modifier>();

    public RacerModifiers(Racer racer) : base(racer)
    {
    }
    public List<Modifier> GetModifiers()
    {
        return modifiers;
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        base.HandleRacePhase(phase);
        switch (phase) {
            case RaceDefines.RacePhase.RaceBegin:
                foreach (Modifier modifier in modifiers)
                {
                    modifier.Restart(Time.time);
                }
                Refresh(true);
                break;
            case RaceDefines.RacePhase.RaceEnd:
                modifiers.Clear();
                break;

        }
    }
    #region Refresh
    bool propRefresh = true;
    bool statRefresh = true;


    public void RefreshModifier(Modifier Modifier)
    {
        if (Modifier.states.Count > 0) RefreshStates();
        if (Modifier.properties.Count > 0) RefreshProperties();
    }
    public void RefreshProperties()
    {
        propRefresh = true;
    }
    public void RefreshStates()
    {
        statRefresh = true;
    }
    public void Refresh(bool force = false)
    {
        if (force)
        {
            statRefresh = propRefresh = true;
        }
        if (statRefresh || propRefresh)
        {
            modifiers.RemoveAll((Modifier mod) => mod.dead);
            if (statRefresh) states = new int[(int)ModifierDefines.State.Total];
            if (propRefresh) properties = new float[(int)ModifierDefines.Property.total];
            foreach (Modifier mod in modifiers)
            {
                if (!mod.dead && !mod.IsExpired())
                {
                    if (statRefresh)
                        UpdateModifierStates(mod);
                    if (propRefresh)
                        UpdateModifierProperties(mod);
                }
                if (propRefresh)
                {
                    racer.stats.UpdateRealSpeed();
                }
            }
        }
        propRefresh = false;
        statRefresh = false;
    }
    #endregion

    #region Create Modifiers
    public bool Add(Modifier Modifier, bool skipImmunityCheck = false, bool refresh = true)
    {
     //   if (!skipImmunityCheck && IsImmuneToModifier(Modifier)) { return false; }
        Modifier.Restart(Time.time);
        switch (Modifier.behavior)
        {
            case ModifierDefines.Behavior.Replace: //Replace 
                if (TryFindModifierByName(Modifier.ModifierName, false, out Modifier found))
                    Remove(found);
                break;
            case ModifierDefines.Behavior.Unique: //Unique 
                if (HasModifier(Modifier.ModifierName))
                {
                    return false;
                }
                break;
            case ModifierDefines.Behavior.IncreaseStacks:
                if (TryFindModifierByName(Modifier.ModifierName, false, out Modifier original))
                {
                    original.SetStackCount(original.GetStackCount() + Modifier.GetStackCount());
                    if (original.properties.Count > 0)
                        RefreshProperties();
                    return false;
                }
                break;
            case ModifierDefines.Behavior.IncreaseDuration:
                if (TryFindModifierByName(Modifier.ModifierName, false, out Modifier first))
                {
                    first.Extend(Modifier.GetDuration());
                    return false;
                }
                break;

        }
        Debug.Log("[Modifiers] Add new modifier " + Modifier.ModifierName);
        OnAddModifier(Modifier);
        if (refresh) Refresh();
        return true;
    }
    void OnAddModifier(Modifier Modifier)
    {
        modifiers.Add(Modifier);
       /* foreach (Modifier Mod in modifiers)
        {
            if (IsImmuneToModifier(Mod))
            {
                Mod.Die(false);
            }
        }*/
        RefreshModifier(Modifier);
    }
    #endregion

    #region Add
    public bool Add(Modifier newModifier)
    {
        //if (IsImmuneToModifier(newModifier)) { return false; }
        newModifier.Restart(Time.time);
    
        switch (newModifier.data.behavior)
        {
            case ModifierDefines.Behavior.Replace: //Replace 
                if (TryFindModifierByName(newModifier.ModifierName, false, out Modifier found))
                    DestroyModifier(found);
                break;
            case ModifierDefines.Behavior.Unique: //Unique 
                if (HasModifier(newModifier.ModifierName))
                {
                    return false;
                }
                break;
            case ModifierDefines.Behavior.IncreaseStacks:
                if (TryFindModifierByName(newModifier.ModifierName, false, out Modifier original))
                {
                    original.Restart(Time.time);
                    original.stacks += newModifier.stacks;
                    return false;
                }
                break;
            case ModifierDefines.Behavior.IncreaseDuration:
                if (TryFindModifierByName(newModifier.ModifierName, false, out original))
                {
                    original.Extend(newModifier.GetDuration());
                    return false;
                }
                break;

        }
        Debug.Log("[Modifiers] Add new modifier " + newModifier.ModifierName);
        OnModifierAdded(newModifier);
        modifiers.Add(newModifier);
        return true;
    }
    void OnModifierAdded(Modifier Modifier)
    {
        UpdateModifierStates(Modifier);
        UpdateModifierProperties(Modifier);

      /*  foreach (Modifier Mod in modifiers)
        {
            if (IsImmuneToModifier(Mod))
            {
                DestroyModifier(Mod);
            }
        }*/
    }
    void Refresh()    //TODO
    {
        states = new int[(int)ModifierDefines.State.Total];
        ResetPropertiesDefaults();
        RemoveIrrelevantModifiers();
        foreach (Modifier mod in modifiers)
        {
            if (!mod.dead && !mod.IsExpired())
            {
                UpdateModifierStates(mod);
                UpdateModifierProperties(mod);
            }
        }
    }
    void RemoveIrrelevantModifiers()
    {
        modifiers.RemoveAll((Modifier Mod) =>
        {
            return Mod.dead;
        });
    }
    #endregion
    #region Update States

    void UpdateModifierStates(Modifier Mod)
    {
        foreach (ModifierDefines.State state in Mod.states)
        {
            UpdateState(state, (int)Mod.priority);
        }
    }

    void UpdateModifierProperties(Modifier Mod)
    {
        foreach (KeyValuePair<ModifierDefines.Property, float> prop in Mod.properties)
        {
            UpdateProperty(prop.Key, prop.Value);
        }
    }
    void ResetPropertiesDefaults()
    {
        properties = new float[(int)ModifierDefines.Property.total];
        for (int iProp = 0; iProp < (int)ModifierDefines.Property.total; iProp++)
        {
            if (ModifierDefines.IsPropertyMultiplicative((ModifierDefines.Property)iProp))
            {
                properties[iProp] = 1;
            }
            else
            {
                properties[iProp] = 0;
            }
        }
    }
    #endregion
    #region Find By Name
    public bool HasModifier(string Name)
    {
        return FindModifierByName(Name) != null;
    }
    public bool HasModifier(string Name, out Modifier mod)
    {
        mod = FindModifierByName(Name);
        return mod != null;
    }
    public Modifier FindModifierByName(string Name)
    {
        Name = Name.ToLower();
        foreach (Modifier Mod in modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
                if (Mod.ModifierName == Name)
                {
                    return Mod;
                }
        }
        return null;
    }
    public bool TryFindModifierByName(string Name, bool last, out Modifier found)
    {
        found = null;
        for (int iM = 0; iM < modifiers.Count; iM++)
        {
            Modifier mod = modifiers[last ? (modifiers.Count - iM - 1) : iM];
            if (mod.ModifierName == Name)
            {
                found = mod;
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Filter
    public Modifier[] Filter(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Nothing, bool includePositives = false, bool includeNegative = false)
    {
        List<Modifier> rest = new List<Modifier>();
        foreach (Modifier Mod in modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
            {
                if (ModifierName == "" || ModifierName == Mod.ModifierName)
                {
                 //   if ((alignment == ModifierDefines.Flag.Nothing || alignment == Mod.data.flag) || (includeNegative && Mod.IsNegative()) || (includePositives && Mod.IsPositive()))
                    {
                        rest.Add(Mod);
                    }
                }
            }
        }
        return rest.ToArray();

    }
    #endregion
    #region Remove Modifiers
    public void DestroyModifier(Modifier Mod)
    {
        Remove(Mod, false);
    }
    public void DestroyFilteredModifiers(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Nothing, bool includePositives = false, bool includeNegative = false, bool refresh = true)
    {
        Remove(Filter(ModifierName, alignment, includePositives, includeNegative), false, refresh);
    }

    public void Remove(Modifier Mod, bool expire = true, bool refresh = true)
    {
        Remove(new Modifier[] { Mod }, expire, refresh);
    }
    public void Remove(Modifier[] Mods, bool expire = true, bool refresh = true)
    {
        foreach (Modifier Modifier in Mods)
        {
            if (Modifier == null)
                continue;

            Modifier.Die(expire);
        }
        if (refresh)
        {
            Refresh();
        }
    }
    #endregion

    /*
    public bool IsImmuneToModifier(Modifier mod)
    {
        return IsImmuneToModifier(mod.data);
    }
    public bool IsImmuneToModifier(ModifierData mod)
    {
        if (mod.flag == ModifierDefines.Flag.Undispellable)
        {
            return false;
        }

        return GetState(ModifierDefines.State.debuff_immune) && mod.IsNegative();
    }*/
}

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
            properties[(int)Property] *= value;
        }
        else
        {
            properties[(int)Property] += value;
        }
    }
    #endregion
}
