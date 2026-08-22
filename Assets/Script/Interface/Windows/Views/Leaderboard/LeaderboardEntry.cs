using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI racerName, racerPosition, racerDescription;
    public Image racerPortrait;

     void ShowRacer(Racer racer)
    {
        racerPortrait.sprite = racer.playerShip.portrait;
        racerName.text = racer.id == 0 ? LanguageController.main.Translate("Leaderboard", "LeaderboardYou") : LanguageController.main.Translate("Racers", racer.playerShip.InternalName);
        if (TourneyController.main.GetPlayerRival() == racer) racerName.text += $" ({LanguageController.main.Translate("Leaderboard", "LeaderboardRival")})";
    }
    public void ShowRacerPosition(Racer racer, int position)
    {
        ShowRacer(racer);
        racerDescription.text = $"{(Mathf.Round(10 * racer.position.distanceTraveled) / 10)}m ({(Mathf.Round(10 * racer.stats.realSpeed) / 10)} km/h)";
        racerPosition.text = position == 0 ? "1<sup>st" : position == 1 ? "2<sup>nd" : position == 2 ? "3<sup>rd" : $"{position + 1}<sup>th";
    }
    public void ShowRacerTournamentStanding(Racer racer, int position)
    {
        ShowRacer(racer);
        racerDescription.text = position == 0 ? "1<sup>st" : position == 1 ? "2<sup>nd" : position == 2 ? "3<sup>rd" : $"{position + 1}<sup>th";
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
        racerPosition.text = position == 0 ? "1<sup>st" : position == 1 ? "2<sup>nd" : position == 2 ? "3<sup>rd" : $"{position + 1}<sup>th";
    }
}
