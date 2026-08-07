using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceTooltip : MonoBehaviour
{
    public TextMeshProUGUI  raceNumber, raceEnvironment, raceWeather;
    public Image environmentDisplay, weatherDisplay;

    public void ShowCurrentRace()
    {
        if (raceNumber != null)
        {
            int cRace = (TourneyController.main?.GetCurrentRaceIndex()??0) + 1;
            int raceTotal = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;
            raceNumber.text = $"Race {cRace}/{Mathf.Ceil(cRace / raceTotal+1)* raceTotal}";
        }

        var environment = TourneyController.main.tournamentEnvironment;
        if (environment == null) return;
            if (raceEnvironment!=null) raceEnvironment.text = environment?.InternalName ?? "";
            if (raceWeather != null) raceWeather.text = TourneyController.main.ongoingRace.modifier.ToString();

            if (environmentDisplay != null) environmentDisplay.sprite = environment.icon;
            if (weatherDisplay != null) weatherDisplay.sprite = environment.weatherIcons[(int)TourneyController.main.ongoingRace.modifier];
    }
}
