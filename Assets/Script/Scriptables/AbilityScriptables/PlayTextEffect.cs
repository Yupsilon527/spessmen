
using UnityEngine;

[CreateAssetMenu(fileName = "Play TextEffect", menuName = "Abilities/Effects/Visual/Play TextEffect")]
public class PlayTextEffect : VisualEffectSO
{
    public string AlertName = "";

    public override bool AffectOnRacer(Racer c, Racer t, Ability s, float m, bool a)
    {
        bool _ = base.AffectOnRacer(c, t, s, m, a);
        if (_ && s != null)
        {
            ArenaController.main.GetToonForRacer(t).Alert(AlertName, Color.white, AttachPoint);
        }
        return _;
    }
}
