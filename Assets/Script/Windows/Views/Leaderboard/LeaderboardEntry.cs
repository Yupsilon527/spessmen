using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI racerName, racerPosition, racerDescription;
    public Image racerPortrait;

    public void RacerPosition(Racer racer, int position)
    {
        racerName.text = racer.id == 0 ? "You" : ("Racer " + racer.id);
        racerDescription.text = (Mathf.Round(10*racer.position.distanceTraveled)/10).ToString();
        racerPosition.text = position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th";
    }
}
