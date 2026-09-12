using System.Linq;
using TMPro;
using UnityEngine;

public class PartTooltip : PartCompBase
{


    public void ShowPart(PurchaseData part)
    {
        ShowPart(part.scriptable);

        if (part.scriptable.combos.Length > 0)
        {
            string mergeLabel = LanguageController.main.Translate("UI Table", "CanMergeWith");
            string[] names = part.scriptable.combos.Select(combo => LanguageController.main.Translate("Parts", combo.other.InternalName)).ToArray();
            description.text += "<br>" + mergeLabel + " " + string.Join(", ", names);
        }
        if (part.scriptable.attach != ItemDefines.PartCondition.Anywhere)
            description.text += "<br>" + LanguageController.main.Translate("UI Table", "condition_" + part.scriptable.attach);
    }
    public void ShowPart(DataItemPart part, bool justPurchase)
    {
        ShowPart(part.scriptable);

        float grantedSpeed = part.GetSpeedGranted(TourneyController.main.GetPlayerRacer());
        if (grantedSpeed > 0)
        {
            string speedLabel = LanguageController.main.Translate("UI Table", "GrantedSpeed").Replace("%value%", grantedSpeed.ToString("F1"));
            description.text += "<br>" + speedLabel;
        }
        float grantedGas = part.GetFuelGranted(TourneyController.main.GetPlayerRacer());
        if (grantedGas > 0)
        {
            string fuelLabel = LanguageController.main.Translate("UI Table", "GrantedFuel").Replace("%value%", grantedGas.ToString("F1"));
            description.text += "<br>" + fuelLabel;
        }

        if (value != null)
        {
            value.text = "$" + Mathf.Ceil(part.scriptable.GetBasePrice() * (justPurchase ? 1 : EconomyDefines.partResellPrice));
        }
    }
}
