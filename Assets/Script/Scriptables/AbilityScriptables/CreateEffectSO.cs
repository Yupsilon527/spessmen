

using UnityEngine;

[CreateAssetMenu(fileName = "Play Special Effect", menuName = "Abilities/Effects/Visual/Play Special Effect")]
public class CreateEffectSO : VisualEffectSO
{
    public bool attached = false;
    public GameObject EffectPrefab;

    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m)
    {
        bool _ = base.AffectOnRacer(c, t, s, m);
        if (_)
        {
            var toonTable = ArenaController.main.GetToonForRacer(t);
            MakeEffectOnToon(toonTable.toon, EffectDelay, t.id);
        }
        return _;
    }
    public virtual GameObject MakeEffectOnToon(Toon toonTarget, float delay, int order)
    {
        if (!toonTarget.gameObject.activeSelf) return null;
        var atp = toonTarget.character.FindAttachPoint(AttachPoint);

        if (attached)
            return ArenaController.main.epool.AttachEffectFromPrefab(atp.gameObject, EffectPrefab, EffectDelay, order);
        else
            return ArenaController.main.epool.EffectFromPrefab(EffectPrefab, atp.position, EffectDelay);
    }
}
