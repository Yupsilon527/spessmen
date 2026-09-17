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
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceSetup:
                foreach (Modifier modifier in modifiers)
                {
                    modifier.Restart(Time.time);
                }
                Refresh(true);
                break;
            case RaceDefines.RacePhase.RaceTick:
                Think();
                break;
            case RaceDefines.RacePhase.RaceEnd:
                modifiers.Clear();
                break;

        }
    }
    #region Events

    public void ListenToEvent(ShipDefines.PartEvent evt)
    {
        DataItemPlayer.main.Inspect($"{racer} activate ability {evt}");
        foreach (Modifier modifier in modifiers)
        {
            if (!modifier.dead && !modifier.IsExpired())
            {
                modifier.ExecuteEvent(evt);
            }
        }
    }
    #endregion
    #region Timely Update
    float nextUpdateTime = 0;
    bool HasUpdates = false;
    public void Think()
    {
        if (HasUpdates && nextUpdateTime < Time.time)
        {
            nextUpdateTime = Time.time + 1;
            HasUpdates = false;
            foreach (Modifier Mod in modifiers.ToArray())
            {
                if (Mod.dead) continue;
                if (!Mod.IsExpired())
                {
                    if (Mod.expire == ModifierDefines.ExpireType.Time)
                    {
                        HasUpdates = true;
                    }
                    nextUpdateTime = Mathf.Min(nextUpdateTime, Mod.GetEndTime());
                }
                else
                {
                    Mod.Die(true);
                }
            }
        }
        Refresh(false);
    }
    #endregion
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
            statRefresh = true;
            propRefresh = true;
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
    public bool Add(Modifier newModifier, bool skipImmunityCheck = false, bool refresh = true)
    {
        if (!skipImmunityCheck && IsImmuneToModifier(newModifier)) { return false; }
        if (newModifier.expire == ModifierDefines.ExpireType.Time)
            newModifier.Restart(Time.time);
        switch (newModifier.behavior)
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
                    original.SetStackCount(original.GetStackCount() + newModifier.GetStackCount());
                    if (original.properties.Count > 0)
                        RefreshProperties();
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
        OnAddModifier(newModifier);
        if (refresh) Refresh();
        return true;
    }
    void OnAddModifier(Modifier Modifier)
    {
        modifiers.Add(Modifier);
        if (Modifier.expire == ModifierDefines.ExpireType.Time)
            HasUpdates = true;
        UpdateModifierStates(Modifier);
        UpdateModifierProperties(Modifier);
        Modifier.ExecuteEvent(ShipDefines.PartEvent.OnActivated);

        foreach (Modifier Mod in modifiers)
        {
            if (IsImmuneToModifier(Mod))
            {
                DestroyModifier(Mod);
            }
        }
        RefreshModifier(Modifier);
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
            if (prop.Key <= ModifierDefines.Property.tank_capacity)
            {
                racer.stats.SetDirty();
            }
        }
    }
    void ResetPropertiesDefaults()
    {
        properties = new float[(int)ModifierDefines.Property.total];
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
        Name = Name?.ToLower() ?? "";
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
    public Modifier[] Filter(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Undispellable, bool includePositives = false, bool includeNegative = false)
    {
        List<Modifier> rest = new List<Modifier>();
        foreach (Modifier Mod in modifiers)
        {
            if (!Mod.dead && !Mod.IsExpired())
            {
                if (ModifierName == "" || ModifierName == Mod.ModifierName)
                {
                    if ((alignment == ModifierDefines.Flag.Undispellable || alignment == Mod.flag) || (includeNegative && Mod.IsNegative()) || (includePositives && Mod.IsPositive()))
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
    public void DestroyFilteredModifiers(string ModifierName = "", ModifierDefines.Flag alignment = ModifierDefines.Flag.Undispellable, bool includePositives = false, bool includeNegative = false, bool refresh = true)
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

    #region Modifier Immunity
    public bool IsImmuneToModifier(Modifier mod)
    {
        if (mod.flag == ModifierDefines.Flag.Undispellable)
        {
            return false;
        }

        return GetState(ModifierDefines.State.DebuffImmune) && mod.IsNegative();
    }
    public bool IsImmuneToModifier(ModifierData mod)
    {
        if (mod.flag == ModifierDefines.Flag.Undispellable)
        {
            return false;
        }

        return GetState(ModifierDefines.State.DebuffImmune) && mod.flag <= ModifierDefines.Flag.Debuff;
    }
    #endregion
}
