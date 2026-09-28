using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Data/Parts/Tag Data")]
public class ApplyTagSo : BaseEffectSo
{
    public string modifierName;

    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        bool _ = base.AffectOnRacer(c, t, s, m);
        if (_)
        {
            t.modifiers.Add(Translate(c, t, m));
        }
        return _;
    }
    public virtual Modifier Translate(Racer s, Racer t, float m)
    {
        return new Modifier(s,t, 0)
        {
            ModifierName = modifierName,
            priority = ModifierDefines.Priority.low,
            flag = ModifierDefines.Flag.Undispellable,
            behavior = ModifierDefines.Behavior.Unique,
        };
    }
    public override string GetDescription()
    {
        string label = LanguageController.main.Translate("Modifiers", "Gain Effect").Replace("%value%", LanguageController.main.Translate("Modifiers", "modifier_" + modifierName));
        label = label.Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
               .Replace("%target%", LanguageController.main.Translate("Abilities", "target_" + effectTarget));
        return label;
    }
}
