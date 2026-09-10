using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Data/Parts/Tag Data")]
public class ApplyTagSo : BaseEffectSo
{
    public string modifierName;

    public override void AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        t.modifiers.Add(Translate(t));
    }
    public virtual Modifier Translate(Racer t)
    {
        return new Modifier(t, 0)
        {
            ModifierName = modifierName,
            priority = ModifierDefines.Priority.low,
            flag = ModifierDefines.Flag.Undispellable,
            behavior = ModifierDefines.Behavior.Unique,
        };
    }
}
