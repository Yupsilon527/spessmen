using System.Linq;
using TMPro;

public class RacerSelDescription : RacerSelComponent
{
    public TextMeshProUGUI title, desc;
    public override void AssignScriptable(ShipScriptable ship)
    {
        base.AssignScriptable(ship);

        bool unlocked = ship.condition.IsUnlocked();

        if (unlocked) { 

        title.text = LanguageController.main.Translate("Racers", ship.InternalName);

        string[] names = ship.startingParts.Select(part => LanguageController.main.Translate("Parts", part.InternalName)).ToArray();

        desc.text = LanguageController.main.Translate("UI Table", "Starting Gold").Replace("%value%", ship.startingGold.ToString())
            + (names.Length > 0 ? ("<br>" +LanguageController.main.Translate("UI Table", "Starting Parts").Replace("%parts%", string.Join(", ", names))) : "")
            + "<br>" + ship.GetEffectDescription();
    }
        else
        {
            title.text = LanguageController.main.Translate("Racers", "racer_locked");
            desc.text = LanguageController.main.Translate("Racers", ship.InternalName+"_unlock_condition");
        }
    }
}
