using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI racerName, racerPosition, racerDescription;
    public Image racerPortrait;

    void ShowRacer(Racer racer)
    {
        racerName.text = racer.id == 0 ? "You" : ("Racer " + racer.id);
        if (TourneyController.main.GetPlayerRival() == racer) racerName.text += " (Rival)";
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
}
