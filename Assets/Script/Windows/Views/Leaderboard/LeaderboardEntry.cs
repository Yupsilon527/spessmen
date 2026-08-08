using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI racerName, racerPosition, racerDescription;
    public Image racerPortrait;

     void ShowRacer(Racer racer)
    {
        racerName.text = racer.id == 0 ? LanguageController.main.Translate("Leaderboard", "LeaderboardYou") : LanguageController.main.Translate("Racers", racer.playerShip.InternalName);
        if (TourneyController.main.GetPlayerRival() == racer) racerName.text += $" ({LanguageController.main.Translate("Leaderboard", "LeaderboardRival")})";
    }
    public void ShowRacerPosition(Racer racer, int position)
    {
        ShowRacer(racer);
        racerDescription.text = (Mathf.Round(10*racer.position.distanceTraveled)/10).ToString("F1");
        racerPosition.text = position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th";
    }
    public void ShowRacerTournamentStanding(Racer racer, int position)
    {
        ShowRacer(racer);
        racerDescription.text = position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th";
        racerPosition.text = TourneyController.main.GetScoreForRacer(racer).ToString() ;
    }
    public void ShowPlayerResults( int raceID)
    {
        var positionVar = DataItemPlayer.main.scope.GetVariable("race_position_"+raceID);
        var distanceVar = DataItemPlayer.main.scope.GetVariable("race_distance_" + raceID);
        var topspeedvar = DataItemPlayer.main.scope.GetVariable("race_topspeed_" + raceID);

        racerName.text = "Race " + (raceID + 1);

        float distance = Mathf.Round(distanceVar.GetFloatValue() * 10) / 10;
        float topspeed = Mathf.Round(topspeedvar.GetFloatValue() * 10) / 10;
        racerDescription.text = $"{distance}m ({topspeed})";

        int position =Mathf.RoundToInt( positionVar.GetFloatValue());
        racerPosition.text = position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th";
    }
}
