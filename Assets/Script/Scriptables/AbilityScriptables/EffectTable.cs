

using UnityEngine;

[CreateAssetMenu(fileName = "Effect Bundle", menuName = "Abilities/Effects/Visual/Effect Bundle")]
public class EffectTable : VisualEffectSO
{
    public BaseEffectSo[] bundle;
    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        foreach (var ef in bundle)
        {
            ef.AffectOnRacer(c, t, s, m) ;
        }
        return false;
    }
}