using System;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Component", menuName = "Data/Parts/Component Data")]
public class PartScriptable : ModifierScriptable
{
    public AbilityScriptable[] abilities;
    public MergeOutput[] combos;
    public Sprite icon;
    public ItemDefines.BoonRarity boonRarity;
    public ItemDefines.PartType partType;
    public ItemDefines.PartCondition attach;
    public float priceMultiplier = 1, weightMultiplier = 1;
    public bool unique = false;
    public bool rotating = true;
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
        return priceMultiplier * EconomyDefines.PartPriceBase * Mathf.Pow(2, (int)boonRarity);
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
[Serializable]
public class AbilityScriptable
{
    public string InternalName;
    public int maxUses = 0;
    public float cooldown = 0;
    public float fuelCost = 0;
    public AiRacer.AiSignal aiSignal;
    public ItemDefines.OverflowBehavior overflow;
    public ItemDefines.PartType classification;
    public ShipDefines.PartEvent function;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public BaseEffectSo[] actions;

    public virtual AbilityData Translate()
    {
        return new()
        {
            InternalName = InternalName,
            maxUses = maxUses,
            cooldown = cooldown,
            fuelCost = fuelCost,
            aiSignal = aiSignal,
            overflow = overflow,
            classification = classification,
            function = function,
            condition = condition,
            conditionCheck = conditionCheck,
             actions = actions.Select ( a=> new AbilityData.AbilityListener() {  
                 effectSource = a.effectSource, 
                 effectTarget = a.effectTarget,  
                 d = (Racer caster, Racer target, Ability source, float mult) => { a.AffectOnRacer(caster, target, source, mult); 
                 }
             } ).ToArray(),
        };
    }
    public string GetAbilityDescription()
    {
        string output = $"{LanguageController.main.Translate("Abilities", "function_" + function)}: ";

        string effects = "";
        foreach (var a in actions)
        {
            string label = a.GetDescription();
            if (label.Length == 0) continue;
            if (a.condition != ShipDefines.PartCondition.Always)
            {
                if (a.condition == ShipDefines.PartCondition.RelativeToRival)
                {
                    label = LanguageController.main.Translate("Abilities", conditionCheck < 0 ? "condition_BehindRival" : "condition_AheadOfRival" + function) + ": " + label;
                }
                else
                {
                    label = LanguageController.main.Translate("Abilities", "condition_" + condition).Replace("%value%", conditionCheck.ToString("F1")) + ": " + label;
                }
            }
            if (effects.Length > 0) effects += ", ";
            effects += label;
        }

        string costs = ". ";
        if (fuelCost > 0)
        {
            costs += LanguageController.main.Translate("Abilities", "Ability Cost").Replace("%value%", fuelCost.ToString());
        }
        if (cooldown > 0)
        {
            if (costs.Length > 0) costs += " ";
            costs += LanguageController.main.Translate("Abilities", "Ability Cooldown").Replace("%value%", cooldown.ToString());
        }
        if (maxUses > 0)
        {
            if (costs.Length > 0) costs += " ";
            costs += LanguageController.main.Translate("Abilities", "Ability Uses").Replace("%value%", maxUses.ToString());
        }


        return output + effects + costs;

    }
}