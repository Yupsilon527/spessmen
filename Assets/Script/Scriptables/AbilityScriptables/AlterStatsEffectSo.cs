using UnityEngine;

[CreateAssetMenu(fileName = "Alteration", menuName = "Data/Parts/Alteration Data")]
public class AlterStatsEffectSo : BaseEffectSo
{
    public PlayerStatsAlteration alteration;
    public override void AffectOnRacer(Racer c, Racer t, Ability s,float m)
    {
        alteration.GiveToPlayer(c, t, s, m);
    }
}
