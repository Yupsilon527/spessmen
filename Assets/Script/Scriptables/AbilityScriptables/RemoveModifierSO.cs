using UnityEngine;

[CreateAssetMenu(fileName = "Remove Modifier", menuName = "Abilities/Effects/Remove Modifier")]
public class RemoveModifierSO : BaseEffectSo
{
    public string modifierName;

    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        bool _ = base.AffectOnRacer(c, t, s, m);
        if (_)
        {
            t.modifiers.Remove(t.modifiers.Filter(modifierName), false, true) ;
        }
        return _;
    }
    public override bool HasDescription()
    {
        return true;
    }
    public override string GetDescription()
    {
        string label = LanguageController.main.Translate("Modifiers", "Gain Effect").Replace("%value%", LanguageController.main.Translate("Modifiers", "modifier_" + modifierName));
        label = label.Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
               .Replace("%target%", LanguageController.main.Translate("Abilities", "target_" + effectTarget));
        return label;
    }
}
