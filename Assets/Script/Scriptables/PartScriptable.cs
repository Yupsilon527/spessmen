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
    protected override void OnValidate()
    {
        base.OnValidate();
        grid.ValidateAndRecreate();
        foreach (var ability in abilities)
        {
            string newName = name + " " + ((ability.condition == ShipDefines.PartCondition.Always) ? "" : ability.condition) + " " + ability.function;
            if (ability.InternalName != newName) { 
            ability.InternalName = newName;
            ability.classification = partType;
        }
        }
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
            if (ab == null) continue;
            if (output.Length > 0) output += "<br>";
            output += ab.GetAbilityDescription();
        }
        return output;
    }
}
