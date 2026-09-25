using UnityEngine;

[CreateAssetMenu(fileName = "Apply Modifier", menuName = "Abilities/Effects/Apply Modifier")]
public class ApplyModifierSo : ApplyBuffSo
{
    public ModifierDefines.ExpireType expiretype;
    public float duration = 10;
    public override Modifier Translate(Racer s,Racer t, float m)
    {
        var modifier = base.Translate(s,t, m);
        modifier.expire = expiretype;
        modifier.Set(duration * m);
        return modifier;
    }
    public override string GetDescription()
    {
        string label = base.GetDescription();
        label += LanguageController.main.Translate("Modifiers", "Modifier Duration").Replace("%duration%", duration.ToString("F1"));
        return label;
    }
}