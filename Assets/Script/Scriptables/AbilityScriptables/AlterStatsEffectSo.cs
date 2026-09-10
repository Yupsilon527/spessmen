using UnityEngine;

[CreateAssetMenu(fileName = "Alteration", menuName = "Data/Parts/Alteration Data")]
public class AlterStatsEffectSo : BaseEffectSo
{
    public PlayerStatsAlteration alteration;
    public override void AffectOnRacer(Racer c, Racer t, Ability s,float m)
    {
        alteration.GiveToPlayer(c, t, s, m);
    }
    public override string GetDescription()
    {
        string label = "";

        string numValue = Mathf.Abs(alteration.value).ToString();
        if (alteration.behavior == ShipDefines.AlterationType.Multiply)
        {
            numValue = Mathf.Abs(alteration.value * 100) + "% ";
            if (numValue[0] == '+')
                numValue = "x" + label.Substring(1);
            else
                numValue = "x" + label;
        }
        if (alteration.scale == ShipDefines.ScaleType.Constant)
        {
            label += LanguageController.main.Translate("Modifiers", alteration.value < 0 ? "Lose Effect" : "Gain Effect")
               .Replace("%value%", numValue)
               .Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
               .Replace("%scale%", LanguageController.main.Translate("Modifiers", "scale_" + alteration.scale));
        }
        else
        {
            label += LanguageController.main.Translate("Modifiers", ((alteration.scale == ShipDefines.ScaleType.Lucky || alteration.scale == ShipDefines.ScaleType.Random) ? "Chance Scale " : "Stat Scale ") + (alteration.value > 0 ? "Pos" : "Neg"))
                .Replace("%value%", numValue)
                .Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
                .Replace("%scale%", LanguageController.main.Translate("Modifiers", "scale_" + alteration.scale));
        }
        label += LanguageController.main.Translate("Abilities", "effect_" + alteration.stat);
        label = LanguageController.main.Translate("Abilities", "target_" + effectTarget).Replace("%effect%", label);
        return label;
    }
}
