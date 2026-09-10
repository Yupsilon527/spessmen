using UnityEngine;

public abstract class BaseEffectSo : ScriptableObject
{
    public RaceDefines.AbilityTarget effectSource, effectTarget;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public virtual void AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {

    }
    public virtual bool CanAffectRacer(Racer target)
    {
        return target != null && ShipDefines.RacerMeetsCondition(target, condition, conditionCheck);
    }
}
