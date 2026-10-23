

using UnityEngine;

[CreateAssetMenu(fileName = "Play Special Effect", menuName = "Abilities/Effects/Visual/Play Special Effect")]
public class CreateEffectSO : VisualEffectSO
{
    public enum EffectType
    {
        oneShot,
        oneShotAttached,
        boost,
    }
    public EffectType effectType;
    public GameObject EffectPrefab;

    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        bool _ = base.AffectOnRacer(c, t, s, m);
        if (_)
        {
            RacerToonTable toonTable = ArenaController.main.GetToonForRacer(t);
            MakeEffectOnToon(toonTable, EffectDelay, t.id);
        }
        return _;
    }
    public virtual GameObject MakeEffectOnToon(RacerToonTable toonTarget, float delay, int order)
    {
        if (!toonTarget.toon.gameObject.activeSelf) return null;
        var atp = toonTarget.toon.character.FindAttachPoint(AttachPoint);

        switch (effectType)
        {
            default:
                return ArenaController.main.epool.EffectFromPrefab(EffectPrefab, atp.position, EffectDelay);
            case EffectType.oneShotAttached:
                return ArenaController.main.epool.AttachEffectFromPrefab(atp.gameObject, EffectPrefab, EffectDelay, order);
            case EffectType.boost:
                var effect = ArenaController.main.epool.AttachEffectFromPrefab(atp.gameObject, EffectPrefab, EffectDelay, order);
                if (effect.TryGetComponent(out SpecialEffectController sfx))
                {
                    toonTarget.AddAfterburnerEffects(sfx);
                }
                return effect;
        }

    }
}
