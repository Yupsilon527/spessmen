using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Built In Modifier", menuName = "Abilities/Effects/Built In Modifier")]
public class BuiltinModifierSO : BasEffectSo
{
    [Header("Duration")]
    public float duration = 1;
    #region Type
    public enum BuiltInModifierType
    {
        Stun = 0,
        Turbo = 1,
    }
    public BuiltInModifierType BuiltinType;
    public override void AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        t.modifiers.Add(GetBuiltinModifier(s));
    }
    Modifier GetBuiltinModifier(Ability source)
    {
        Modifier newModifier = null;
        switch (BuiltinType)
        {
            case BuiltInModifierType.Stun:
                newModifier = new Modifier(source.caster, 1)
                {
                    ModifierName = "Stun",
                    behavior = ModifierDefines.Behavior.IncreaseDuration,
                    expire = ModifierDefines.ExpireType.Time,
                    states = new List<ModifierDefines.State> { ModifierDefines.State.Stunned }
                };
                break;
            case BuiltInModifierType.Turbo:
                newModifier = new Modifier(source.caster, 1)
                {
                    ModifierName = "Turbo",
                    behavior = ModifierDefines.Behavior.IncreaseDuration,
                    expire = ModifierDefines.ExpireType.Time,
                    functions = new Dictionary<ShipDefines.PartEvent, ModifierDefines.ModifierAction>()
                    {
                         {
                             ShipDefines.PartEvent.OnActivated,
                                (Modifier self) =>
                                {
                                        self.SetProperty(ModifierDefines.Property.boost_speed_bonus, self.GetParameter("boost_speed"));
                                }
                         }
                     }
                };
                break;

        }
        if (newModifier != null)
        {
            foreach (ModifierParameter parain in vars)
            {
                newModifier.SetParameter(parain.name, parain.value);
            }
        }
        return newModifier;
    }
    #endregion
    #region Builtin ability variables
    [System.Serializable]
    public class ModifierParameter
    {
        public string name;
        public float value;

        public ModifierParameter(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }
    public ModifierParameter[] vars;
    public float GetVarValue(string name, float def = 1)
    {
        foreach (ModifierParameter abv in vars)
        {
            if (abv.name == name)
            {
                return abv.value;
            }
        }
        return def;
    }
    private void OnValidate()
    {
        if (vars != null) return;
        switch (BuiltinType)
        {
            case BuiltInModifierType.Turbo:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("boost_speed", 1),

                };
                break;
            default:
                vars = new ModifierParameter[]
                {
                    new ModifierParameter("delete_this", 1),
                };
                break;
        }
    }
    #endregion
}
