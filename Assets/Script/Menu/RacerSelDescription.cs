using System.Linq;
using TMPro;
using UnityEngine.UI;

public class RacerSelDescription : RacerSelComponent
{
    public TextMeshProUGUI title, desc;
    public Image firstCup, secondCup, thirdCup,firstCupG, secondCupG, thirdCupG;
    public override void AssignScriptable(ShipScriptable ship)
    {
        base.AssignScriptable(ship);
        Clear();

        bool unlocked = ship.condition.IsUnlocked();

        if (unlocked) { 

        title.text = LanguageController.main.Translate("Racers", ship.InternalName);

        string[] names = ship.startingParts.Select(part => LanguageController.main.Translate("Parts", part.InternalName)).ToArray();

        desc.text = LanguageController.main.Translate("UI Table", "Starting Gold").Replace("%value%", ship.startingGold.ToString())
            + (names.Length > 0 ? ("<br>" +LanguageController.main.Translate("UI Table", "Starting Parts").Replace("%parts%", string.Join(", ", names))) : "")
            + "<br>" + ship.GetEffectDescription();

            var characterAttempts = PlayerConfig.main.globalScope.GetVariable("attempts_with_" + ship.InternalName);
            var characterWins = PlayerConfig.main.globalScope.GetVariable("seasons_won_with_" + ship.InternalName);
            desc.text += $"<br><br>{LanguageController.main.Translate("UI Table", "Attempts")}: {(int)characterAttempts.GetFloatValue()}<br>{LanguageController.main.Translate("UI Table", "Wins")}: {(int)characterWins.GetFloatValue()}";


            var playerCup = PlayerConfig.main.globalScope.GetVariable("highest_cup_with_" + ship.InternalName);

            if (playerCup.GetFloatValue() > 0) {
              var perfectCup = PlayerConfig.main.globalScope.GetVariable("perfect_season0_with_" + ship.InternalName);
                firstCup.gameObject.SetActive(!perfectCup.GetBoolValue());
                firstCupG.gameObject.SetActive(perfectCup.GetBoolValue());
            }
            else { 
                firstCup.gameObject.SetActive(false);
                firstCupG.gameObject.SetActive(false); 
            }
            if (playerCup.GetFloatValue() > 1)
            {
                var perfectCup = PlayerConfig.main.globalScope.GetVariable("perfect_season1_with_" + ship.InternalName);
                secondCup.gameObject.SetActive(!perfectCup.GetBoolValue());
                secondCupG.gameObject.SetActive(perfectCup.GetBoolValue());
            }
            else
            {
                secondCup.gameObject.SetActive(false);
                secondCupG.gameObject.SetActive(false);
            }
            if (playerCup.GetFloatValue() > 2)
            {
                var perfectCup = PlayerConfig.main.globalScope.GetVariable("perfect_season2_with_" + ship.InternalName);
                thirdCup.gameObject.SetActive(!perfectCup.GetBoolValue());
                thirdCupG.gameObject.SetActive(perfectCup.GetBoolValue());
            }
            else
            {
                thirdCup.gameObject.SetActive(false);
                thirdCupG.gameObject.SetActive(false);
            }
        }
        else
        {
            title.text = LanguageController.main.Translate("Racers", "racer_locked");
            desc.text = LanguageController.main.Translate("Racers", ship.InternalName+"_unlock_condition");
        }
    }
    public override void Clear()
    {
        base.Clear();
        firstCup.gameObject.SetActive(false);
        secondCup.gameObject.SetActive(false);
        thirdCup.gameObject.SetActive(false);
        firstCupG.gameObject.SetActive(false);
        secondCupG.gameObject.SetActive(false);
        thirdCupG.gameObject.SetActive(false);
    }
}
