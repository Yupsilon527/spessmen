using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Component", menuName = "Data/Component Data")]
public class PartScriptable : ModifierScriptable
{
    public PartAbility[] abilities;
    public MergeOutput[] combos;
    public Sprite icon;
    public ItemDefines.BoonRarity boonRarity;
    public ItemDefines.PartType partType;
    public ItemDefines.PartCondition attach;
    public float priceMultiplier = 1, weightMultiplier = 1;
    public bool unique = false;
    public bool lockedForSomeReason = false;
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.ValidateAndRecreate();
        foreach (var ability in abilities)
        {
            ability.InternalName = name + " " + ((ability.condition == ShipDefines.PartCondition.Always) ? "" : ability.condition) + " " + ability.function;
            ability.classification = partType;
        }
    }
    public virtual bool IsUnlocked()
    {
        return !lockedForSomeReason;
    }
    public virtual float GetBasePrice()
    {
        return priceMultiplier * 20 * Mathf.Pow(2, (int)boonRarity);
    }
    public override string GetEffectDescription()
    {
        string output = base.GetEffectDescription();
        foreach (var ab in abilities)
        {
            if (output.Length > 0) output += "<br>";
            output += ab.GetAbilityDescription();
        }
        return output;
    }
}

[Serializable]
public class MergeOutput
{
    public PartScriptable other, result;
}