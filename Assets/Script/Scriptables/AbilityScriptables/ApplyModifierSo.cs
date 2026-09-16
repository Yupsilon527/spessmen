using UnityEngine;

[CreateAssetMenu(fileName = "Apply Modifier", menuName = "Abilities/Effects/Apply Modifier")]
public class ApplyModifierSo : ApplyBuffSo
{
    public ModifierDefines.ExpireType expiretype;
    public float duration = 10;
    public override Modifier Translate(Racer t, float m)
    {
        var modifier = base.Translate(t, m);
        modifier.Set(duration * m);
        return modifier;
    }
}