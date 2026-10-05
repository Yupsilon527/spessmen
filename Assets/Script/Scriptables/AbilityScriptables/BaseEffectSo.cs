using UnityEngine;

public abstract class BaseEffectSo : ScriptableObject
{
    public RaceDefines.AbilityTarget effectSource, effectTarget;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public virtual bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        return  (m != 0  && CanAffectRacer(t) && (c == t || !t.GetState(ModifierDefines.State.AbilityImmune)));

    }
    public virtual bool CanAffectRacer(Racer target)
    {
        return target != null && ShipDefines.RacerMeetsCondition(target, condition, conditionCheck);
    }
    public virtual bool HasDescription()
    {
        return true;
    }
    public virtual string GetDescription()
    {
        return "NOT IMPLEMENTED";
    }
}
