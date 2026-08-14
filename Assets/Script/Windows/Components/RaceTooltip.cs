using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceTooltip : MonoBehaviour
{
    public TextMeshProUGUI raceNumber, rivalDist, raceEnvironment, raceWeather;
    public Image environmentDisplay, weatherDisplay;

    public  void ShowCurrentRace()
    {
        Clear();
        if (TourneyController.main?.ongoingRace == null) return;
        int cRace = (TourneyController.main?.GetCurrentRaceIndex() ?? 0) + 1;
        if (raceNumber != null)
        {
            int raceTotal = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;

            string raceLabel =  LanguageController.main.Translate("UI Table", "Race Label");

            raceNumber.text = raceLabel.Replace("%raceID%", cRace.ToString()).Replace("%raceTotal%", (Mathf.Ceil(cRace / raceTotal + 1) * raceTotal).ToString());
        }
        if (rivalDist!= null)
        {
            string distLabel = LanguageController.main.Translate("UI Table", "Rival Label");
            string recordLabel = LanguageController.main.Translate("UI Table", "Record Label");

            rivalDist.text = distLabel.Replace("%rivalDistance%", TourneyController.main.ongoingRace.modifier != RaceDefines.RaceModifiers.Nothing ? "???" :  TourneyController.main.ongoingRace.GetRivalDistance().ToString("F1"));
            if (cRace>0)
                rivalDist.text+= "<br>" + recordLabel.Replace("%ownDistance%", DataItemPlayer.main.scope.GetVariable("race_distance_" + (cRace-2 )).GetFloatValue().ToString("F1")+"m")
                .Replace("%ownSpeed%", DataItemPlayer.main.scope.GetVariable("race_topspeed_" + (cRace -2)).GetFloatValue().ToString("F1"));
        }
        var environment = TourneyController.main.tournamentEnvironment;
        if (environment == null) return;
        if (raceEnvironment != null)
        {
            string envName =  LanguageController.main.Translate("Environments", environment?.InternalName ?? "None");
            raceEnvironment.text = envName;
        }
            if (raceWeather != null) 
        {
            string weatherName =  LanguageController.main.Translate("Environments", TourneyController.main.ongoingRace.modifier.ToString());
            string weatherDesc =  LanguageController.main.Translate("Environments", TourneyController.main.ongoingRace.modifier.ToString()+"_desc");
            raceWeather.text = $"{LanguageController.main.Translate("UI Table", "ModifierTitle")}<br>{weatherName}<br>{weatherDesc}";
        }

        if (environmentDisplay != null) environmentDisplay.sprite = environment.icon;
            if (weatherDisplay != null) weatherDisplay.sprite = environment.weatherIcons[(int)TourneyController.main.ongoingRace.modifier];
    }
    void Clear()
    {
        if (raceNumber != null) raceNumber.text = "";
        if (raceEnvironment != null) raceEnvironment.text = "";
        if (raceWeather != null) raceWeather.text = "";
    }
}
