using UnityEngine;

[CreateAssetMenu(fileName = "Alteration", menuName = "Abilities/Effects/Alter Stats")]
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

        if (alteration.scale != ShipDefines.ScaleType.Constant)
            numValue = LanguageController.main.Translate("Modifiers", ((alteration.scale == ShipDefines.ScaleType.Lucky || alteration.scale == ShipDefines.ScaleType.Random) ? "Chance Scale " : "Stat Scale ") + (alteration.value > 0 ? "Pos" : "Neg")).Replace("%value%", numValue);



        if (alteration.stat == ShipDefines.StatType.BaseSpeed 
        || alteration.stat == ShipDefines.StatType.BoostSpeed
        || alteration.stat == ShipDefines.StatType.FillGas
        || alteration.stat == ShipDefines.StatType.GasAbsolute
        || alteration.stat == ShipDefines.StatType.TotalSpeed )
        {
            if (effectSource ==  RaceDefines.AbilityTarget.Self && effectTarget == RaceDefines.AbilityTarget.Self)
            {
                label += numValue + " " + LanguageController.main.Translate("Abilities", "effect_" + alteration.stat);
            }
            else { 
            label += LanguageController.main.Translate("Modifiers", alteration.value < 0 ? "Lose Effect" : "Gain Effect").Replace("%value%", numValue);
            label += LanguageController.main.Translate("Abilities", "effect_" + alteration.stat);
        }
        }
        else
        {
            label += LanguageController.main.Translate("Abilities", "effect_" + alteration.stat).Replace("%value%", numValue);
        }
        label= label.Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + effectSource))
               .Replace("%scale%", LanguageController.main.Translate("Modifiers", "scale_" + alteration.scale))
               .Replace("%target%", LanguageController.main.Translate("Abilities", "target_" + effectTarget));
        return label;
    }
}

