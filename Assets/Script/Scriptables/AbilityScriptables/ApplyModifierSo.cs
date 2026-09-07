using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Data/Parts/Modifier Data")]
public class ApplyModifierSo : ApplyBuffSo
{
    public ModifierDefines.ExpireType expiretype;
    public float duration = 10;
    public override Modifier Translate(Racer t)
    {
        var modifier =  base.Translate(t);
        modifier.Set(duration);
        return modifier;
    }
}